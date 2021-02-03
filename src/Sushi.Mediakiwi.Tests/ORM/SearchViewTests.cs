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
    public class SearchViewTests : BaseTest
    {

        #region Select All

        [TestMethod]
        public void X_SelectAllForFolder()
        {
            int folderId = 6;

            // Function that we are testing BELOW...
            var searchViews = SearchView.SelectAll(folderId);

            if (searchViews?.Length > 0)
                Trace.WriteLine($"FOUND SearchView: {searchViews.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("SearchView NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllForFolderAsync()
        {
            int folderId = 6;

            // Function that we are testing BELOW...
            var searchViews = await SearchView.SelectAllAsync(folderId);

            if (searchViews?.Length > 0)
                Trace.WriteLine($"FOUND SearchView: {searchViews.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("SearchView NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectAllIDs()
        {
            List<string> IDs = new List<string>() { "2_5", "2_9", "7_2" };

            // Function that we are testing BELOW...
            var searchViews = SearchView.SelectAll(IDs.ToArray());
            
            if (searchViews?.Length > 0)
                Trace.WriteLine($"FOUND SearchView: {searchViews.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("SearchView NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllIDsAsync()
        {
            List<string> IDs = new List<string>() { "2_5", "2_9", "7_2" };

            // Function that we are testing BELOW...
            var searchViews = await SearchView.SelectAllAsync(IDs.ToArray());

            if (searchViews?.Length > 0)
                Trace.WriteLine($"FOUND SearchView: {searchViews.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("SearchView NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectAllForSiteTypeAndSearch()
        {
            int siteId = 2;
            string search = "werk";
            int? filterType = null;

            // Function that we are testing BELOW...
            var searchViews = SearchView.SelectAll(siteId, filterType, search);
            
            if (searchViews?.Length > 0)
                Trace.WriteLine($"FOUND SearchView: {searchViews.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("SearchView NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllForSiteTypeAndSearchAsync()
        {
            int siteId = 2;
            string search = "werk";
            int? filterType = null;

            // Function that we are testing BELOW...
            var searchViews = await SearchView.SelectAllAsync(siteId, filterType, search);

            if (searchViews?.Length > 0)
                Trace.WriteLine($"FOUND SearchView: {searchViews.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("SearchView NOT FOUND...");
        }

        #endregion Select All
    }
}
