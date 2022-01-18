using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.MicroORM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Test.ORM
{
    [TestClass]
    public class ComponentVersionTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_ComponentVersions";
        private string _key = "ComponentVersion_Key";

        private static DateTime _date = DateTime.Now;
        // Test object
        private ComponentVersion _TestObj = new ComponentVersion()
        {
            ApplicationUserID = 26,
            MasterID = 47,
            Created = _date,
            Updated = _date,
            GUID = new Guid("85D85381-A2C5-401E-93E3-2987E355C996"),
            PageID = 26,
            AvailableTemplateID = 47,
            TemplateID = 23,
            IsFixed = true,
            IsAlive = true,
            IsActive = true,
            SiteID = 666,
            IsSecundary = true,
            FixedFieldName = "C000_unitTest",
            Target = "unitTest",
            InstanceName = "xUNIT TESTx",
            Serialized_XML = @"<Content xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><Fields></Fields></Content>",
            SortField_Date = _date,
            SortOrder = 666
        };
        // Async test object
        private ComponentVersion _TestObjAsync = new ComponentVersion()
        {
            ApplicationUserID = 26,
            MasterID = 47,
            Created = _date,
            Updated = _date,
            GUID = new Guid("8480C583-3705-4726-AADF-FA0CDE7F3B5E"),
            PageID = 26,
            AvailableTemplateID = 47,
            TemplateID = 23,
            IsFixed = true,
            IsAlive = true,
            IsActive = true,
            SiteID = 666,
            IsSecundary = true,
            FixedFieldName = "C000_unitAsyncTest",
            Target = "unitAsyncTest",
            InstanceName = "xASYNC UNIT TESTx",
            Serialized_XML = @"<Content xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><Fields></Fields></Content>",
            SortField_Date = _date,
            SortOrder = 666
        };
        #endregion Test Data

        #region  Create
        public void A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj?.ID > 0);

            ComponentVersion db = ComponentVersion.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.ID, db.ID);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED ComponentVersion: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            var db = await ComponentVersion.SelectOneAsync(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.ID, db.ID);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED ComponentVersion: {_TestObjAsync.ID}");
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
                var componentVersion = ComponentVersion.SelectOne(_TestObj.ID);

                if (componentVersion?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersion.ID}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
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
                var componentVersion = await ComponentVersion.SelectOneAsync(_TestObjAsync.ID);

                if (componentVersion?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersion.ID}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
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
                var componentVersion = ComponentVersion.SelectOne(_TestObj.GUID);

                if (componentVersion?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersion.ID}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
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
                var componentVersion = await ComponentVersion.SelectOneAsync(_TestObjAsync.GUID);

                if (componentVersion?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersion.ID}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByPageAndType()
        {
            // ComponentTemplate is needed for this test.
            ComponentTemplateTests componentTemplateTests = new ComponentTemplateTests();
            _TestObj.TemplateID = componentTemplateTests.A_Create_TestObj().ID;

            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentVersion = ComponentVersion.SelectOneBasedOnType((int)_TestObj.PageID, componentTemplateTests.GetType());

                if (componentVersion?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersion.ID}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
            }
            finally
            {
                componentTemplateTests.D_Delete_TestObj();
                // Deleting ComponentTemplate also deletes the ComponentVersion
                // D_Delete_TestObj();

                componentTemplateTests.F_Reset_AutoIndent();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByPageAndTypeAsync()
        {
            // ComponentTemplate is needed for this test.
            ComponentTemplateTests componentTemplateTests = new ComponentTemplateTests();
            var tempObj = await componentTemplateTests.A_Create_TestObjAsync();
            _TestObjAsync.TemplateID = tempObj.ID;

            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentVersion = await ComponentVersion.SelectOneBasedOnTypeAsync((int)_TestObjAsync.PageID, componentTemplateTests.GetType());

                if (componentVersion?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersion.ID}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
            }
            finally
            {
                await componentTemplateTests.D_Delete_TestObjAsync();
                // Deleting ComponentTemplate also deletes the ComponentVersion
                // await D_Delete_TestObjAsync();

                await componentTemplateTests.F_Reset_AutoIndentAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneFixedByPageAndName()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentVersion = ComponentVersion.SelectOneFixed((int)_TestObj.PageID, _TestObj.FixedFieldName);

                if (componentVersion?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersion.ID}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneFixedByPageAndNameAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentVersion = await ComponentVersion.SelectOneFixedAsync((int)_TestObjAsync.PageID, _TestObjAsync.FixedFieldName);

                if (componentVersion?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersion.ID}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneSharedByComponentTemplateID()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentVersion = ComponentVersion.SelectOneShared(_TestObj.TemplateID);

                if (componentVersion?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersion.ID}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneSharedByComponentTemplateIDAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentVersion = await ComponentVersion.SelectOneSharedAsync(_TestObjAsync.TemplateID);

                if (componentVersion?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersion.ID}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }


        #endregion

        #region Select ALL

        [TestMethod]
        public void X_SelectAll()
        {
            // Function that we are testing BELOW...
            var componentVersions = ComponentVersion.SelectAll();

            if (componentVersions?.Length > 0)
                Trace.WriteLine($"FOUND ComponentVersion: {componentVersions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ComponentVersion NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllAsync()
        {
            // Function that we are testing BELOW...
            var componentVersions = await ComponentVersion.SelectAllAsync();

            if (componentVersions?.Length > 0)
                Trace.WriteLine($"FOUND ComponentVersion: {componentVersions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ComponentVersion NOT FOUND...");
        }

        [TestMethod]
        public void B_SelectAllForPage()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentVersions = ComponentVersion.SelectAll((int)_TestObj.PageID, false);

                if (componentVersions?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllForPageAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentVersions = await ComponentVersion.SelectAllAsync((int)_TestObjAsync.PageID, false);

                if (componentVersions?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }


        [TestMethod]
        public void B_SelectAllForPageAndTarget()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentVersions = ComponentVersion.SelectAll((int)_TestObj.PageID, _TestObj.Target);

                if (componentVersions?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }


        [TestMethod]
        public async Task B_SelectAllForPageAndTargetAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentVersions = await ComponentVersion.SelectAllAsync((int)_TestObjAsync.PageID, _TestObjAsync.Target);

                if (componentVersions?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }


        [TestMethod]
        public void B_SelectAllForPageFixed()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentVersions = ComponentVersion.SelectAllFixed((int)_TestObj.PageID, false);

                if (componentVersions?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllForPageFixedAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentVersions = await ComponentVersion.SelectAllFixedAsync((int)_TestObjAsync.PageID, false);

                if (componentVersions?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectAllForPageAlive()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentVersions = ComponentVersion.SelectAllOnPage((int)_TestObj.PageID);

                if (componentVersions?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllForPageAliveAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentVersions = await ComponentVersion.SelectAllOnPageAsync((int)_TestObjAsync.PageID);

                if (componentVersions?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectAllSharedForSite()
        {
            _TestObj.PageID = null;
            _TestObj.TemplateIsShared = true;

            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentVersions = ComponentVersion.SelectAllSharedForSite((int)_TestObj.SiteID);

                if (componentVersions?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllSharedForSiteAsync()
        {
            _TestObjAsync.PageID = null;
            _TestObjAsync.TemplateIsShared = true;

            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentVersions = await ComponentVersion.SelectAllSharedForSiteAsync((int)_TestObjAsync.SiteID);

                if (componentVersions?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentVersion: {componentVersions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("ComponentVersion NOT FOUND...");
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
            var allAssert = ComponentVersion.SelectAll();
            var ass = allAssert.Where(x => x.Target == _TestObj.Target);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test ComponentVersion wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE ComponentVersion Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                a.Delete();
                Trace.WriteLine($"DELETE ComponentVersion: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = ComponentVersion.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test ComponentVersion not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await ComponentVersion.SelectAllAsync();
            var ass = allAssert.Where(x => x.Target == _TestObjAsync.Target);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test ComponentVersion wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE ComponentVersion Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                await a.DeleteAsync();
                Trace.WriteLine($"DELETE ComponentVersion: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = await ComponentVersion.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test ComponentVersion not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateQuery();

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
            var connector = ConnectorFactory.CreateConnector<ComponentVersion>();
            var filter = connector.CreateQuery();

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
