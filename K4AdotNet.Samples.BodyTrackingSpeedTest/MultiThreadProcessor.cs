using System;
using System.Threading;

namespace K4AdotNet.Samples.BodyTrackingSpeedTest
{
    internal sealed class MultiThreadProcessor : Processor
    {
        private volatile bool processing;
        private volatile int queueSize;
        private volatile int processedFrameCount;
        private volatile int frameWithBodyCount;

        public MultiThreadProcessor(ExecutionParameters executionParameters)
            : base(executionParameters)
        {
            processing = true;
            new Thread(ProcessingLoop) { Priority = ThreadPriority.AboveNormal, IsBackground = true }.Start();
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
                if (executionParameters.EndTime.HasValue)
                {
                    var timestamp = GetTimestamp(captureHandle);
                    if (timestamp.HasValue && !executionParameters.IsTimeInStartEndInterval(timestamp.Value))
                    {
                        WaitForProcessingOfQueueTail();
                        return false;
                    }
                }

                EnqueueCapture(captureHandle, Timeout.Infinite);
                Interlocked.Increment(ref queueSize);
            }

            return true;
        }

        private void WaitForProcessingOfQueueTail()
        {
            while (queueSize > 0)
            {
                Thread.Sleep(0);
            }
        }

        private void ProcessingLoop()
        {
            while (processing)
            {
                using (var frameHandle = TryPopBodyFrame(Timeout.NoWait))
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
                }
            }
        }
    }
}