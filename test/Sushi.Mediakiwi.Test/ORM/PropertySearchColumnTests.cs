using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemanticComparison;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.MicroORM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Tests.ORM
{
    [TestClass]
    public class PropertySearchColumnTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_PropertySearchColumns";
        private string _key = "PropertySearchColumn_Key";

        private PropertySearchColumn _TestObj = new PropertySearchColumn()
        {
            ListID = 69420,
            PropertyID = 69421,
            ColumnWidth = 100,
            Title = "xUNIT TESTx",
            IsHighlight = true,
            TotalType = 0,
            IsOnlyExport = false
        };

        private PropertySearchColumn _TestObjAsync = new PropertySearchColumn()
        {
            ListID = 69460,
            PropertyID = 69461,
            ColumnWidth = 101,
            Title = "xASYNC UNIT TESTx",
            IsHighlight = true,
            TotalType = 0,
            IsOnlyExport = false
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj.ID > 0);

            // MJ: check if saved db record is ok            
            var expected = new Likeness<PropertySearchColumn, PropertySearchColumn>(_TestObj);
            PropertySearchColumn db = PropertySearchColumn.SelectOne(_TestObj.ID);
            Assert.AreEqual(expected, db);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED PropertySearchColumn: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync.ID > 0);

            // MJ: check if saved db record is ok            
            var expected = new Likeness<PropertySearchColumn, PropertySearchColumn>(_TestObjAsync);
            PropertySearchColumn db = PropertySearchColumn.SelectOne(_TestObjAsync.ID);
            Assert.AreEqual(expected, db);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED PropertySearchColumn: {_TestObjAsync.ID}");
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
                var propertySearchColumn = PropertySearchColumn.SelectOne(_TestObj.ID);

                if (propertySearchColumn?.ID > 0)
                    Trace.WriteLine($"FOUND PropertySearchColumn: {propertySearchColumn.ID}");
                else
                    Assert.Fail("PropertySearchColumn NOT FOUND...");
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
                var propertySearchColumn = await PropertySearchColumn.SelectOneAsync(_TestObjAsync.ID);

                if (propertySearchColumn?.ID > 0)
                    Trace.WriteLine($"FOUND PropertySearchColumn: {propertySearchColumn.ID}");
                else
                    Assert.Fail("PropertySearchColumn NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneHighlightForComponentList()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var propertySearchColumn = PropertySearchColumn.SelectOneHighlight(_TestObj.ListID);

                if (propertySearchColumn?.ID > 0)
                    Trace.WriteLine($"FOUND PropertySearchColumn: {propertySearchColumn.ID}");
                else
                    Assert.Fail("PropertySearchColumn NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneHighlightForComponentListAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var propertySearchColumn = await PropertySearchColumn.SelectOneHighlightAsync(_TestObjAsync.ListID);

                if (propertySearchColumn?.ID > 0)
                    Trace.WriteLine($"FOUND PropertySearchColumn: {propertySearchColumn.ID}");
                else
                    Assert.Fail("PropertySearchColumn NOT FOUND...");
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
        public void B_SelectAllForComponentList()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var propertySearchColumns = PropertySearchColumn.SelectAll(_TestObj.ListID);

                if (propertySearchColumns?.Length > 0)
                    Trace.WriteLine($"FOUND PropertySearchColumn: {propertySearchColumns.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("PropertySearchColumn NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllForComponentListAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var propertySearchColumns = await PropertySearchColumn.SelectAllAsync(_TestObjAsync.ListID);

                if (propertySearchColumns?.Length > 0)
                    Trace.WriteLine($"FOUND PropertySearchColumn: {propertySearchColumns.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("PropertySearchColumn NOT FOUND...");
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
            var allAssert = PropertySearchColumn.SelectAll(_TestObj.ListID);
            var ass = allAssert.Where(x => x.Title == _TestObj.Title);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test PropertySearchColumn wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE PropertySearchColumn Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (PropertySearchColumn a in ass)
            {
                // PropertySearchColumn does not contain a Delete(), Replace code below when Delete() is added.
                var connector = ConnectorFactory.CreateConnector<PropertySearchColumn>();
                connector.Delete(a);
                Trace.WriteLine($"DELETE PropertySearchColumn: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = PropertySearchColumn.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test PropertySearchColumn not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await PropertySearchColumn.SelectAllAsync(_TestObjAsync.ListID);
            var ass = allAssert.Where(x => x.Title == _TestObjAsync.Title);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test PropertySearchColumn wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE PropertySearchColumn Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (PropertySearchColumn a in ass)
            {
                // PropertySearchColumn does not contain a DeleteAsync(), Replace code below when DeleteAsync() is added.
                var connector = ConnectorFactory.CreateConnector<PropertySearchColumn>();
                await connector.DeleteAsync(a);
                Trace.WriteLine($"DELETE PropertySearchColumn: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = await PropertySearchColumn.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test PropertySearchColumn not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
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
