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
        /// </summary>
        /// <param name="filePath">File system path for the new recording. Not <see langword="null"/>, not empty.</param>
        /// <param name="device">The Azure Kinect device that is being recorded. The device is used to store device calibration and serial
        /// number information. May be <see langword="null"/> if recording user-generated data.</param>
        /// <param name="config">The configuration the Azure Kinect device was started with. Must be valid: see <see cref="Sensor.DeviceConfiguration.IsValid"/>.</param>
        /// <remarks><para>
        /// The file will be created if it doesn't exist, or overwritten if an existing file is specified.
        /// </para><para>
        /// Streaming does not need to be started on the device at the time this function is called, but when it is started
        /// it should be started with the same configuration provided in <paramref name="config"/>.
        /// </para><para>
        /// Subsequent calls to <see cref="WriteCapture"/> will need to have images in the resolution and format defined
        /// in <paramref name="config"/>.
        /// </para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is null or empty.</exception>
        /// <exception cref="ArgumentException"><paramref name="config"/> is invalid or <paramref name="filePath"/> contains some invalid character. Also, right now non-Latin letters are not supported in <paramref name="filePath"/> under Windows.</exception>
        /// <exception cref="RecordingException">Cannot initialize recording to <paramref name="filePath"/>.</exception>
        public Recorder(string filePath, Sensor.Device? device, Sensor.DeviceConfiguration config)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));
            if (filePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                throw new ArgumentException($"Path \"{filePath}\" contains invalid characters.", nameof(filePath));
            if (!config.IsValid(out var message))
                throw new ArgumentException(message, nameof(config));

            var pathAsBytes = Helpers.FilePathNameToBytes(filePath);

            var res = NativeApi.RecordCreate(pathAsBytes, Sensor.Device.ToHandle(device), config, out var handle);
            if (res != NativeCallResults.Result.Succeeded || !handle.IsValid)
                throw new RecordingException($"Cannot initialize recording to file \"{filePath}\".", filePath);

            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;

            FilePath = filePath;
            DeviceConfiguration = config;

            CustomTracks = new RecorderCustomTrackCollection(this);
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

        /// <summary>Gets a value indicating whether the object has been disposed of.</summary>
        /// <seealso cref="Dispose"/>
        public bool IsDisposed => handle.IsDisposed;

        /// <summary>Raised on object disposing (only once).</summary>
        /// <seealso cref="Dispose"/>
        public event EventHandler? Disposed;

        /// <summary>File system path to recording. Not <see langword="null"/>. Not empty.</summary>
        public string FilePath { get; }

        /// <summary>Device configuration for which this recorder has been created.</summary>
        public readonly Sensor.DeviceConfiguration DeviceConfiguration;

        /// <summary>Collection of custom tracks to be recorded to destination file. Not <see langword="null"/>.</summary>
        /// <remarks><para>
        /// Unlike <see cref="Playback.Tracks"/> this collection contains information only about custom tracks.
        /// </para><para>
        /// Use <see cref="RecorderCustomTrackCollection.AddVideoTrack(string, string, byte[], RecordVideoSettings)"/> and <see cref="RecorderCustomTrackCollection.AddCustomSubtitleTrack(string, string, byte[], RecordSubtitleSettings)"/>
        /// to add new custom tracks to recording.
        /// </para></remarks>
        /// <seealso cref="RecorderCustomTrack"/>
        /// <seealso cref="Playback.Tracks"/>
        public RecorderCustomTrackCollection CustomTracks { get; }

        /// <summary>Adds a tag to the recording.</summary>
        /// <param name="tagName">The name of the tag to write. Not <see langword="null"/>, not empty. Must be ALL CAPS and may only contain A-Z, 0-9, '-' and '_'.</param>
        /// <param name="tagValue">The string value to store in the tag. Not <see langword="null"/>. UTF-8 encoding is used to store.</param>
        /// <remarks><para>
        /// Tags are global to a file, and should store data related to the entire recording, such as camera configuration or
        /// recording location.
        /// </para><para>
        /// All tags need to be added before the recording header is written.
        /// </para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="tagName"/> is <see langword="null"/> or empty, or <paramref name="tagValue"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="tagName"/> contains invalid characters.</exception>
        /// <exception cref="InvalidOperationException"><see cref="AddTag"/> must be called before <see cref="WriteHeader"/>.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <seealso cref="Playback.TryGetTag"/>
        public void AddTag(string tagName, string tagValue)
        {
            Helpers.CheckTagName(tagName);
            if (tagValue is null)
                throw new ArgumentNullException(tagValue);

            var tagNameAsBytes = Helpers.StringToBytes(tagName, Encoding.ASCII);
            var tagValueAsBytes = Helpers.StringToBytes(tagValue, Encoding.UTF8);

            var res = NativeApi.RecordAddTag(handle.ValueNotDisposed, tagNameAsBytes, tagValueAsBytes);
            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException($"Cannot add tag \"{tagName}\" with value \"{tagValue}\". Method {nameof(AddTag)}() must be called before {nameof(WriteHeader)}().");
        }

        /// <summary>Adds the track header for recording IMU.</summary>
        /// <remarks>The track needs to be added before the recording header is written.</remarks>
        /// <exception cref="InvalidOperationException"><see cref="AddImuTrack"/> must be called before <see cref="WriteHeader"/>.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        public void AddImuTrack()
        {
            var res = NativeApi.RecordAddImuTrack(handle.ValueNotDisposed);
            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException($"{nameof(AddImuTrack)}() must be called before {nameof(WriteHeader)}().");
        }

        /// <summary>Adds an attachment to the recording.</summary>
        /// <param name="attachmentName">The name of the attachment to be stored in the recording file. This name should be a valid filename with an extension. Not <see langword="null"/>.</param>
        /// <param name="attachmentData">The attachment data to be added to recording. Not <see langword="null"/>.</param>
        /// <remarks>All attachments need to be added before the recording header is written.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="attachmentName"/> or <paramref name="attachmentData"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="attachmentName"/> is not a valid file name.</exception>
        /// <exception cref="InvalidOperationException"><see cref="AddAttachment(string, byte[])"/> must be called before <see cref="WriteHeader"/>.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <seealso cref="Playback.TryGetAttachment"/>
        public void AddAttachment(string attachmentName, byte[] attachmentData)
        {
            if (string.IsNullOrEmpty(attachmentName))
                throw new ArgumentNullException(nameof(attachmentName));
            if (attachmentName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                throw new ArgumentException($"Invalid value \"{attachmentName}\" of {nameof(attachmentName)}. This name should be a valid filename with an extension.", nameof(attachmentName));
            if (attachmentData is null)
                throw new ArgumentNullException(nameof(attachmentData));

            var attachmentNameAsBytes = Helpers.StringToBytes(attachmentName, Encoding.UTF8);
            var attachmentDataLength = Helpers.Int32ToUIntPtr(attachmentData.Length);

            var res = NativeApi.RecordAddAttachment(handle.ValueNotDisposed, attachmentNameAsBytes, attachmentData, attachmentDataLength);

            if (res != NativeCallResults.Result.Succeeded)
                throw new InvalidOperationException($"{nameof(AddAttachment)}() must be called before {nameof(WriteHeader)}().");
        }

        /// <summary>Writes the recording header and metadata to file.</summary>
        /// <remarks>This must be called before captures can be written.</remarks>
        /// <exception cref="RecordingException">Some error during recording to file. See logs for details.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        public void WriteHeader()
            => CheckResult(NativeApi.RecordWriteHeader(handle.ValueNotDisposed));

        /// <summary>
        /// Writes a camera capture to file.
        /// </summary>
        /// <remarks><para>
        /// This method will write all images in the capture to the corresponding tracks in the recording file.
        /// If any of the images fail to write, other images will still be written before throwing an exception.
        /// </para><para>
        /// Captures must be written in increasing order of timestamp, and the file's header must already be written.
        /// </para></remarks>
        /// <param name="capture">Capture to write to file. Not <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="capture"/> is <see langword="null"/>.</exception>
        /// <exception cref="RecordingException">Some error during recording to file. See logs for details.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        public void WriteCapture(Sensor.Capture capture)
        {
            if (capture is null)
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

        internal static NativeHandles.RecordHandle ToHandle(Recorder? recorder)
            => recorder?.handle.ValueNotDisposed ?? default;
    }
}
