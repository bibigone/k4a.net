using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K4AdotNet.Tests.Unit
{
    [TestClass]
    public class Common
    {
        [AssemblyInitialize]
        public static void GlobalInit(TestContext _)
        {
#if AZURE
            Sdk.Init(ComboMode.Azure);
#elif ORBBEC
            Sdk.Init(ComboMode.Orbbec);
#else
#error Please define AZURE or ORBBEC constant
#endif
        }
    }
}
