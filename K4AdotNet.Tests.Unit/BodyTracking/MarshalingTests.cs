using K4AdotNet.BodyTracking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;

namespace K4AdotNet.Tests.Unit.BodyTracking
{
    [TestClass]
    public class MarshalingTests
    {
        [TestMethod]
        public void TestSizesOfStructures()
        {
            // sizeof(k4abt_joint_t) == 32
            Assert.AreEqual(32, Marshal.SizeOf<Joint>());

            // sizeof(k4abt_skeleton_t) == 1024
            Assert.AreEqual(1024, Marshal.SizeOf<Skeleton>());
        }
    }
}
