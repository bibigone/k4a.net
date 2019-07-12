using System;

namespace K4AdotNet.Samples.BodyTrackingSpeedTest
{
    internal abstract class Processor : IDisposable
    {
        public static Processor Create(ProcessingParameters processingParameters)
        {
            switch (processingParameters.Implementation)
            {
                case ProcessingImplementation.SingleThread: return new SingleThreadProcessor(processingParameters);
                case ProcessingImplementation.PopInBackground: return new PopInBackgroundProcessor(processingParameters);
                case ProcessingImplementation.EnqueueInBackground: return new EnqueueInBackgroundProcessor(processingParameters);
                default: throw new NotSupportedException();
            }
        }

        protected readonly ProcessingParameters processingParameters;
        protected readonly NativeHandles.PlaybackHandle playbackHandle;
        protected readonly Playback.RecordConfiguration recordConfig;
        protected readonly Sensor.Calibration calibration;
        protected readonly NativeHandles.TrackerHandle trackerHandle;

        protected Processor(ProcessingParameters processingParameters)
        {
            this.processingParameters = processingParameters;
            playbackHandle = OpenRecord();
            GetRecordConfig(out recordConfig);
            RecordLength = GetRecordLength();
            GetCalibration(out calibration);
            if (processingParameters.StartTime.HasValue)
                Seek(processingParameters.StartTime.Value);
            trackerHandle = CreateTracker(ref calibration);
        }

        public virtual void Dispose()
        {
            trackerHandle?.Dispose();
            playbackHandle?.Dispose();
        }

        public Playback.RecordConfiguration RecordConfig => recordConfig;

        public TimeSpan RecordLength { get; }

        public abstract int TotalFrameCount { get; }

        public abstract int FrameWithBodyCount { get; }

        public abstract int QueueSize { get; }

        public abstract bool NextFrame();

        private NativeHandles.PlaybackHandle OpenRecord()
        {
            var res = Playback.NativeApi.PlaybackOpen(processingParameters.MkvPath, out var playbackHandle);
            if (res != NativeCallResults.Result.Succeeded)
                throw new ApplicationException($"Cannot open \"{processingParameters.MkvPath}\" file for playback.");
            return playbackHandle;
        }

        private void GetCalibration(out Sensor.Calibration calibration)
        {
            var res = Playback.NativeApi.PlaybackGetCalibration(playbackHandle, out calibration);
            if (res != NativeCallResults.Result.Succeeded)
                throw new ApplicationException("Cannot read calibration information from recording");
        }

        private void GetRecordConfig(out Playback.RecordConfiguration config)
        {
            var res = Playback.NativeApi.PlaybackGetRecordConfiguration(playbackHandle, out config);
            if (res != NativeCallResults.Result.Succeeded)
                throw new ApplicationException("Cannot read configuration information from recording");
        }

        private TimeSpan GetRecordLength()
        {
            var res = Playback.NativeApi.PlaybackGetLastTimestamp(playbackHandle);
            if (res == Timestamp.Zero)
                throw new ApplicationException("Cannot get length of recording");
            return res;
        }

        private NativeHandles.TrackerHandle CreateTracker(ref Sensor.Calibration calibration)
        {
            var res = BodyTracking.NativeApi.TrackerCreate(ref calibration, out var trackerHandle);
            if (res != NativeCallResults.Result.Succeeded)
                throw new ApplicationException("Cannot create body tracker");
            return trackerHandle;
        }

        private void Seek(TimeSpan value)
        {
            var res = Playback.NativeApi.PlaybackSeekTimestamp(playbackHandle, value, Playback.PlaybackSeekOrigin.Begin);
            if (res != NativeCallResults.Result.Succeeded)
                throw new ApplicationException("Cannot seek playback to " + value);
        }

        protected static Timestamp? GetTimestamp(NativeHandles.CaptureHandle captureHandle)
        {
            var imageHandle = Sensor.NativeApi.CaptureGetDepthImage(captureHandle);
            if (imageHandle != null && imageHandle.IsInvalid)
            {
                using (imageHandle)
                {
                    return Sensor.NativeApi.ImageGetTimestamp(imageHandle);
                }
            }
            return null;
        }

        protected bool TryEnqueueCapture(NativeHandles.CaptureHandle captureHandle, Timeout timeout)
        {
            var waitResult = BodyTracking.NativeApi.TrackerEnqueueCapture(trackerHandle, captureHandle, timeout);
            if (waitResult == NativeCallResults.WaitResult.Succeeded)
                return true;
            if (waitResult == NativeCallResults.WaitResult.Timeout)
                return false;
            throw new ApplicationException("Cannot enqueue capture to body tracking pipeline.");
        }

        protected NativeHandles.BodyFrameHandle TryPopBodyFrame(Timeout timeout)
        {
            var waitResult = BodyTracking.NativeApi.TrackerPopResult(trackerHandle, out var frameHandle, timeout);
            if (waitResult == NativeCallResults.WaitResult.Succeeded)
                return frameHandle;
            if (waitResult == NativeCallResults.WaitResult.Failed)
                throw new ApplicationException("Cannot get next body tracking data from pipeline.");
            return null;
        }

    }
}