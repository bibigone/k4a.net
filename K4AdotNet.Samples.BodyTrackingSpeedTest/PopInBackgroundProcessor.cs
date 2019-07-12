using System;
using System.Threading;

namespace K4AdotNet.Samples.BodyTrackingSpeedTest
{
    internal sealed class PopInBackgroundProcessor : Processor
    {
        private volatile bool processing;
        private volatile int queueSize;
        private volatile int processedFrameCount;
        private volatile int frameWithBodyCount;

        public PopInBackgroundProcessor(ProcessingParameters processingParameters)
            : base(processingParameters)
        {
            processing = true;
            new Thread(ProcessingLoop) { IsBackground = true }.Start();
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
            var streamRes = Playback.NativeApi.PlaybackGetNextCapture(playbackHandle, out var captureHandle);
            if (streamRes == NativeCallResults.StreamResult.Eof)
            {
                WaitForProcessingOfQueueTail();
                return false;
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
                        WaitForProcessingOfQueueTail();
                        return false;
                    }
                }

                TryEnqueueCapture(captureHandle, Timeout.Infinite);
                Interlocked.Increment(ref queueSize);
            }

            return true;
        }

        private void WaitForProcessingOfQueueTail()
        {
            while (queueSize > 0)
            {
                Thread.Sleep(1);
            }
        }

        private void ProcessingLoop()
        {
            while (processing)
            {
                using (var frameHandle = TryPopBodyFrame(Timeout.FromMilliseconds(10)))
                {
                    if (frameHandle != null)
                    {
                        Interlocked.Decrement(ref queueSize);
                        Interlocked.Increment(ref processedFrameCount);

                        var bodyCount = (int)BodyTracking.NativeApi.FrameGetNumBodies(frameHandle).ToUInt32();
                        if (bodyCount > 0)
                        {
                            Interlocked.Increment(ref frameWithBodyCount);
                        }
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
            }
        }
    }
}