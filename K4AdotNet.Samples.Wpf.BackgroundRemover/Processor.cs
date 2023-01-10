using K4AdotNet.Sensor;
using System;
using System.Collections.Generic;
using System.Threading;

namespace K4AdotNet.Samples.Wpf.BackgroundRemover
{
    class Processor : IDisposable
    {
        private readonly object sync = new object();
        private readonly int frameWidth;
        private readonly int frameHeight;
        private readonly Queue<FrameData> queue = new Queue<FrameData>();
        private readonly ManualResetEvent enqueued = new ManualResetEvent(false);
        private readonly Image image;
        private Thread? thread;
        private CancellationTokenSource? cancellation;

        public Processor(int frameWidth, int frameHeight)
        {
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;

            image = new(ImageFormat.ColorBgra32, frameWidth, frameHeight);
        }

        public void Dispose()
        {
            Stop();
            enqueued.Dispose();
            image.Dispose();
        }


        public ushort DepthLimitMillimeters
        {
            get => _depthLimitMillimeters;
            set => _depthLimitMillimeters = value;
        }
        private volatile ushort _depthLimitMillimeters;

        public byte BackgroundOpacity
        {
            get => _backgroundOpacity;
            set => _backgroundOpacity = value;
        }
        private volatile byte _backgroundOpacity;

        public bool UnknownDepthIsBackground
        {
            get => _unknownDepthIsBackground;
            set => _unknownDepthIsBackground = value;
        }
        private volatile bool _unknownDepthIsBackground;


        public event EventHandler? ImageUpdated;


        public void Start()
        {
            lock (sync)
            {
                if (thread != null)
                    throw new InvalidOperationException("Already started");

                cancellation = new CancellationTokenSource();

                thread = new Thread(Process)
                {
                    IsBackground = true,
                    Name = nameof(Processor)
                };
                thread.Start();
            }
        }

        public void Stop()
        {
            lock (sync)
            {
                if (thread != null)
                {
                    cancellation!.Cancel();
                    try
                    {
                        thread.Join();
                    }
                    catch (ThreadStateException) { }
                    catch (ThreadInterruptedException) { }

                    lock (queue)
                    {
                        ClearQueue();
                    }

                    cancellation.Dispose();
                    thread = null;
                }
            }
        }

        private void Process()
        {
            while (!cancellation!.IsCancellationRequested)
            {
                WaitHandle.WaitAny(new[] { enqueued, cancellation.Token.WaitHandle });

                while (!cancellation.IsCancellationRequested)
                {
                    FrameData frameData;
                    lock (queue)
                    {
                        if (queue.Count == 0)
                            break;

                        frameData = queue.Dequeue();
                    }

                    using (frameData)
                    {
                        Process(frameData);
                    }

                    ImageUpdated?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private unsafe void Process(FrameData frameData)
        {
            var depthLimit = DepthLimitMillimeters;
            var backgroundOpacity = BackgroundOpacity;
            var unknownDepthIsBackground = UnknownDepthIsBackground;

            var colorPtr = (byte*)frameData.ColorFrame.Buffer + 3;
            var depthPtr = (ushort*)frameData.DepthFrame.Buffer;
            for (var count = frameWidth * frameHeight; count > 0; count--)
            {
                var depth = *depthPtr;
                var isBackground = depth > depthLimit ? true
                    : depth == 0 ? unknownDepthIsBackground
                    : false;
                if (isBackground)
                {
                    *colorPtr = backgroundOpacity;
                }
                colorPtr += 4;
                depthPtr++;
            }

            lock (image)
            {
                image.DeviceTimestamp = frameData.ColorFrame.DeviceTimestamp;
                image.Exposure = frameData.ColorFrame.Exposure;
                image.IsoSpeed = frameData.ColorFrame.IsoSpeed;
                image.WhiteBalance = frameData.ColorFrame.WhiteBalance;
                Buffer.MemoryCopy((void*)frameData.ColorFrame.Buffer, (void*)image.Buffer, image.SizeBytes, frameData.ColorFrame.SizeBytes);
            }
        }

        /// <summary>
        /// Enqueues frame data for processing.
        /// </summary>
        /// <remarks>
        /// Color frame should be in Bgra32 format and match dimensions specified when constructing this processor.
        /// Depth frame should be in Depth16 format and mapped into color space. That is, having the same dimensions as the color frame.
        /// </remarks>
        public void Enqueue(FrameData frameData)
        {
            if (frameData.ColorFrame.WidthPixels != frameWidth || frameData.ColorFrame.HeightPixels != frameHeight)
                throw new ArgumentOutOfRangeException(nameof(frameData), "Wrong dimensions");

            if (frameData.ColorFrame.Format != ImageFormat.ColorBgra32)
                throw new ArgumentOutOfRangeException(nameof(frameData), "Wrong format");

            if (frameData.DepthFrame.WidthPixels != frameWidth || frameData.DepthFrame.HeightPixels != frameHeight)
                throw new ArgumentOutOfRangeException(nameof(frameData), "Wrong dimensions");

            if (frameData.DepthFrame.Format != ImageFormat.Depth16)
                throw new ArgumentOutOfRangeException(nameof(frameData), "Wrong format");

            lock (queue)
            {
                ClearQueue();
                queue.Enqueue(frameData);
                enqueued.Set();
            }
        }

        public void ReadImage(Action<Image> reader)
        {
            lock (image)
            {
                reader.Invoke(image);
            }
        }

        private void ClearQueue()
        {
            while (queue.Count > 0)
            {
                queue.Dequeue().Dispose();
            }
        }
    }
}
