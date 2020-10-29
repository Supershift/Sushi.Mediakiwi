﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Sushi.Mediakiwi.Data.Configuration;

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
        public void ReadJsonGenerals()
        {
            WimServerConfiguration.LoadJsonConfig();
            var sets = WimServerConfiguration.Instance;
            foreach (var item in sets.General)
                Console.WriteLine($"{item.Name} : {item.Value}");

            Assert.IsTrue(sets.General.Count > 0);
        }


        [TestMethod]
        public void ReadJsonDBMappings()
        {
            WimServerConfiguration.LoadJsonConfig();
            var sets = WimServerConfiguration.Instance;
            foreach (var item in sets.DatabaseMappings)
                Console.WriteLine($"{item.Name} : {item.Portal}");

            Assert.IsTrue(sets.DatabaseMappings.Count > 0);
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
