using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class AssetTypeTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_AssetTypes";
        private string _key = "AssetType_Key";

        private AssetType _TestObj = new AssetType()
        {
            Guid = Guid.NewGuid(),
            Tag = "Test",
            Name = "xUNIT TESTx",
            Created = DateTime.Now,
            IsVariant = false
        };

        private AssetType _TestObjAsync = new AssetType()
        {
            Guid = Guid.NewGuid(),
            Tag = "Async Test",
            Name = "xASYNC UNIT TESTx",
            Created = DateTime.Now,
            IsVariant = false
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj?.ID > 0);
         
            var dbAssetType = AssetType.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.Name, dbAssetType.Name);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED AssetType: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync?.ID > 0);
         
            var dbAssetType = await AssetType.SelectOneAsync(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.Name, dbAssetType.Name);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED AssetType: {_TestObjAsync.ID}");
            }
        }
        #endregion Create

        #region Select One
        [TestMethod]
        public void B_SelectOneByID()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var asset = AssetType.SelectOne(_TestObj.ID);

                if (asset?.ID > 0)
                    Trace.WriteLine($"FOUND AssetType: {asset.ID}");
                else
                    Assert.Fail("AssetType NOT FOUND...");
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
                var asset = await AssetType.SelectOneAsync(_TestObjAsync.ID);

                if (asset?.ID > 0)
                    Trace.WriteLine($"FOUND AssetType: {asset.ID}");
                else
                    Assert.Fail("AssetType NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByTag()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var asset = AssetType.SelectOne(_TestObj.Tag);

                if (asset?.ID > 0)
                    Trace.WriteLine($"FOUND AssetType: {asset.ID}");
                else
                    Assert.Fail("AssetType NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByTagAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var asset = await AssetType.SelectOneAsync(_TestObjAsync.Tag);

                if (asset?.ID > 0)
                    Trace.WriteLine($"FOUND AssetType: {asset.ID}");
                else
                    Assert.Fail("AssetType NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }
        #endregion Select One

        #region Select All
        [TestMethod]
        public void X_SelectAll()
        {
            // Function that we are testing BELOW...
            var assetType = AssetType.SelectAll();

            if (assetType?.Length > 0)
                Trace.WriteLine($"FOUND AssetType: {assetType.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("AssetType NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllAsync()
        {
            // Function that we are testing BELOW...
            var assetType = await AssetType.SelectAllAsync();

            if (assetType?.Length > 0)
                Trace.WriteLine($"FOUND AssetType: {assetType.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("AssetType NOT FOUND...");
        }
        #endregion Select All

        #region Delete
        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = AssetType.SelectAll();
            var ass = allAssert.Where(x => x.Name == _TestObj.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test AssetType wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE AssetType Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (AssetType a in ass)
            {
                if (a?.ID > 0)
                {
                    // AssetType does not contain a Delete(), Replace code below when Delete() is added.
                    var connector = ConnectorFactory.CreateConnector<AssetType>();
                    connector.Delete(a);
                    Trace.WriteLine($"DELETE AssetType: {a?.ID}");

                    // Check if delete in the DB is succesfull, for this record
                    var testDelete = AssetType.SelectOne((int)a?.ID);
                    if (testDelete?.ID > 0) // ERROR
                        errorList.Add($"{a?.ID}");
                }
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test AssetType not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all posible test records, we will delete them ALL
            var allAssert = AssetType.SelectAll();
            var ass = allAssert.Where(x => x.Name == _TestObjAsync.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test AssetType wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE AssetType Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (AssetType a in ass)
            {
                if (a?.ID > 0)
                {
                    // AssetType does not contain a DeleteAsync(), Replace code below when DeleteAsync() is added.
                    var connector = ConnectorFactory.CreateConnector<AssetType>();
                    await connector.DeleteAsync(a);
                    Trace.WriteLine($"DELETE AssetType: {a?.ID}");

                    // Check if delete in the DB is succesfull, for this record
                    var testDelete = await AssetType.SelectOneAsync((int)a?.ID);
                    if (testDelete?.ID > 0) // ERROR
                        errorList.Add($"{a?.ID}");
                }
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test AssetType not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<AssetType>();
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
            var connector = ConnectorFactory.CreateConnector<AssetType>();
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
