using System;
using System.IO;
using System.Text;

namespace K4AdotNet.Record
{
    /// <summary>Kinect for Azure recording opened for writing.</summary>
    /// <seealso cref="Playback"/>
    public sealed class Recorder : IDisposablePlus
    {
        private readonly NativeHandles.HandleWrapper<NativeHandles.RecordHandle> handle;        // This class is an wrapper around this handle

        /// <summary>
        /// Opens a new recording file for writing.
        /// The file will be created if it doesn't exist, or overwritten if an existing file is specified.
        /// </summary>
        /// <param name="filePath">File system path for the new recording. Not <see langword="null"/>, not empty.</param>
        /// <param name="device">The Azure Kinect device that is being recorded. The device is used to store device calibration and serial
        /// number information. May be <see langword="null"/> if recording user-generated data.</param>
        /// <param name="config">The configuration the Azure Kinect device was started with. Must be valid: see <see cref="Sensor.DeviceConfiguration.IsValid"/>.</param>
        /// <remarks><para>
        /// Streaming does not need to be started on the device at the time this function is called, but when it is started
        /// it should be started with the same configuration provided in <paramref name="config"/>.
        /// </para><para>
        /// Subsequent calls to <see cref="WriteCapture"/> will need to have images in the resolution and format defined
        /// in <paramref name="config"/>.
        /// </para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is null or empty.</exception>
        /// <exception cref="ArgumentException"><paramref name="config"/> is invalid or <paramref name="filePath"/> contains some invalid character. Also, right now non-Latin letters are not supported in <paramref name="filePath"/> under Windows.</exception>
        public Recorder(string filePath, Sensor.Device device, Sensor.DeviceConfiguration config)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));
            if (filePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                throw new ArgumentException($"Path \"{filePath}\" contains invalid characters.", nameof(filePath));
            if (!config.IsValid(out var message))
                throw new ArgumentException(message, nameof(config));

            var pathAsBytes = Helpers.FilePathNameToBytes(filePath);

            var res = NativeApi.RecordCreate(pathAsBytes, Sensor.Device.ToHandle(device), config, out var handle);
            if (res != NativeCallResults.Result.Succeeded || handle == null || handle.IsInvalid)
                throw new RecordingException($"Cannot initialize recording to file \"{filePath}\".", filePath);

            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;

            FilePath = filePath;
        }

        private void Handle_Disposed(object sender, EventArgs e)
        {
            handle.Disposed -= Handle_Disposed;
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Call this method to flush all data to disk, close file and free all unmanaged resources associated with current instance.
        /// </summary>
        /// <seealso cref="Disposed"/>
        /// <seealso cref="IsDisposed"/>
        public void Dispose()
            => handle.Dispose();

        /// <summary>Gets a value indicating whether the image has been disposed of.</summary>
        /// <seealso cref="Dispose"/>
        public bool IsDisposed => handle.IsDisposed;

        /// <summary>Raised on object disposing (only once).</summary>
        /// <seealso cref="Dispose"/>
        public event EventHandler Disposed;

        /// <summary>File system path to recording. Not <see langword="null"/>. Not empty.</summary>
        public string FilePath { get; }

        /// <summary>Adds a tag to the recording. All tags need to be added before the recording header is written.</summary>
        /// <param name="name">The name of the tag to write. Not <see langword="null"/>, not empty. Can contain only ASCII characters.</param>
        /// <param name="value">The string value to store in the tag. Not <see langword="null"/>. UTF-8 encoding is used to store.</param>
        /// <remarks>
        /// Tags are global to a file, and should store data related to the entire recording, such as camera configuration or
        /// recording location.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/> or empty, or <paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> contains non-ASCII characters.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <seealso cref="Playback.TryGetTag"/>
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

        /// <summary>Adds the track header for recording IMU. The track needs to be added before the recording header is written.</summary>
        /// <exception cref="InvalidOperationException"><see cref="AddImuTrack"/> must be called before <see cref="WriteHeader"/>.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        public void AddImuTrack()
        {
            var res = NativeApi.RecordAddImuTrack(handle.ValueNotDisposed);
            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException($"{nameof(AddImuTrack)}() must be called before {nameof(WriteHeader)}().");
        }

        /// <summary>Writes the recording header and metadata to file. This must be called before captures can be written.</summary>
        /// <exception cref="RecordingException">Some error during recording to file. See logs for details.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        public void WriteHeader()
            => CheckResult(NativeApi.RecordWriteHeader(handle.ValueNotDisposed));

        /// <summary>
        /// Writes a camera capture to file.
        /// Captures must be written in increasing order of timestamp, and the file's header must already be written.
        /// </summary>
        /// <remarks>
        /// This method will write all images in the capture to the corresponding tracks in the recording file.
        /// If any of the images fail to write, other images will still be written before throwing an exception.
        /// </remarks>
        /// <param name="capture">Capture to write to file. Not <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="capture"/> is <see langword="null"/>.</exception>
        /// <exception cref="RecordingException">Some error during recording to file. See logs for details.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        public void WriteCapture(Sensor.Capture capture)
        {
            if (capture == null)
                throw new ArgumentNullException(nameof(capture));
            CheckResult(NativeApi.RecordWriteCapture(handle.ValueNotDisposed, Sensor.Capture.ToHandle(capture)));
        }

        /// <summary>Writes an IMU sample to file.</summary>
        /// <param name="imuSample">A structure containing the IMU sample data and time stamps.</param>
        /// <remarks>
        /// Samples must be written in increasing order of timestamp, and the file's header must already be written.
        /// When writing IMU samples at the same time as captures, the samples should be within 1 second of the most recently
        /// written capture.
        /// </remarks>
        /// <exception cref="RecordingException">Some error during recording to file. See logs for details.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        public void WriteImuSample(Sensor.ImuSample imuSample)
            => CheckResult(NativeApi.RecordWriteImuSample(handle.ValueNotDisposed, imuSample));

        /// <summary>Flushes all pending recording data to disk.</summary>
        /// <remarks><para>
        /// This method ensures that all data passed to the recording API prior to calling flush is written to disk.
        /// If continuing to write recording data, care must be taken to ensure no new time stamps are added from before the flush.
        /// <para></para>
        /// If an error occurs, best effort is made to flush as much data to disk as possible, but the integrity of the file is
        /// not guaranteed.
        /// </para></remarks>
        /// <exception cref="RecordingException">Some error during recording to file. See logs for details.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        public void Flush()
            => CheckResult(NativeApi.RecordFlush(handle.ValueNotDisposed));

        private void CheckResult(NativeCallResults.Result result)
        {
            if (result != NativeCallResults.Result.Succeeded)
                throw new RecordingException(FilePath);
        }
    }
}
