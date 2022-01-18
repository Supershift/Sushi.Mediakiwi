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
    public class VisitorTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Visitors";
        private string _key = "Visitor_Key";

        private static DateTime _date = DateTime.Now;

        private Visitor _TestObj = new Visitor()
        {
            ProfileID = 666,
            Updated = _date,
            GUID = new Guid("4B308223-7F4C-42F0-A76E-1D5878A68723"),
            Created = _date,
            RememberMe = true,
            DataString = @"<ArrayOfData xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><Data></Data></ArrayOfData>"
        };

        private Visitor _TestObjAsync = new Visitor()
        {
            ProfileID = 777,
            Updated = _date,
            GUID = new Guid("C9310092-5F33-42DC-A070-C41695F04E7F"),
            Created = _date,
            RememberMe = true,
            DataString = @"<ArrayOfData xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><Data></Data></ArrayOfData>"
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj()
        {
            // Visitor does not contain a Save(), Replace code below when Save() is added.
            var connector = ConnectorFactory.CreateConnector<Visitor>();
            connector.Save(_TestObj);

            Assert.IsTrue(_TestObj?.ID > 0);

            Visitor db = (Visitor)Visitor.SelectOne(_TestObj.ID);

            Assert.AreEqual(_TestObj.GUID, db.GUID);
            var TimeDiff = _TestObj.Created - db.Created;
            Assert.IsTrue(TimeDiff.Ticks < 20000 && TimeDiff.Ticks > -20000, "Dates aren't close enough to each other!");

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED Visitor: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            // Visitor does not contain a SaveAsync(), Replace code below when SaveAsync() is added.
            var connector = ConnectorFactory.CreateConnector<Visitor>();
            await connector.SaveAsync(_TestObjAsync);

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            Visitor db = (Visitor)await Visitor.SelectOneAsync(_TestObjAsync.ID);

            Assert.AreEqual(_TestObjAsync.GUID, db.GUID);
            var TimeDiff = _TestObjAsync.Created - db.Created;
            Assert.IsTrue(TimeDiff.Ticks < 20000 && TimeDiff.Ticks > -20000, "Dates aren't close enough to each other!");

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED Visitor: {_TestObjAsync.ID}");
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
                var visitor = Visitor.SelectOne(_TestObj.ID);

                if (visitor?.ID > 0)
                    Trace.WriteLine($"FOUND Visitor: {visitor.ID}");
                else
                    Assert.Fail("Visitor NOT FOUND...");
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
                var visitor = await Visitor.SelectOneAsync(_TestObjAsync.ID);

                if (visitor?.ID > 0)
                    Trace.WriteLine($"FOUND Visitor: {visitor.ID}");
                else
                    Assert.Fail("Visitor NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByGuid()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var visitor = Visitor.Select(_TestObj.GUID);

                if (visitor?.ID > 0)
                    Trace.WriteLine($"FOUND Visitor: {visitor.ID}");
                else
                    Assert.Fail("Visitor NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByGuidAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var visitor = await Visitor.SelectAsync(_TestObjAsync.GUID);

                if (visitor?.ID > 0)
                    Trace.WriteLine($"FOUND Visitor: {visitor.ID}");
                else
                    Assert.Fail("Visitor NOT FOUND...");
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
        public void B_SelectAllByProfile()
        {
            var excludeVisitorId = 10452;

            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var visitors = Visitor.SelectAllByProfile((int)_TestObj.ProfileID, excludeVisitorId);

                if (visitors?.Length > 0)
                    Trace.WriteLine($"FOUND Visitor: {visitors.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Visitor NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllByProfileAsync()
        {
            var excludeVisitorId = 10452;

            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var visitors = await Visitor.SelectAllByProfileAsync((int)_TestObjAsync.ProfileID, excludeVisitorId);

                if (visitors?.Length > 0)
                    Trace.WriteLine($"FOUND Visitor: {visitors.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Visitor NOT FOUND...");
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
            var allAssert = Visitor.SelectAllByProfile((int)_TestObj.ProfileID, 0);
            if (allAssert.Count() == 0)
            {
                Assert.Fail("Test Visitor wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Visitor Found: [{allAssert.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (Visitor a in allAssert)
            {
                // Visitor does not contain a Delete(), Replace code below when Delete() is added.
                var connector = ConnectorFactory.CreateConnector<Visitor>();
                connector.Delete(a);
                Trace.WriteLine($"DELETE Visitor: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = Visitor.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Visitor not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await Visitor.SelectAllByProfileAsync((int)_TestObjAsync.ProfileID, 0);
            if (allAssert.Count() == 0)
            {
                Assert.Fail("Test Visitor wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Visitor Found: [{allAssert.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (Visitor a in allAssert)
            {
                // Visitor does not contain a DeleteAsync(), Replace code below when DeleteAsync() is added.
                var connector = ConnectorFactory.CreateConnector<Visitor>();
                await connector.DeleteAsync(a);
                Trace.WriteLine($"DELETE Visitor: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = await Visitor.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Visitor not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<Visitor>();
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
            var connector = ConnectorFactory.CreateConnector<Visitor>();
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
