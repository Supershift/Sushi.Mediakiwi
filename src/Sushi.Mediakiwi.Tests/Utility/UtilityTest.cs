using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sushi.Mediakiwi.Data.Configuration;

namespace Sushi.Mediakiwi.Tests.Utility
{
    [TestClass]
    public class UtilityTest
    {
        [TestInitialize]
        public void Initialize()
        {
            WimServerConfiguration.LoadJsonConfig();
        }
    }
}
