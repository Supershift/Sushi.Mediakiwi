using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class CatalogTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Catalogs";
        private string _key = "Catalog_Key";

        private Catalog _TestObj = new Catalog()
        {
            GUID = new Guid("B29DB35F-76D5-434F-B1B4-3B0CF408050F"),
            Title = "xUNIT TESTx",
            Table = "tst_UnitTest",
            IsActive = true,
            ColumnPrefix = "UnitTest",
            Created = DateTime.Now,
            HasSortOrder = false,
            HasCatalogBaseStructure = false,
            ConnectionIndex = 0,
            PortalName = null,
            ColumnKey = null,
            ColumnGuid = null,
            ColumnData = null
        };

        private Catalog _TestObjAsync = new Catalog()
        {
            GUID = new Guid("75D89F58-70D8-4B86-9B04-BF1941F9F5D6"),
            Title = "xASYNC UNIT TESTx",
            Table = "tst_UnitAsyncTest",
            IsActive = true,
            ColumnPrefix = "UnitAsyncTest",
            Created = DateTime.Now,
            HasSortOrder = false,
            HasCatalogBaseStructure = false,
            ConnectionIndex = 0,
            PortalName = null,
            ColumnKey = null,
            ColumnGuid = null,
            ColumnData = null
        };
        #endregion Test Data

        #region Create

        public void A_Create_TestObj()
        {
            // Catalog does not contain a Save(), Replace code below when Save() is added.
            var connector = ConnectorFactory.CreateConnector<Catalog>();
            connector.Save(_TestObj);

            Assert.IsTrue(_TestObj?.ID > 0);

            var db = Catalog.SelectOne(_TestObj.Table);
            Assert.AreEqual(_TestObj.ID, db.ID);

            var filter = connector.CreateDataFilter();
            // Query for create test table
            string sql = $"CREATE TABLE [dbo].[{_TestObj.Table}]([{_TestObj.ColumnPrefix}_Key] [int] IDENTITY(1,1) NOT NULL)";
            try
            {
                connector.ExecuteNonQuery(sql, filter);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Error during create of test table: {ex.Message}");
            }

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED Catalog: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            // Catalog does not contain a SaveAsync(), Replace code below when SaveAsync() is added.
            var connector = ConnectorFactory.CreateConnector<Catalog>();
            await connector.SaveAsync(_TestObjAsync);

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            var db = Catalog.SelectOne(_TestObjAsync.Table);
            Assert.AreEqual(_TestObjAsync.ID, db.ID);

            var filter = connector.CreateDataFilter();
            // Query for create test table
            string sql = $"CREATE TABLE [dbo].[{_TestObjAsync.Table}]([{_TestObj.ColumnPrefix}_Key] [int] IDENTITY(1,1) NOT NULL)";
            try
            {
                await connector.ExecuteNonQueryAsync(sql, filter);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Error during create of test table: {ex.Message}");
            }

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED Catalog: {_TestObjAsync.ID}");
            }
        }

        #endregion Create

        #region Select ONE

        [TestMethod]
        public void B_SelectOneByTable()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var catalog = Catalog.SelectOne(_TestObj.Table);

                if (catalog?.ID > 0)
                    Trace.WriteLine($"FOUND Catalog: {catalog.ID}");
                else
                    Assert.Fail("Catalog NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByTableAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var catalog = await Catalog.SelectOneAsync(_TestObjAsync.Table);

                if (catalog?.ID > 0)
                    Trace.WriteLine($"FOUND Catalog: {catalog.ID}");
                else
                    Assert.Fail("Catalog NOT FOUND...");
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
            var catalogs = Catalog.SelectAll();

            if (catalogs?.Length > 0)
                Trace.WriteLine($"FOUND Catalog: {catalogs.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Catalog NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllAsync()
        {
            // Function that we are testing BELOW...
            var catalogs = await Catalog.SelectAllAsync();

            if (catalogs?.Length > 0)
                Trace.WriteLine($"FOUND Catalog: {catalogs.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Catalog NOT FOUND...");
        }

        #endregion Select ALL

        #region Delete
        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = Catalog.SelectAll();
            var ass = allAssert.Where(x => x.Table == _TestObj.Table);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Catalog wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Catalog Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                if (a?.ID > 0)
                {
                    a.DeleteSqlTable();

                    // Catalog does not contain a Delete(), Replace code below when Delete() is added.
                    var connector = ConnectorFactory.CreateConnector<Catalog>();
                    connector.Delete(a);

                    // Check if delete in the DB is succesfull, for this record
                    var testDelete = Catalog.SelectOne(a?.Table);
                    if (testDelete?.ID > 0) // ERROR
                        errorList.Add($"{a?.ID}");
                }
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Catalog not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all posible test records, we will delete them ALL
            var allAssert = await Catalog.SelectAllAsync();
            var ass = allAssert.Where(x => x.Table == _TestObjAsync.Table);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Catalog wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Catalog Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                if (a?.ID > 0)
                {
                    await a.DeleteSqlTableAsync();
                    // Catalog does not contain a DeleteAsync(), Replace code below when DeleteAsync() is added.
                    var connector = ConnectorFactory.CreateConnector<Catalog>();
                    await connector.DeleteAsync(a);

                    // Check if delete in the DB is succesfull, for this record
                    var testDelete = await Catalog.SelectOneAsync(a?.Table);
                    if (testDelete?.ID > 0) // ERROR
                        errorList.Add($"{a?.ID}");
                }
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Catalog not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<Catalog>();
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
            var connector = ConnectorFactory.CreateConnector<Catalog>();
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
