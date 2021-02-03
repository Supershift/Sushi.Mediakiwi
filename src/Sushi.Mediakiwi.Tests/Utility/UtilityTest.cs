using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sushi.Mediakiwi.Data.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

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
