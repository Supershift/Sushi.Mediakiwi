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
    public class ArticleTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_WikiArticles";
        private string _key = "Wiki_Key";

        private int _listId = 690;
        private string _listTitle = "Test voor lijst 690";
        private int _listAsyncId = 691;
        private string _listAsyncTitle = "Async Test voor lijst 691";

        private int _pageId = 420;
        private string _pageTitle = "Test voor pagina 420";
        private int _pageAsyncId = 421;
        private string _pageAsyncTitle = "Async Test voor pagina 421";
        #endregion Test Data

        #region Create
        public Article A_CreateForList()
        {

            var article = Article.CreateNew(_listId, null, _listTitle);
            Assert.IsFalse(article.ID == 0);

            int articleId = Article.Save(article);
            Assert.IsFalse(articleId == 0);
            if (articleId > 0)
            {
                Trace.WriteLine($"CREATED Article: {articleId}");
            }
            if (article?.ID > 0 && articleId > 0)
            {
                article = Article.CheckIfItemExists(_listId, null);
                if (article?.ID > 0)
                {
                    Trace.WriteLine($"FOUND Article: {article.ID}");
                }
            }
            else
                Assert.IsTrue(false);

            return article;
        }

        public async Task<Article> A_CreateForListAsync()
        {
            var article = await Article.CreateNewAsync(_listAsyncId, null, _listAsyncTitle);
            Assert.IsFalse(article.ID == 0);

            int articleId = await Article.SaveAsync(article);
            Assert.IsFalse(articleId == 0);
            if (articleId > 0)
            {
                Trace.WriteLine($"CREATED Article: {articleId}");
            }
            if (article?.ID > 0 && articleId > 0)
            {
                article = await Article.CheckIfItemExistsAsync(_listAsyncId, null);
                if (article?.ID > 0)
                {
                    Trace.WriteLine($"FOUND Article: {article.ID}");
                }
            }
            else
                Assert.IsTrue(false);

            return article;
        }

        public Article A_CreateForPage()
        {
            var article = Article.CreateNew(null, _pageId, _pageTitle);
            Assert.IsFalse(article.ID == 0);

            int articleId = Article.Save(article);
            Assert.IsFalse(articleId == 0);

            if (article?.ID > 0 && articleId > 0)
            {
                article = Article.CheckIfItemExists(null, _pageId);
                if (article?.ID > 0)
                {
                    Trace.WriteLine($"FOUND Article: {article.ID}");
                }
            }
            else
                Assert.IsTrue(false);

            return article;
        }

        public async Task<Article> A_CreateForPageAsync()
        {
            var article = await Article.CreateNewAsync(null, _pageAsyncId, _pageAsyncTitle);
            Assert.IsFalse(article.ID == 0);

            int articleId = await Article.SaveAsync(article);
            Assert.IsFalse(articleId == 0);

            if (article?.ID > 0 && articleId > 0)
            {
                article = await Article.CheckIfItemExistsAsync(null, _pageAsyncId);
                if (article?.ID > 0)
                {
                    Trace.WriteLine($"FOUND Article: {article.ID}");
                }
            }
            else
                Assert.IsTrue(false);

            return article;
        }
        #endregion Create

        #region Creation Tests
        [TestMethod]
        public void B_InitialTestList()
        {
            try
            {
                A_CreateForList();
            }
            finally
            {
                D_DeleteForList();

                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_InitialTestListAsync()
        {
            try
            {
                await A_CreateForListAsync();
            }
            finally
            {
                await D_DeleteForListAsync();

                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_InitialTestPage()
        {
            try
            {
                A_CreateForPage();
            }
            finally
            {
                D_DeleteForPage();

                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_InitialTestPageAsync()
        {
            try
            {
                await A_CreateForPageAsync();
            }
            finally
            {
                await D_DeleteForPageAsync();

                await F_Reset_AutoIndentAsync();
            }
        }
        #endregion Creation Tests

        #region Select One
        [TestMethod]
        public void B_SelectOneByID()
        {
            try
            {
                Article a = A_CreateForList();

                // Function that we are testing BELOW...
                var article = Article.SelectOne(a.ID);

                Assert.IsFalse(article.ID == 0);
                if (article?.ID > 0)
                    Trace.WriteLine($"FOUND Article: {article.ID}");
            }
            finally
            {
                D_DeleteForList();

                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByIDAsync()
        {
            try
            {
                Article a = await A_CreateForListAsync();

                // Function that we are testing BELOW...
                var article = await Article.SelectOneAsync(a.ID);

                Assert.IsFalse(article.ID == 0);
                if (article?.ID > 0)
                    Trace.WriteLine($"FOUND Article: {article.ID}");
            }
            finally
            {
                await D_DeleteForListAsync();

                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneForListOrNew()
        {
            string title = "Default NEW title";
            try
            {
                Article a = A_CreateForList();

                // Function that we are testing BELOW...
                var article = Article.SelectOneForListOrNew(a.ID, title);

                Assert.IsFalse(article.ID == 0 && article.Title == title);

                if (article?.ID > 0)
                    Trace.WriteLine($"FOUND Article: {article.ID}");
            }
            finally
            {
                D_DeleteForList();

                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneForListOrNewAsync()
        {
            string title = "Default NEW ASYNC title";
            try
            {
                Article a = await A_CreateForListAsync();

                // Function that we are testing BELOW...
                var article = await Article.SelectOneForListOrNewAsync(a.ID, title);

                Assert.IsFalse(article.ID == 0 && article.Title == title);

                if (article?.ID > 0)
                    Trace.WriteLine($"FOUND Article: {article.ID}");
            }
            finally
            {
                await D_DeleteForListAsync();

                await F_Reset_AutoIndentAsync();
            }
        }
        #endregion Select One

        #region Select All
        [TestMethod]
        public void X_SelectAllForList()
        {
            // Function that we are testing BELOW...
            var article = Article.SelectList(47);

            Assert.IsFalse(article.Count == 0);

            if (article?.Count > 0)
                Trace.WriteLine($"FOUND Article: {article.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
        }

        [TestMethod]
        public async Task X_SelectAllForListAsync()
        {
            // Function that we are testing BELOW...
            var article = await Article.SelectListAsync(47);

            Assert.IsFalse(article.Count == 0);

            if (article?.Count > 0)
                Trace.WriteLine($"FOUND Article: {article.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
        }
        #endregion Select All        

        #region Delete
        public void D_DeleteForList()
        {
            var ass = Article.SelectOneForListOrNew(_listId, _listTitle);
            if (ass?.ID > 0)
            {
                Article.Delete(ass);
                Trace.WriteLine($"DELETE Article: {ass?.ID}");

                ass = Article.CheckIfItemExists(_listId, null);
                if (ass?.ID > 0)
                {
                    Assert.Fail($"Test Article not deleted, found {ass?.ID}");
                }
                else
                    Assert.IsTrue(true);
            }
            else
            {
                Assert.Fail("Test Article wasn't found, you created it yet?");
            }
        }

        public async Task D_DeleteForListAsync()
        {
            var ass = await Article.SelectOneForListOrNewAsync(_listAsyncId, _listAsyncTitle);
            if (ass?.ID > 0)
            {
                await Article.DeleteAsync(ass);
                Trace.WriteLine($"DELETE Article: {ass?.ID}");

                ass = await Article.CheckIfItemExistsAsync(_listAsyncId, null);
                if (ass?.ID > 0)
                {
                    Assert.Fail($"Test Article not deleted, found {ass?.ID}");
                }
                else
                    Assert.IsTrue(true);
            }
            else
            {
                Assert.Fail("Test Article wasn't found, you created it yet?");
            }
        }

        public void D_DeleteForPage()
        {
            var ass = Article.SelectOneForPageOrNew(_pageId, _pageTitle);
            Assert.IsTrue(ass?.ID > 0);

            if (ass?.ID > 0)
            {
                Article.Delete(ass);
                Trace.WriteLine($"DELETE Article: {ass?.ID}");

                ass = Article.CheckIfItemExists(null, _pageId);
                if (ass?.ID > 0)
                {
                    Assert.Fail($"Test Article not deleted, found {ass?.ID}");
                }
                else
                    Assert.IsTrue(true);
            }
            else
            {
                Assert.Fail("Test Article wasn't found, you created it yet?");
            }
        }

        public async Task D_DeleteForPageAsync()
        {
            var ass = await Article.SelectOneForPageOrNewAsync(_pageAsyncId, _pageAsyncTitle);
            Assert.IsTrue(ass?.ID > 0);

            if (ass?.ID > 0)
            {
                await Article.DeleteAsync(ass);
                Trace.WriteLine($"DELETE Article: {ass?.ID}");

                ass = await Article.CheckIfItemExistsAsync(null, _pageAsyncId);
                if (ass?.ID > 0)
                {
                    Assert.Fail($"Test Article not deleted, found {ass?.ID}");
                }
                else
                    Assert.IsTrue(true);
            }
            else
            {
                Assert.Fail("Test Article wasn't found, you created it yet?");
            }
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<Article>();
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
            var connector = ConnectorFactory.CreateConnector<Article>();
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
