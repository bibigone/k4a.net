using K4AdotNet.Record;
using K4AdotNet.Sensor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace K4AdotNet.Tests.Record
{
    [TestClass]
    public class RecordThenPlaybackTests
    {
        [TestMethod]
        public void TestBasicScenario()
        {
            var mkvPath = GenerateTempMkvFilePath();

            var config = new DeviceConfiguration
            {
                CameraFps = FrameRate.Thirty,
                ColorFormat = ImageFormat.ColorNV12,
                ColorResolution = ColorResolution.R720p,
                DepthMode = DepthMode.NarrowView2x2Binned,
                SubordinateDelayOffMaster = Microseconds32.FromSeconds(0.00001),
                WiredSyncMode = WiredSyncMode.Subordinate,
                DepthDelayOffColor = Microseconds32.FromMilliseconds(-0.005),
            };

            var deviceTimestamp0 = Microseconds64.FromMilliseconds(1.0);
            var deviceTimestamp1 = Microseconds64.FromMilliseconds(4.4);
            var deviceTimestamp2 = Microseconds64.FromMilliseconds(7.7);

            using (var recorder = new Recorder(mkvPath, null, config))
            {
                Assert.IsFalse(recorder.IsDisposed);
                Assert.AreEqual(mkvPath, recorder.FilePath);
                Assert.AreEqual(0, recorder.CustomTracks.Count);

                recorder.WriteHeader();

                WriteTestCaptures(recorder, deviceTimestamp0, deviceTimestamp1, deviceTimestamp2);
            }

            using (var playback = new Playback(mkvPath))
            {
                Assert.IsFalse(playback.IsDisposed);
                Assert.AreEqual(mkvPath, playback.FilePath);

                var trackCount = GetBuiltInTrackCount(config);
                Assert.AreEqual(trackCount, playback.Tracks.Count);
                Assert.AreEqual(trackCount, playback.Tracks.Count(t => t.IsBuiltIn));
                Assert.AreEqual(0, playback.Tracks.Count(t => !t.IsBuiltIn));

                playback.GetRecordConfiguration(out var recordConfig);
                Assert.AreEqual(config.CameraFps, recordConfig.CameraFps);
                Assert.AreEqual(config.ColorFormat, recordConfig.ColorFormat);
                Assert.AreEqual(config.ColorResolution, recordConfig.ColorResolution);
                Assert.AreEqual(config.ColorResolution != ColorResolution.Off, recordConfig.ColorTrackEnabled);
                Assert.AreEqual(config.DepthDelayOffColor, recordConfig.DepthDelayOffColor);
                Assert.AreEqual(config.DepthMode, recordConfig.DepthMode);
                Assert.AreEqual(config.DepthMode.HasDepth(), recordConfig.DepthTrackEnabled);
                Assert.IsFalse(recordConfig.ImuTrackEnabled);
                Assert.AreEqual(config.DepthMode.HasPassiveIR(), recordConfig.IRTrackEnabled);
                var startTimestamp = GetStartTimestamp(config, deviceTimestamp0);
                Assert.AreEqual(startTimestamp.ValueUsec, recordConfig.StartTimeOffset.ValueUsec);
                Assert.AreEqual(config.SubordinateDelayOffMaster, recordConfig.SubordinateDelayOffMaster);
                Assert.AreEqual(config.WiredSyncMode, recordConfig.WiredSyncMode);

                var length = playback.RecordLength;
                var endTimestamp = GetEndTimestamp(config, deviceTimestamp2);
                Assert.AreEqual(endTimestamp.ValueUsec - startTimestamp.ValueUsec, length.ValueUsec);

                // Try play backward from start
                Assert.IsFalse(playback.TryGetPreviousCapture(out var capture));
                Assert.IsNull(capture);

                // Play forward
                Assert.IsTrue(playback.TryGetNextCapture(out capture));
                Assert.IsNotNull(capture);
                CheckCapture(capture, config, deviceTimestamp0);
                capture.Dispose();

                Assert.IsTrue(playback.TryGetNextCapture(out capture));
                Assert.IsNotNull(capture);
                CheckCapture(capture, config, deviceTimestamp1);
                capture.Dispose();

                Assert.IsTrue(playback.TryGetNextCapture(out capture));
                Assert.IsNotNull(capture);
                CheckCapture(capture, config, deviceTimestamp2);
                capture.Dispose();

                // EoF
                Assert.IsFalse(playback.TryGetNextCapture(out capture));
                Assert.IsNull(capture);

                // Play backward
                Assert.IsTrue(playback.TryGetPreviousCapture(out capture));
                Assert.IsNotNull(capture);
                CheckCapture(capture, config, deviceTimestamp2);
                capture.Dispose();

                Assert.IsTrue(playback.TryGetPreviousCapture(out capture));
                Assert.IsNotNull(capture);
                CheckCapture(capture, config, deviceTimestamp1);
                capture.Dispose();

                Assert.IsTrue(playback.TryGetPreviousCapture(out capture));
                Assert.IsNotNull(capture);
                CheckCapture(capture, config, deviceTimestamp0);
                capture.Dispose();

                // EoF
                Assert.IsFalse(playback.TryGetPreviousCapture(out capture));
                Assert.IsNull(capture);

                // Seek to end
                Assert.IsTrue(playback.TrySeekTimestamp(Microseconds64.Zero, PlaybackSeekOrigin.End));
                Assert.IsFalse(playback.TryGetNextCapture(out capture));
                Assert.IsNull(capture);

                Assert.IsTrue(playback.TryGetPreviousCapture(out capture));
                Assert.IsNotNull(capture);
                CheckCapture(capture, config, deviceTimestamp2);
                capture.Dispose();

                // Seek to start
                Assert.IsTrue(playback.TrySeekTimestamp(Microseconds64.Zero, PlaybackSeekOrigin.Begin));
                Assert.IsFalse(playback.TryGetPreviousCapture(out capture));
                Assert.IsNull(capture);

                Assert.IsTrue(playback.TryGetNextCapture(out capture));
                Assert.IsNotNull(capture);
                CheckCapture(capture, config, deviceTimestamp0);
                capture.Dispose();

                // Seek to device timestamp
                var ts = GetStartTimestamp(config, deviceTimestamp1);
                Assert.IsTrue(playback.TrySeekTimestamp(ts, PlaybackSeekOrigin.DeviceTime));

                Assert.IsTrue(playback.TryGetNextCapture(out capture));
                Assert.IsNotNull(capture);
                CheckCapture(capture, config, deviceTimestamp1);
                capture.Dispose();
            }

            File.Delete(mkvPath);
        }

        [TestMethod]
        public void TestAttachments()
        {
            var mkvPath = GenerateTempMkvFilePath();

            var config = new DeviceConfiguration
            {
                CameraFps = FrameRate.Fifteen,
                ColorFormat = ImageFormat.ColorNV12,
                ColorResolution = ColorResolution.R720p,
                DepthMode = DepthMode.NarrowView2x2Binned,
            };

            var deviceTimestamp0 = Microseconds64.FromMilliseconds(1.0);
            var deviceTimestamp1 = Microseconds64.FromMilliseconds(7.7);

            var attachment1Name = "\ud182\ud0b5\ud181\ud182 \ud0ba\ud0be\ud0b4\ud0b8\ud180\ud0be\ud0b2\ud0ba\ud0b8";
            var attachment1Data = new byte[] { 1, 255, 0, 8, 7, 254, 128, 3, 127, 65, 179 };

            var attachment2Name = "another_attachment.file";
            var attachment2Data = new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250 };

            using (var recorder = new Recorder(mkvPath, null, config))
            {
                recorder.AddAttachment(attachment1Name, attachment1Data);
                recorder.AddAttachment(attachment2Name, attachment2Data);

                recorder.WriteHeader();

                WriteTestCaptures(recorder, deviceTimestamp0, deviceTimestamp1);
            }

            using (var playback = new Playback(mkvPath))
            {
                Assert.IsTrue(playback.TryGetAttachment(attachment1Name, out var data1));
                AssertAreEqual(attachment1Data, data1);

                Assert.IsTrue(playback.TryGetAttachment(attachment2Name, out var data2));
                AssertAreEqual(attachment2Data, data2);

                Assert.IsFalse(playback.TryGetAttachment("some_unknown_attachment_name", out var tmp));
                Assert.IsNull(tmp);
            }

            File.Delete(mkvPath);
        }

        [TestMethod]
        public void TestCustomVideoTracks()
        {
            var mkvPath = GenerateTempMkvFilePath();

            var config = new DeviceConfiguration
            {
                CameraFps = FrameRate.Five,
                ColorFormat = ImageFormat.ColorNV12,
                ColorResolution = ColorResolution.R720p,
                DepthMode = DepthMode.NarrowView2x2Binned,
            };

            var deviceTimestamp0 = Microseconds64.FromMilliseconds(0);
            var deviceTimestamp1 = Microseconds64.FromMilliseconds(20);

            var track1Name = "TEST_TRACK_NAME";
            var track1CodecId = "V_CUSTOM_VIDEO_CODEC";
            var track1CodecContext = new byte[] { 255, 254, 253, 0, 1, 2, 3, 4, 8, 16, 32, 64, 128 };
            var track1Settings = new RecordVideoSettings { Width = 2, Height = 4, FrameRate = 5 };

            var track2Name = "ANOTHER_CUSTOM_TRACK";
            var track2CodecId = "V_XYZ_CODEC";
            var track2CodecContext = new byte[] { 20, 19, 8, 27, 22, 29, 59 };
            var track2Settings = new RecordVideoSettings { Width = 20, Height = 40, FrameRate = 4 };

            using (var recorder = new Recorder(mkvPath, null, config))
            {
                var track1 = recorder.CustomTracks.AddVideoTrack(track1Name, track1CodecId, track1CodecContext, track1Settings);
                Assert.AreEqual(track1Name, track1.Name);
                Assert.AreEqual(track1CodecId, track1.CodecId);
                Assert.AreSame(track1CodecContext, track1.CodecContext);

                var track2 = recorder.CustomTracks.AddVideoTrack(track2Name, track2CodecId, track2CodecContext, track2Settings);
                Assert.AreEqual(track2Name, track2.Name);
                Assert.AreEqual(track2CodecId, track2.CodecId);
                Assert.AreSame(track2CodecContext, track2.CodecContext);

                recorder.WriteHeader();

                WriteTestCaptures(recorder, deviceTimestamp0, deviceTimestamp1);

                track1.WriteData(deviceTimestamp0, new byte[] { 1, 0 });
                track1.WriteData(deviceTimestamp1, new byte[] { 1, 1 });

                track2.WriteData(deviceTimestamp0, new byte[] { 2, 0 });
                track2.WriteData(deviceTimestamp1, new byte[] { 2, 1 });
            }

            using (var playback = new Playback(mkvPath))
            {
                var builtInTrackCount = GetBuiltInTrackCount(config);
                var trackCount = builtInTrackCount + 2;
                Assert.AreEqual(trackCount, playback.Tracks.Count);
                Assert.AreEqual(builtInTrackCount, playback.Tracks.Count(t => t.IsBuiltIn));
                Assert.AreEqual(2, playback.Tracks.Count(t => !t.IsBuiltIn));

                Assert.IsTrue(playback.Tracks.Exists(track1Name));
                Assert.IsTrue(playback.Tracks.Exists(track2Name));
                Assert.IsFalse(playback.Tracks.Exists("SOME_UNKNOWN_TRACK_NAME"));

                var track1 = playback.Tracks[track1Name];
                Assert.IsNotNull(track1);
                Assert.AreEqual(track1Name, track1.Name);
                Assert.AreEqual(track1CodecId, track1.CodecId);
                AssertAreEqual(track1CodecContext, track1.CodecContext);
                Assert.AreEqual(track1Settings, track1.VideoSettings);
                Assert.IsFalse(track1.IsBuiltIn);

                var track2 = playback.Tracks[track2Name];
                Assert.IsNotNull(track2);
                Assert.AreEqual(track2Name, track2.Name);
                Assert.AreEqual(track2CodecId, track2.CodecId);
                AssertAreEqual(track2CodecContext, track2.CodecContext);
                Assert.AreEqual(track2Settings, track2.VideoSettings);
                Assert.IsFalse(track2.IsBuiltIn);

                Assert.IsNull(playback.Tracks["SOME_UNKNOWN_TRACK_NAME"]);

                // Reading of track 1

                Assert.IsFalse(track1.TryGetPreviousDataBlock(out var dataBlock));
                Assert.IsNull(dataBlock);

                // forward
                Assert.IsTrue(track1.TryGetNextDataBlock(out dataBlock));
                Assert.IsNotNull(dataBlock);
                Assert.AreEqual(deviceTimestamp0, dataBlock.DeviceTimestamp);
                Assert.AreEqual(2, dataBlock.SizeBytes);
                Assert.AreEqual(1, Marshal.ReadByte(dataBlock.Buffer, 0));
                Assert.AreEqual(0, Marshal.ReadByte(dataBlock.Buffer, 1));
                dataBlock.Dispose();

                Assert.IsTrue(track1.TryGetNextDataBlock(out dataBlock));
                Assert.IsNotNull(dataBlock);
                Assert.AreEqual(deviceTimestamp1, dataBlock.DeviceTimestamp);
                Assert.AreEqual(2, dataBlock.SizeBytes);
                Assert.AreEqual(1, Marshal.ReadByte(dataBlock.Buffer, 0));
                Assert.AreEqual(1, Marshal.ReadByte(dataBlock.Buffer, 1));
                dataBlock.Dispose();

                // eof
                Assert.IsFalse(track1.TryGetNextDataBlock(out dataBlock));
                Assert.IsNull(dataBlock);

                // backward
                Assert.IsTrue(track1.TryGetPreviousDataBlock(out dataBlock));
                Assert.IsNotNull(dataBlock);
                Assert.AreEqual(deviceTimestamp1, dataBlock.DeviceTimestamp);
                Assert.AreEqual(2, dataBlock.SizeBytes);
                Assert.AreEqual(1, Marshal.ReadByte(dataBlock.Buffer, 0));
                Assert.AreEqual(1, Marshal.ReadByte(dataBlock.Buffer, 1));
                dataBlock.Dispose();

                Assert.IsTrue(track1.TryGetPreviousDataBlock(out dataBlock));
                Assert.IsNotNull(dataBlock);
                Assert.AreEqual(deviceTimestamp0, dataBlock.DeviceTimestamp);
                Assert.AreEqual(2, dataBlock.SizeBytes);
                Assert.AreEqual(1, Marshal.ReadByte(dataBlock.Buffer, 0));
                Assert.AreEqual(0, Marshal.ReadByte(dataBlock.Buffer, 1));
                dataBlock.Dispose();

                // eof
                Assert.IsFalse(track1.TryGetPreviousDataBlock(out dataBlock));
                Assert.IsNull(dataBlock);

                // Reading of track 2

                Assert.IsFalse(track2.TryGetPreviousDataBlock(out dataBlock));
                Assert.IsNull(dataBlock);

                // forward
                Assert.IsTrue(track2.TryGetNextDataBlock(out dataBlock));
                Assert.IsNotNull(dataBlock);
                Assert.AreEqual(deviceTimestamp0, dataBlock.DeviceTimestamp);
                Assert.AreEqual(2, dataBlock.SizeBytes);
                Assert.AreEqual(2, Marshal.ReadByte(dataBlock.Buffer, 0));
                Assert.AreEqual(0, Marshal.ReadByte(dataBlock.Buffer, 1));
                dataBlock.Dispose();

                Assert.IsTrue(track2.TryGetNextDataBlock(out dataBlock));
                Assert.IsNotNull(dataBlock);
                Assert.AreEqual(deviceTimestamp1, dataBlock.DeviceTimestamp);
                Assert.AreEqual(2, dataBlock.SizeBytes);
                Assert.AreEqual(2, Marshal.ReadByte(dataBlock.Buffer, 0));
                Assert.AreEqual(1, Marshal.ReadByte(dataBlock.Buffer, 1));
                dataBlock.Dispose();

                // eof
                Assert.IsFalse(track2.TryGetNextDataBlock(out dataBlock));
                Assert.IsNull(dataBlock);

                // backward
                Assert.IsTrue(track2.TryGetPreviousDataBlock(out dataBlock));
                Assert.IsNotNull(dataBlock);
                Assert.AreEqual(deviceTimestamp1, dataBlock.DeviceTimestamp);
                Assert.AreEqual(2, dataBlock.SizeBytes);
                Assert.AreEqual(2, Marshal.ReadByte(dataBlock.Buffer, 0));
                Assert.AreEqual(1, Marshal.ReadByte(dataBlock.Buffer, 1));
                dataBlock.Dispose();

                Assert.IsTrue(track2.TryGetPreviousDataBlock(out dataBlock));
                Assert.IsNotNull(dataBlock);
                Assert.AreEqual(deviceTimestamp0, dataBlock.DeviceTimestamp);
                Assert.AreEqual(2, dataBlock.SizeBytes);
                Assert.AreEqual(2, Marshal.ReadByte(dataBlock.Buffer, 0));
                Assert.AreEqual(0, Marshal.ReadByte(dataBlock.Buffer, 1));
                dataBlock.Dispose();

                // eof
                Assert.IsFalse(track2.TryGetPreviousDataBlock(out dataBlock));
                Assert.IsNull(dataBlock);
            }

            File.Delete(mkvPath);
        }

        [TestMethod]
        public void TestCustomSubtitleTracks()
        {
            var mkvPath = GenerateTempMkvFilePath();

            var config = new DeviceConfiguration
            {
                CameraFps = FrameRate.Five,
                ColorFormat = ImageFormat.ColorNV12,
                ColorResolution = ColorResolution.R720p,
                DepthMode = DepthMode.Off,
            };

            var deviceTimestamp0 = Microseconds64.FromMilliseconds(0);
            var deviceTimestamp1 = Microseconds64.FromMilliseconds(20);

            var track1Name = "TEST_TRACK_NAME";
            var track1CodecId = "S_CUSTOM_SUBTITLE_CODEC";
            var track1CodecContext = new byte[] { 0, 1, 2, 3 };
            var track1Settings = new RecordSubtitleSettings { HighFreqData = false };

            var track2Name = "ANOTHER_CUSTOM_TRACK";
            var track2CodecId = "S_XYZ_CODEC";
            var track2CodecContext = new byte[] { 255, 254, 253 };
            var track2Settings = new RecordSubtitleSettings { HighFreqData = true };

            using (var recorder = new Recorder(mkvPath, null, config))
            {
                var track1 = recorder.CustomTracks.AddCustomSubtitleTrack(track1Name, track1CodecId, track1CodecContext, track1Settings);
                Assert.AreEqual(track1Name, track1.Name);
                Assert.AreEqual(track1CodecId, track1.CodecId);
                Assert.AreSame(track1CodecContext, track1.CodecContext);

                var track2 = recorder.CustomTracks.AddCustomSubtitleTrack(track2Name, track2CodecId, track2CodecContext, track2Settings);
                Assert.AreEqual(track2Name, track2.Name);
                Assert.AreEqual(track2CodecId, track2.CodecId);
                Assert.AreSame(track2CodecContext, track2.CodecContext);

                recorder.WriteHeader();

                WriteTestCaptures(recorder, deviceTimestamp0, deviceTimestamp1);

                track1.WriteData(deviceTimestamp0, new byte[] { 10 });
                track1.WriteData(deviceTimestamp0 + 200, new byte[] { 12 });
                track1.WriteData(deviceTimestamp0 + 400, new byte[] { 14 });
                track1.WriteData(deviceTimestamp0 + 600, new byte[] { 16 });
                track1.WriteData(deviceTimestamp0 + 800, new byte[] { 18 });
                track1.WriteData(deviceTimestamp1, new byte[] { 20 });
                track1.WriteData(deviceTimestamp1 + 200, new byte[] { 22 });
                track1.WriteData(deviceTimestamp1 + 400, new byte[] { 24 });
                track1.WriteData(deviceTimestamp1 + 600, new byte[] { 26 });
                track1.WriteData(deviceTimestamp1 + 800, new byte[] { 28 });

                track2.WriteData(deviceTimestamp0, new byte[] { 2, 0 });
                track2.WriteData(deviceTimestamp1, new byte[] { 2, 1 });
            }

            using (var playback = new Playback(mkvPath))
            {
                var builtInTrackCount = GetBuiltInTrackCount(config);
                var trackCount = builtInTrackCount + 2;
                Assert.AreEqual(trackCount, playback.Tracks.Count);
                Assert.AreEqual(builtInTrackCount, playback.Tracks.Count(t => t.IsBuiltIn));
                Assert.AreEqual(2, playback.Tracks.Count(t => !t.IsBuiltIn));

                Assert.IsTrue(playback.Tracks.Exists(track1Name));
                Assert.IsTrue(playback.Tracks.Exists(track2Name));
                Assert.IsFalse(playback.Tracks.Exists("SOME_UNKNOWN_TRACK_NAME"));

                var track1 = playback.Tracks[track1Name];
                Assert.IsNotNull(track1);
                Assert.AreEqual(track1Name, track1.Name);
                Assert.AreEqual(track1CodecId, track1.CodecId);
                AssertAreEqual(track1CodecContext, track1.CodecContext);
                Assert.IsFalse(track1.IsBuiltIn);

                var track2 = playback.Tracks[track2Name];
                Assert.IsNotNull(track2);
                Assert.AreEqual(track2Name, track2.Name);
                Assert.AreEqual(track2CodecId, track2.CodecId);
                AssertAreEqual(track2CodecContext, track2.CodecContext);
                Assert.IsFalse(track2.IsBuiltIn);

                Assert.IsNull(playback.Tracks["SOME_UNKNOWN_TRACK_NAME"]);

                // Reading of track 1

                Assert.IsFalse(track1.TryGetPreviousDataBlock(out var dataBlock));
                Assert.IsNull(dataBlock);

                // forward
                for (var i = 0; i < 10; i += 2)
                {
                    Assert.IsTrue(track1.TryGetNextDataBlock(out dataBlock));
                    Assert.IsNotNull(dataBlock);
                    Assert.AreEqual(deviceTimestamp0.ValueUsec + i * 100, dataBlock.DeviceTimestamp.ValueUsec);
                    Assert.AreEqual(1, dataBlock.SizeBytes);
                    Assert.AreEqual(10 + i, Marshal.ReadByte(dataBlock.Buffer));
                    dataBlock.Dispose();
                }

                for (var i = 0; i < 10; i += 2)
                {
                    Assert.IsTrue(track1.TryGetNextDataBlock(out dataBlock));
                    Assert.IsNotNull(dataBlock);
                    Assert.AreEqual(deviceTimestamp1.ValueUsec + i * 100, dataBlock.DeviceTimestamp.ValueUsec);
                    Assert.AreEqual(1, dataBlock.SizeBytes);
                    Assert.AreEqual(20 + i, Marshal.ReadByte(dataBlock.Buffer));
                    dataBlock.Dispose();
                }

                // eof
                Assert.IsFalse(track1.TryGetNextDataBlock(out dataBlock));
                Assert.IsNull(dataBlock);

                // Reading of track 2

                Assert.IsFalse(track2.TryGetPreviousDataBlock(out dataBlock));
                Assert.IsNull(dataBlock);

                // forward
                Assert.IsTrue(track2.TryGetNextDataBlock(out dataBlock));
                Assert.IsNotNull(dataBlock);
                Assert.AreEqual(deviceTimestamp0, dataBlock.DeviceTimestamp);
                Assert.AreEqual(2, dataBlock.SizeBytes);
                Assert.AreEqual(2, Marshal.ReadByte(dataBlock.Buffer, 0));
                Assert.AreEqual(0, Marshal.ReadByte(dataBlock.Buffer, 1));
                dataBlock.Dispose();

                Assert.IsTrue(track2.TryGetNextDataBlock(out dataBlock));
                Assert.IsNotNull(dataBlock);
                Assert.AreEqual(deviceTimestamp1, dataBlock.DeviceTimestamp);
                Assert.AreEqual(2, dataBlock.SizeBytes);
                Assert.AreEqual(2, Marshal.ReadByte(dataBlock.Buffer, 0));
                Assert.AreEqual(1, Marshal.ReadByte(dataBlock.Buffer, 1));
                dataBlock.Dispose();

                // eof
                Assert.IsFalse(track2.TryGetNextDataBlock(out dataBlock));
                Assert.IsNull(dataBlock);

                // backward
                Assert.IsTrue(track2.TryGetPreviousDataBlock(out dataBlock));
                Assert.IsNotNull(dataBlock);
                Assert.AreEqual(deviceTimestamp1, dataBlock.DeviceTimestamp);
                Assert.AreEqual(2, dataBlock.SizeBytes);
                Assert.AreEqual(2, Marshal.ReadByte(dataBlock.Buffer, 0));
                Assert.AreEqual(1, Marshal.ReadByte(dataBlock.Buffer, 1));
                dataBlock.Dispose();

                Assert.IsTrue(track2.TryGetPreviousDataBlock(out dataBlock));
                Assert.IsNotNull(dataBlock);
                Assert.AreEqual(deviceTimestamp0, dataBlock.DeviceTimestamp);
                Assert.AreEqual(2, dataBlock.SizeBytes);
                Assert.AreEqual(2, Marshal.ReadByte(dataBlock.Buffer, 0));
                Assert.AreEqual(0, Marshal.ReadByte(dataBlock.Buffer, 1));
                dataBlock.Dispose();

                // eof
                Assert.IsFalse(track2.TryGetPreviousDataBlock(out dataBlock));
                Assert.IsNull(dataBlock);
            }

            File.Delete(mkvPath);
        }

        #region Helper methods

        private static void AssertAreEqual(byte[] expected, byte[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (var i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);
        }

        private static string GenerateTempMkvFilePath()
            => Path.GetTempFileName() + ".mkv";

        private static void CheckCapture(Capture capture, DeviceConfiguration config, Microseconds64 colorTimestamp)
        {
            using (var colorImage = capture.ColorImage)
            {
                if (config.ColorResolution == ColorResolution.Off)
                {
                    Assert.IsNull(colorImage);
                }
                else
                {
                    Assert.IsNotNull(colorImage);
                    Assert.AreEqual(config.ColorFormat, colorImage.Format);
                    Assert.AreEqual(config.ColorResolution.WidthPixels(), colorImage.WidthPixels);
                    Assert.AreEqual(config.ColorResolution.HeightPixels(), colorImage.HeightPixels);
                    Assert.AreEqual(colorTimestamp, colorImage.DeviceTimestamp);
                }
            }

            Microseconds64 depthTimestamp = colorTimestamp + config.DepthDelayOffColor;

            using (var depthImage = capture.DepthImage)
            {
                if (!config.DepthMode.HasDepth())
                {
                    Assert.IsNull(depthImage);
                }
                else
                {
                    Assert.IsNotNull(depthImage);
                    Assert.AreEqual(ImageFormat.Depth16, depthImage.Format);
                    Assert.AreEqual(config.DepthMode.WidthPixels(), depthImage.WidthPixels);
                    Assert.AreEqual(config.DepthMode.HeightPixels(), depthImage.HeightPixels);
                    Assert.AreEqual(depthTimestamp, depthImage.DeviceTimestamp);
                }
            }

            using (var irImage = capture.IRImage)
            {
                if (!config.DepthMode.HasPassiveIR())
                {
                    Assert.IsNull(irImage);
                }
                else
                {
                    Assert.IsNotNull(irImage);
                    Assert.AreEqual(ImageFormat.IR16, irImage.Format);
                    Assert.AreEqual(config.DepthMode.WidthPixels(), irImage.WidthPixels);
                    Assert.AreEqual(config.DepthMode.HeightPixels(), irImage.HeightPixels);
                    Assert.AreEqual(depthTimestamp, irImage.DeviceTimestamp);
                }
            }
        }

        private static Microseconds64 GetStartTimestamp(DeviceConfiguration config, Microseconds64 colorTimestampFirst)
        {
            var res = colorTimestampFirst;
            if (config.DepthDelayOffColor < 0 && config.DepthMode != DepthMode.Off
                || config.ColorResolution == ColorResolution.Off)
            {
                res += config.DepthDelayOffColor;
            }
            return res;
        }

        private static Microseconds64 GetEndTimestamp(DeviceConfiguration config, Microseconds64 colorTimestampLast)
        {
            var res = colorTimestampLast;
            if (config.DepthDelayOffColor > 0 && config.DepthMode != DepthMode.Off
                || config.ColorResolution == ColorResolution.Off)
            {
                res += config.DepthDelayOffColor;
            }
            return res;
        }

        private static int GetBuiltInTrackCount(DeviceConfiguration config)
        {
            var count = 0;
            if (config.ColorResolution != ColorResolution.Off)
                count++;
            if (config.DepthMode.HasDepth())
                count++;
            if (config.DepthMode.HasPassiveIR())
                count++;
            return count;
        }

        private static void WriteTestCaptures(Recorder recorder, params Microseconds64[] colorTimestamps)
        {
            foreach (var ts in colorTimestamps)
                WriteTestCapture(recorder, ts);
        }

        private static void WriteTestCapture(Recorder recorder, Microseconds64 colorTimestamp)
        {
            using (var capture = CreateTestCapture(recorder.DeviceConfiguration, colorTimestamp))
                recorder.WriteCapture(capture);
        }

        private static Capture CreateTestCapture(DeviceConfiguration config, Microseconds64 colorTimestamp)
        {
            var capture = new Capture();
            using (var image = CreateTestColorImage(config, colorTimestamp))
                capture.ColorImage = image;
            using (var image = CreateTestIRImage(config, colorTimestamp))
                capture.IRImage = image;
            using (var image = CreateTestDepthImage(config, colorTimestamp))
                capture.DepthImage = image;
            return capture;
        }

        private static Image CreateTestColorImage(DeviceConfiguration config, Microseconds64 colorTimestamp)
        {
            if (config.ColorResolution == ColorResolution.Off)
                return null;

            var width = config.ColorResolution.WidthPixels();
            var height = config.ColorResolution.HeightPixels();
            var stride = config.ColorFormat.StrideBytes(width);
            var buffer = new byte[config.ColorFormat.ImageSizeBytes(stride, height)];

            var image = Image.CreateFromArray(buffer, config.ColorFormat, width, height);
            image.DeviceTimestamp = colorTimestamp;

            return image;
        }

        private static Image CreateTestIRImage(DeviceConfiguration config, Microseconds64 colorTimestamp)
        {
            if (!config.DepthMode.HasPassiveIR())
                return null;

            var width = config.DepthMode.WidthPixels();
            var height = config.DepthMode.HeightPixels();
            var buffer = new short[width * height];

            var image = Image.CreateFromArray(buffer, ImageFormat.IR16, width, height);
            image.DeviceTimestamp = colorTimestamp + config.DepthDelayOffColor;

            return image;
        }

        private static Image CreateTestDepthImage(DeviceConfiguration config, Microseconds64 colorTimestamp)
        {
            if (!config.DepthMode.HasDepth())
                return null;

            var width = config.DepthMode.WidthPixels();
            var height = config.DepthMode.HeightPixels();
            var buffer = new short[width * height];

            var image = Image.CreateFromArray(buffer, ImageFormat.Depth16, width, height);
            image.DeviceTimestamp = colorTimestamp + config.DepthDelayOffColor;

            return image;
        }

        #endregion
    }
}
