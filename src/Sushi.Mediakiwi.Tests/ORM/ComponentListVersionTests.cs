using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Tests.ORM
{
    [TestClass]
    public class ComponentListVersionTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_ComponentListVersions";
        private string _key = "ComponentListVersion_Key";

        private ComponentListVersion _TestObj = new ComponentListVersion()
        {
            SiteID = 69,
            ComponentListID = 2,
            ApplicationUserID = 4, // Mark Rienstra
            ComponentListItemID = 2,
            Created = DateTime.Now,
            Serialized_XML = @"<Content xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><Fields></Fields></Content>",
            IsActive = true,
            DescriptiveTag = "xUNIT TESTx",
            Version = 2,
            TypeID = 2
        };

        private ComponentListVersion _TestObjAsync = new ComponentListVersion()
        {
            SiteID = 71,
            ComponentListID = 3,
            ApplicationUserID = 4, // Mark Rienstra
            ComponentListItemID = 3,
            Created = DateTime.Now,
            Serialized_XML = @"<Content xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><Fields></Fields></Content>",
            IsActive = true,
            DescriptiveTag = "xASYNC UNIT TESTx",
            Version = 3,
            TypeID = 3
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj?.ID > 0);

            var dbAsset = ComponentListVersion.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.ID, dbAsset.ID);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED ComponentListVersion: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            var dbAsset = ComponentListVersion.SelectOne(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.ID, dbAsset.ID);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED ComponentListVersion: {_TestObjAsync.ID}");
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
                var componentListVersion = ComponentListVersion.SelectOne(_TestObj.ID);

                if (componentListVersion?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentListVersion: (ID {componentListVersion.ID})");
                else
                    Assert.Fail("ComponentListVersion NOT FOUND...");
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
                var componentListVersion = await ComponentListVersion.SelectOneAsync(_TestObjAsync.ID);

                if (componentListVersion?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentListVersion: (ID {componentListVersion.ID})");
                else
                    Assert.Fail("ComponentListVersion NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByComponentListAndItemID()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentListVersion = ComponentListVersion.SelectOne(_TestObj.ComponentListID, _TestObj.ComponentListItemID);

                if (componentListVersion?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentListVersion: (ID {componentListVersion.ID})");
                else
                    Assert.Fail("ComponentListVersion NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByComponentListAndItemIDAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentListVersion = await ComponentListVersion.SelectOneAsync(_TestObjAsync.ComponentListID, _TestObjAsync.ComponentListItemID);

                if (componentListVersion?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentListVersion: (ID {componentListVersion.ID})");
                else
                    Assert.Fail("ComponentListVersion NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneBySiteComponentListAndItemID()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentListVersion = ComponentListVersion.SelectOne(_TestObj.SiteID, _TestObj.ComponentListID, _TestObj.ComponentListItemID);

                if (componentListVersion?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentListVersion: (ID {componentListVersion.ID})");
                else
                    Assert.Fail("ComponentListVersion NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneBySiteComponentListAndItemIDAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentListVersion = await ComponentListVersion.SelectOneAsync(_TestObjAsync.SiteID, _TestObjAsync.ComponentListID, _TestObjAsync.ComponentListItemID);

                if (componentListVersion?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentListVersion: (ID {componentListVersion.ID})");
                else
                    Assert.Fail("ComponentListVersion NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void X_SelectOneByVersionGUID()
        {
            Guid guid = new Guid("770DBAEA-19A9-48E0-B3D0-B2E8630C4F99");

            // Function that we are testing BELOW...
            var componentListVersion = ComponentListVersion.SelectOne(guid, 2);

            if (componentListVersion?.ID > 0)
                Trace.WriteLine($"FOUND ComponentListVersion: (ID {componentListVersion.ID})");
            else
                Assert.Fail("ComponentListVersion NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectOneByVersionGUIDAsync()
        {
            Guid guid = new Guid("770DBAEA-19A9-48E0-B3D0-B2E8630C4F99");

            // Function that we are testing BELOW...
            var componentListVersion = await ComponentListVersion.SelectOneAsync(guid, 2);

            if (componentListVersion?.ID > 0)
                Trace.WriteLine($"FOUND ComponentListVersion: (ID {componentListVersion.ID})");
            else
                Assert.Fail("ComponentListVersion NOT FOUND...");
        }
        /*
        [TestMethod]
        public void B_SelectOneByTemplate()
        {
            //  NOTE: Function for this test is possibly become obsolete.

            // ComponentList is needed for this test
            ComponentListTests componentListTests = new ComponentListTests();
            ComponentList componentList = componentListTests.A_Create_TestObj();

            _TestObj.ComponentListID = componentList.ID;
            A_Create_TestObj();

            try
            {
                // Function that we are testing BELOW...
                var componentListVersion = ComponentListVersion.SelectOneByTemplate((int)componentList.ComponentTemplateID, _TestObj.ComponentListItemID);

                if (componentListVersion?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentListVersion: (ID {componentListVersion.ID})");
                else
                    Assert.Fail("ComponentListVersion NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                componentListTests.D_Delete_TestObj();

                F_Reset_AutoIndent();
                componentListTests.F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByTemplateAsync()
        {
            //  NOTE: Function for this test is possibly become obsolete.

            // ComponentList is needed for this test
            ComponentListTests componentListTests = new ComponentListTests();
            ComponentList componentList = await componentListTests.A_Create_TestObjAsync();

            _TestObjAsync.ComponentListID = componentList.ID;
            await A_Create_TestObjAsync();

            try
            {
                // Function that we are testing BELOW...
                var componentListVersion = await ComponentListVersion.SelectOneByTemplateAsync((int)componentList.ComponentTemplateID, _TestObjAsync.ComponentListItemID);

                if (componentListVersion?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentListVersion: (ID {componentListVersion.ID})");
                else
                    Assert.Fail("ComponentListVersion NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await componentListTests.D_Delete_TestObjAsync();

                await F_Reset_AutoIndentAsync();
                await componentListTests.F_Reset_AutoIndentAsync();
            }
        }
        */
        #endregion Select ONE

        #region Select ALL
        [TestMethod]
        public void B_SelectAllByComponentListAndSite()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentListVersion = ComponentListVersion.SelectAll(_TestObj.ComponentListID, _TestObj.SiteID);

                if (componentListVersion?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentListVersion: {componentListVersion.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("ComponentListVersion NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllByComponentListAndSiteAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentListVersion = await ComponentListVersion.SelectAllAsync(_TestObjAsync.ComponentListID, _TestObjAsync.SiteID);

                if (componentListVersion?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentListVersion: {componentListVersion.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("ComponentListVersion NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }
        #endregion Select ALL

        #region Delete
        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = ComponentListVersion.SelectAll(_TestObj.ComponentListID, _TestObj.SiteID);
            var ass = allAssert.Where(x => x.DescriptiveTag == _TestObj.DescriptiveTag);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test ComponentListVersion wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE ComponentListVersion Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                // This Delete sets IsActive to 'false'. Does NOT actually delete the record...
                a.Delete();
                Trace.WriteLine($"DELETE ComponentListVersion: {a.ID}");
                try
                {
                    // Check for IsActive in DB
                    var db = ComponentListVersion.SelectOne((int)a.ID);                    
                    Assert.IsFalse((bool)db?.IsActive, "Delete() should set IsActive to 'false'. This did not happen.");
                }
                finally
                {
                    // Cleanup Test Record
                    var connector = ConnectorFactory.CreateConnector<ComponentListVersion>();
                    connector.Delete(a);

                    // Check if delete in the DB is succesfull, for this record
                    var testDelete = ComponentListVersion.SelectOne((int)a?.ID);
                    if (testDelete?.ID > 0) // ERROR
                        errorList.Add($"{a.ID}");
                }
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test ComponentListVersion not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await ComponentListVersion.SelectAllAsync(_TestObjAsync.ComponentListID, _TestObjAsync.SiteID);
            var ass = allAssert.Where(x => x.DescriptiveTag == _TestObjAsync.DescriptiveTag);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test ComponentListVersion wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE ComponentListVersion Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                // This DeleteAsync sets IsActive to 'false'. Does NOT actually delete the record...
                await a.DeleteAsync();
                Trace.WriteLine($"DELETE ComponentListVersion: {a?.ID}");
                try
                {
                    // Check for IsActive in DB
                    var db = ComponentListVersion.SelectOne((int)a?.ID);
                    Assert.IsFalse((bool)db?.IsActive, "DeleteAsync() should set IsActive to 'false'. This did not happen.");
                }
                finally
                {
                    // Cleanup Test Record first, in case of assert failure
                    var connector = ConnectorFactory.CreateConnector<ComponentListVersion>();
                    await connector.DeleteAsync(a);

                    // Check if delete in the DB is succesfull, for this record
                    var testDelete = await ComponentListVersion.SelectOneAsync((int)a?.ID);
                    if (testDelete?.ID > 0) // ERROR
                        errorList.Add($"{a?.ID}");
                }
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test ComponentListVersion not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentListVersion>();
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
            var connector = ConnectorFactory.CreateConnector<ComponentListVersion>();
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
