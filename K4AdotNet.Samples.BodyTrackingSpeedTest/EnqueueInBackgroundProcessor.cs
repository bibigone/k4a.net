using System;
using System.Threading;

namespace K4AdotNet.Samples.BodyTrackingSpeedTest
{
    internal sealed class EnqueueInBackgroundProcessor : Processor
    {
        private volatile bool processing;
        private volatile int queueSize;
        private int processedFrameCount;
        private int frameWithBodyCount;

        public EnqueueInBackgroundProcessor(ProcessingParameters processingParameters)
            : base(processingParameters)
        {
            processing = true;
            new Thread(EnqueueLoop) { IsBackground = true }.Start();
        }

        public override void Dispose()
        {
            processing = false;
            base.Dispose();
        }

        public override int TotalFrameCount => processedFrameCount;

        public override int FrameWithBodyCount => frameWithBodyCount;

        public override int QueueSize => queueSize;

        public override bool NextFrame()
        {
            while (processing || queueSize > 0)
            {
                using (var frameHandle = TryPopBodyFrame(Timeout.NoWait))
                {
                    if (frameHandle != null)
                    {
                        Interlocked.Decrement(ref queueSize);
                        processedFrameCount++;

                        var bodyCount = (int)BodyTracking.NativeApi.FrameGetNumBodies(frameHandle).ToUInt32();
                        if (bodyCount > 0)
                        {
                            frameWithBodyCount++;
                        }

                        return true;
                    }
                    else
                    {
                        Thread.Sleep(0);
                    }
                }
            }

            return false;
        }

        private void EnqueueLoop()
        {
            while (processing)
            {
                var streamRes = Playback.NativeApi.PlaybackGetNextCapture(playbackHandle, out var captureHandle);
                if (streamRes == NativeCallResults.StreamResult.Eof)
                {
                    break;
                }
                if (streamRes == NativeCallResults.StreamResult.Failed)
                    throw new ApplicationException("Cannot read next frame from recording");

                using (captureHandle)
                {
                    if (processingParameters.EndTime.HasValue)
                    {
                        var timestamp = GetTimestamp(captureHandle);
                        if (timestamp.HasValue && !processingParameters.IsTimeInStartEndInterval(timestamp.Value))
                        {
                            break;
                        }
                    }

                    while (processing)
                    {
                        if (TryEnqueueCapture(captureHandle, Timeout.NoWait))
                        {
                            Interlocked.Increment(ref queueSize);
                            break;
                        }
                        Thread.Sleep(0);
                    }
                }
            }

            processing = false;
        }
    }
}