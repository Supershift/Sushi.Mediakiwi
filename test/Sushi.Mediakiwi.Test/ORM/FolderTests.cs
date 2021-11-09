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
    public class FolderTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Folders";
        private string _key = "Folder_Key";

        private static DateTime _date = DateTime.Now;
        // Test object
        private Folder _TestObj = new Folder()
        {
            GUID = new Guid("6559A237-CBAC-4F40-B215-DB42774F1CAB"),
            ParentID = 69,
            SortOrderMethod = 0,
            MasterID = 42,
            IsVisible = false,
            SiteID = 69,
            Type = FolderType.Administration,
            Name = "xUNIT TESTx",
            Description = "Test Asset voor UNIT TEST",
            CompletePath = "/test/path/",
            Changed = _date
        };

        private Folder _TestObjAsync = new Folder()
        {
            GUID = new Guid("C82DA682-CD1A-4CF2-8361-CAB9119F8EB6"),
            ParentID = 70,
            SortOrderMethod = 0,
            MasterID = 43,
            IsVisible = false,
            SiteID = 70,
            Type = FolderType.Administration,
            Name = "xASYNC UNIT TESTx",
            Description = "Test Asset voor UNIT ASYNC TEST",
            CompletePath = "/test/async/path/",
            Changed = _date
        };
        #endregion Test Data

        #region Create
        public Folder A_Create_TestObj(int siteID = 0)
        {
            if (siteID > 0)
                _TestObj.SiteID = siteID;

            _TestObj.Save();

            Assert.IsTrue(_TestObj?.ID > 0);

            Folder db = Folder.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.Name, db.Name);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED Folder: {_TestObj.ID}");
            }

            return _TestObj;
        }

        public async Task<Folder> A_Create_TestObjAsync(int siteID = 0)
        {
            if (siteID > 0)
                _TestObjAsync.SiteID = siteID;

            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            Folder db = Folder.SelectOne(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.Name, db.Name);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED Folder: {_TestObjAsync.ID}");
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
                var folder = Folder.SelectOne(_TestObj.ID);

                if (folder?.ID > 0)
                    Trace.WriteLine($"FOUND Folder: {folder.ID}");
                else
                    Assert.Fail("Folder NOT FOUND...");
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
                var folder = await Folder.SelectOneAsync(_TestObjAsync.ID);

                if (folder?.ID > 0)
                    Trace.WriteLine($"FOUND Folder: {folder.ID}");
                else
                    Assert.Fail("Folder NOT FOUND...");
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
                var folder = Folder.SelectOne(_TestObj.GUID);

                if (folder?.ID > 0)
                    Trace.WriteLine($"FOUND Folder: {folder.ID}");
                else
                    Assert.Fail("Folder NOT FOUND...");
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
                var folder = await Folder.SelectOneAsync(_TestObjAsync.GUID);

                if (folder?.ID > 0)
                    Trace.WriteLine($"FOUND Folder: {folder.ID}");
                else
                    Assert.Fail("Folder NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByMasterAndSiteID()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var folder = Folder.SelectOne((int)_TestObj.MasterID, _TestObj.SiteID);

                if (folder?.ID > 0)
                    Trace.WriteLine($"FOUND Folder: {folder.ID}");
                else
                    Assert.Fail("Folder NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByMasterAndSiteIDAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var folder = await Folder.SelectOneAsync((int)_TestObjAsync.MasterID, _TestObjAsync.SiteID);

                if (folder?.ID > 0)
                    Trace.WriteLine($"FOUND Folder: {folder.ID}");
                else
                    Assert.Fail("Folder NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByParentAndName()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var folder = Folder.SelectOne((int)_TestObj.ParentID, _TestObj.Name);

                if (folder?.ID > 0)
                    Trace.WriteLine($"FOUND Folder: {folder.ID}");
                else
                    Assert.Fail("Folder NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByParentAndNameAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var folder = await Folder.SelectOneAsync((int)_TestObjAsync.ParentID, _TestObjAsync.Name);

                if (folder?.ID > 0)
                    Trace.WriteLine($"FOUND Folder: {folder.ID}");
                else
                    Assert.Fail("Folder NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneBySiteAndType()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var folder = Folder.SelectOneBySite(_TestObj.SiteID, _TestObj.Type);

                if (folder?.ID > 0)
                    Trace.WriteLine($"FOUND Folder: {folder.ID}");
                else
                    Assert.Fail("Folder NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }


        [TestMethod]
        public async Task B_SelectOneBySiteAndTypeAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var folder = await Folder.SelectOneBySiteAsync(_TestObjAsync.SiteID, _TestObjAsync.Type);

                if (folder?.ID > 0)
                    Trace.WriteLine($"FOUND Folder: {folder.ID}");
                else
                    Assert.Fail("Folder NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneChildByFolderAndSite()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var folder = Folder.SelectOneChild(_TestObj.ID, _TestObj.SiteID);

                if (folder?.ID > 0)
                    Trace.WriteLine($"FOUND Folder: {folder.ID}");
                else
                    Assert.Fail("Folder NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneChildByFolderAndSiteAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var folder = await Folder.SelectOneChildAsync(_TestObjAsync.ID, _TestObjAsync.SiteID);

                if (folder?.ID > 0)
                    Trace.WriteLine($"FOUND Folder: {folder.ID}");
                else
                    Assert.Fail("Folder NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        #endregion Select ONE

        #region Select ALL

        [TestMethod]
        public void X_SelectAll()
        {
            // Function that we are testing BELOW...
            var folders = Folder.SelectAll();

            if (folders?.Length > 0)
                Trace.WriteLine($"FOUND Folder: {folders.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Folder NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllAsync()
        {
            // Function that we are testing BELOW...
            var folders = await Folder.SelectAllAsync();

            if (folders?.Length > 0)
                Trace.WriteLine($"FOUND Folder: {folders.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Folder NOT FOUND...");
        }

        [TestMethod]
        public void B_SelectAllByTypeAndSite()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var folders = Folder.SelectAll(_TestObj.Type, _TestObj.SiteID);

                if (folders?.Length > 0)
                    Trace.WriteLine($"FOUND Folder: {folders.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Folder NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllByTypeAndSiteAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var folders = await Folder.SelectAllAsync(_TestObjAsync.Type, _TestObjAsync.SiteID);

                if (folders?.Length > 0)
                    Trace.WriteLine($"FOUND Folder: {folders.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Folder NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void X_SelectAllByIDs()
        {
            int[] IDs = new int[] { 10, 11, 12, 13 };

            // Function that we are testing BELOW...
            var folders = Folder.SelectAll(IDs);

            if (folders?.Length > 0)
                Trace.WriteLine($"FOUND Folder: {folders.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Folder NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllByIDsAsync()
        {
            int[] IDs = new int[] { 10, 11, 12, 13 };

            // Function that we are testing BELOW...
            var folders = await Folder.SelectAllAsync(IDs);

            if (folders?.Length > 0)
                Trace.WriteLine($"FOUND Folder: {folders.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Folder NOT FOUND...");
        }


        [TestMethod]
        public void X_SelectAllByTypeSiteAndQuery()
        {
            int siteId = 2;
            string searchQuery = "Ondernemers";

            // Function that we are testing BELOW...
            var folders = Folder.SelectAll(FolderType.List, siteId, searchQuery, true);

            if (folders?.Length > 0)
                Trace.WriteLine($"FOUND Folder: {folders.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Folder NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllByTypeSiteAndQueryAsync()
        {
            int siteId = 2;
            string searchQuery = "Ondernemers";

            // Function that we are testing BELOW...
            var folders = await Folder.SelectAllAsync(FolderType.List, siteId, searchQuery, true);

            if (folders?.Length > 0)
                Trace.WriteLine($"FOUND Folder: {folders.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Folder NOT FOUND...");
        }


        [TestMethod]
        public void X_SelectAllAccessibleForUser()
        {
            var user = ApplicationUser.SelectOne("Mark Rienstra");

            // Function that we are testing BELOW...
            var folders = Folder.SelectAllAccessible(user);

            if (folders?.Length > 0)
                Trace.WriteLine($"FOUND Folder: {folders.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Folder NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllAccessibleForUserAsync()
        {
            var user = ApplicationUser.SelectOne("Mark Rienstra");

            // Function that we are testing BELOW...
            var folders = await Folder.SelectAllAccessibleAsync(user);

            if (folders?.Length > 0)
                Trace.WriteLine($"FOUND Folder: {folders.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Folder NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectAllByBackwardTrail()
        {
            var topFolderId = 10;

            // Function that we are testing BELOW...
            var folders = Folder.SelectAllByBackwardTrail(topFolderId);

            if (folders?.Length > 0)
                Trace.WriteLine($"FOUND Folder: {folders.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Folder NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllByBackwardTrailAsync()
        {
            var topFolderId = 10;

            // Function that we are testing BELOW...
            var folders = await Folder.SelectAllByBackwardTrailAsync(topFolderId);

            if (folders?.Length > 0)
                Trace.WriteLine($"FOUND Folder: {folders.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Folder NOT FOUND...");
        }


        [TestMethod]
        public void X_SelectAllByParent()
        {
            var parentFolderId = 7;

            // Function that we are testing BELOW...
            var folders = Folder.SelectAllByParent(parentFolderId);

            if (folders?.Length > 0)
                Trace.WriteLine($"FOUND Folder: {folders.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Folder NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllByParentAsync()
        {
            var parentFolderId = 7;

            // Function that we are testing BELOW...
            var folders = await Folder.SelectAllByParentAsync(parentFolderId);

            if (folders?.Length > 0)
                Trace.WriteLine($"FOUND Folder: {folders.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Folder NOT FOUND...");
        }

        #endregion Select ALL

        #region Delete
        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();
            bool isError = false;

            // Get all test records, we will delete them ALL
            var allAssert = Folder.SelectAll();
            var ass = allAssert.Where(x => x.Name == _TestObj.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Folder wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Folder Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                try
                {
                    a.Delete();
                    Trace.WriteLine($"DELETE Folder: {a?.ID}");
                }
                catch (Exception e)
                {
                    isError = true;

                    // If there is a problem with the Folder.Delete(), try to delete via normal connector.
                    var connectorE = ConnectorFactory.CreateConnector<Folder>();
                    connectorE.Delete(a);
                }

                // Check if delete in the DB is succesfull, for this record
                var connector = ConnectorFactory.CreateConnector<Folder>();
                var testDelete = Folder.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (isError)
                Assert.Fail("Folder.Delete() failed.");
            if (errorList.Count > 0)
                Assert.Fail($"Test Folder not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");

        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();
            bool isError = false;

            // Get all test records, we will delete them ALL
            var allAssert = await Folder.SelectAllAsync();
            var ass = allAssert.Where(x => x.Name == _TestObjAsync.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Folder wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Folder Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                try
                {
                    await a.DeleteAsync();
                    Trace.WriteLine($"DELETE Folder: {a?.ID}");
                }
                catch (Exception)
                {
                    isError = true;

                    // If there is a problem with the Folder.DeleteAsync(), try to delete via normal connector.
                    var connectorE = ConnectorFactory.CreateConnector<Folder>();
                    await connectorE.DeleteAsync(a);
                }

                // Check if delete in the DB is succesfull, for this record
                var connector = ConnectorFactory.CreateConnector<Folder>();
                var testDelete = await Folder.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (isError)
                Assert.Fail("Folder.DeleteAsync() failed.");
            if (errorList.Count > 0)
                Assert.Fail($"Test Folder not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<Folder>();
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
            var connector = ConnectorFactory.CreateConnector<Folder>();
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
