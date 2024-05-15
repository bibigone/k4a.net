using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K4AdotNet.Tests.Unit
{
    [TestClass]
    public class Common
    {
        [AssemblyInitialize]
        public static void GlobalInit(TestContext _)
        {
            // Sdk.Init(ComboMode.Orbbec);
            Sdk.Init(ComboMode.Azure);
        }
    }
}
