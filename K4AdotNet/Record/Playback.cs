using System;
using System.IO;
using System.Text;

namespace K4AdotNet.Record
{
    public sealed class Playback : IDisposablePlus
    {
        private readonly NativeHandles.HandleWrapper<NativeHandles.PlaybackHandle> handle;

        public Playback(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));
            if (filePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                throw new ArgumentException($"Path \"{filePath}\" contains invalid characters.", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Cannot find file \"{filePath}\".", filePath);

            var pathAsBytes = Helpers.FilePathNameToBytes(filePath);

            var res = NativeApi.PlaybackOpen(pathAsBytes, out var handle);
            if (res != NativeCallResults.Result.Succeeded || handle == null || handle.IsInvalid)
                throw new PlaybackException($"Cannot open file \"{filePath}\" for playback.");

            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;

            FilePath = filePath;
        }

        private void Handle_Disposed(object sender, EventArgs e)
        {
            handle.Disposed -= Handle_Disposed;
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
            => handle.Dispose();

        public bool IsDisposed => handle.IsDisposed;

        public event EventHandler Disposed;

        public string FilePath { get; }

        public Microseconds64 LastTimestamp => NativeApi.PlaybackGetLastTimestamp(handle.ValueNotDisposed);

        public override string ToString()
            => "Playback from " + FilePath;

        public byte[] GetRawCalibration()
        {
            if (!Helpers.TryGetValueInByteBuffer(NativeApi.PlaybackGetRawCalibration, handle.ValueNotDisposed, out var result))
                throw new PlaybackException(ExceptionMessage);
            return result;
        }

        public void GetCalibration(out Sensor.Calibration calibration)
            => CheckResult(NativeApi.PlaybackGetCalibration(handle.ValueNotDisposed, out calibration));

        public void GetRecordConfiguration(out RecordConfiguration config)
            => CheckResult(NativeApi.PlaybackGetRecordConfiguration(handle.ValueNotDisposed, out config));

        public bool TryGetTag(string name, out string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (!Helpers.IsAsciiCompatible(name))
                throw new ArgumentException("Tag name can contain only ASCII symbols.", nameof(name));

            var nameAsBytes = Helpers.StringToBytes(name, Encoding.ASCII);

            if (!Helpers.TryGetValueInByteBuffer(GetTag, nameAsBytes, out var valueAsBytes))
            {
                value = null;
                return false;
            }

            value = Encoding.UTF8.GetString(valueAsBytes, 0, valueAsBytes.Length - 1);
            return true;
        }

        private NativeCallResults.BufferResult GetTag(byte[] name, byte[] value, ref UIntPtr valueSize)
            => NativeApi.PlaybackGetTag(handle.ValueNotDisposed, name, value, ref valueSize);

        public void SetColorConversion(Sensor.ImageFormat format)
        {
            var res = NativeApi.PlaybackSetColorConversion(handle.ValueNotDisposed, format);
            if (res != NativeCallResults.Result.Succeeded)
                throw new NotSupportedException($"Format {format} is not supported for color conversion");
        }

        public bool TrySeekTimestamp(Microseconds64 offset, PlaybackSeekOrigin origin)
            => NativeApi.PlaybackSeekTimestamp(handle.ValueNotDisposed, offset, origin) == NativeCallResults.Result.Succeeded;

        public void SeekTimestamp(Microseconds64 offset, PlaybackSeekOrigin origin)
        {
            var res = TrySeekTimestamp(offset, origin);
            if (!res)
                throw new PlaybackException($"Cannot seek playback of \"{FilePath}\" to {offset} from {origin}.");
        }

        public bool TryGetNextCapture(out Sensor.Capture capture)
        {
            var res = NativeApi.PlaybackGetNextCapture(handle.ValueNotDisposed, out var captureHandle);
            if (res == NativeCallResults.StreamResult.Eof)
            {
                capture = null;
                return false;
            }
            if (res == NativeCallResults.StreamResult.Succeeded)
            {
                capture = Sensor.Capture.Create(captureHandle);
                return capture != null;
            }
            throw new PlaybackException(ExceptionMessage);
        }

        public bool TryGetPreviousCapture(out Sensor.Capture capture)
        {
            var res = NativeApi.PlaybackGetPreviousCapture(handle.ValueNotDisposed, out var captureHandle);
            if (res == NativeCallResults.StreamResult.Eof)
            {
                capture = null;
                return false;
            }
            if (res == NativeCallResults.StreamResult.Succeeded)
            {
                capture = Sensor.Capture.Create(captureHandle);
                return capture != null;
            }
            throw new PlaybackException(ExceptionMessage);
        }

        public bool TryGetNextImuSample(out Sensor.ImuSample imuSample)
        {
            var res = NativeApi.PlaybackGetNextImuSample(handle.ValueNotDisposed, out imuSample);
            if (res == NativeCallResults.StreamResult.Eof)
                return false;
            if (res == NativeCallResults.StreamResult.Succeeded)
                return true;
            throw new PlaybackException(ExceptionMessage);
        }

        public bool TryGetPreviousImuSample(out Sensor.ImuSample imuSample)
        {
            var res = NativeApi.PlaybackGetPreviousImuSample(handle.ValueNotDisposed, out imuSample);
            if (res == NativeCallResults.StreamResult.Eof)
                return false;
            if (res == NativeCallResults.StreamResult.Succeeded)
                return true;
            throw new PlaybackException(ExceptionMessage);
        }

        private void CheckResult(NativeCallResults.Result result)
        {
            if (result != NativeCallResults.Result.Succeeded)
                throw new PlaybackException(ExceptionMessage);
        }

        private string ExceptionMessage => $"Error during reading from file \"{FilePath}\".";
    }
}
