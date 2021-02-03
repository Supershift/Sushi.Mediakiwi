using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemanticComparison;
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
    public class MenuTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Menus";
        private string _key = "Menu_Key";

        private Menu _TestObj = new Menu()
        {
            Name = "xUNIT TESTx",
            SiteID = 69420,
            RoleID = 69420,
            IsActive = true
        };

        private Menu _TestObjAsync = new Menu()
        {
            Name = "xASYNC UNIT TESTx",
            SiteID = 69421,
            RoleID = 69421,
            IsActive = true
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj?.ID > 0);

            // MJ: check if saved db record is ok            
            var expected = new Likeness<Menu, Menu>(_TestObj);
            var db = Menu.SelectOne(_TestObj.ID);
            Assert.AreEqual(expected, db);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED Menu: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            // MJ: check if saved db record is ok            
            var expected = new Likeness<Menu, Menu>(_TestObjAsync);
            var db = Menu.SelectOne(_TestObjAsync.ID);
            Assert.AreEqual(expected, db);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED Menu: {_TestObjAsync.ID}");
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
                var menu = Menu.SelectOne(_TestObj.ID);

                if (menu?.ID > 0)
                    Trace.WriteLine($"FOUND Menu: {menu.ID}");
                else
                    Assert.Fail("Menu NOT FOUND...");
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
                var menu = await Menu.SelectOneAsync(_TestObjAsync.ID);

                if (menu?.ID > 0)
                    Trace.WriteLine($"FOUND Menu: {menu.ID}");
                else
                    Assert.Fail("Menu NOT FOUND...");
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
        public void X_SelectAll()
        {
            // Function that we are testing BELOW...
            var menus = Menu.SelectAll();

            if (menus?.Length > 0)
                Trace.WriteLine($"FOUND Menu: {menus.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Menu NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllAsync()
        {
            // Function that we are testing BELOW...
            var menus = await Menu.SelectAllAsync();

            if (menus?.Length > 0)
                Trace.WriteLine($"FOUND Menu: {menus.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Menu NOT FOUND...");
        }
        #endregion Select All

        #region Delete
        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = Menu.SelectAll();
            var ass = allAssert.Where(x => x.Name == _TestObj.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Menu wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Menu Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                a.Delete();
                Trace.WriteLine($"DELETE Menu: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = Menu.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Menu not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await Menu.SelectAllAsync();
            var ass = allAssert.Where(x => x.Name == _TestObjAsync.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Menu wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Menu Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                await a.DeleteAsync();
                Trace.WriteLine($"DELETE Menu: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = await Menu.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Menu not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<Menu>();
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
            var connector = ConnectorFactory.CreateConnector<Menu>();
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
