using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.MicroORM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Tests.ORM
{
    [TestClass]
    public class PortalTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Portals";
        private string _key = "Portal_Key";

        private static DateTime _date = DateTime.Now;
        // Test object
        private Mediakiwi.Data.Portal _TestObj = new Mediakiwi.Data.Portal()
        {
            GUID = new Guid("8165C2B9-3A40-4ACB-A0E3-889B0B7A24AA"),
            UserID = 69,
            Name = "xUNIT TESTx",
            Domain = "TEST domain",
            Authenticode = "553CD22F132E36CEF0F81B9FE0A4C657C13241D640EB4D02B3DBC0032A1A86034E6EF27383FF8EEC8F7ADC789BB4B2A7BAAE76744A998841C984BA8DA7EF6421",
            Authentication = "7AFE17241A2CCF9381504DAABED33BC1561AE6F0E6A9D91D9A442878E926BC1902D48B8B8C87D24ECD19C88E18CEB1396142407EF4FC3E295048D3633DC39F8A",
            IsActive = true,
            Created = _date,
            DataString = @"<Data xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><Settings></Settings></Data>"
        };

        // Async Test object
        private Mediakiwi.Data.Portal _TestObjAsync = new Mediakiwi.Data.Portal()
        {
            GUID = new Guid("31CFCE63-5943-4825-B9BB-9C3C2A31EC2D"),
            UserID = 71,
            Name = "xASYNC UNIT TESTx",
            Domain = "ASYNC TEST domain",
            Authenticode = "D8B665CC7F21E03DA8580A1B51AE265E60DAA9ED73F9C79E44174057C949C72F46BD018DCCCD21E2E0475ADB991E1AF6380EFDF652E5E095ABBFF1BC5815A2AE",
            Authentication = "7848EE05BAB6A4D709914336E0AB776EE659207666B4567DA5B8884F8E5EC18FECB74F39F386932F5AD17374E803E2B764447295A9A3B535AA5EEC4766C7A8A6",
            IsActive = true,
            Created = _date,
            DataString = @"<Data xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><Settings></Settings></Data>"
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj.ID > 0);

            Mediakiwi.Data.Portal db = (Mediakiwi.Data.Portal)Mediakiwi.Data.Portal.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.GUID, db.GUID);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED Portal: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync.ID > 0);

            Mediakiwi.Data.Portal db = (Mediakiwi.Data.Portal)Mediakiwi.Data.Portal.SelectOne(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.GUID, db.GUID);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED Portal: {_TestObjAsync.ID}");
            }
        }
        #endregion Create

        #region Select ONE
        [TestMethod]
        public void B_SelectOneByID()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var portal = Mediakiwi.Data.Portal.SelectOne(_TestObj.ID);

                if (portal?.ID > 0)
                    Trace.WriteLine($"FOUND Portal: {portal.ID}");
                else
                    Assert.Fail("Portal NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByIDAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var portal = await Mediakiwi.Data.Portal.SelectOneAsync(_TestObjAsync.ID);

                if (portal?.ID > 0)
                    Trace.WriteLine($"FOUND Portal: {portal.ID}");
                else
                    Assert.Fail("Portal NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByGUID()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var portal = Mediakiwi.Data.Portal.SelectOne(_TestObj.GUID);

                if (portal?.ID > 0)
                    Trace.WriteLine($"FOUND Portal: {portal.ID}");
                else
                    Assert.Fail("Portal NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByGUIDAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var portal = await Mediakiwi.Data.Portal.SelectOneAsync(_TestObjAsync.GUID);

                if (portal?.ID > 0)
                    Trace.WriteLine($"FOUND Portal: {portal.ID}");
                else
                    Assert.Fail("Portal NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }


        [TestMethod]
        public void B_SelectOneByAuthentiCode()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var portal = Mediakiwi.Data.Portal.SelectOne(_TestObj.Authenticode);

                if (portal?.ID > 0)
                    Trace.WriteLine($"FOUND Portal: {portal.ID}");
                else
                    Assert.Fail("Portal NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByAuthentiCodeAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var portal = await Mediakiwi.Data.Portal.SelectOneAsync(_TestObjAsync.Authenticode);

                if (portal?.ID > 0)
                    Trace.WriteLine($"FOUND Portal: {portal.ID}");
                else
                    Assert.Fail("Portal NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        #endregion Select ONE

        #region Select All
        [TestMethod]
        public void B_SelectAll()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var portals = Mediakiwi.Data.Portal.SelectAll();

                if (portals?.Length > 0)
                    Trace.WriteLine($"FOUND Portal: {portals.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Portal NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var portals = await Mediakiwi.Data.Portal.SelectAllAsync();

                if (portals?.Length > 0)
                    Trace.WriteLine($"FOUND Portal: {portals.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Portal NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }
        #endregion Select All

        #region Delete
        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = Mediakiwi.Data.Portal.SelectAll();
            var ass = allAssert.Where(x => x.Name == _TestObj.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Portal wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Portal Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (Mediakiwi.Data.Portal a in ass)
            {
                a.Delete();
                Trace.WriteLine($"DELETE Portal: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var connector = ConnectorFactory.CreateConnector<PortalRight>();
                var testDelete = PortalRight.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Portal not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await Mediakiwi.Data.Portal.SelectAllAsync();
            var ass = allAssert.Where(x => x.Name == _TestObjAsync.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Portal wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Portal Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (Mediakiwi.Data.Portal a in ass)
            {
                await a.DeleteAsync();
                Trace.WriteLine($"DELETE Portal: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var connector = ConnectorFactory.CreateConnector<Mediakiwi.Data.Portal>();
                var testDelete = await Mediakiwi.Data.Portal.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Portal not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<Mediakiwi.Data.Portal>();
            var filter = connector.CreateDataFilter();

            // Query for resetting the autoincrement in the table
            string sql = $"DECLARE @max int;  SELECT @max = ISNULL(MAX([{_key}]), 1) FROM [dbo].[{_table}];  DBCC CHECKIDENT({_table}, RESEED, @max)";

            try
            {
                connector.ExecuteNonQuery(sql, filter);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Error during resetting of autoincrement: {ex.Message}");
            }
        }

        public async Task F_Reset_AutoIndentAsync()
        {
            var connector = ConnectorFactory.CreateConnector< Mediakiwi.Data.Portal >();
            var filter = connector.CreateDataFilter();

            // Query for resetting the autoincrement in the table
            string sql = $"DECLARE @max int;  SELECT @max = ISNULL(MAX([{_key}]), 1) FROM [dbo].[{_table}];  DBCC CHECKIDENT({_table}, RESEED, @max)";

            try
            {
                await connector.ExecuteNonQueryAsync(sql, filter);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Error during resetting of autoincrement: {ex.Message}");
            }
        }
        #endregion ResetAutoIndent
    }
}
