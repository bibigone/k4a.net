namespace K4AdotNet.Samples.BodyTrackingSpeedTest
{
    internal sealed class SingleThreadProcessor : Processor
    {
        private int totalFrameCount;
        private int frameWithBodyCount;

        public SingleThreadProcessor(ProcessingParameters processingParameters)
            : base(processingParameters)
        { }

        public override int TotalFrameCount => totalFrameCount;

        public override int FrameWithBodyCount => frameWithBodyCount;

        public override bool NextFrame()
        {
            if (QueueSize == BodyTracking.Tracker.MaxQueueSize)
            {
                Pop(wait: true);
                return true;
            }

            var res = playback.TryGetNextCapture(out var capture);
            using (capture)
            {
                if (!res || !IsCaptureInInterval(capture))
                {
                    ProcessQueueTail();
                    return false;
                }

                tracker.TryEnqueueCapture(capture, Timeout.Infinite);

                Pop(wait: false);
            }

            return true;
        }

        private void Pop(bool wait)
        {
            var timeout = wait ? Timeout.Infinite : Timeout.NoWait;
            if (!tracker.TryPopResult(out var frame, timeout))
                return;
            using (frame)
            {
                totalFrameCount++;

                if (frame.BodyCount > 0)
                    frameWithBodyCount++;
            }
        }

        private void ProcessQueueTail()
        {
            while (QueueSize > 0)
                Pop(wait: true);
        }
    }
}