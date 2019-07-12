using System;

namespace K4AdotNet.Samples.BodyTrackingSpeedTest
{
    internal sealed class SingleThreadProcessor : Processor
    {
        private int totalFrameCount;
        private int frameWithBodyCount;
        private int queueSize;

        public SingleThreadProcessor(ProcessingParameters processingParameters)
            : base(processingParameters)
        { }

        public override int TotalFrameCount => totalFrameCount;

        public override int FrameWithBodyCount => frameWithBodyCount;

        public override int QueueSize => queueSize;

        public override bool NextFrame()
        {
            if (QueueSize == BodyTracking.NativeApi.MAX_TRACKING_QUEUE_SIZE)
            {
                Pop(wait: true);
                return true;
            }

            var streamRes = Playback.NativeApi.PlaybackGetNextCapture(playbackHandle, out var captureHandle);
            if (streamRes == NativeCallResults.StreamResult.Eof)
            {
                ProcessQueueTail();
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
                        ProcessQueueTail();
                        return false;
                    }
                }

                TryEnqueueCapture(captureHandle, Timeout.Infinite);
                queueSize++;

                Pop(wait: false);
            }

            return true;
        }

        private void Pop(bool wait)
        {
            var timeout = wait ? Timeout.Infinite : Timeout.NoWait;
            using (var frameHandle = TryPopBodyFrame(timeout))
            {
                if (frameHandle == null)
                    return;

                queueSize--;
                totalFrameCount++;

                var bodyCount = (int)BodyTracking.NativeApi.FrameGetNumBodies(frameHandle).ToUInt32();
                if (bodyCount > 0)
                {
                    frameWithBodyCount++;
                }
            }
        }

        private void ProcessQueueTail()
        {
            while (QueueSize > 0)
            {
                Pop(wait: true);
            }
        }
    }
}