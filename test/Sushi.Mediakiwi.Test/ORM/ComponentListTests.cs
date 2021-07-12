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
    public class ComponentListTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_ComponentLists";
        private string _key = "ComponentList_Key";
        
        private ComponentList _TestObj = new ComponentList()
        {
            Name = "xUNIT TESTx",
            ReferenceID = 0,
            SingleItemName = "",
            IsVisible = false,
            AssemblyName = "Sushi.Mediakiwi.Tests.dll",
            ClassName = "Sushi.Mediakiwi.Tests.ORM.ComponentListTests",
            Icon = null,
            Description = "Test Asset voor UNIT TEST",
            GUID = new Guid("DF197673-A021-46AD-A59D-1E78330B4D48"),
            SiteID = 69,
            IsInherited = false,
            FolderID = 69420,
            Target = ComponentListTarget.List,
            Type = ComponentListType.Undefined,
            ComponentTemplateID = null,
            SenseInterval = null,
            SenseScheduled = null,
            HasOneChild = false,
            IsTemplate = false,
            CatalogID = 0,
            IsSingleInstance = false,
            Group = null,
            CanSortOrder = false,
            Class = null,
            SortOrder = 0
        };

        private ComponentList _TestObjAsync = new ComponentList()
        {
            Name = "xASYNC UNIT TESTx",
            ReferenceID = 0,
            SingleItemName = "",
            IsVisible = false,
            AssemblyName = "Sushi.Mediakiwi.Tests.dll",
            ClassName = "Sushi.Mediakiwi.Tests.ORM.ComponentListTests",
            Icon = null,
            Description = "Test Asset voor UNIT ASYNC TEST",
            GUID = new Guid("77E2F74B-E355-469D-9EFD-4D7C96A9FD63"),
            SiteID = 71,
            IsInherited = false,
            FolderID = 69421,
            Target = ComponentListTarget.List,
            Type = ComponentListType.Undefined,
            ComponentTemplateID = null,
            SenseInterval = null,
            SenseScheduled = null,
            HasOneChild = false,
            IsTemplate = false,
            CatalogID = 0,
            IsSingleInstance = false,
            Group = null,
            CanSortOrder = false,
            Class = null,
            SortOrder = 0
        };
        #endregion Test Data

        #region Create
        public ComponentList A_Create_TestObj()
        {
            var type = this.GetType();
            var db = (ComponentList)ComponentList.Add(
                _TestObj.GUID,
                type,
                _TestObj.Name,
                "",
                _TestObj.FolderID,
                _TestObj.SiteID,
                false,
                _TestObj.Description
                );

            Assert.IsTrue(db.ID > 0);
            _TestObj.ID = db.ID;

            Assert.AreEqual(_TestObj.GUID, db.GUID);

            if (db.ID > 0)
            {
                Trace.WriteLine($"CREATED ComponentList: {db.ID}");
            }

            return db;
        }

        public async Task<ComponentList> A_Create_TestObjAsync()
        {
            var type = this.GetType();
            var db = (ComponentList) await ComponentList.AddAsync(
                _TestObjAsync.GUID,
                type,
                _TestObjAsync.Name,
                "",
                _TestObjAsync.FolderID,
                _TestObjAsync.SiteID,
                false,
                _TestObjAsync.Description
                );

            Assert.IsTrue(db.ID > 0);
            _TestObjAsync.ID = db.ID;

            Assert.AreEqual(_TestObjAsync.GUID, db.GUID);

            if (db.ID > 0)
            {
                Trace.WriteLine($"CREATED ComponentList: {db.ID}");
            }

            return db;
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
                var componentList = ComponentList.SelectOne(_TestObj.ID);

                if (componentList?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentList: {componentList.ID}");
                else
                    Assert.Fail("ComponentList NOT FOUND...");
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
                var componentList = await ComponentList.SelectOneAsync(_TestObjAsync.ID);

                if (componentList?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentList: {componentList.ID}");
                else
                    Assert.Fail("ComponentList NOT FOUND...");
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
                var componentList = ComponentList.SelectOne(_TestObj.GUID);

                if (componentList?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentList: {componentList.ID}");
                else
                    Assert.Fail("ComponentList NOT FOUND...");
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
                var componentList = await ComponentList.SelectOneAsync(_TestObjAsync.GUID);

                if (componentList?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentList: {componentList.ID}");
                else
                    Assert.Fail("ComponentList NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }


        [TestMethod]
        public void B_SelectOneByPredefinedType()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentList = ComponentList.SelectOne(ComponentListType.Undefined);

                if (componentList?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentList: {componentList.ID}");
                else
                    Assert.Fail("ComponentList NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }


        [TestMethod]
        public async Task B_SelectOneByPredefinedTypeAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentList = await ComponentList.SelectOneAsync(ComponentListType.Undefined);

                if (componentList?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentList: {componentList.ID}");
                else
                    Assert.Fail("ComponentList NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByClassName()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentList = ComponentList.SelectOne(_TestObj.ClassName);

                if (componentList?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentList: {componentList.ID}");
                else
                    Assert.Fail("ComponentList NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }


        [TestMethod]
        public async Task B_SelectOneByClassNameAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentList = await ComponentList.SelectOneAsync(_TestObjAsync.ClassName);

                if (componentList?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentList: {componentList.ID}");
                else
                    Assert.Fail("ComponentList NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByType()
        {
            try
            {
                A_Create_TestObj();

                var type = typeof(ComponentListTests);
                // Function that we are testing BELOW...
                var componentList = ComponentList.SelectOne(type);

                if (componentList?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentList: {componentList.ID}");
                else
                    Assert.Fail("ComponentList NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByTypeAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                var type = typeof(ComponentListTests);
                // Function that we are testing BELOW...
                var componentList = await ComponentList.SelectOneAsync(type);

                if (componentList?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentList: {componentList.ID}");
                else
                    Assert.Fail("ComponentList NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByReference()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentList = ComponentList.SelectOneByReference(0);

                if (componentList?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentList: {componentList.ID}");
                else
                    Assert.Fail("ComponentList NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }


        [TestMethod]
        public async Task B_SelectOneByReferenceAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentList = await ComponentList.SelectOneByReferenceAsync(0);

                if (componentList?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentList: {componentList.ID}");
                else
                    Assert.Fail("ComponentList NOT FOUND...");
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
            var componentList = ComponentList.SelectAll();

            if (componentList?.Length > 0)
                Trace.WriteLine($"FOUND ComponentList: {componentList.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ComponentList NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllAsync()
        {
            // Function that we are testing BELOW...
            var componentList = await ComponentList.SelectAllAsync();

            if (componentList?.Length > 0)
                Trace.WriteLine($"FOUND ComponentList: {componentList.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ComponentList NOT FOUND...");
        }

        [TestMethod]
        public void B_SelectAllByFolderID()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentList = ComponentList.SelectAll((int)_TestObj.FolderID, true);

                if (componentList?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentList: {componentList.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("ComponentList NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllByFolderIDAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentList = await ComponentList.SelectAllAsync((int)_TestObjAsync.FolderID, true);

                if (componentList?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentList: {componentList.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("ComponentList NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectAllByText()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentList = ComponentList.SelectAll(_TestObj.Name, null);

                if (componentList?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentList: {componentList.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("ComponentList NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllByTextAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentList = await ComponentList.SelectAllAsync(_TestObjAsync.Name, null);

                if (componentList?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentList: {componentList.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("ComponentList NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void X_SelectAllByFolderAndUser()
        {
            int folderId = 2;
            var user = ApplicationUser.SelectOne("Mark Rienstra");
            // Function that we are testing BELOW...
            var componentList = ComponentList.SelectAll(folderId, user, true);

            if (componentList?.Length > 0)
                Trace.WriteLine($"FOUND ComponentList: {componentList.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ComponentList NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllByFolderAndUserAsync()
        {
            int folderId = 2;
            var user = ApplicationUser.SelectOne("Mark Rienstra");
            // Function that we are testing BELOW...
            var componentList = await ComponentList.SelectAllAsync(folderId, user, true);

            if (componentList?.Length > 0)
                Trace.WriteLine($"FOUND ComponentList: {componentList.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ComponentList NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectAllAccessible()
        {
            var user = ApplicationUser.SelectOne("Mark Rienstra");
            // Function that we are testing BELOW...
            var componentList = ComponentList.SelectAllAccessibleLists(user, RoleRightType.List);

            if (componentList?.Length > 0)
                Trace.WriteLine($"FOUND ComponentList: {componentList.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ComponentList NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllAccessibleAsync()
        {
            var user = ApplicationUser.SelectOne("Mark Rienstra");
            // Function that we are testing BELOW...
            var componentList = await ComponentList.SelectAllAccessibleListsAsync(user, RoleRightType.List);

            if (componentList?.Length > 0)
                Trace.WriteLine($"FOUND ComponentList: {componentList.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ComponentList NOT FOUND...");
        }

        [TestMethod]
        public void B_SelectAllBySite()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentList = ComponentList.SelectAllBySite((int)_TestObj.SiteID);

                if (componentList?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentList: {componentList.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("ComponentList NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllBySiteAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentList = await ComponentList.SelectAllBySiteAsync((int)_TestObjAsync.SiteID);

                if (componentList?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentList: {componentList.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("ComponentList NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }
        #endregion Select ALL

        #region Update

        [TestMethod]
        public void C_UpdateSortOrder()
        {
            try
            {
                A_Create_TestObj();

                var allAssert = ComponentList.SelectAll();
                var ass = allAssert.Where(x => x.Name == _TestObj.Name).FirstOrDefault();
                if (ass?.ID > 0)
                {
                    // Function that we are testing BELOW...
                    ComponentList.UpdateSortOrder(ass.ID, 69);

                    allAssert = ComponentList.SelectAll();
                    ass = allAssert.Where(x => x.Name == _TestObj.Name).FirstOrDefault();
                    Assert.AreEqual(69, ass.SortOrder);
                    Trace.WriteLine($"FOUND ComponentList SortOrder: {ass.SortOrder}");
                }
                else
                {
                    Assert.Fail("Test ComponentList wasn't found, you created it yet?");
                }
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task C_UpdateSortOrderAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                var allAssert = await ComponentList.SelectAllAsync();
                var ass = allAssert.Where(x => x.Name == _TestObjAsync.Name).FirstOrDefault();
                if (ass?.ID > 0)
                {
                    // Function that we are testing BELOW...
                    await ComponentList.UpdateSortOrderAsync(ass.ID, 69);

                    allAssert = await ComponentList.SelectAllAsync();
                    ass = allAssert.Where(x => x.Name == _TestObjAsync.Name).FirstOrDefault();
                    Assert.AreEqual(69, ass.SortOrder);
                    Trace.WriteLine($"FOUND ComponentList SortOrder: {ass.SortOrder}");
                }
                else
                {
                    Assert.Fail("Test ComponentList wasn't found, you created it yet?");
                }
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }
        #endregion Update

        #region Delete

        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = ComponentList.SelectAll();
            var ass = allAssert.Where(x => x.Name == _TestObj.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test ComponentList wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE ComponentList Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                if (a?.ID > 0)
                {
                    a.Delete();
                    Trace.WriteLine($"DELETE ComponentList: {a?.ID}");

                    // Check if delete in the DB is succesfull, for this record
                    var testDelete = ComponentList.SelectOne((int)a?.ID);
                    if (testDelete?.ID > 0) // ERROR
                        errorList.Add($"{a?.ID}");
                }
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test ComponentList not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all posible test records, we will delete them ALL
            var allAssert = await ComponentList.SelectAllAsync();
            var ass = allAssert.Where(x => x.Name == _TestObjAsync.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test ComponentList wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE ComponentList Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                if (a?.ID > 0)
                {
                    await a.DeleteAsync();
                    Trace.WriteLine($"DELETE ComponentList: {a?.ID}");

                    // Check if delete in the DB is succesfull, for this record
                    var testDelete = await ComponentList.SelectOneAsync((int)a?.ID);
                    if (testDelete?.ID > 0) // ERROR
                        errorList.Add($"{a?.ID}");
                }
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test ComponentList not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
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
            var connector = ConnectorFactory.CreateConnector<ComponentList>();
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
