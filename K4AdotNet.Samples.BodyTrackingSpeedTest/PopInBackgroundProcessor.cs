using System.Threading;

namespace K4AdotNet.Samples.BodyTrackingSpeedTest
{
    internal sealed class PopInBackgroundProcessor : Processor
    {
        private volatile bool processing;
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

        public override bool NextFrame()
        {
            var res = playback.TryGetNextCapture(out var capture);
            using (capture)
            {
                if (!res || !IsCaptureInInterval(capture))
                {
                    WaitForProcessingOfQueueTail();
                    return false;
                }

                tracker.TryEnqueueCapture(capture, Timeout.Infinite);
            }

            return true;
        }

        private void WaitForProcessingOfQueueTail()
        {
            while (QueueSize > 0)
                Thread.Sleep(1);
        }

        private void ProcessingLoop()
        {
            while (processing)
            {
                if (tracker.TryPopResult(out var frame, Timeout.FromMilliseconds(10)))
                {
                    using (frame)
                    {
                        Interlocked.Increment(ref processedFrameCount);

                        if (frame.BodyCount > 0)
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