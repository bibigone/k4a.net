using K4AdotNet.Record;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;

namespace K4AdotNet.Tests.Unit.Record
{
    [TestClass]
    public class MarshalingTests
    {
        [TestMethod]
        public void TestSizesOfStructures()
        {
#if !ORBBECSDK_K4A_WRAPPER
            // sizeof(k4a_record_configuration_t) == 36
            Assert.AreEqual(36, Marshal.SizeOf<RecordConfiguration>());
#endif

            // sizeof(k4a_record_video_settings_t) == 24
            Assert.AreEqual(24, Marshal.SizeOf<RecordVideoSettings>());

            // sizeof(k4a_record_subtitle_settings_t) == 1
            Assert.AreEqual(1, Marshal.SizeOf<RecordSubtitleSettings>());
        }
    }
}
