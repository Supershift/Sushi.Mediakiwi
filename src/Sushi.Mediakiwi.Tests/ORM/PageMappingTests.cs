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
    public class PageMappingTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_PageMappings";
        private string _key = "PageMap_Key";

        private static DateTime _date = DateTime.Now;
        // Test object
        private PageMapping _TestObj = new PageMapping()
        {
            Path = "/test/path",
            Created = _date,
            ListID = 69420,
            ItemID = 69,
            TargetType = 1,
            TypeID = 2,
            AssetID = 666,
            PageID = 420,
            Query = "?test=1",
            Title = "xUNIT TESTx",
            IsActive = true
        };

        private PageMapping _TestObjAsync = new PageMapping()
        {
            Path = "/test/async/path",
            Created = _date,
            ListID = 69421,
            ItemID = 71,
            TargetType = 1,
            TypeID = 2,
            AssetID = 777,
            PageID = 420,
            Query = "?asynctest=1",
            Title = "xASYNC UNIT TESTx",
            IsActive = true
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj?.ID > 0);

            var db = (PageMapping)PageMapping.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.Title, db.Title);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED PageMapping: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            var db = (PageMapping)PageMapping.SelectOne(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.Title, db.Title);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED PageMapping: {_TestObjAsync.ID}");
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
                var pageMapping = PageMapping.SelectOne(_TestObj.ID);

                if (pageMapping?.ID > 0)
                    Trace.WriteLine($"FOUND PageMapping: {pageMapping.ID}");
                else
                    Assert.Fail("PageMapping NOT FOUND...");
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
                var pageMapping = await PageMapping.SelectOneAsync(_TestObjAsync.ID);

                if (pageMapping?.ID > 0)
                    Trace.WriteLine($"FOUND PageMapping: {pageMapping.ID}");
                else
                    Assert.Fail("PageMapping NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByListItemAndPage()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var pageMapping = PageMapping.SelectOne(_TestObj.ListID, (int)_TestObj.ItemID, _TestObj.PageID);

                if (pageMapping?.ID > 0)
                    Trace.WriteLine($"FOUND PageMapping: {pageMapping.ID}");
                else
                    Assert.Fail("PageMapping NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByListItemAndPageAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var pageMapping = await PageMapping.SelectOneAsync(_TestObjAsync.ListID, (int)_TestObjAsync.ItemID, _TestObjAsync.PageID);

                if (pageMapping?.ID > 0)
                    Trace.WriteLine($"FOUND PageMapping: {pageMapping.ID}");
                else
                    Assert.Fail("PageMapping NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByPageAndQuery()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var pageMapping = PageMapping.SelectOneByPageAndQuery(_TestObj.PageID, _TestObj.Query);

                if (pageMapping?.ID > 0)
                    Trace.WriteLine($"FOUND PageMapping: {pageMapping.ID}");
                else
                    Assert.Fail("PageMapping NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByPageAndQueryAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var pageMapping = await PageMapping.SelectOneByPageAndQueryAsync(_TestObjAsync.PageID, _TestObjAsync.Query);

                if (pageMapping?.ID > 0)
                    Trace.WriteLine($"FOUND PageMapping: {pageMapping.ID}");
                else
                    Assert.Fail("PageMapping NOT FOUND...");
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
            var pageMappings = PageMapping.SelectAll();

            if (pageMappings?.Length > 0)
                Trace.WriteLine($"FOUND PageMapping: {pageMappings.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("PageMapping NOT FOUND...");
        }


        [TestMethod]
        public async Task X_SelectAllAsync()
        {
            // Function that we are testing BELOW...
            var pageMappings = await PageMapping.SelectAllAsync();

            if (pageMappings?.Length > 0)
                Trace.WriteLine($"FOUND PageMapping: {pageMappings.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("PageMapping NOT FOUND...");
        }
        #endregion Select ALL

        #region Delete
        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = PageMapping.SelectAll();
            var ass = allAssert.Where(x => x.Title == _TestObj.Title);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test PageMapping wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE PageMapping Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (PageMapping a in ass)
            {
                a.Delete();
                Trace.WriteLine($"DELETE PageMapping: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = PageMapping.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test PageMapping not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await PageMapping.SelectAllAsync();
            var ass = allAssert.Where(x => x.Title == _TestObjAsync.Title);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test PageMapping wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE PageMapping Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (PageMapping a in ass)
            {
                await a.DeleteAsync();
                Trace.WriteLine($"DELETE PageMapping: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = await PageMapping.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test PageMapping not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
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
            var connector = ConnectorFactory.CreateConnector<PageMapping>();
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
