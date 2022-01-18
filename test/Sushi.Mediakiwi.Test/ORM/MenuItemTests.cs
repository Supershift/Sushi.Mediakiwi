using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemanticComparison;
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
    public class MenuItemItemTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_MenuItems";
        private string _key = "MenuItem_Key";

        private MenuItem _TestObj = new MenuItem()
        {
            MenuID = 69420,
            DashboardID = 69420,
            ItemID = 69420,
            Position = 69420,
            Sort = 69420,
            TypeID = 69420
        };

        private MenuItem _TestObjAsync = new MenuItem()
        {
            MenuID = 69421,
            DashboardID = 69421,
            ItemID = 69421,
            Position = 69421,
            Sort = 69421,
            TypeID = 69421
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj?.ID > 0);

            // MJ: check if saved db record is ok            
            var expected = new Likeness<MenuItem, MenuItem>(_TestObj);
            var db = MenuItem.SelectAll(_TestObj.MenuID);
            MenuItem ass = (MenuItem)db.Where(x => x.ItemID == _TestObj.ItemID).FirstOrDefault();
            Assert.AreEqual(expected, ass);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED MenuItem: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            // MJ: check if saved db record is ok            
            var expected = new Likeness<MenuItem, MenuItem>(_TestObjAsync);
            var db = MenuItem.SelectAll(_TestObjAsync.MenuID);
            MenuItem ass = (MenuItem)db.Where(x => x.ItemID == _TestObjAsync.ItemID).FirstOrDefault();
            Assert.AreEqual(expected, ass);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED MenuItem: {_TestObjAsync.ID}");
            }
        }
        #endregion Create

        #region Select All
        [TestMethod]
        public void B_SelectAllForMenu()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var menuItems = MenuItem.SelectAll(_TestObj.MenuID);

                if (menuItems?.Length > 0)
                    Trace.WriteLine($"FOUND MenuItem: {menuItems.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("MenuItem NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllForMenuAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var menuItems = await MenuItem.SelectAllAsync(_TestObjAsync.MenuID);

                if (menuItems?.Length > 0)
                    Trace.WriteLine($"FOUND MenuItem: {menuItems.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("MenuItem NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectAllForDashBoard()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var menuItems = MenuItem.SelectAll_Dashboard(_TestObj.DashboardID);

                if (menuItems?.Length > 0)
                    Trace.WriteLine($"FOUND MenuItem: {menuItems.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("MenuItem NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllForDashBoardAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var menuItems = await MenuItem.SelectAll_DashboardAsync(_TestObjAsync.DashboardID);

                if (menuItems?.Length > 0)
                    Trace.WriteLine($"FOUND MenuItem: {menuItems.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("MenuItem NOT FOUND...");
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
            var db = MenuItem.SelectAll(_TestObj.MenuID);
            var ass = db.Where(x => x.ItemID == _TestObj.ItemID);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test MenuItem wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE MenuItem Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (MenuItem a in ass)
            {
                a.Delete();
                Trace.WriteLine($"DELETE MenuItem: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                // Replace SelectOne function when this gets added.
                var connector = ConnectorFactory.CreateConnector<MenuItem>();
                var testDelete = connector.FetchSingle((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test MenuItem not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var db = MenuItem.SelectAll(_TestObjAsync.MenuID);
            var ass = db.Where(x => x.ItemID == _TestObjAsync.ItemID);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test MenuItem wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE MenuItem Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                await a.DeleteAsync();
                Trace.WriteLine($"DELETE MenuItem: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                // Replace SelectOne function when this gets added.
                var connector = ConnectorFactory.CreateConnector<MenuItem>();
                var testDelete = await connector.FetchSingleAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test MenuItem not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<MenuItem>();
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
            var connector = ConnectorFactory.CreateConnector<MenuItem>();
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
