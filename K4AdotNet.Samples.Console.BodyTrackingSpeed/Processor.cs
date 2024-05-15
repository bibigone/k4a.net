using System;

namespace K4AdotNet.Samples.Console.BodyTrackingSpeed
{
    internal abstract class Processor : IDisposable
    {
        public static Processor Create(ProcessingParameters processingParameters) => processingParameters.Implementation switch
        {
            ProcessingImplementation.SingleThread => new SingleThreadProcessor(processingParameters),
            ProcessingImplementation.PopInBackground => new PopInBackgroundProcessor(processingParameters),
            ProcessingImplementation.EnqueueInBackground => new EnqueueInBackgroundProcessor(processingParameters),
            _ => throw new NotSupportedException(),
        };

        protected readonly ProcessingParameters processingParameters;
        protected readonly Record.Playback playback;
        protected readonly Record.RecordConfiguration recordConfig;
        protected readonly Sensor.Calibration calibration;
        protected readonly BodyTracking.Tracker tracker;

        protected Processor(ProcessingParameters processingParameters)
        {
            this.processingParameters = processingParameters;
            playback = new(processingParameters.MkvPath!);
            playback.GetRecordConfiguration(out recordConfig);
            RecordLength = playback.RecordLength;
            calibration = playback.GetCalibration();
            if (processingParameters.StartTime.HasValue)
                Seek(processingParameters.StartTime.Value);
            var config = BodyTracking.TrackerConfiguration.Default;
            config.ProcessingMode = processingParameters.ProcessingMode;
            config.ModelPath = GetModelPath(processingParameters.DnnModel);
            tracker = new(in calibration.Data, config);
        }

        private static string GetModelPath(DnnModel dnnModel)
            => dnnModel switch
            {
                DnnModel.Default => Sdk.BODY_TRACKING_DNN_MODEL_FILE_NAME,
                DnnModel.Lite => Sdk.BODY_TRACKING_DNN_MODEL_LITE_FILE_NAME,
                _ => throw new NotSupportedException(),
            };

        public virtual void Dispose()
        {
            tracker.Dispose();
            playback.Dispose();
        }

        public Record.RecordConfiguration RecordConfig => recordConfig;

        public TimeSpan RecordLength { get; }

        public abstract int TotalFrameCount { get; }

        public abstract int FrameWithBodyCount { get; }

        public int QueueSize => tracker.QueueSize;

        public abstract bool NextFrame();

        private void Seek(TimeSpan value)
        {
            if (!playback.TrySeekTimestamp(value, Record.PlaybackSeekOrigin.Begin))
                throw new ApplicationException("Cannot seek playback to " + value);
        }

        protected bool IsCaptureInInterval(Sensor.Capture? capture)
        {
            if (capture == null)
                return false;
            if (!processingParameters.EndTime.HasValue)
                return true;
            var deviceTimestamp = GetDeviceTimestamp(capture);
            if (deviceTimestamp.HasValue)
                deviceTimestamp = deviceTimestamp.Value - RecordConfig.StartTimeOffset;
            return deviceTimestamp.HasValue
                && processingParameters.IsTimeInStartEndInterval(deviceTimestamp.Value);
        }

        private static Microseconds64? GetDeviceTimestamp(Sensor.Capture capture)
        {
            using (var image = capture.DepthImage)
            {
                return image?.DeviceTimestamp;
            }
        }
    }
}