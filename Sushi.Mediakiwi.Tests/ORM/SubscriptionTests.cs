using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemanticComparison;
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
    public class SubscriptionTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Subscriptions";
        private string _key = "Subscription_Key";

        private static DateTime _date = DateTime.Now;
        // Test object
        private Subscription _TestObj = new Subscription()
        {
            Title2 = "xUNIT TESTx",
            UserID = 4, //Mark Rienstra
            ComponentListID = 69420,
            SiteID = 666,
            SetupXml = @"<Content xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><Fields></Fields></Content>",
            IntervalType = 1500,
            Scheduled = _date.AddDays(10),
            Created = _date,
            IsActive = true
        };
        // Async Test object
        private Subscription _TestObjAsync = new Subscription()
        {
            Title2 = "xASYNC UNIT TESTx",
            UserID = 4, //Mark Rienstra
            ComponentListID = 69421,
            SiteID = 777,
            SetupXml = @"<Content xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><Fields></Fields></Content>",
            IntervalType = 1500,
            Scheduled = _date.AddDays(10),
            Created = _date,
            IsActive = true
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj.ID > 0);

            Subscription db = (Subscription)Subscription.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.Title2, db.Title2);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED Subscription: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync.ID > 0);

            Subscription db = (Subscription)Subscription.SelectOne(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.Title2, db.Title2);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED Subscription: {_TestObjAsync.ID}");
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
                var subscription = Subscription.SelectOne((int)_TestObj?.ID);

                if (subscription?.ID > 0)
                    Trace.WriteLine($"FOUND Subscription: {subscription.ID}");
                else
                    Assert.Fail("Subscription NOT FOUND...");
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
                var subscription = await Subscription.SelectOneAsync((int)_TestObjAsync?.ID);

                if (subscription?.ID > 0)
                    Trace.WriteLine($"FOUND Subscription: {subscription.ID}");
                else
                    Assert.Fail("Subscription NOT FOUND...");
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
        public void B_SelectAll()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var subscriptions = Subscription.SelectAll();

                if (subscriptions?.Count > 0)
                    Trace.WriteLine($"FOUND Subscription: {subscriptions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Subscription NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var subscriptions = await Subscription.SelectAllAsync();

                if (subscriptions?.Count > 0)
                    Trace.WriteLine($"FOUND Subscription: {subscriptions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Subscription NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectAllByListAndUser()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var subscriptions = Subscription.SelectAll((int)_TestObj?.ComponentListID, (int)_TestObj?.UserID);

                if (subscriptions?.Count > 0)
                    Trace.WriteLine($"FOUND Subscription: {subscriptions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Subscription NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllByListAndUserAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var subscriptions = await Subscription.SelectAllAsync((int)_TestObjAsync?.ComponentListID, (int)_TestObjAsync?.UserID);

                if (subscriptions?.Count > 0)
                    Trace.WriteLine($"FOUND Subscription: {subscriptions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Subscription NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }


        [TestMethod]
        public void B_SelectAllActive()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var subscriptions = Subscription.SelectAllActive();

                if (subscriptions?.Count > 0)
                    Trace.WriteLine($"FOUND Subscription: {subscriptions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Subscription NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllActiveAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var subscriptions = await Subscription.SelectAllActiveAsync();

                if (subscriptions?.Count > 0)
                    Trace.WriteLine($"FOUND Subscription: {subscriptions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Subscription NOT FOUND...");
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
            var allAssert = Subscription.SelectAll();
            var ass = allAssert.Where(x => x.Title2 == _TestObj.Title2);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Subscription wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Subscription Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (Subscription a in ass)
            {
                a.Delete();
                Trace.WriteLine($"DELETE Subscription: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = Subscription.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Subscription not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await Subscription.SelectAllAsync();
            var ass = allAssert.Where(x => x.Title2 == _TestObjAsync.Title2);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Subscription wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Subscription Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (Subscription a in ass)
            {
                await a.DeleteAsync();
                Trace.WriteLine($"DELETE Subscription: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = await Subscription.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Subscription not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        [TestMethod]
        public async Task E_ClearTest()
        {
            var preTest = Subscription.SelectAll();
            if (preTest.Count == 0)
            {
                try
                {
                    A_Create_TestObj();
                    await A_Create_TestObjAsync();

                    // Function that we are testing BELOW...
                    Subscription.Clear();

                    var assert = Subscription.SelectAll();
                    if (assert.Count > 0)
                    {
                        D_Delete_TestObj();
                        await D_Delete_TestObjAsync();
                        Assert.Fail("Subscription.Clear() did not delete all Test Subscriptions");
                    }
                }
                finally
                {
                    F_Reset_AutoIndent();
                }
            }
            else
            {
                Assert.Fail("Cannot perform test. There is data in the [wim_Subscriptions] table.");
            }
        }

        [TestMethod]
        public async Task E_ClearTestAsync()
        {
            var preTest = await Subscription.SelectAllAsync();
            if (preTest.Count == 0)
            {
                try
                {
                    A_Create_TestObj();
                    await A_Create_TestObjAsync();

                    // Function that we are testing BELOW...
                    await Subscription.ClearAsync();

                    var assert = await Subscription.SelectAllAsync();
                    if (assert.Count > 0)
                    {
                        D_Delete_TestObj();
                        await D_Delete_TestObjAsync();
                        Assert.Fail("Subscription.Clear() did not delete all Test Subscriptions");
                    }
                }
                finally
                {
                    F_Reset_AutoIndent();
                }
            }
            else
            {
                Assert.Fail("Cannot perform test. There is data in the [wim_Subscriptions] table.");
            }
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<Subscription>();
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
            var connector = ConnectorFactory.CreateConnector<Subscription>();
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
