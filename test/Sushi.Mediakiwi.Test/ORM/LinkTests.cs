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
    public class LinkTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Links";
        private string _key = "Link_Key";

        private static DateTime _date = DateTime.Now;
        // Test object
        private Link _TestObj = new Link()
        {
            ExternalUrl = "https://www.unitTest.not",
            Text = "xUNIT TESTx",
            Created = _date,
        };
        // Async test object
        private Link _TestObjAsync = new Link()
        {
            ExternalUrl = "https://www.unitAsyncTest.not/extrastuff.html",
            Text = "xASYNC UNIT TESTx",
            Created = DateTime.UtcNow,
        };
        #endregion Test Data

        #region  Create
        public void A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj?.ID > 0);

            Link db = Link.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.GUID, db.GUID);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED Link: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            var db = await Link.SelectOneAsync(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.GUID, db.GUID);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED Link: {_TestObjAsync.ID}");
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
                var link = Link.SelectOne(_TestObj.ID);

                if (link?.ID > 0)
                    Trace.WriteLine($"FOUND Link: {link.Text}");
                else
                    Assert.Fail("Link NOT FOUND...");
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
                var link = await Link.SelectOneAsync(_TestObjAsync.ID);

                if (link?.ID > 0)
                    Trace.WriteLine($"FOUND Link: {link.Text}");
                else
                    Assert.Fail("Link NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }
        #endregion Select ONE

        #region Delete
        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            // Link does not contain a SelectAll(), Replace code below when SelectAll() is added.
            var connector = ConnectorFactory.CreateConnector<Link>();
            var filter = connector.CreateQuery();
            var allAssert = connector.FetchAll(filter).ToArray();
            var ass = allAssert.Where(x => x.Target == _TestObj.Target);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Link wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Link Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (Link a in ass)
            {
                // Link does not contain a Delete(), Replace code below when Delete() is added.
                connector.Delete(a);
                Trace.WriteLine($"DELETE Link: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = Link.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Link not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            // Link does not contain a SelectAllAsync(), Replace code below when SelectAllAsync() is added.
            var connector = ConnectorFactory.CreateConnector<Link>();
            var filter = connector.CreateQuery();
            var allAssert = await connector.FetchAllAsync(filter);
            var ass = allAssert.ToArray().Where(x => x.Target == _TestObjAsync.Target);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Link wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Link Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (Link a in ass)
            {
                // Link does not contain a DeleteAsync(), Replace code below when DeleteAsync() is added.
                await connector.DeleteAsync(a);
                Trace.WriteLine($"DELETE Link: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = await Link.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Link not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<Link>();
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
            var connector = ConnectorFactory.CreateConnector<Link>();
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
