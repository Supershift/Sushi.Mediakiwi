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
    public class PageVersionTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_PageVersions";
        private string _key = "PageVersion_Key";

        private static DateTime _date = DateTime.Now;
        // Test object
        private PageVersion _TestObj = new PageVersion()
        {
            PageID = 69420,
            TemplateID = 1,
            Created = _date,
            ContentXML = @"<ArrayOfComponentVersion xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><ComponentVersion></ComponentVersion></ArrayOfComponentVersion>",
            MetaDataXML = @"<Page xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><Template></Template><Folder></Folder></Page>",
            Name = "xUNIT TESTx",
            CompletePath = "/unit/test",
            Hash = "9d4fdd73709892ce583ff371bf1c6dc6",
            IsArchived = false
        };
        // Async Test object
        private PageVersion _TestObjAsync = new PageVersion()
        {
            PageID = 69421,
            TemplateID = 1,
            Created = _date,
            ContentXML = @"<ArrayOfComponentVersion xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><ComponentVersion></ComponentVersion></ArrayOfComponentVersion>",
            MetaDataXML = @"<Page xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><Template></Template><Folder></Folder></Page>",
            Name = "xASYNC UNIT TESTx",
            CompletePath = "/unit/async/test",
            Hash = "695729ff9810d9407dc48910d941660d",
            IsArchived = false
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj?.ID > 0);

            PageVersion db = (PageVersion)PageVersion.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.Hash, db.Hash);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED PageVersion: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            PageVersion db = (PageVersion)PageVersion.SelectOne(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.Hash, db.Hash);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED PageVersion: {_TestObjAsync.ID}");
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
                var pageVersion = PageVersion.SelectOne(_TestObj.ID);

                if (pageVersion?.ID > 0)
                    Trace.WriteLine($"FOUND PageVersion: {pageVersion.ID}");
                else
                    Assert.Fail("PageVersion NOT FOUND...");
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
                var pageVersion = await PageVersion.SelectOneAsync(_TestObjAsync.ID);

                if (pageVersion?.ID > 0)
                    Trace.WriteLine($"FOUND PageVersion: {pageVersion.ID}");
                else
                    Assert.Fail("PageVersion NOT FOUND...");
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
        public void B_SelectAllForPage()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var pageVersions = PageVersion.SelectAllOfPage(_TestObj.PageID);

                if (pageVersions?.Count > 0)
                    Trace.WriteLine($"FOUND PageVersion: {pageVersions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("PageVersion NOT FOUND...");
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
                var pageVersions = await PageVersion.SelectAllOfPageAsync(_TestObjAsync.PageID);

                if (pageVersions?.Count > 0)
                    Trace.WriteLine($"FOUND PageVersion: {pageVersions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("PageVersion NOT FOUND...");
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
            var allAssert = PageVersion.SelectAllOfPage(_TestObj.PageID);
            var ass = allAssert.Where(x => x.Name == _TestObj.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test PageVersion wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE PageVersion Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (PageVersion a in ass)
            {
                // PageVersion does not contain a Delete(), Replace code below when Delete() is added.
                var connector = ConnectorFactory.CreateConnector<PageVersion>();
                connector.Delete(a);
                Trace.WriteLine($"DELETE PageVersion: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = PageVersion.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test PageVersion not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await PageVersion.SelectAllOfPageAsync(_TestObjAsync.PageID);
            var ass = allAssert.Where(x => x.Name == _TestObjAsync.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test PageVersion wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE PageVersion Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (PageVersion a in ass)
            {
                // PageVersion does not contain a DeleteAsync(), Replace code below when DeleteAsync() is added.
                var connector = ConnectorFactory.CreateConnector<PageVersion>();
                await connector.DeleteAsync(a);
                Trace.WriteLine($"DELETE PageVersion: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = await PageVersion.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test PageVersion not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<PageVersion>();
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
            var connector = ConnectorFactory.CreateConnector<PageVersion>();
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
