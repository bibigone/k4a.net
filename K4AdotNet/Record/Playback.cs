using System;
using System.IO;
using System.Text;

namespace K4AdotNet.Record
{
    public sealed class Playback : IDisposable
    {
        private readonly NativeHandles.PlaybackHandle handle;

        public Playback(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));
            if (filePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                throw new ArgumentException($"Path \"{filePath}\" contains invalid characters.", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Cannot find file \"{filePath}\".", filePath);

            var pathAsBytes = Helpers.FilePathNameToBytes(filePath);

            var res = NativeApi.PlaybackOpen(pathAsBytes, out handle);
            if (res != NativeCallResults.Result.Succeeded)
                throw new PlaybackException($"Cannot open file \"{filePath}\" for playback.");

            FilePath = filePath;
        }

        public void Dispose()
            => handle.Dispose();

        public string FilePath { get; }

        public Microseconds64 LastTimestamp => NativeApi.PlaybackGetLastTimestamp(handle);

        public override string ToString()
            => "Playback from " + FilePath;

        public byte[] GetRawCalibration()
        {
            if (!Helpers.TryGetValueInByteBuffer(NativeApi.PlaybackGetRawCalibration, handle, out var result))
                ThrowException();
            return result;
        }

        public void GetCalibration(out Sensor.Calibration calibration)
            => CheckResult(NativeApi.PlaybackGetCalibration(handle, out calibration));

        public void GetRecordConfiguration(out RecordConfiguration config)
            => CheckResult(NativeApi.PlaybackGetRecordConfiguration(handle, out config));

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
            => NativeApi.PlaybackGetTag(handle, name, value, ref valueSize);

        public void SetColorConversion(Sensor.ImageFormat format)
        {
            var res = NativeApi.PlaybackSetColorConversion(handle, format);
            if (res != NativeCallResults.Result.Succeeded)
                throw new NotSupportedException($"Format {format} is not supported for color conversion");
        }

        public bool TrySeekTimestamp(Microseconds64 offset, PlaybackSeekOrigin origin)
            => NativeApi.PlaybackSeekTimestamp(handle, offset, origin) == NativeCallResults.Result.Succeeded;

        public Sensor.Capture TryGetNextCapture()
        {
            var res = NativeApi.PlaybackGetNextCapture(handle, out var captureHandle);
            if (res == NativeCallResults.StreamResult.Eof)
                return null;
            if (res == NativeCallResults.StreamResult.Succeeded)
                return new Sensor.Capture(captureHandle);
            ThrowException();
            return null;
        }

        public Sensor.Capture TryGetPreviousCapture()
        {
            var res = NativeApi.PlaybackGetPreviousCapture(handle, out var captureHandle);
            if (res == NativeCallResults.StreamResult.Eof)
                return null;
            if (res == NativeCallResults.StreamResult.Succeeded)
                return new Sensor.Capture(captureHandle);
            ThrowException();
            return null;
        }

        public bool TryGetNextImuSample(out Sensor.ImuSample imuSample)
        {
            var res = NativeApi.PlaybackGetNextImuSample(handle, out imuSample);
            if (res == NativeCallResults.StreamResult.Eof)
                return false;
            if (res == NativeCallResults.StreamResult.Succeeded)
                return true;
            ThrowException();
            return false;
        }

        public bool TryGetPreviousImuSample(out Sensor.ImuSample imuSample)
        {
            var res = NativeApi.PlaybackGetPreviousImuSample(handle, out imuSample);
            if (res == NativeCallResults.StreamResult.Eof)
                return false;
            if (res == NativeCallResults.StreamResult.Succeeded)
                return true;
            ThrowException();
            return false;
        }

        private void CheckResult(NativeCallResults.Result result)
        {
            if (result != NativeCallResults.Result.Succeeded)
                ThrowException();
        }

        private void ThrowException()
            => throw new PlaybackException($"Error during reading from file \"{FilePath}\".");
    }
}
