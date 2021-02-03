using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class PageTemplateTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_PageTemplates";
        private string _key = "PageTemplate_Key";

        private static DateTime _date = DateTime.Now;

        private PageTemplate _TestObj = new PageTemplate()
        {
            GUID = new Guid("61911E59-9AB7-47EE-BBBB-70790FD605D2"),
            SiteID = 69,
            OverwriteSiteKey = 69420,
            OverwriteTemplateKey = 666,
            IsSourceBased = false,
            DataString = @"<ArrayOfData xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><Data></Data></ArrayOfData>",
            Description = "Unit_Test",
            Name = "xUNIT TESTx",
            OnlyOneInstancePossible = true,
            HasCustomDate = false,
            IsAddedOutputCache = false,
            OutputCacheDuration = null,
            HasSecundaryContentContainer = false,
            Location = "/Templates/Page/UnitTest.aspx",
            LastWriteTimeUtc = _date,
            Source = null
        };

        private PageTemplate _TestObjAsync = new PageTemplate()
        {
            GUID = new Guid("B83A39F4-8283-4AF9-8E77-2977E9D978F1"),
            SiteID = 71,
            OverwriteSiteKey = 69421,
            OverwriteTemplateKey = 777,
            IsSourceBased = false,
            DataString = @"<ArrayOfData xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><Data></Data></ArrayOfData>",
            Description = "Async_Unit_Test",
            Name = "xASYNC UNIT TESTx",
            OnlyOneInstancePossible = true,
            HasCustomDate = false,
            IsAddedOutputCache = false,
            OutputCacheDuration = null,
            HasSecundaryContentContainer = false,
            Location = "/Templates/Page/AsyncUnitTest.aspx",
            LastWriteTimeUtc = _date,
            Source = null
        };
        #endregion Test Data

        #region  Create
        public void A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj?.ID > 0);

            PageTemplate db = PageTemplate.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.ID, db.ID);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED PageTemplate: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            var db = await PageTemplate.SelectOneAsync(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.ID, db.ID);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED PageTemplate: {_TestObjAsync.ID}");
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
                var pageTemplate = PageTemplate.SelectOne(_TestObj.ID);

                if (pageTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND PageTemplate: {pageTemplate.ID}");
                else
                    Assert.Fail("PageTemplate NOT FOUND...");
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
                var pageTemplate = await PageTemplate.SelectOneAsync(_TestObjAsync.ID);

                if (pageTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND PageTemplate: {pageTemplate.ID}");
                else
                    Assert.Fail("PageTemplate NOT FOUND...");
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
                var pageTemplate = PageTemplate.SelectOne(_TestObj.GUID);

                if (pageTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND PageTemplate: {pageTemplate.ID}");
                else
                    Assert.Fail("PageTemplate NOT FOUND...");
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
                var pageTemplate = await PageTemplate.SelectOneAsync(_TestObjAsync.GUID);

                if (pageTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND PageTemplate: {pageTemplate.ID}");
                else
                    Assert.Fail("PageTemplate NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByLocation()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var pageTemplate = PageTemplate.SelectOne(_TestObj.Location);

                if (pageTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND PageTemplate: {pageTemplate.ID}");
                else
                    Assert.Fail("PageTemplate NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByLocationAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var pageTemplate = await PageTemplate.SelectOneAsync(_TestObjAsync.Location);

                if (pageTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND PageTemplate: {pageTemplate.ID}");
                else
                    Assert.Fail("PageTemplate NOT FOUND...");
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
                var pageTemplate = PageTemplate.SelectOneByReference(_TestObj.ReferenceID);

                if (pageTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND PageTemplate: {pageTemplate.ID}");
                else
                    Assert.Fail("PageTemplate NOT FOUND...");
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
                var pageTemplate = await PageTemplate.SelectOneByReferenceAsync(_TestObjAsync.ReferenceID);

                if (pageTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND PageTemplate: {pageTemplate.ID}");
                else
                    Assert.Fail("PageTemplate NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneOverwrite()
        {
            _TestObj.OverwriteTemplateKey = 666;

            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var pageTemplate = PageTemplate.SelectOneOverwrite((int)_TestObj.OverwriteSiteKey, (int)_TestObj.OverwriteTemplateKey);

                if (pageTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND PageTemplate: {pageTemplate.ID}");
                else
                    Assert.Fail("PageTemplate NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }

        }

        [TestMethod]
        public async Task B_SelectOneOverwriteAsync()
        {
            _TestObjAsync.OverwriteTemplateKey = 747;

            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var pageTemplate = await PageTemplate.SelectOneOverwriteAsync((int)_TestObjAsync.OverwriteSiteKey, (int)_TestObjAsync.OverwriteTemplateKey);

                if (pageTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND PageTemplate: {pageTemplate.ID}");
                else
                    Assert.Fail("PageTemplate NOT FOUND...");
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
            var pageTemplates = PageTemplate.SelectAll();

            if (pageTemplates?.Length > 0)
                Trace.WriteLine($"FOUND PageTemplate: {pageTemplates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("PageTemplate NOT FOUND...");
        }


        [TestMethod]
        public async Task X_SelectAllAsync()
        {
            // Function that we are testing BELOW...
            var pageTemplates = await PageTemplate.SelectAllAsync();

            if (pageTemplates?.Length > 0)
                Trace.WriteLine($"FOUND PageTemplate: {pageTemplates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("PageTemplate NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectAllSortedByName()
        {
            // Function that we are testing BELOW...
            var pageTemplates = PageTemplate.SelectAllSortedByName();

            if (pageTemplates?.Length > 0)
                Trace.WriteLine($"FOUND PageTemplate: {pageTemplates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("PageTemplate NOT FOUND...");
        }


        [TestMethod]
        public async Task X_SelectAllSortedByNameAsync()
        {
            // Function that we are testing BELOW...
            var pageTemplates = await PageTemplate.SelectAllSortedByNameAsync();

            if (pageTemplates?.Length > 0)
                Trace.WriteLine($"FOUND PageTemplate: {pageTemplates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("PageTemplate NOT FOUND...");
        }

        #endregion Select ALL

        #region Delete
        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = PageTemplate.SelectAll();
            var ass = allAssert.Where(x => x.Name == _TestObj.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test PageTemplate wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE PageTemplate Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                a.Delete();
                Trace.WriteLine($"DELETE PageTemplate: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = PageTemplate.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test PageTemplate not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await PageTemplate.SelectAllAsync();
            var ass = allAssert.Where(x => x.Name == _TestObjAsync.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test PageTemplate wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE PageTemplate Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                await a.DeleteAsync();
                Trace.WriteLine($"DELETE PageTemplate: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = await PageTemplate.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test PageTemplate not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
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
            var connector = ConnectorFactory.CreateConnector<PageTemplate>();
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
