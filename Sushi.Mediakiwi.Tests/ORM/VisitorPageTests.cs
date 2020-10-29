using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemanticComparison;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.Mediakiwi.Data.Statistics;

namespace Sushi.Mediakiwi.Tests.ORM
{
    [TestClass]
    public class VisitorPageTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_VisitorPages";
        private string _key = "VisitorPage_Key";

        private static DateTime _date = DateTime.Now;

        private VisitorPage _TestObj = new VisitorPage()
        {
            UrlID = 69,
            Name = "xUNIT TESTx",
            GUID = new Guid("85AE99D7-F9BA-4846-B98E-6ACB31E84E92"),
            Created = _date
        };

        private VisitorPage _TestObjAsync = new VisitorPage()
        {
            UrlID = 71,
            Name = "xASYNC UNIT TESTx",
            GUID = new Guid("9E13FDB1-44E8-4C18-B0C3-0BF53C07697A"),
            Created = _date
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj()
        {
            // VisitorPage does not contain a Save(), Replace code below when Save() is added.
            var connector = ConnectorFactory.CreateConnector<VisitorPage>();
            connector.Save(_TestObj);

            Assert.IsTrue(_TestObj?.ID > 0);

            VisitorPage db = VisitorPage.SelectOne(_TestObj.UrlID, _TestObj.GUID, _TestObj.Name);

            Assert.AreEqual(_TestObj.UrlID, db.UrlID);
            Assert.AreEqual(_TestObj.Name, db.Name);
            Assert.AreEqual(_TestObj.GUID, db.GUID);

            var TimeDiff = _TestObj.Created - db.Created;
            Assert.IsTrue(TimeDiff.Ticks < 20000 && TimeDiff.Ticks > -20000, "Dates aren't close enough to each other!");

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED VisitorPage: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            // VisitorPage does not contain a SaveAsync(), Replace code below when SaveAsync() is added.
            var connector = ConnectorFactory.CreateConnector<VisitorPage>();
            await connector.SaveAsync(_TestObjAsync);

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            VisitorPage db = VisitorPage.SelectOne(_TestObjAsync.UrlID, _TestObjAsync.GUID, _TestObjAsync.Name);

            Assert.AreEqual(_TestObjAsync.UrlID, db.UrlID);
            Assert.AreEqual(_TestObjAsync.Name, db.Name);
            Assert.AreEqual(_TestObjAsync.GUID, db.GUID);

            var TimeDiff = _TestObjAsync.Created - db.Created;
            Assert.IsTrue(TimeDiff.Ticks < 20000 && TimeDiff.Ticks > -20000, "Dates aren't close enough to each other!");

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED VisitorPage: {_TestObjAsync.ID}");
            }
        }
        #endregion Create

        #region Select ONE
        [TestMethod]
        public void B_SelectOneByIdGuidAndPath()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var visitorPage = VisitorPage.SelectOne(_TestObj.UrlID, _TestObj.GUID, _TestObj.Name);

                if (visitorPage?.ID > 0)
                    Trace.WriteLine($"FOUND VisitorPage: {visitorPage.ID}");
                else
                    Assert.Fail("VisitorPage NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }


        [TestMethod]
        public async Task B_SelectOneByIdGuidAndPathAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var visitorPage = await VisitorPage.SelectOneAsync(_TestObjAsync.UrlID, _TestObjAsync.GUID, _TestObjAsync.Name);

                if (visitorPage?.ID > 0)
                    Trace.WriteLine($"FOUND VisitorPage: {visitorPage.ID}");
                else
                    Assert.Fail("VisitorPage NOT FOUND...");
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
        public void B_SelectKeysByPath()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var visitorsPageIDs = VisitorPage.SelectKeys(_TestObj.UrlID, _TestObj.Name);

                if (visitorsPageIDs?.Length > 0)
                    Trace.WriteLine($"FOUND VisitorPage: {visitorsPageIDs.Select(x => x.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("VisitorPage NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }


        [TestMethod]
        public async Task B_SelectKeysByPathAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var visitorsPageIDs = await VisitorPage.SelectKeysAsync(_TestObjAsync.UrlID, _TestObjAsync.Name);

                if (visitorsPageIDs?.Length > 0)
                    Trace.WriteLine($"FOUND VisitorPage: {visitorsPageIDs.Select(x => x.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("VisitorPage NOT FOUND...");
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
            var visitorsPageIDs = VisitorPage.SelectKeys(_TestObj.UrlID, _TestObj.Name);
            if (visitorsPageIDs.Count() == 0)
            {
                Assert.Fail("Test VisitorPage wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE VisitorPage Found: [{visitorsPageIDs.Aggregate("", (a, b) => a + ", " + b)}]");

            foreach (int a in visitorsPageIDs)
            {
                // VisitorPage does not contain a Delete(), Replace code below when Delete() is added.
                var connector = ConnectorFactory.CreateConnector<VisitorPage>();
                var del = connector.FetchSingle(a);
                connector.Delete(del);
                Trace.WriteLine($"DELETE VisitorPage: {a}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = VisitorPage.SelectOne(del.UrlID, del.GUID, del.Name);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test VisitorPage not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var visitorsPageIDs = await VisitorPage.SelectKeysAsync(_TestObjAsync.UrlID, _TestObjAsync.Name);
            if (visitorsPageIDs.Count() == 0)
            {
                Assert.Fail("Test VisitorPage wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE VisitorPage Found: [{visitorsPageIDs.Aggregate("", (a, b) => a + ", " + b)}]");

            foreach (int a in visitorsPageIDs)
            {
                // VisitorPage does not contain a DeleteAsync(), Replace code below when DeleteAsync() is added.
                var connector = ConnectorFactory.CreateConnector<VisitorPage>();
                var del = await connector.FetchSingleAsync(a);
                await connector.DeleteAsync(del);
                Trace.WriteLine($"DELETE VisitorPage: {a}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = await VisitorPage.SelectOneAsync(del.UrlID, del.GUID, del.Name);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test VisitorPage not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<VisitorPage>();
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
            var connector = ConnectorFactory.CreateConnector<VisitorPage>();
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
