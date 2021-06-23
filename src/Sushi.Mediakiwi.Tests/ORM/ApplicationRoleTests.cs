using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemanticComparison;
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
    public class ApplicationRoleTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Roles";
        private string _key = "Role_Key";

        private ApplicationRole _TestObj = new ApplicationRole()
        {
            All_Folders = true,
            All_Galleries = true,
            All_Lists = true,
            All_Sites = true,

            CanChangeList = true,
            CanChangePage = true,
            CanCreateList = true,
            CanCreatePage = true,
            CanDeletePage = true,
            CanPublishPage = true,
            CanSeeAdmin = true,
            CanSeeGallery = true,
            CanSeeList = true,
            CanSeePage = true,
            Dashboard = 0,
            Description = "Test Rol voor UNIT TEST",

            IsAccessFolder = true,
            IsAccessGallery = true,
            IsAccessList = true,
            IsAccessSite = true,

            Name = "xUNIT TESTx"
        };

        private ApplicationRole _TestObjAsync = new ApplicationRole()
        {
            All_Folders = true,
            All_Galleries = true,
            All_Lists = true,
            All_Sites = true,

            CanChangeList = true,
            CanChangePage = true,
            CanCreateList = true,
            CanCreatePage = true,
            CanDeletePage = true,
            CanPublishPage = true,
            CanSeeAdmin = true,
            CanSeeGallery = true,
            CanSeeList = true,
            CanSeePage = true,
            Dashboard = 0,
            Description = "Test Rol voor UNIT TEST",

            IsAccessFolder = true,
            IsAccessGallery = true,
            IsAccessList = true,
            IsAccessSite = true,     

            Name = "xASYNC UNIT TESTx"
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj.ID > 0);

            // Check if saved DB record is ok with SemanticComparison
            var expected = new Likeness<ApplicationRole, ApplicationRole>(_TestObj);
            ApplicationRole db = (ApplicationRole)ApplicationRole.SelectOne(_TestObj.ID);
            Assert.AreEqual(expected, db);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED ApplicationRole: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync.ID > 0);

            // Check if saved DB record is ok with SemanticComparison
            var expected = new Likeness<ApplicationRole, ApplicationRole>(_TestObjAsync);
            var db = await ApplicationRole.SelectOneAsync(_TestObjAsync.ID);
            Assert.AreEqual(expected, (ApplicationRole)db);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED ApplicationRole: {_TestObjAsync.ID}");
            }
        }
        #endregion Create

        #region Select One
        [TestMethod]
        public void B_SelectOneByID()
        {
            try
            {
                A_Create_TestObj();

                var allRoles = ApplicationRole.SelectAll();
                var role = allRoles.Where(x => x.Name == _TestObj.Name).FirstOrDefault();
                if (role?.ID > 0)
                {
                    // Function that we are testing BELOW...
                    var Role = ApplicationRole.SelectOne((int)role?.ID);

                    if (Role.IsNewInstance == false && Role.ID > 0)
                        Trace.WriteLine($"FOUND ApplicationRole: {Role.ID}");
                    else
                        Assert.Fail("ApplicationRole NOT FOUND...");
                }
                else
                {
                    Assert.Fail("Test ApplicationRole wasn't found, you created it yet?");
                }
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

                var allRoles = ApplicationRole.SelectAll();
                var role = allRoles.Where(x => x.Name == _TestObjAsync.Name).FirstOrDefault();
                if (role?.ID > 0)
                {
                    // Function that we are testing BELOW...
                    var Role = await ApplicationRole.SelectOneAsync((int)role?.ID);

                    if (Role.IsNewInstance == false && Role.ID > 0)
                        Trace.WriteLine($"FOUND ApplicationRole: {Role.ID}");
                    else
                        Assert.Fail("ApplicationRole NOT FOUND...");
                }
                else
                {
                    Assert.Fail("Test ApplicationRole wasn't found, you created it yet?");
                }
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }
        #endregion Select One

        #region Select All
        [TestMethod]
        public void X_SelectAllForFolder()
        {
            int appFolderID = 10;
            // Function that we are testing BELOW...
            var Roles = ApplicationRole.SelectAll(appFolderID);

            if (Roles?.Length > 0)
                Trace.WriteLine($"FOUND ApplicationRole: {Roles.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ApplicationRole NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllForFolderAsync()
        {
            int appFolderID = 10;
            // Function that we are testing BELOW...
            var Roles = await ApplicationRole.SelectAllAsync(appFolderID);

            if (Roles?.Length > 0)
                Trace.WriteLine($"FOUND ApplicationRole: {Roles.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ApplicationRole NOT FOUND...");
        }
        #endregion Select All

        #region Select Assets

        [TestMethod]
        public void X_SelectFolders()
        {
            IApplicationUser user = ApplicationUser.SelectOne(1);
            var Role = ApplicationRole.SelectOne(1);
            Assert.IsFalse(Role.IsNewInstance);

            // Function that we are testing BELOW...
            var folders = Role.Folders(user);

            if (folders?.Length > 0)
                Trace.WriteLine($"FOUND ApplicationRole: {folders.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ApplicationRole NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectGalleries()
        {
            IApplicationUser user = ApplicationUser.SelectOne(1);
            var Role = ApplicationRole.SelectOne(1);
            Assert.IsFalse(Role.IsNewInstance);

            // Function that we are testing BELOW...
            var galleries = Role.Galleries(user);

            if (galleries?.Length > 0)
                Trace.WriteLine($"FOUND ApplicationRole: {galleries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ApplicationRole NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectLists()
        {
            IApplicationUser user = ApplicationUser.SelectOne(1);
            var Role = ApplicationRole.SelectOne(1);
            Assert.IsFalse(Role.IsNewInstance);

            // Function that we are testing BELOW...
            var lists = Role.Lists(user);

            if (lists?.Length > 0)
                Trace.WriteLine($"FOUND ApplicationRole: {lists.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ApplicationRole NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectSites()
        {
            IApplicationUser user = ApplicationUser.SelectOne(1);
            var Role = ApplicationRole.SelectOne(1);
            Assert.IsFalse(Role.IsNewInstance);

            // Function that we are testing BELOW...
            var sites = Role.Sites(user);

            if (sites?.Length > 0)
                Trace.WriteLine($"FOUND ApplicationRole: {sites.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ApplicationRole NOT FOUND...");
        }
        #endregion Select Assets

        #region Update 
        [TestMethod]
        public void C_UpdateTestRole()
        {
            try
            {
                A_Create_TestObj();

                var allRoles = ApplicationRole.SelectAll();
                var role = allRoles.Where(x => x.Name == _TestObj.Name).FirstOrDefault();

                if (role?.ID > 0)
                {
                    role.IsAccessFolder = false;
                    // Function that we are testing BELOW...
                    role.Save();

                    var test = ApplicationRole.SelectOne(role.ID);
                    Assert.AreEqual(test.GUID, role.GUID);
                    Assert.IsFalse(test.IsAccessFolder);
                }
                else
                {
                    Assert.Fail("Test Role wasn't found, you created it yet?");
                }
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task C_UpdateTestRoleAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                var allRoles = await ApplicationRole.SelectAllAsync();
                var role = allRoles.Where(x => x.Name == _TestObjAsync.Name).FirstOrDefault();

                if (role?.ID > 0)
                {
                    role.IsAccessFolder = false;
                    // Function that we are testing BELOW...
                    await role.SaveAsync();
                    Assert.IsTrue(true);

                    var test = await ApplicationRole.SelectOneAsync(role.ID);
                    Assert.AreEqual(test.GUID, role.GUID);
                    Assert.IsFalse(test.IsAccessFolder);
                }
                else
                {
                    Assert.Fail("Test Role wasn't found, you created it yet?");
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
            var allAssert = ApplicationRole.SelectAll();
            var ass = allAssert.Where(x => x.Name == _TestObj.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test ApplicationRole wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE ApplicationRole Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (ApplicationRole a in ass)
            {
                if (a?.ID > 0)
                {
                    // ApplicationRole does not contain a Delete(), Replace code below when Delete() is added.
                    var connector = ConnectorFactory.CreateConnector<ApplicationRole>();
                    connector.Delete(a);
                    Trace.WriteLine($"DELETE ApplicationRole: {a?.ID}");

                    // Check if delete in the DB is succesfull, for this record
                    connector = ConnectorFactory.CreateConnector<ApplicationRole>();
                    var testDelete = ApplicationRole.SelectOne((int)a?.ID);
                    if (testDelete?.ID > 0) // ERROR
                        errorList.Add($"{a?.ID}");
                }
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test ApplicationRole not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all posible test records, we will delete them ALL
            var allAssert = await ApplicationRole.SelectAllAsync();
            var ass = allAssert.Where(x => x.Name == _TestObjAsync.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test ApplicationRole wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE ApplicationRole Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (ApplicationRole a in ass)
            {
                if (a?.ID > 0)
                {
                    // ApplicationRole does not contain a DeleteAsync(), Replace code below when DeleteAsync() is added.
                    var connector = ConnectorFactory.CreateConnector<ApplicationRole>();
                    await connector.DeleteAsync(a);
                    Trace.WriteLine($"DELETE ApplicationRole: {a?.ID}");

                    // Check if delete in the DB is succesfull, for this record
                    connector = ConnectorFactory.CreateConnector<ApplicationRole>();
                    var testDelete = await ApplicationRole.SelectOneAsync((int)a?.ID);
                    if (testDelete?.ID > 0) // ERROR
                        errorList.Add($"{a?.ID}");
                }
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test ApplicationRole not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationRole>();
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
            var connector = ConnectorFactory.CreateConnector<ApplicationRole>();
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
