using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.Mediakiwi.Data;

namespace Sushi.Mediakiwi.Tests.ORM
{
    [TestClass]
    public class CachingTest : BaseTest
    {
        [TestMethod]
        public void SelectOneCached()
        {
            int pageID = 1;
            //get the page
            var page = Page.SelectOne(pageID);

            Assert.AreEqual(pageID, page.ID);
            //get it again
            var page2 = Page.SelectOne(pageID);

            //if cached, they have reference equality
            Assert.AreSame(page, page2);
        }

        [TestMethod]
        public async Task SelectOneCachedAsync()
        {
            int pageID = 1;
            //get the page
            var page = await Page.SelectOneAsync(pageID);

            Assert.AreEqual(pageID, page.ID);
            //get it again
            var page2 = await Page.SelectOneAsync(pageID);

            //if cached, they have reference equality
            Assert.AreSame(page, page2);
        }

        [TestMethod]
        public void SelectAllCached()
        {
            //get all pages
            var pages = Page.SelectAll();

            Assert.AreNotEqual(0, pages.Length);
            //get them again
            var pages2 = Page.SelectAll();

            //we cannot check for reference equality on the collection, becaue pages.SelectAll transforms the cached list to an array
            //we need to check for reference equality of all items
            Assert.IsTrue(pages.SequenceEqual(pages2));
        }

        [TestMethod]
        public async Task SelectAllCachedAsync()
        {
            //get all pages
            var pages = await Page.SelectAllAsync();

            Assert.AreNotEqual(0, pages.Length);
            //get them again
            var pages2 = await Page.SelectAllAsync();

            //we cannot check for reference equality on the collection, becaue pages.SelectAll transforms the cached list to an array
            //we need to check for reference equality of all items
            Assert.IsTrue(pages.SequenceEqual(pages2));
        }

        [TestMethod]
        public void CacheFlushBySave()
        {
            int pageID = 9;
            //get the page
            var page = Page.SelectOne(pageID);

            Assert.AreEqual(pageID, page.ID);

            //save it
            page.Save();

            //get it again
            var page2 = Page.SelectOne(pageID);

            //they should not have reference equality now
            Assert.AreNotSame(page, page2);
        }

        [TestMethod]
        public async Task CacheFlushBySaveAsync()
        {
            int pageID = 9;
            //get the page
            var page = await Page.SelectOneAsync(pageID);

            Assert.AreEqual(pageID, page.ID);

            //save it
            await page.SaveAsync();

            //get it again
            var page2 = await Page.SelectOneAsync(pageID);

            //they should not have reference equality now
            Assert.AreNotSame(page, page2);
        }

        [TestMethod]
        public void CacheFlushByDelete()
        {
            int pageID = 9;
            //get the page
            var page = Page.SelectOne(pageID);

            Assert.AreEqual(pageID, page.ID);

            //run a delete command on the entity 'Page'
            //this will not delete anything in DB, but still will trigger a flush
            var connector = ConnectorFactory.CreateConnector<Page>(new Page.PageMap(true));
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ID, -1);
            connector.Delete(filter);

            //get it again
            var page2 = Page.SelectOne(pageID);

            //they should not have reference equality now
            Assert.AreNotSame(page, page2);
        }

        [TestMethod]
        public async Task CacheFlushByDeleteAsync()
        {
            int pageID = 9;
            //get the page
            var page = await Page.SelectOneAsync(pageID);

            Assert.AreEqual(pageID, page.ID);

            //run a delete command on the entity 'Page'
            //this will not delete anything in DB, but still will trigger a flush
            var connector = ConnectorFactory.CreateConnector<Page>(new Page.PageMap(true));
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.ID, -1);
            await connector.DeleteAsync(filter);

            //get it again
            var page2 = await Page.SelectOneAsync(pageID);

            //they should not have reference equality now
            Assert.AreNotSame(page, page2);
        }

        [TestMethod]
        public void BypassCache()
        {
            int pageID = 1;
            var connector = ConnectorFactory.CreateConnector<Page>();
            //get the page
            var page = connector.FetchSingle(pageID);
            Assert.AreEqual(pageID, page.ID);
            //disable cache and get it again
            connector.UseCacheOnSelect = false;
            var page2 = connector.FetchSingle(pageID);

            //they should not have reference equality
            Assert.AreNotSame(page, page2);
        }

        [TestMethod]
        public async Task BypassCacheAsync()
        {
            int pageID = 1;
            var connector = ConnectorFactory.CreateConnector<Page>();
            //get the page
            var page = await connector.FetchSingleAsync(pageID);
            Assert.AreEqual(pageID, page.ID);
            //disable cache and get it again
            connector.UseCacheOnSelect = false;
            var page2 = await connector.FetchSingleAsync(pageID);

            //they should not have reference equality
            Assert.AreNotSame(page, page2);
        }
    }
}
