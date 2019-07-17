using System.Threading;

namespace K4AdotNet.Samples.BodyTrackingSpeedTest
{
    internal sealed class EnqueueInBackgroundProcessor : Processor
    {
        private volatile bool processing;
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

        public override bool NextFrame()
        {
            while (processing || QueueSize > 0)
            {
                if (tracker.TryPopResult(out var frame, Timeout.FromMilliseconds(10)))
                {
                    using (frame)
                    {
                        processedFrameCount++;

                        if (frame.BodyCount > 0)
                            frameWithBodyCount++;

                        return true;
                    }
                }

                Thread.Sleep(1);
            }

            return false;
        }

        private void EnqueueLoop()
        {
            while (processing)
            {
                if (!playback.TryGetNextCapture(out var capture))
                    break;

                using (capture)
                {
                    if (!IsCaptureInInterval(capture))
                        break;

                    while (processing)
                    {
                        if (tracker.TryEnqueueCapture(capture, Timeout.FromMilliseconds(10)))
                            break;

                        Thread.Sleep(1);
                    }
                }
            }

            processing = false;
        }
    }
}