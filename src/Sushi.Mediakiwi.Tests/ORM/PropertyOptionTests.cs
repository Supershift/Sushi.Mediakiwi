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
    public class PropertyOptionTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_PropertyOptions";
        private string _key = "PropertyOption_Key";

        private PropertyOption _TestObj = new PropertyOption()
        {
            PropertyID = 69420,
            Name = "xUNIT TESTx",
            Value = "TEST Value",
            SortOrderID = 1
        };

        private PropertyOption _TestObjAsync = new PropertyOption()
        {
            PropertyID = 69421,
            Name = "xASYNC UNIT TESTx",
            Value = "ASYNC TEST Value",
            SortOrderID = 1
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj.ID > 0);

            // Save also sets the SortOrder to record ID
            _TestObj.SortOrderID = _TestObj.ID;

            // MJ: check if saved db record is ok            
            var expected = new Likeness<PropertyOption, PropertyOption>(_TestObj);
            PropertyOption db = PropertyOption.SelectOne(_TestObj.ID);
            Assert.AreEqual(expected, db);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED PropertyOption: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync.ID > 0);

            // Save also sets the SortOrder to record ID
            _TestObjAsync.SortOrderID = _TestObjAsync.ID;

            // MJ: check if saved db record is ok            
            var expected = new Likeness<PropertyOption, PropertyOption>(_TestObjAsync);
            PropertyOption db = PropertyOption.SelectOne(_TestObjAsync.ID);
            Assert.AreEqual(expected, db);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED PropertyOption: {_TestObjAsync.ID}");
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
                var propertyOption = PropertyOption.SelectOne(_TestObj.ID);

                if (propertyOption?.ID > 0)
                    Trace.WriteLine($"FOUND PropertyOption: {propertyOption.ID}");
                else
                    Assert.Fail("PropertyOption NOT FOUND...");
            }
            finally
            {
                D_DeleteCollectionTest();
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
                var propertyOption = await PropertyOption.SelectOneAsync(_TestObjAsync.ID);

                if (propertyOption?.ID > 0)
                    Trace.WriteLine($"FOUND PropertyOption: {propertyOption.ID}");
                else
                    Assert.Fail("PropertyOption NOT FOUND...");
            }
            finally
            {
                await D_DeleteCollectionTestAsync();
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
                var propertyOptions = PropertyOption.SelectAll();

                if (propertyOptions?.Length > 0)
                    Trace.WriteLine($"FOUND PropertyOption: {propertyOptions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("PropertyOption NOT FOUND...");
            }
            finally
            {
                D_DeleteCollectionTest();
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
                var propertyOptions = await PropertyOption.SelectAllAsync();

                if (propertyOptions?.Length > 0)
                    Trace.WriteLine($"FOUND PropertyOption: {propertyOptions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("PropertyOption NOT FOUND...");
            }
            finally
            {
                await D_DeleteCollectionTestAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectAllForPropertyID()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var propertyOptions = PropertyOption.SelectAll(_TestObj.PropertyID);

                if (propertyOptions?.Length > 0)
                    Trace.WriteLine($"FOUND PropertyOption: {propertyOptions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("PropertyOption NOT FOUND...");
            }
            finally
            {
                D_DeleteCollectionTest();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllForPropertyIDAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var propertyOptions = await PropertyOption.SelectAllAsync(_TestObjAsync.PropertyID);

                if (propertyOptions?.Length > 0)
                    Trace.WriteLine($"FOUND PropertyOption: {propertyOptions.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("PropertyOption NOT FOUND...");
            }
            finally
            {
                await D_DeleteCollectionTestAsync();
                await F_Reset_AutoIndentAsync();
            }
        }
        #endregion Select All

        #region Delete
        public void D_DeleteCollectionTest()
        {
            List<string> errorList = new List<string>();
            var allAssert = PropertyOption.SelectAll();
            var ass = allAssert.Where(x => x.Name == _TestObj.Name);
            if (ass?.Count() == 0)
            {
                Assert.Fail("Test PropertyOption wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE PropertyOption Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");
            if (ass.First().ID > 0)
            {
                PropertyOption.DeleteCollection(ass.First().PropertyID);
                Trace.WriteLine($"DELETE COLLECTION PropertyOption: {ass.First().PropertyID}");

                allAssert = PropertyOption.SelectAll();
                ass = allAssert.Where(x => x.Name == _TestObj.Name);
                if (ass?.Count() > 0) // ERROR
                {
                    Assert.Fail($"Test PropertyOption not deleted, Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");
                }
            }
        }

        public async Task D_DeleteCollectionTestAsync()
        {
            List<string> errorList = new List<string>();
            var allAssert = await PropertyOption.SelectAllAsync();
            var ass = allAssert.Where(x => x.Name == _TestObjAsync.Name);
            if (ass?.Count() == 0)
            {
                Assert.Fail("Test PropertyOption wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE PropertyOption Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");
            if (ass.First().ID > 0)
            {
                await PropertyOption.DeleteCollectionAsync(ass.First().PropertyID);
                Trace.WriteLine($"DELETE COLLECTION PropertyOption: {ass.First().PropertyID}");

                allAssert = await PropertyOption.SelectAllAsync();
                ass = allAssert.Where(x => x.Name == _TestObjAsync.Name);
                if (ass?.Count() > 0) // ERROR
                {
                    Assert.Fail($"Test PropertyOption not deleted, Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");
                }
            }
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<PortalRight>();
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
            var connector = ConnectorFactory.CreateConnector<PortalRight>();
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
