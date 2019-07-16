using System;
using System.IO;
using System.Text;

namespace K4AdotNet.Record
{
    public sealed class Recorder : IDisposablePlus
    {
        private readonly NativeHandles.HandleWrapper<NativeHandles.RecordHandle> handle;

        public Recorder(string filePath, Sensor.Device device, Sensor.DeviceConfiguration config)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));
            if (filePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                throw new ArgumentException($"Path \"{filePath}\" contains invalid characters.", nameof(filePath));

            var pathAsBytes = Helpers.FilePathNameToBytes(filePath);

            var res = NativeApi.RecordCreate(pathAsBytes, Sensor.Device.ToHandle(device), config, out var handle);
            if (res != NativeCallResults.Result.Succeeded || handle == null || handle.IsInvalid)
                throw new RecordingException($"Cannot initialize recording to file \"{filePath}\".");

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

        public void AddTag(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(name);
            if (!Helpers.IsAsciiCompatible(name))
                throw new ArgumentException("Tag name can contain only ASCII characters.", nameof(name));
            if (value == null)
                throw new ArgumentNullException(value);

            var nameAsBytes = Helpers.StringToBytes(name, Encoding.ASCII);
            var valueAsBytes = Helpers.StringToBytes(value, Encoding.UTF8);

            var res = NativeApi.RecordAddTag(handle.ValueNotDisposed, nameAsBytes, valueAsBytes);
            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException($"Cannot add tag \"{name}\" with value \"{value}\". Method {nameof(AddTag)}() must be called before {nameof(WriteHeader)}().");
        }

        public void AddImuTrack()
        {
            var res = NativeApi.RecordAddImuTrack(handle.ValueNotDisposed);
            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException($"{nameof(AddImuTrack)}() must be called before {nameof(WriteHeader)}().");
        }

        public void WriteHeader()
            => CheckResult(NativeApi.RecordWriteHeader(handle.ValueNotDisposed));

        public void WriteCapture(Sensor.Capture capture)
        {
            if (capture == null)
                throw new ArgumentNullException(nameof(capture));
            CheckResult(NativeApi.RecordWriteCapture(handle.ValueNotDisposed, Sensor.Capture.ToHandle(capture)));
        }

        public void WriteImuSample(Sensor.ImuSample imuSample)
            => CheckResult(NativeApi.RecordWriteImuSample(handle.ValueNotDisposed, imuSample));

        public void Flush()
            => CheckResult(NativeApi.RecordFlush(handle.ValueNotDisposed));

        private void CheckResult(NativeCallResults.Result result)
        {
            if (result != NativeCallResults.Result.Succeeded)
                throw new RecordingException($"Cannot write to file \"{FilePath}\"");
        }
    }
}
