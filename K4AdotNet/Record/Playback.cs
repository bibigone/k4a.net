using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace K4AdotNet.Record
{
    /// <summary>Kinect for Azure recording opened for playback.</summary>
    /// <seealso cref="Recorder"/>
    public sealed class Playback : IDisposablePlus
    {
        private readonly NativeHandles.HandleWrapper<NativeHandles.PlaybackHandle> handle;      // This class is an wrapper around this handle
        private readonly Lazy<PlaybackTrackCollection> tracks;

        /// <summary>Opens an existing recording file for reading.</summary>
        /// <param name="filePath">File system path of the existing recording. Not <see langword="null"/>. Not empty.</param>
        /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is null or empty.</exception>
        /// <exception cref="ArgumentException"><paramref name="filePath"/> contains some invalid character. Also, right now non-Latin letters are not supported in <paramref name="filePath"/> under Windows.</exception>
        /// <exception cref="FileNotFoundException">Files specified in <paramref name="filePath"/> does not exist.</exception>
        /// <exception cref="PlaybackException">Cannot open file specified in <paramref name="filePath"/> for playback. See logs for details.</exception>
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
                throw new PlaybackException($"Cannot open file \"{filePath}\" for playback.", filePath);

            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;

            FilePath = filePath;

            tracks = new Lazy<PlaybackTrackCollection>(() => new PlaybackTrackCollection(this), isThreadSafe: true);
        }

        private void Handle_Disposed(object sender, EventArgs e)
        {
            handle.Disposed -= Handle_Disposed;
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Call this method to close file and free all unmanaged resources associated with current instance.
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

        /// <summary>File system path to recording opened for playback. Not <see langword="null"/>. Not empty.</summary>
        public string FilePath { get; }

        /// <summary>Gets the length of the recording in microseconds (the difference between the first and last timestamp in the file).</summary>
        /// <remarks>
        /// The recording length may be longer than an individual track if, for example, the IMU continues to run after the last
        /// color image is recorded.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">This property cannot be asked for disposed object.</exception>
        /// <seealso cref="Sensor.Image.DeviceTimestamp"/>
        public Microseconds64 RecordLength => NativeApi.PlaybackGetRecordingLength(handle.ValueNotDisposed);

        /// <summary>Gets the last timestamp in a recording, relative to the start of the recording.</summary>
        /// <remarks>
        /// This function returns a file timestamp, not an absolute device timestamp, meaning it is relative to the start of the
        /// recording. This function is equivalent to the length of the recording.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">This property cannot be asked for disposed object.</exception>
        /// <seealso cref="Sensor.Image.DeviceTimestamp"/>
        [Obsolete("Deprecated starting version 1.2.0 of Sensor SDK. Please use RecordLength property")]
        public Microseconds64 LastTimestamp => RecordLength;

        /// <summary>Gets the list of tracks in the playback file. Not <see langword="null"/>.</summary>
        /// <remarks>
        /// This collection includes information about built-in and custom tracks.
        /// To check if track is built-in, use <see cref="PlaybackTrack.IsBuiltIn"/> property.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">This property cannot be asked for disposed object.</exception>
        /// <seealso cref="PlaybackTrack"/>
        /// <seealso cref="Recorder.CustomTracks"/>
        public PlaybackTrackCollection Tracks => tracks.Value;

        /// <summary>Convenient string representation of object.</summary>
        /// <returns><c>Playback from {FilePath}</c></returns>
        public override string ToString()
            => "Playback from " + FilePath;

        /// <summary>Get the raw calibration blob for the Azure Kinect device used during recording.</summary>
        /// <returns>Raw calibration data terminated by <c>0</c> byte. Not <see langword="null"/>.</returns>
        /// <remarks>The raw calibration may not exist if the device was not specified during recording.</remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="PlaybackException">Cannot read calibration data from recording. See logs for details.</exception>
        /// <seealso cref="Sensor.Device.GetRawCalibration"/>
        public byte[] GetRawCalibration()
        {
            if (!Helpers.TryGetValueInByteBuffer(NativeApi.PlaybackGetRawCalibration, handle.ValueNotDisposed, out var result))
                throw new PlaybackException(FilePath);
            return result;
        }

        /// <summary>Get the camera calibration for Azure Kinect device used during recording.</summary>
        /// <param name="calibration">Output: calibration data.</param>
        /// <remarks>The calibration may not exist if the device was not specified during recording.</remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="PlaybackException">Cannot read calibration data from recording. See logs for details.</exception>
        public void GetCalibration(out Sensor.Calibration calibration)
            => CheckResult(NativeApi.PlaybackGetCalibration(handle.ValueNotDisposed, out calibration));

        /// <summary>Get the device configuration used during recording.</summary>
        /// <param name="config">Output: recording configuration.</param>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="PlaybackException">Cannot read configuration from recording. See logs for details.</exception>
        public void GetRecordConfiguration(out RecordConfiguration config)
            => CheckResult(NativeApi.PlaybackGetRecordConfiguration(handle.ValueNotDisposed, out config));

        /// <summary>Reads the value of a tag from a recording.</summary>
        /// <param name="name">The name of the tag to read. Not <see langword="null"/> and not empty. Can contain only ASCII characters.</param>
        /// <param name="value">Output: tag value on success.</param>
        /// <returns>
        /// <see langword="true"/> tag value successfully read,
        /// <see langword="false"/> if tag value cannot be read (most likely, tag doesn't exist in recording).
        /// </returns>
        /// <remarks>
        /// Tags are global to a file, and should store data related to the entire recording, such as camera configuration or
        /// recording location.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> contains non-ASCII characters.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <seealso cref="Recorder.AddTag(string, string)"/>
        public bool TryGetTag(string name, [NotNullWhen(returnValue: true)] out string? value)
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

        private NativeCallResults.BufferResult GetTag(byte[] name, byte[]? value, ref UIntPtr valueSize)
            => NativeApi.PlaybackGetTag(handle.ValueNotDisposed, name, value, ref valueSize);

        /// <summary>Reads an attachment file from a recording.</summary>
        /// <param name="attachmentName">Attachment file name. Not <see langword="null"/>, not empty.</param>
        /// <param name="attachmentData">Output: attachment data.</param>
        /// <returns>
        /// <see langword="true"/> attachment successfully read,
        /// <see langword="false"/> if attachment cannot be read (most likely, attachment with specified name doesn't exist in recording).
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="attachmentName"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <seealso cref="Recorder.AddAttachment(string, byte[])"/>
        public bool TryGetAttachment(string attachmentName, [NotNullWhen(returnValue: true)] out byte[]? attachmentData)
        {
            if (string.IsNullOrEmpty(attachmentName))
                throw new ArgumentNullException(nameof(attachmentName));

            var attachmentNameAsBytes = Helpers.StringToBytes(attachmentName, Encoding.UTF8);
            return Helpers.TryGetValueInByteBuffer(GetAttachment, attachmentNameAsBytes, out attachmentData);
        }

        private NativeCallResults.BufferResult GetAttachment(byte[] attachmentName, byte[]? data, ref UIntPtr dataSize)
            => NativeApi.PlaybackGetAttachment(handle.ValueNotDisposed, attachmentName, data, ref dataSize);

        /// <summary>
        /// Sets the image format that color captures will be converted to. By default the conversion format will be the same as
        /// the image format stored in the recording file, and no conversion will occur.
        /// </summary>
        /// <param name="format">The target image format to be returned in captures.</param>
        /// <remarks><para>
        /// After the color conversion format is set, all <see cref="Sensor.Capture"/> objects returned from the playback handle will have
        /// their color images converted to the <paramref name="format"/>.
        /// </para><para>
        /// Color format conversion occurs in the user-thread, so setting <paramref name="format"/> to anything other than the format
        /// stored in the file may significantly increase the latency of <see cref="TryGetNextCapture(out Sensor.Capture)"/> and
        /// <see cref="TryGetPreviousCapture(out Sensor.Capture)"/>.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="InvalidOperationException">Format <paramref name="format"/> is not supported for color conversion.</exception>
        public void SetColorConversion(Sensor.ImageFormat format)
        {
            var res = NativeApi.PlaybackSetColorConversion(handle.ValueNotDisposed, format);
            if (res != NativeCallResults.Result.Succeeded)
                throw new NotSupportedException($"Format {format} is not supported for color conversion.");
        }

        /// <summary>
        /// Seeks to a specific timestamp within a recording.
        /// Like <see cref="SeekTimestamp(Microseconds64, PlaybackSeekOrigin)"/> but returns <see langword="false"/> in case of failure.
        /// </summary>
        /// <param name="offset">The timestamp offset to seek to relative to <paramref name="origin"/>.</param>
        /// <param name="origin">
        /// Specifies how the given timestamp should be interpreted. Seek can be done relative to the beginning or end of the
        /// recording, or using an absolute device timestamp.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the seek operation was successful, or <see langword="false"/>
        /// if an error occurred. The current seek position is left unchanged if <see langword="false"/> is returned.
        /// </returns>
        /// <remarks><para>
        /// The first device timestamp in a recording is usually non-zero. The recording file starts at the device timestamp
        /// defined by <see cref="RecordConfiguration.StartTimeOffset"/>,
        /// which is accessible via <see cref="GetRecordConfiguration(out RecordConfiguration)"/>.
        /// </para><para>
        /// The first call to <see cref="TryGetNextCapture(out Sensor.Capture)"/> after this method
        /// will return the first capture containing an image timestamp greater than or equal to the seek time.
        /// </para><para>
        /// The first call to <see cref="TryGetPreviousCapture(out Sensor.Capture)"/> after this method
        /// will return the firs capture with all image timestamps less than the seek time.
        /// </para><para>
        /// The first call to <see cref="TryGetNextImuSample(out Sensor.ImuSample)"/> after this method
        /// will return the first IMU sample with a timestamp greater than or equal to the seek time.
        /// </para><para>
        /// The first call to <see cref="TryGetPreviousImuSample(out Sensor.ImuSample)"/> after this method
        /// will return the first IMU sample with a timestamp less than the seek time.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <seealso cref="SeekTimestamp(Microseconds64, PlaybackSeekOrigin)"/>
        public bool TrySeekTimestamp(Microseconds64 offset, PlaybackSeekOrigin origin)
            => NativeApi.PlaybackSeekTimestamp(handle.ValueNotDisposed, offset, origin) == NativeCallResults.Result.Succeeded;

        /// <summary>
        /// Seeks to a specific timestamp within a recording.
        /// Like <see cref="TrySeekTimestamp(Microseconds64, PlaybackSeekOrigin)"/> but throws exception in case of failure.
        /// </summary>
        /// <param name="offset">The timestamp offset to seek to relative to <paramref name="origin"/>.</param>
        /// <param name="origin">
        /// Specifies how the given timestamp should be interpreted. Seek can be done relative to the beginning or end of the
        /// recording, or using an absolute device timestamp.
        /// </param>
        /// <remarks><para>
        /// The first device timestamp in a recording is usually non-zero. The recording file starts at the device timestamp
        /// defined by <see cref="RecordConfiguration.StartTimeOffset"/>,
        /// which is accessible via <see cref="GetRecordConfiguration(out RecordConfiguration)"/>.
        /// </para><para>
        /// The first call to <see cref="TryGetNextCapture(out Sensor.Capture)"/> after this method
        /// will return the first capture containing an image timestamp greater than or equal to the seek time.
        /// </para><para>
        /// The first call to <see cref="TryGetPreviousCapture(out Sensor.Capture)"/> after this method
        /// will return the firs capture with all image timestamps less than the seek time.
        /// </para><para>
        /// The first call to <see cref="TryGetNextImuSample(out Sensor.ImuSample)"/> after this method
        /// will return the first IMU sample with a timestamp greater than or equal to the seek time.
        /// </para><para>
        /// The first call to <see cref="TryGetPreviousImuSample(out Sensor.ImuSample)"/> after this method
        /// will return the first IMU sample with a timestamp less than the seek time.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="PlaybackException">Cannot seek playback to position specified.</exception>
        /// <seealso cref="TrySeekTimestamp(Microseconds64, PlaybackSeekOrigin)"/>
        public void SeekTimestamp(Microseconds64 offset, PlaybackSeekOrigin origin)
        {
            var res = TrySeekTimestamp(offset, origin);
            if (!res)
                throw new PlaybackException($"Cannot seek playback of \"{FilePath}\" to {offset} from {origin}.", FilePath);
        }

        /// <summary>Reads the next capture in the recording sequence.</summary>
        /// <param name="capture">If successful this contains capture data read from playback. Don't forget to dispose this object after usage.</param>
        /// <returns>
        /// <see langword="true"/> - if a capture is returned,
        /// <see langword="false"/> - if the end of the recording is reached.
        /// All other failures will throw <see cref="PlaybackException"/> exception.
        /// </returns>
        /// <remarks><para>
        /// This method always returns the next capture in sequence after the most recently returned capture.
        /// </para><para>
        /// The first call to this method after <see cref="SeekTimestamp"/> will return the capture
        /// in the recording closest to the seek time with an image timestamp greater than or equal to the seek time.
        /// </para><para>
        /// If a call was made to <see cref="TryGetPreviousCapture(out Sensor.Capture)"/> that returned <see langword="false"/> (which means EOF), the playback
        /// position is at the beginning of the stream and this method will return the first capture in the recording.
        /// </para><para>
        /// Capture objects returned by the playback API will always contain at least one image, but may have images missing if
        /// frames were dropped in the original recording. When calling <see cref="Sensor.Capture.ColorImage"/>,
        /// <see cref="Sensor.Capture.DepthImage"/>, or <see cref="Sensor.Capture.IRImage"/>,
        /// the image should be checked for <see langword="null"/>.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="PlaybackException">Error during reading from recording. See logs for details.</exception>
        public bool TryGetNextCapture([NotNullWhen(returnValue: true)] out Sensor.Capture? capture)
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
            throw new PlaybackException(FilePath);
        }

        /// <summary>Reads the previous capture in the recording sequence.</summary>
        /// <param name="capture">If successful this contains capture data read from playback. Don't forget to dispose this object after usage.</param>
        /// <returns>
        /// <see langword="true"/> - if a capture is returned,
        /// <see langword="false"/> - if the start of the recording is reached.
        /// All other failures will throw <see cref="PlaybackException"/> exception.
        /// </returns>
        /// <remarks><para>
        /// This method always returns the previous capture in sequence before the most recently returned capture.
        /// </para><para>
        /// The first call to this method after <see cref="SeekTimestamp"/> will return the capture
        /// in the recording closest to the seek time with all image time stamps less than the seek time.
        /// </para><para>
        /// If a call was made to <see cref="TryGetNextCapture(out Sensor.Capture)"/> that returned <see langword="false"/> (which means EOF), the playback
        /// position is at the end of the stream and this method will return the last capture in the recording.
        /// </para><para>
        /// Capture objects returned by the playback API will always contain at least one image, but may have images missing if
        /// frames were dropped in the original recording. When calling <see cref="Sensor.Capture.ColorImage"/>,
        /// <see cref="Sensor.Capture.DepthImage"/>, or <see cref="Sensor.Capture.IRImage"/>,
        /// the image should be checked for <see langword="null"/>.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="PlaybackException">Error during reading from recording. See logs for details.</exception>
        public bool TryGetPreviousCapture([NotNullWhen(returnValue: true)] out Sensor.Capture? capture)
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
            throw new PlaybackException(FilePath);
        }

        /// <summary>Reads the next IMU sample in the recording sequence.</summary>
        /// <param name="imuSample">If successful this contains IMU sample.</param>
        /// <returns>
        /// <see langword="true"/> - if a sample is returned,
        /// <see langword="false"/> - if the end of the recording is reached.
        /// All other failures will throw <see cref="PlaybackException"/> exception.
        /// </returns>
        /// <remarks><para>
        /// This method always returns the next IMU sample in sequence after the most recently returned sample.
        /// </para><para>
        /// The first call to this method after <see cref="SeekTimestamp"/> will return the IMU sample
        /// in the recording closest to the seek time with timestamp greater than or equal to the seek time.
        /// </para><para>
        /// If a call was made to <see cref="TryGetPreviousImuSample(out Sensor.ImuSample)"/> that returned <see langword="false"/> (which means EOF), the playback
        /// position is at the beginning of the stream and this method will return the first IMU sample in the recording.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="PlaybackException">Error during reading from recording. See logs for details.</exception>
        public bool TryGetNextImuSample(out Sensor.ImuSample imuSample)
        {
            var res = NativeApi.PlaybackGetNextImuSample(handle.ValueNotDisposed, out imuSample);
            if (res == NativeCallResults.StreamResult.Eof)
                return false;
            if (res == NativeCallResults.StreamResult.Succeeded)
                return true;
            throw new PlaybackException(FilePath);
        }

        /// <summary>Reads the previous IMU sample in the recording sequence.</summary>
        /// <param name="imuSample">If successful this contains IMU sample.</param>
        /// <returns>
        /// <see langword="true"/> - if a sample is returned,
        /// <see langword="false"/> - if the start of the recording is reached.
        /// All other failures will throw <see cref="PlaybackException"/> exception.
        /// </returns>
        /// <remarks><para>
        /// This method always returns the previous IMU sample in sequence before the most recently returned sample.
        /// </para><para>
        /// The first call to this method after <see cref="SeekTimestamp"/> will return the IMU sample
        /// in the recording closest to the seek time with timestamp less than the seek time.
        /// </para><para>
        /// If a call was made to <see cref="TryGetNextImuSample(out Sensor.ImuSample)"/> that returned <see langword="false"/> (which means EOF), the playback
        /// position is at the end of the stream and this method will return the last IMU sample in the recording.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed object.</exception>
        /// <exception cref="PlaybackException">Error during reading from recording. See logs for details.</exception>
        public bool TryGetPreviousImuSample(out Sensor.ImuSample imuSample)
        {
            var res = NativeApi.PlaybackGetPreviousImuSample(handle.ValueNotDisposed, out imuSample);
            if (res == NativeCallResults.StreamResult.Eof)
                return false;
            if (res == NativeCallResults.StreamResult.Succeeded)
                return true;
            throw new PlaybackException(FilePath);
        }

        private void CheckResult(NativeCallResults.Result result)
        {
            if (result != NativeCallResults.Result.Succeeded)
                throw new PlaybackException(FilePath);
        }

        internal static NativeHandles.PlaybackHandle ToHandle(Playback? playback)
            => playback?.handle.ValueNotDisposed ?? NativeHandles.PlaybackHandle.Zero;
    }
}
