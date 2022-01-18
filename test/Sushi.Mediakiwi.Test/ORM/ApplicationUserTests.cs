using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.MicroORM;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Test.ORM
{
    [TestClass]
    public class ApplicationUserTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Users";
        private string _key = "User_Key";

        // Test object
        private ApplicationUser _TestObj = new ApplicationUser()
        {
            Displayname = "xTEST USERs",
            Email = "xTESTUSERx@mailinator.com",
            IsActive = true,
            IsDeveloper = true,
            Language = 1,
            LastLoggedVisit = DateTime.Now.AddDays(-1),
            Name = "xTEST USERs",
            RememberMe = false,
            Type = 1
        };
        // Test async object
        private ApplicationUser _TestObjAsync = new ApplicationUser()
        {
            Displayname = "xTEST ASYNC USERs",
            Email = "xTESTASYNCUSERx@mailinator.com",
            IsActive = true,
            IsDeveloper = true,
            Language = 1,
            LastLoggedVisit = DateTime.Now.AddDays(-1),
            Name = "xTEST ASYNC USERs",
            RememberMe = false,
            Type = 1
        };
        #endregion Test Data

        #region Create
        public IApplicationUser A_CreateTestUser()
        {
            _TestObj.ApplyPassword("test123D#221");
            _TestObj.RoleID = ApplicationRole.SelectAll().FirstOrDefault().ID;
            try
            {
                _TestObj.Save();
            }
            catch (Exception e)
            {
                Assert.Fail($"ApplicationUser Failed due to: {e.Message}");
            }

            Assert.IsTrue(_TestObj?.ID > 0);

            if (_TestObj?.ID > 0)
            {
                Trace.WriteLine($"CREATED ApplicationUser: {_TestObj.ID}");
            }

            return ApplicationUser.Select(_TestObj.Email);
        }

        public async Task<IApplicationUser> A_CreateTestUserAsync()
        {
            _TestObjAsync.ApplyPassword("test123D#221");
            var roles = await ApplicationRole.SelectAllAsync();
            _TestObjAsync.RoleID = roles.FirstOrDefault().ID;

            try
            {
                await _TestObjAsync.SaveAsync();
            }
            catch (Exception e)
            {
                Assert.Fail($"ApplicationUser Failed due to: {e.Message}");
            }

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            if (_TestObjAsync?.ID > 0)
            {
                Trace.WriteLine($"CREATED ApplicationUser: {_TestObjAsync.ID}");
            }

            return await ApplicationUser.SelectAsync(_TestObjAsync.Email);
        }
        #endregion Create

        #region Select ONE
        [TestMethod]
        public void X_SelectOneByID()
        {
            // Function that we are testing BELOW...
            var User = ApplicationUser.SelectOne(4);

            Assert.IsFalse(User == null || User.IsNewInstance, "ApplicationUser NOT FOUND...");
            if (User.IsNewInstance == false)
                Trace.WriteLine($"FOUND ApplicationUser: {User.ID}");
        }


        [TestMethod]
        public async Task X_SelectOneByIDAsync()
        {
            // Function that we are testing BELOW...
            var User = await ApplicationUser.SelectOneAsync(4);

            Assert.IsFalse(User == null || User.IsNewInstance, "ApplicationUser NOT FOUND...");
            if (User.IsNewInstance == false)
                Trace.WriteLine($"FOUND ApplicationUser: {User.ID}");
        }


        [TestMethod]
        public void B_SelectOnyByEmail()
        {
            try
            {
                A_CreateTestUser();

                // Function that we are testing BELOW...
                var User = ApplicationUser.Select(_TestObj.Email);

                if (User?.IsNewInstance == false && User?.ID > 0)
                    Trace.WriteLine($"FOUND ApplicationUser: {User.ID}");
                else
                    Assert.Fail("ApplicationUser NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }


        [TestMethod]
        public async Task B_SelectOnyByEmailAsync()
        {
            try
            {
                await A_CreateTestUserAsync();

                // Function that we are testing BELOW...
                var User = await ApplicationUser.SelectAsync(_TestObjAsync.Email);

                if (User?.IsNewInstance == false && User?.ID > 0)
                    Trace.WriteLine($"FOUND ApplicationUser: {User.ID}");
                else
                    Assert.Fail("ApplicationUser NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByUserName()
        {
            try
            {
                A_CreateTestUser();

                // Function that we are testing BELOW...
                var User = ApplicationUser.SelectOne(_TestObj.Name);

                if (User?.IsNewInstance == false && User?.ID > 0)
                    Trace.WriteLine($"FOUND ApplicationUser: {User.ID}");
                else
                    Assert.Fail("ApplicationUser NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByUserNameAsync()
        {
            try
            {
                await A_CreateTestUserAsync();

                // Function that we are testing BELOW...
                var User = await ApplicationUser.SelectOneAsync(_TestObjAsync.Name);

                if (User?.IsNewInstance == false && User?.ID > 0)
                    Trace.WriteLine($"FOUND ApplicationUser: {User.ID}");
                else
                    Assert.Fail("ApplicationUser NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByUserNameIgnoreApplicationUserID()
        {
            try
            {
                A_CreateTestUser();
                await A_CreateTestUserAsync();

                // Test setup
                var NormalUser = ApplicationUser.SelectOne(_TestObj.Name);
                var AsyncUser = ApplicationUser.SelectOne(_TestObjAsync.Name);

                // Function that we are testing BELOW...
                var User = ApplicationUser.SelectOne(NormalUser.Name, AsyncUser.ID);

                if (User?.IsNewInstance == false && User?.ID > 0)
                    Trace.WriteLine($"FOUND ApplicationUser: {User.ID}");
                else
                    Assert.Fail("ApplicationUser NOT FOUND...");

                Assert.AreEqual(User.ID, NormalUser.ID);
            }
            finally
            {
                D_Delete_TestObj();
                await D_Delete_TestObjAsync();

                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByUserNameIgnoreApplicationUserIDAsync()
        {
            try
            {
                A_CreateTestUser();
                await A_CreateTestUserAsync();

                // Test setup
                var NormalUser = ApplicationUser.SelectOne(_TestObj.Name);
                var AsyncUser = ApplicationUser.SelectOne(_TestObjAsync.Name);

                // Function that we are testing BELOW...
                var User = await ApplicationUser.SelectOneAsync(AsyncUser.Name, NormalUser.ID);

                if (User?.IsNewInstance == false && User?.ID > 0)
                    Trace.WriteLine($"FOUND ApplicationUser: {User.ID}");
                else
                    Assert.Fail("ApplicationUser NOT FOUND...");

                Assert.AreEqual(User.ID, AsyncUser.ID);
            }
            finally
            {
                D_Delete_TestObj();
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByEmail()
        {
            try
            {
                A_CreateTestUser();

                // Function that we are testing BELOW...
                var User = ApplicationUser.SelectOneByEmail(_TestObj.Email);

                Assert.IsFalse(User == null || User.IsNewInstance, "ApplicationUser NOT FOUND...");
                if (User.IsNewInstance == false && User.ID > 0)
                    Trace.WriteLine($"FOUND ApplicationUser: {User.ID}");
                else
                    Assert.Fail("ApplicationUser NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByEmailAsync()
        {
            try
            {
                await A_CreateTestUserAsync();

                // Function that we are testing BELOW...
                var User = await ApplicationUser.SelectOneByEmailAsync(_TestObjAsync.Email);

                if (User?.IsNewInstance == false && User?.ID > 0)
                    Trace.WriteLine($"FOUND ApplicationUser: {User.ID}");
                else
                    Assert.Fail("ApplicationUser NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void X_SelectOneByEmailIgnoreApplicationUserID()
        {
            string email = "mark.rienstra@supershift.nl";

            // Function that we are testing BELOW...
            var User = ApplicationUser.SelectOneByEmail(email, 1);

            if (User?.IsNewInstance == false && User?.ID > 0)
                Trace.WriteLine($"FOUND ApplicationUser: {User.ID}");
            else
                Assert.Fail("ApplicationUser NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectOneByEmailIgnoreApplicationUserIDAsync()
        {
            string email = "mark.rienstra@supershift.nl";

            // Function that we are testing BELOW...
            var User = await ApplicationUser.SelectOneByEmailAsync(email, 1);

            if (User?.IsNewInstance == false && User?.ID > 0)
                Trace.WriteLine($"FOUND ApplicationUser: {User.ID}");
            else
                Assert.Fail("ApplicationUser NOT FOUND...");
        }

        [TestMethod]
        public void B_SelectOneByGUID()
        {
            try
            {
                A_CreateTestUser();

                // Function that we are testing BELOW...
                var User = ApplicationUser.SelectOne(_TestObj.GUID);

                if (User?.IsNewInstance == false && User?.ID > 0)
                    Trace.WriteLine($"FOUND ApplicationUser: {User.ID}");
                else
                    Assert.Fail("ApplicationUser NOT FOUND...");
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
                await A_CreateTestUserAsync();

                // Function that we are testing BELOW...
                var User = await ApplicationUser.SelectOneAsync(_TestObjAsync.GUID);

                if (User?.IsNewInstance == false && User?.ID > 0)
                    Trace.WriteLine($"FOUND ApplicationUser: {User.ID}");
                else
                    Assert.Fail("ApplicationUser NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }


        [TestMethod]
        public void X_SelectOneByUserNamePassword()
        {
            string userName = "Mark Rienstra";
            string passWord = "a1999322edb3237c004c8863c074decc";

            // Function that we are testing BELOW...
            var User = ApplicationUser.SelectOne(userName, passWord);

            if (User?.IsNewInstance == false && User?.ID > 0)
                Trace.WriteLine($"FOUND ApplicationUser: {User.ID}");
            else
                Assert.Fail("ApplicationUser NOT FOUND...");
        }


        [TestMethod]
        public async Task X_SelectOneByUserNamePasswordAsync()
        {
            string userName = "Mark Rienstra";
            string passWord = "a1999322edb3237c004c8863c074decc";

            // Function that we are testing BELOW...
            var User = await ApplicationUser.SelectOneAsync(userName, passWord);

            if (User?.IsNewInstance == false && User?.ID > 0)
                Trace.WriteLine($"FOUND ApplicationUser: {User.ID}");
            else
                Assert.Fail("ApplicationUser NOT FOUND...");
        }

        #endregion

        #region Select ALL

        [TestMethod]
        public void X_SelectAll()
        {
            // Function that we are testing BELOW...
            var Users = ApplicationUser.SelectAll();

            Assert.IsFalse(Users == null, "ApplicationUser List is NULL");
            if (Users.Length > 0)
                Trace.WriteLine($"FOUND ApplicationUser: {Users.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ApplicationUser NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllAsync()
        {
            // Function that we are testing BELOW...
            var Users = await ApplicationUser.SelectAllAsync();

            Assert.IsFalse(Users == null, "ApplicationUser List is NULL");
            if (Users.Length > 0)
                Trace.WriteLine($"FOUND ApplicationUser: {Users.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ApplicationUser NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectAllForRole()
        {
            int RoleId = 1;

            // Function that we are testing BELOW...
            var Users = ApplicationUser.SelectAll(RoleId);

            Assert.IsFalse(Users == null, "ApplicationUser List is NULL");
            if (Users.Length > 0)
                Trace.WriteLine($"FOUND ApplicationUser: {Users.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ApplicationUser NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllForRoleAsync()
        {
            int RoleId = 1;

            // Function that we are testing BELOW...
            var Users = await ApplicationUser.SelectAllAsync(RoleId);

            Assert.IsFalse(Users == null, "ApplicationUser List is NULL");
            if (Users.Length > 0)
                Trace.WriteLine($"FOUND ApplicationUser: {Users.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ApplicationUser NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectAllForUsernameAndRole()
        {
            int RoleId = 1;
            string userName = "Mark Rienstra";

            // Function that we are testing BELOW...
            var Users = ApplicationUser.SelectAll(userName, RoleId);

            Assert.IsFalse(Users == null, "ApplicationUser List is NULL");
            if (Users.Length > 0)
                Trace.WriteLine($"FOUND ApplicationUser: {Users.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ApplicationUser NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllForUsernameAndRoleAsync()
        {
            int RoleId = 1;
            string userName = "Mark Rienstra";

            // Function that we are testing BELOW...
            var Users = await ApplicationUser.SelectAllAsync(userName, RoleId);

            Assert.IsFalse(Users == null, "ApplicationUser List is NULL");
            if (Users.Length > 0)
                Trace.WriteLine($"FOUND ApplicationUser: {Users.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ApplicationUser NOT FOUND...");
        }

        [TestMethod]
        public void X_HasUserName()
        {
            int userId = 4;
            string userName = "Mark Rienstra";

            // Function that we are testing BELOW...
            var Users = ApplicationUser.HasUserName(userName, userId);

            Assert.IsFalse(Users, $"ApplicationUser with userName '{userName}' NOT FOUND...");
        }

        [TestMethod]
        public async Task X_HasUserNameAsync()
        {
            int userId = 4;
            string userName = "Mark Rienstra";

            // Function that we are testing BELOW...
            var Users = await ApplicationUser.HasUserNameAsync(userName, userId);

            Assert.IsFalse(Users, $"ApplicationUser with userName '{userName}' NOT FOUND...");
        }

        [TestMethod]
        public void X_HasUserNameOnUser()
        {
            string userName = "Mark Rienstra";

            // Function that we are testing BELOW...
            var User = ApplicationUser.SelectOne(userName);
            Assert.IsFalse(User == null || User.IsNewInstance);

            // Test Below if User.HasUserName
            Assert.IsFalse(User.HasUserName(userName), $"ApplicationUser with userName '{userName}' NOT FOUND...");
        }

        [TestMethod]
        public void X_HasEmail()
        {
            int userId = 4;
            string email = "mark.rienstra@supershift.nl";

            // Function that we are testing BELOW...
            var Users = ApplicationUser.HasEmail(email, userId);

            Assert.IsFalse(Users, $"ApplicationUser with email '{email}' NOT FOUND...");
        }

        [TestMethod]
        public async Task X_HasEmailAsync()
        {
            int userId = 4;
            string email = "mark.rienstra@supershift.nl";

            // Function that we are testing BELOW...
            var Users = await ApplicationUser.HasEmailAsync(email, userId);

            Assert.IsFalse(Users, $"ApplicationUser with email '{email}' NOT FOUND...");
        }

        [TestMethod]
        public void X_HasEmailOnUser()
        {
            string email = "mark.rienstra@supershift.nl";

            // Function that we are testing BELOW...
            var User = ApplicationUser.SelectOneByEmail(email);

            if (User?.IsNewInstance == false && User?.ID > 0)
                Trace.WriteLine($"FOUND ApplicationUser: {User.ID}");
            else
                Assert.Fail("ApplicationUser NOT FOUND...");

            // Test if User.HasUserName
            Assert.IsFalse(User.HasUserName(email), $"ApplicationUser with email '{email}' NOT FOUND...");
        }

        [TestMethod]
        public void X_GetAlternativeNameSuggestion()
        {
            int userId = 4;
            string userName = "Mark Rienstra";
            // Function that we are testing BELOW...
            var Users = ApplicationUser.GetAlternativeNameSuggestion(userName, userId);
            Assert.AreEqual(Users, "Mark Rienstra");

            userId = 0;
            userName = "Mark Rienstra";
            // Function that we are testing BELOW...
            Users = ApplicationUser.GetAlternativeNameSuggestion(userName, userId);
            Assert.AreEqual(Users, "Mark Rienstra1");
        }

        [TestMethod]
        public void X_SelectAllForRoleActive()
        {
            int RoleId = 1;

            // Function that we are testing BELOW...
            var Users = ApplicationUser.SelectAll(RoleId, true);

            Assert.IsFalse(Users == null, "ApplicationUser List is NULL");
            if (Users.Length > 0)
                Trace.WriteLine($"FOUND ApplicationUser: {Users.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
        }

        [TestMethod]
        public async Task X_SelectAllForRoleActiveAsync()
        {
            int RoleId = 1;

            // Function that we are testing BELOW...
            var Users = await ApplicationUser.SelectAllAsync(RoleId, true);

            Assert.IsFalse(Users == null, "ApplicationUser List is NULL");
            if (Users.Length > 0)
                Trace.WriteLine($"FOUND ApplicationUser: {Users.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
        }
        #endregion Select ALL

        #region Update
        [TestMethod]
        public void C_UpdateTestUser()
        {
            try
            {
                A_CreateTestUser();
                var tUser = ApplicationUser.SelectOne(_TestObj.Name);
                if (tUser?.ID > 0)
                {
                    tUser.IsDeveloper = false;
                    // Function that we are testing BELOW...
                    tUser.Save();

                    tUser = ApplicationUser.SelectOne(_TestObj.Name);
                    Assert.IsFalse(tUser.IsDeveloper);
                }
                else
                {
                    Assert.Fail("Test user wasn't found, you created it yet?");
                }
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task C_UpdateTestUserAsync()
        {
            try
            {
                await A_CreateTestUserAsync();
                var tUser = ApplicationUser.SelectOne(_TestObjAsync.Name);
                if (tUser?.ID > 0)
                {
                    tUser.IsDeveloper = false;
                    // Function that we are testing BELOW...
                    await tUser.SaveAsync();

                    tUser = ApplicationUser.SelectOne(_TestObjAsync.Name);
                    Assert.IsFalse(tUser.IsDeveloper);
                }
                else
                {
                    Assert.Fail("Test user wasn't found, you created it yet?");
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
            // There Should only be one user with this name
            var tUser = ApplicationUser.SelectOne(_TestObj.Name);
            if (tUser?.ID > 0)
            {
                Trace.WriteLine($"DELETE ApplicationUser: {tUser?.ID}");
                Assert.IsTrue(tUser.Delete());

                // Check if delete in the DB is succesfull
                var testDelete = ApplicationUser.SelectOne((int)tUser?.ID);
                if (testDelete?.ID > 0) // ERROR
                    Assert.Fail($"Test ApplicationUser not deleted, found {tUser?.ID}");
                else
                    Assert.IsTrue(true);
            }
            else
            {
                Assert.Fail("Test ApplicationUser wasn't found, you created it yet?");
            }
        }

        public async Task D_Delete_TestObjAsync()
        {
            // There Should only be one user with this name
            var tUser = await ApplicationUser.SelectOneAsync(_TestObjAsync.Name);
            if (tUser?.ID > 0)
            {
                Trace.WriteLine($"DELETE ApplicationUser: {tUser?.ID}");
                Assert.IsTrue(await tUser.DeleteAsync());

                // Check if delete in the DB is succesfull
                var testDelete = await ApplicationUser.SelectOneAsync((int)tUser?.ID);
                if (testDelete?.ID > 0) // ERROR
                {
                    Assert.Fail($"Test ApplicationUser not deleted, found {tUser?.ID}");
                }
                else
                    Assert.IsTrue(true);
            }
            else
            {
                Assert.Fail("Test ApplicationUser wasn't found, you created it yet?");
            }
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
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
            var connector = ConnectorFactory.CreateConnector<ApplicationUser>();
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
