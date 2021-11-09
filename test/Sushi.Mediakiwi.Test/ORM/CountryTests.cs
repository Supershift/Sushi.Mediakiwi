using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemanticComparison;
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
    public class CountryTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Countries";
        private string _key = "Country_Key";

        private Country _TestObj = new Country()
        {
            GUID = new Guid("382F52C1-F2AD-4BEE-AA28-A8AABEBFCC19"),
            Country_EN = "xUNIT TESTx",
            Country_NL = "xUNIT TESTx",
            IsActive = true
        };

        private Country _TestObjAsync = new Country()
        {
            GUID = new Guid("E89F3118-C553-46B0-8849-E19FB2B7EBC4"),
            Country_EN = "xASYNC UNIT TESTx",
            Country_NL = "xASYNC UNIT TESTx",
            IsActive = true
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj?.ID > 0);

            // MJ: check if saved db record is ok            
            var expected = new Likeness<Country, Country>(_TestObj);
            Country db = (Country)Country.SelectOne(_TestObj.ID);
            Assert.AreEqual(expected, db);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED ComponentVersion: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            // MJ: check if saved db record is ok            
            var expected = new Likeness<Country, Country>(_TestObjAsync);
            Country db = (Country)Country.SelectOne(_TestObjAsync.ID);
            Assert.AreEqual(expected, db);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED ComponentVersion: {_TestObjAsync.ID}");
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
                var country = Country.SelectOne(_TestObj.ID);

                if (country?.ID > 0)
                    Trace.WriteLine($"FOUND Country: {country.ID}");
                else
                    Assert.Fail("Country NOT FOUND...");
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
                var country = await Country.SelectOneAsync(_TestObjAsync.ID);

                if (country?.ID > 0)
                    Trace.WriteLine($"FOUND Country: {country.ID}");
                else
                    Assert.Fail("Country NOT FOUND...");
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
                var country = Country.SelectOne(_TestObj.GUID);

                if (country?.ID > 0)
                    Trace.WriteLine($"FOUND Country: {country.ID}");
                else
                    Assert.Fail("Country NOT FOUND...");
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
                var country = await Country.SelectOneAsync(_TestObjAsync.GUID);

                if (country?.ID > 0)
                    Trace.WriteLine($"FOUND Country: {country.ID}");
                else
                    Assert.Fail("Country NOT FOUND...");
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
                var country = Country.SelectOne(_TestObj.Country_EN);

                if (country?.ID > 0)
                    Trace.WriteLine($"FOUND Country: {country.ID}");
                else
                    Assert.Fail("Country NOT FOUND...");
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
                var country = await Country.SelectOneAsync(_TestObjAsync.Country_NL);

                if (country?.ID > 0)
                    Trace.WriteLine($"FOUND Country: {country.ID}");
                else
                    Assert.Fail("Country NOT FOUND...");
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
            var countries = Country.SelectAll();

            if (countries?.Length > 0)
                Trace.WriteLine($"FOUND Country: {countries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Country NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllAsync()
        {
            // Function that we are testing BELOW...
            var countries = await Country.SelectAllAsync();

            if (countries?.Length > 0)
                Trace.WriteLine($"FOUND Country: {countries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Country NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectAllOrdered()
        {
            // Function that we are testing BELOW...
            var countries = Country.SelectAll("nl");

            if (countries?.Length > 0)
                Trace.WriteLine($"FOUND Country: {countries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Country NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllOrderedAsync()
        {
            // Function that we are testing BELOW...
            var countries = await Country.SelectAllAsync("nl");

            if (countries?.Length > 0)
                Trace.WriteLine($"FOUND Country: {countries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Country NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectAllActiveOrdered()
        {
            // Function that we are testing BELOW...
            var countries = Country.SelectAll(true, "nl");

            if (countries?.Length > 0)
                Trace.WriteLine($"FOUND Country: {countries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Country NOT FOUND...");
        }


        [TestMethod]
        public async Task X_SelectAllActiveOrderedAsync()
        {
            // Function that we are testing BELOW...
            var countries = await Country.SelectAllAsync(true, "nl");

            if (countries?.Length > 0)
                Trace.WriteLine($"FOUND Country: {countries.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Country NOT FOUND...");
        }
        #endregion Select ALL

        #region Delete
        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = Country.SelectAll();
            var ass = allAssert.Where(x => x.Country_EN == _TestObj.Country_EN);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Country wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Country Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (Country a in ass)
            {
                // Country does not contain a Delete(), Replace code below when Delete() is added.
                var connector = ConnectorFactory.CreateConnector<Country>();
                connector.Delete(a);
                Trace.WriteLine($"DELETE Country: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = Country.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Country not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await Country.SelectAllAsync();
            var ass = allAssert.Where(x => x.Country_EN == _TestObjAsync.Country_EN);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Country wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Country Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (Country a in ass)
            {
                // Country does not contain a DeleteAsync(), Replace code below when DeleteAsync() is added.
                var connector = ConnectorFactory.CreateConnector<Country>();
                await connector.DeleteAsync(a);
                Trace.WriteLine($"DELETE Country: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = await Country.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Country not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<Country>();
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
            var connector = ConnectorFactory.CreateConnector<Country>();
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
