using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sushi.Mediakiwi.Data.Configuration;
using System;

namespace Sushi.Mediakiwi.Tests.Config
{
    [TestClass]
    public class jsonConfigTest
    {
        [TestMethod]
        public void ReadJsonImplicit()
        {
            WimServerConfiguration.LoadJsonConfig();
            var sets = WimServerConfiguration.Instance;
            Assert.AreEqual("OTAP-P", sets.DefaultPortal);
        }

        [TestMethod]
        public void ReadJsonExplicitNoPath()
        {
            WimServerConfiguration.LoadJsonConfig("appsettings.json");
            var sets = WimServerConfiguration.Instance;
            Assert.AreEqual("OTAP-P", sets.DefaultPortal);
        }

        [TestMethod]
        public void ReadJsonExplicitWithPath()
        {
            WimServerConfiguration.LoadJsonConfig($"{AppDomain.CurrentDomain.BaseDirectory}appsettings.json");
            var sets = WimServerConfiguration.Instance;
            Assert.AreEqual("OTAP-P", sets.DefaultPortal);
        }


        [TestMethod]
        public void ReadJsonGalleryMappings()
        {
            WimServerConfiguration.LoadJsonConfig();
            var sets = WimServerConfiguration.Instance;
            foreach (var item in sets.GalleryMappings)
                Console.WriteLine($"{item.Path} : {item.Portal}");

            Assert.IsTrue(sets.GalleryMappings.Count > 0);
        }


        [TestMethod]
        public void ReadJsonPortalMapping()
        {
            WimServerConfiguration.LoadJsonConfig();
            var sets = WimServerConfiguration.Instance;
            foreach (var item in sets.Portals)
                Console.WriteLine($"{item.Name} : {item.Connection}");

            Assert.IsTrue(sets.Portals.Count > 0);
        }


        [TestMethod]
        public void ReadJsonUrlMapping()
        {
            WimServerConfiguration.LoadJsonConfig();
            var sets = WimServerConfiguration.Instance;
            foreach (var item in sets.UrlMappings)
                Console.WriteLine($"{item.Name} : {item.Path}");

            Assert.IsTrue(sets.UrlMappings.Count > 0);
        }
    }
}
