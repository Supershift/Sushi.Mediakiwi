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
    public class PageTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Pages";
        private string _key = "Page_Key";

        private static DateTime _date = DateTime.Now;
        // Test object
        private Page _TestObj = new Page()
        {
            GUID = new Guid("0B9DC1D0-7A84-4116-A1B8-A50F349932A4"), //  = new Guid("BFB67281-961E-4549-B53E-B5DA392BEAF1"),
            Name = "xUNIT TESTx",
            LinkText = "LINK UNIT TEST",
            Title = "TITLE UNIT TEST",
            Keywords = null,
            Description = "unit test",
            FolderID = 69420,
            SubFolderID = 69,
            TemplateID = 666,
            MasterID = null,
            Created = _date,
            Published = _date.AddDays(-1),
            Updated = _date,
            InheritContent = false,
            InheritContentEdited = false,
            IsSearchable = false,
            IsFixed = false,
            IsPublished = true,
            IsFolderDefault = false,
            IsSecure = false,
            Publication = _date.AddDays(-1),
            Expiration = _date.AddDays(1),
            CustomDate = _date,
            InternalPath = "/unit/test",
            SortOrder = 0
        };
        // Async test object
        private Page _TestObjAsync = new Page()
        {
            GUID = new Guid("BFB67281-961E-4549-B53E-B5DA392BEAF1"),
            Name = "xASYNC UNIT TESTx",
            LinkText = "LINK ASYNC UNIT TEST",
            Title = "TITLE ASYNC UNIT TEST",
            Keywords = null,
            Description = "async unit test",
            FolderID = 71420,
            SubFolderID = 71,
            TemplateID = 777,
            MasterID = null,
            Created = _date,
            Published = _date.AddDays(-1),
            Updated = _date,
            InheritContent = false,
            InheritContentEdited = false,
            IsSearchable = false,
            IsFixed = false,
            IsPublished = true,
            IsFolderDefault = false,
            IsSecure = false,
            Publication = _date.AddDays(-1),
            Expiration = _date.AddDays(1),
            CustomDate = _date,
            InternalPath = "/unit/async/test",
            SortOrder = 0
        };
        #endregion Test Data

        #region  Create
        public void A_Create_TestObj()
        {
            _TestObj.Insert();

            Assert.IsTrue(_TestObj?.ID > 0);

            Page db = Page.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.ID, db.ID);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED Page: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.InsertAsync();

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            var db = await Page.SelectOneAsync(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.ID, db.ID);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED Page: {_TestObjAsync.ID}");
            }
        }
        #endregion Create

        #region Other

        [TestMethod]
        public void B_IsPageAlreadyTaken()
        {
            try
            {
                A_Create_TestObj();

                Page page = Page.SelectOne(_TestObj.ID);
                // Function that we are testing BELOW...
                var result = page.IsPageAlreadyTaken(_TestObj.FolderID, _TestObj.Name);
                Assert.IsTrue(result);
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_IsPageAlreadyTakenAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                Page page = Page.SelectOne(_TestObjAsync.ID);
                // Function that we are testing BELOW...
                var result = await page.IsPageAlreadyTakenAsync(_TestObjAsync.FolderID, _TestObjAsync.Name);
                Assert.IsTrue(result);
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }
        #endregion Other

        #region Select ONE
        [TestMethod]
        public void B_SelectOneByID()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var page = Page.SelectOne(_TestObj.ID);

                if (page?.ID > 0)
                    Trace.WriteLine($"FOUND Page: {page.ID}");
                else
                    Assert.Fail("Page NOT FOUND...");
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
                var page = await Page.SelectOneAsync(_TestObjAsync.ID);

                if (page?.ID > 0)
                    Trace.WriteLine($"FOUND Page: {page.ID}");
                else
                    Assert.Fail("Page NOT FOUND...");
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
                var page = Page.SelectOne(_TestObj.GUID);

                if (page?.ID > 0)
                    Trace.WriteLine($"FOUND Page: {page.ID}");
                else
                    Assert.Fail("Page NOT FOUND...");
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
                var page = await Page.SelectOneAsync(_TestObjAsync.GUID);

                if (page?.ID > 0)
                    Trace.WriteLine($"FOUND Page: {page.ID}");
                else
                    Assert.Fail("Page NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByName()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var page = Page.SelectOneBasedOnName("test", true);

                if (page?.ID > 0)
                    Trace.WriteLine($"FOUND Page: {page.ID}");
                else
                    Assert.Fail("Page NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByNameAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var page = await Page.SelectOneBasedOnNameAsync("test", true);

                if (page?.ID > 0)
                    Trace.WriteLine($"FOUND Page: {page.ID}");
                else
                    Assert.Fail("Page NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }


        [TestMethod]
        public void B_SelectOneBySubFolder()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var page = Page.SelectOneBySubFolder(_TestObj.SubFolderID, true);

                if (page?.ID > 0)
                    Trace.WriteLine($"FOUND Page: {page.ID}");
                else
                    Assert.Fail("Page NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneBySubFolderAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var page = await Page.SelectOneBySubFolderAsync(_TestObjAsync.SubFolderID, true);

                if (page?.ID > 0)
                    Trace.WriteLine($"FOUND Page: {page.ID}");
                else
                    Assert.Fail("Page NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }


        [TestMethod]
        public void B_SelectOneDefault()
        {
            _TestObj.IsFolderDefault = true;

            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var page = Page.SelectOneDefault(_TestObj.FolderID, true);

                if (page?.ID > 0)
                    Trace.WriteLine($"FOUND Page: {page.ID}");
                else
                    Assert.Fail("Page NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneDefaultAsync()
        {
            _TestObjAsync.IsFolderDefault = true;

            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var page = await Page.SelectOneDefaultAsync(_TestObjAsync.FolderID, true);

                if (page?.ID > 0)
                    Trace.WriteLine($"FOUND Page: {page.ID}");
                else
                    Assert.Fail("Page NOT FOUND...");
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
            var pages = Page.SelectAll();

            if (pages?.Length > 0)
                Trace.WriteLine($"FOUND Page: {pages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Page NOT FOUND...");
        }


        [TestMethod]
        public async Task X_SelectAllAsync()
        {
            // Function that we are testing BELOW...
            var pages = await Page.SelectAllAsync();

            if (pages?.Length > 0)
                Trace.WriteLine($"FOUND Page: {pages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Page NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectAllByIDs()
        {
            List<int> IDs = new List<int>() { 5, 6, 7, 9, 10, 11 };

            // Function that we are testing BELOW...
            var pages = Page.SelectAll(IDs.ToArray());

            if (pages?.Length > 0)
                Trace.WriteLine($"FOUND Page: {pages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Page NOT FOUND...");
        }


        [TestMethod]
        public async Task X_SelectAllByIDsAsync()
        {
            List<int> IDs = new List<int>() { 5, 6, 7, 9, 10, 11 };

            // Function that we are testing BELOW...
            var pages = await Page.SelectAllAsync(IDs.ToArray());

            if (pages?.Length > 0)
                Trace.WriteLine($"FOUND Page: {pages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Page NOT FOUND...");
        }


        [TestMethod]
        public void B_SelectAllBySearch()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var pages = Page.SelectAll(_TestObj.Description, true);

                if (pages?.Length > 0)
                    Trace.WriteLine($"FOUND Page: {pages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Page NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllBySearchAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var pages = await Page.SelectAllAsync(_TestObjAsync.Description, true);

                if (pages?.Length > 0)
                    Trace.WriteLine($"FOUND Page: {pages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Page NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectAllByPageTemplate()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var pages = Page.SelectAllBasedOnPageTemplate(_TestObj.TemplateID);

                if (pages?.Length > 0)
                    Trace.WriteLine($"FOUND Page: {pages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Page NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }


        [TestMethod]
        public async Task B_SelectAllByPageTemplateAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var pages = await Page.SelectAllBasedOnPageTemplateAsync(_TestObjAsync.TemplateID);

                if (pages?.Length > 0)
                    Trace.WriteLine($"FOUND Page: {pages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Page NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void X_SelectAllByPageTemplates()
        {
            List<int> pageTemplateIds = new List<int>() { 1, 13 };

            // Function that we are testing BELOW...
            var pages = Page.SelectAllBasedOnPageTemplate(pageTemplateIds.ToArray());

            if (pages?.Length > 0)
                Trace.WriteLine($"FOUND Page: {pages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Page NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllByPageTemplatesAsync()
        {
            List<int> pageTemplateIds = new List<int>() { 1, 13 };

            // Function that we are testing BELOW...
            var pages = await Page.SelectAllBasedOnPageTemplateAsync(pageTemplateIds.ToArray());

            if (pages?.Length > 0)
                Trace.WriteLine($"FOUND Page: {pages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Page NOT FOUND...");
        }

        [TestMethod]
        public void B_SelectAllByCustomDateFolder()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var pages = Page.SelectAllByCustomDate(_TestObj.FolderID);

                if (pages?.Length > 0)
                    Trace.WriteLine($"FOUND Page: {pages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Page NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllByCustomDateFolderAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var pages = await Page.SelectAllByCustomDateAsync(_TestObjAsync.FolderID);

                if (pages?.Length > 0)
                    Trace.WriteLine($"FOUND Page: {pages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Page NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectAllBySiteID()
        {
            // Voor deze test is een Site nodig
            SiteTests siteTest = new SiteTests();
            Site site = siteTest.A_Create_TestObj();

            // Voor deze test is een Folder nodig
            FolderTests folderTest = new FolderTests();
            Folder folder = folderTest.A_Create_TestObj(site.ID);
            _TestObj.FolderID = folder.ID;

            try
            {
                A_Create_TestObj();

                // Get record with the JOIN
                _TestObj = Page.SelectOne(_TestObj.ID);

                // Function that we are testing BELOW...
                var pages = Page.SelectAllBySite(_TestObj.SiteID);

                if (pages?.Length > 0)
                    Trace.WriteLine($"FOUND Page: {pages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Page NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                folderTest.D_Delete_TestObj();
                siteTest.D_Delete_TestObj();

                F_Reset_AutoIndent();
                folderTest.F_Reset_AutoIndent();
                siteTest.F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllBySiteIDAsync()
        {
            // Voor deze test is een Site nodig
            SiteTests siteTest = new SiteTests();
            Site site = await siteTest.A_Create_TestObjAsync();

            // Voor deze test is een Folder nodig
            FolderTests folderTest = new FolderTests();
            Folder folder = await folderTest.A_Create_TestObjAsync(site.ID);
            _TestObjAsync.FolderID = folder.ID;

            try
            {
                await A_Create_TestObjAsync();

                // Get record with the JOIN
                _TestObjAsync = await Page.SelectOneAsync(_TestObjAsync.ID);

                // Function that we are testing BELOW...
                var pages = await Page.SelectAllBySiteAsync(_TestObjAsync.SiteID);

                if (pages?.Length > 0)
                    Trace.WriteLine($"FOUND Page: {pages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Page NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await folderTest.D_Delete_TestObjAsync();
                await siteTest.D_Delete_TestObjAsync();

                await F_Reset_AutoIndentAsync();
                await folderTest.F_Reset_AutoIndentAsync();
                await siteTest.F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void X_SelectAllDated()
        {
            // Function that we are testing BELOW...
            var pages = Page.SelectAllDated();

            if (pages?.Length > 0)
                Trace.WriteLine($"FOUND Page: {pages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Page NOT FOUND...");
        }


        [TestMethod]
        public async Task X_SelectAllDatedAsync()
        {
            // Function that we are testing BELOW...
            var pages = await Page.SelectAllDatedAsync();

            if (pages?.Length > 0)
                Trace.WriteLine($"FOUND Page: {pages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Page NOT FOUND...");
        }

        [TestMethod]
        public void B_SelectAllChildren()
        {
            _TestObj.MasterID = 123456;

            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var pages = Page.SelectAllChildren((int)_TestObj.MasterID);

                if (pages?.Length > 0)
                    Trace.WriteLine($"FOUND Page: {pages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Page NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllChildrenAsync()
        {
            _TestObjAsync.MasterID = 234567;

            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var pages = await Page.SelectAllChildrenAsync((int)_TestObjAsync.MasterID);

                if (pages?.Length > 0)
                    Trace.WriteLine($"FOUND Page: {pages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Page NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectAllByCustomDate()
        {
            var SecondObj = _TestObj;

            A_Create_TestObj(); // Create first

            SecondObj.CustomDate = _date.AddDays(1);
            _TestObj = SecondObj;
            A_Create_TestObj(); // Create second

            try
            {
                // Function that we are testing BELOW...
                var pages = Page.SelectAllByCustomDate(_TestObj.FolderID, _TestObj.TemplateID, true, 10).ToList();
                Assert.IsFalse(pages == null || pages.Count == 0);

                Assert.IsTrue((DateTime)pages.FirstOrDefault().CustomDate > (DateTime)pages.LastOrDefault().CustomDate);
            }
            finally
            {
                D_Delete_TestObj(); // This will delete both records
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllByCustomDateAsync()
        {
            var SecondObj = _TestObjAsync;

            await A_Create_TestObjAsync(); // Create first

            SecondObj.CustomDate = _date.AddDays(1);
            _TestObjAsync = SecondObj;
            await A_Create_TestObjAsync(); // Create second

            try
            {
                // Function that we are testing BELOW...
                var pages = await Page.SelectAllByCustomDateAsync(_TestObjAsync.FolderID, _TestObjAsync.TemplateID, true, 10);
                List<Page> pagesList = pages.ToList();

                Assert.IsFalse(pagesList == null || pagesList.Count == 0);
                Assert.IsTrue((DateTime)pagesList.FirstOrDefault().CustomDate > (DateTime)pagesList.LastOrDefault().CustomDate);
            }
            finally
            {
                await D_Delete_TestObjAsync(); // This will delete both records
                await F_Reset_AutoIndentAsync();
            }
        }
        #endregion Select ALL

        #region INSERT
        [TestMethod]
        public void C_Insert()
        {
            try
            {
                // Function that we are testing BELOW...
                _TestObj.Insert();
                Assert.IsTrue(_TestObj.ID > 0);
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task C_InsertAsync()
        {
            try
            {
                // Function that we are testing BELOW...
                await _TestObjAsync.InsertAsync();
                Assert.IsTrue(_TestObjAsync.ID > 0);
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }
        #endregion INSERT

        #region UPDATE

        [TestMethod]
        public void C_Update()
        {
            try
            {
                A_Create_TestObj();

                var page = Page.SelectOne(_TestObj.ID);
                Assert.IsFalse(page == null);
                string ogName = page.Name;

                page.Name = "page_test_update";
                page.Updated = DateTime.UtcNow;
                page.Expiration = DateTime.MaxValue;
                page.Publication = DateTime.MaxValue;
                // Function that we are testing BELOW...
                page.Update();

                page = Page.SelectOne(_TestObj.ID);
                Assert.IsTrue(page.Name == "page_test_update");

                page.Name = ogName;
                page.Update();
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task C_UpdateAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                var page = await Page.SelectOneAsync(_TestObjAsync.ID);
                Assert.IsFalse(page == null);
                string ogName = page.Name;

                page.Name = "page_test_update";
                page.Updated = DateTime.UtcNow;
                page.Expiration = DateTime.MaxValue;
                page.Publication = DateTime.MaxValue;
                // Function that we are testing BELOW...
                await page.UpdateAsync();

                page = Page.SelectOne(_TestObjAsync.ID);
                Assert.IsTrue(page.Name == "page_test_update");

                page.Name = ogName;
                await page.UpdateAsync();
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void C_UpdateSortOrder()
        {
            try
            {
                A_Create_TestObj();

                var page = Page.SelectOne(_TestObj.ID);
                Assert.IsFalse(page == null);
                int s = page.SortOrder;
                // Function that we are testing BELOW...
                Page.UpdateSortOrder(1, s + 1);

                page = Page.SelectOne(1);
                Assert.IsTrue(page.SortOrder != s);

                Page.UpdateSortOrder(1, s);
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

                var page = await Page.SelectOneAsync(_TestObjAsync.ID);
                Assert.IsFalse(page == null);
                int s = page.SortOrder;
                // Function that we are testing BELOW...
                await Page.UpdateSortOrderAsync(1, s + 1);

                page = await Page.SelectOneAsync(1);
                Assert.IsTrue(page.SortOrder != s);

                await Page.UpdateSortOrderAsync(1, s);
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void C_UpdateDefault()
        {
            _TestObj.IsFolderDefault = true;
            try
            {
                A_Create_TestObj();

                var page = Page.SelectOne(_TestObj.ID);
                Assert.AreNotEqual(0, page.ID);

                // Function that we are testing BELOW, should not update the PageID
                Page.UpdateDefault(page.ID, page.FolderID);
                page = Page.SelectOne(page.ID);
                Assert.IsTrue(page.IsFolderDefault);

                // Using ID = 4, so our test record gets updated.
                Page.UpdateDefault(4, page.FolderID);
                page = Page.SelectOne(page.ID);
                Assert.IsFalse(page.IsFolderDefault);
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task C_UpdateDefaultAsync()
        {
            _TestObjAsync.IsFolderDefault = true;
            try
            {
                await A_Create_TestObjAsync();

                var page = await Page.SelectOneAsync(_TestObjAsync.ID);
                Assert.AreNotEqual(0, page.ID);

                // Function that we are testing BELOW, should not update the PageID
                await Page.UpdateDefaultAsync(page.ID, page.FolderID);
                page = Page.SelectOne(page.ID);
                Assert.IsTrue(page.IsFolderDefault);

                // Using ID = 4, so out test record gets updated.
                await Page.UpdateDefaultAsync(4, page.FolderID);
                page = await Page.SelectOneAsync(page.ID);
                Assert.IsFalse(page.IsFolderDefault);
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        #endregion UPDATE

        #region Delete
        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = Page.SelectAll();
            var ass = allAssert.Where(x => x.Name == _TestObj.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Page wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Page Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                a.Delete();
                Trace.WriteLine($"DELETE Page: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = Page.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Page not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await Page.SelectAllAsync();
            var ass = allAssert.Where(x => x.Name == _TestObjAsync.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Page wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Page Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                await a.DeleteAsync();
                Trace.WriteLine($"DELETE Page: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = await Page.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Page not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<Page>();
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
            var connector = ConnectorFactory.CreateConnector<Page>();
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
