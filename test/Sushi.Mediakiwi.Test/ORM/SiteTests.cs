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
    public class SiteTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Sites";
        private string _key = "Site_key";

        private static DateTime _date = DateTime.Now;
        // Test object
        private Site _TestObj = new Site()
        {
            Name = "xUNIT TESTx",
            CountryID = 666,
            Language = "nl-NL",
            TimeZoneIndex = "W. Europe Standard Time",
            Culture = "nl-NL",
            MasterID = null,
            AutoPublishInherited = false,
            IsActive = true,
            Type = null,
            HasPages = true,
            HasLists = true,
            Created = _date,
            DefaultPageTitle = "UNIT TEST",
            DefaultFolder = "/test",
            Domain = null,
            HomepageID = null,
            PageNotFoundID = null,
            ErrorPageID = null,
            GUID = new Guid("4DCB1F0D-60F2-451F-A240-5350ED185275")
        };
        // Async Test object
        private Site _TestObjAsync = new Site()
        {
            Name = "xASYNC UNIT TESTx",
            CountryID = 667,
            Language = "nl-NL",
            TimeZoneIndex = "W. Europe Standard Time",
            Culture = "nl-NL",
            MasterID = null,
            AutoPublishInherited = false,
            IsActive = true,
            Type = null,
            HasPages = true,
            HasLists = true,
            Created = _date,
            DefaultPageTitle = "UNIT ASYNC TEST",
            DefaultFolder = "/async_test",
            Domain = null,
            HomepageID = null,
            PageNotFoundID = null,
            ErrorPageID = null,
            GUID = new Guid("0F6B30A1-7157-42AC-ABDB-2A71F5F0F4A2")
        };

        private Folder _TestFolderObj = new Folder()
        {
            GUID = new Guid("2CCC9364-1A31-4863-856C-8A78A3C64AA2"),
            ParentID = null, // Deze moet null zijn
            SortOrderMethod = 0,
            MasterID = 42,
            IsVisible = false,
            SiteID = 0, // Nader invullen
            Type = FolderType.Page,
            Name = "xUNIT TESTx",
            Description = "Test Asset voor UNIT TEST",
            CompletePath = "/test",
            Changed = _date
        };

        private Folder _AsyncTestFolderObj = new Folder()
        {
            GUID = new Guid("CF6BFD32-4FD8-45B8-BF9A-4BD32770A4DD"),
            ParentID = null, // Deze moet null zijn
            SortOrderMethod = 0,
            MasterID = 43,
            IsVisible = false,
            SiteID = 0, // Nader invullen
            Type = FolderType.Page,
            Name = "xASYNC UNIT TESTx",
            Description = "Test Asset voor UNIT ASYNC TEST",
            CompletePath = "/async_test",
            Changed = _date
        };
        #endregion Test Data

        #region Create
        public Site A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj.ID > 0);

            Site db = Site.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.GUID, db.GUID);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED Site: {_TestObj.ID}");
            }

            return _TestObj;
        }

        public async Task<Site> A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync.ID > 0);

            Site db = Site.SelectOne(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.GUID, db.GUID);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED Site: {_TestObjAsync.ID}");
            }

            return _TestObjAsync;
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
                var site = Site.SelectOne(_TestObj.ID);

                if (site?.ID > 0)
                    Trace.WriteLine($"FOUND Site: {site.ID}");
                else
                    Assert.Fail("Site NOT FOUND...");
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
                var site = await Site.SelectOneAsync(_TestObjAsync.ID);

                if (site?.ID > 0)
                    Trace.WriteLine($"FOUND Site: {site.ID}");
                else
                    Assert.Fail("Site NOT FOUND...");
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
                var site = Site.SelectOne(_TestObj.GUID);

                if (site?.ID > 0)
                    Trace.WriteLine($"FOUND Site: {site.ID}");
                else
                    Assert.Fail("Site NOT FOUND...");
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
                var site = await Site.SelectOneAsync(_TestObjAsync.GUID);

                if (site?.ID > 0)
                    Trace.WriteLine($"FOUND Site: {site.ID}");
                else
                    Assert.Fail("Site NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneBySearchPath()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var site = Site.SelectOne(_TestObj.DefaultFolder);

                if (site?.ID > 0)
                    Trace.WriteLine($"FOUND Site: {site.ID}");
                else
                    Assert.Fail("Site NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneBySearchPathAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var site = await Site.SelectOneAsync(_TestObjAsync.DefaultFolder);

                if (site?.ID > 0)
                    Trace.WriteLine($"FOUND Site: {site.ID}");
                else
                    Assert.Fail("Site NOT FOUND...");
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
                var sites = Site.SelectAll();

                if (sites?.Count > 0)
                    Trace.WriteLine($"FOUND Site: {sites.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Site NOT FOUND...");
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
                var sites = await Site.SelectAllAsync();

                if (sites?.Count > 0)
                    Trace.WriteLine($"FOUND Site: {sites.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Site NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectAllByTitle()
        {
            A_Create_TestObj();
            Assert.IsTrue(_TestObj?.ID > 0);

            // Test Folder is vereist voor deze selectie
            _TestFolderObj.SiteID = _TestObj.ID;
            _TestFolderObj.Save();

            try
            {
                // Function that we are testing BELOW...
                var sites = Site.SelectAll(_TestObj.Name);

                if (sites?.Count > 0)
                    Trace.WriteLine($"FOUND Site: {sites.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Site NOT FOUND...");
            }
            finally
            {
                // Delete test Folder
                var connector = ConnectorFactory.CreateConnector<Folder>();
                var folderID = _TestFolderObj.ID;
                connector.Delete(_TestFolderObj);
                var folder = Folder.SelectOne(folderID);
                if (folder?.ID > 0)
                {
                    Assert.Fail($"Test Folder not deleted, found {folder?.ID}");
                }

                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllByTitleAsync()
        {
            await A_Create_TestObjAsync();
            Assert.IsTrue(_TestObjAsync?.ID > 0);

            // Test Folder is vereist voor deze selectie
            _AsyncTestFolderObj.SiteID = _TestObjAsync.ID;
            await _AsyncTestFolderObj.SaveAsync();

            try
            {
                // Function that we are testing BELOW...
                var sites = await Site.SelectAllAsync(_TestObjAsync.Name);

                if (sites?.Count > 0)
                    Trace.WriteLine($"FOUND Site: {sites.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Site NOT FOUND...");
            }
            finally
            {
                // Delete test Folder
                var connector = ConnectorFactory.CreateConnector<Folder>();
                var folderID = _AsyncTestFolderObj.ID;
                await connector.DeleteAsync(_AsyncTestFolderObj);
                var folder = await Folder.SelectOneAsync(folderID);
                if (folder?.ID > 0)
                {
                    Assert.Fail($"Test Folder not deleted, found {folder?.ID}");
                }

                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void X_SelectAllAccessible()
        {
            var user = ApplicationUser.SelectOne("Mark Rienstra");

            // Function that we are testing BELOW...
            var sites = Site.SelectAllAccessible(user, AccessFilter.RoleAndUser);

            if (sites?.Length > 0)
                Trace.WriteLine($"FOUND Site: {sites.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Site NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllAccessibleAsync()
        {
            var user = ApplicationUser.SelectOne("Mark Rienstra");

            // Function that we are testing BELOW...
            var sites = await Site.SelectAllAccessibleAsync(user, AccessFilter.RoleAndUser);

            if (sites?.Length > 0)
                Trace.WriteLine($"FOUND Site: {sites.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Site NOT FOUND...");
        }
        #endregion Select All

        #region Delete
        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = Site.SelectAll();
            var ass = allAssert.Where(x => x.Name == _TestObj.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Site wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Site Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (Site a in ass)
            {
                // Booleans below need to be set to 'false' in order to delete the record...
                a.HasPages = false;
                a.HasLists = false;

                a.Delete();
                Trace.WriteLine($"DELETE Site: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = Site.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0)
                {
                    errorList.Add($"{a?.ID}"); // ERROR

                    // Manual Delete if in error, to ensure cleanup.
                    var connector = ConnectorFactory.CreateConnector<Site>();
                    connector.Delete(a);
                }
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Site not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await Site.SelectAllAsync();
            var ass = allAssert.Where(x => x.Name == _TestObjAsync.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Site wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Site Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (Site a in ass)
            {
                // Booleans below need to be set to 'false' in order to delete the record...
                a.HasPages = false;
                a.HasLists = false;

                await a.DeleteAsync();
                Trace.WriteLine($"DELETE Site: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = await Site.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0)
                {
                    errorList.Add($"{a?.ID}"); // ERROR

                    // Manual Delete if in error, to ensure cleanup.
                    var connector = ConnectorFactory.CreateConnector<Site>();
                    await connector.DeleteAsync(a);
                }
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Site not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<Site>();
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
            var connector = ConnectorFactory.CreateConnector<Site>();
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
