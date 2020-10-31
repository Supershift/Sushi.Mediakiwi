using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Sushi.Mediakiwi.Data.Configuration;

namespace Sushi.Mediakiwi.Tests.Config
{
    [TestClass]
    public class commonConfigTest
    {
        [TestInitialize]
        public void Initialize()
        {
            WimServerConfiguration.LoadJsonConfig();
        }


        [TestMethod]
        public void CheckIfMappedUrlExists()
        {
            var exists = Common.CheckIfMappedUrlExists("testMapping1");
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void GetCurrentPortal()
        {
            var exists = Common.CurrentPortal;
            Console.WriteLine(exists.Name);
            Assert.IsTrue(string.IsNullOrEmpty(exists.Name) == false);
        }


        [TestMethod]
        public void GetCurrentConnectionString()
        {
            var connString = Common.DatabaseConnectionString;
            Console.WriteLine(connString);
            Assert.IsTrue(string.IsNullOrEmpty(connString) == false);
        }


        [TestMethod]
        public void GetCurrentDatabaseDateTime()
        {
            var dateTime = Common.DatabaseDateTime;
            Console.WriteLine(dateTime);
            Assert.IsTrue(dateTime > DateTime.MinValue);
        }

        [TestMethod]
        public void GetCurrentGalleryMapping()
        {
            var mapping = Common.GetCurrentGalleryMappingUrl("/products/");
            Console.WriteLine(mapping.Path);
            Assert.IsNotNull(mapping);
        }

        [TestMethod]
        public void GetCurrentMappingConnectionByType()
        {
            var mapping = Common.GetCurrentMappingConnection(this.GetType());
            Console.WriteLine(mapping.Name);
            Assert.IsNotNull(mapping);
        }

        [TestMethod]
        public void GetCurrentMappingConnectionByString()
        {
            var typeString = this.GetType().ToString();

            var mapping = Common.GetCurrentMappingConnection(typeString);
            Console.WriteLine(mapping.Name);
            Assert.IsNotNull(mapping);
        }

        [TestMethod]
        public void GetCurrentMappingConnectionByName()
        {
            var mapping = Common.GetCurrentMappingConnectionByName("testMappingDB");
            Console.WriteLine(mapping.Name);
            Assert.IsNotNull(mapping);
        }

        [TestMethod]
        public void GetPortal()
        {
            var portal = Common.GetPortal("CENTRAAL");
            Console.WriteLine(portal.Name);
            Assert.IsNotNull(portal);
        }


        [TestMethod]
        public void GetSqlConnection()
        {
            var portal = Common.GetSqlConnection("CENTRAAL");
            Console.WriteLine(portal.ConnectionString);
            Assert.IsNotNull(portal);
        }


        [TestMethod]
        public void GetUrlMappingByNameAndType()
        {
            var map = Common.GetUrlMappingConfiguration("testMapping1", 2);
            Console.WriteLine(map.Name);
            Assert.IsNotNull(map);
        }
    }
}

