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
    public class AssetTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Assets";
        private string _key = "Asset_Key";

        private Asset _TestObj = new Asset()
        {
            GUID = new Guid("8D6435DD-32CF-4155-BE1B-25B73DC39F32"),
            Title = "xUNIT TESTx",
            FileName = "Test.txt",
            Extension = "txt",
            Size = 69,
            Type = "text/plain",
            Description = "Test Asset voor UNIT TEST",
            Created = DateTime.Now,
            IsOldStyle = false,
            IsNewStyle = true,
            IsArchived = false,
            IsImage = false,
            IsActive = false,
            RemoteLocation = null,
            ParentID = null,
            GalleryID = 1,
            CompletePath = "/",

            AssetTypeID = 1
        };

        private Asset _TestObjAsync = new Asset()
        {
            GUID = new Guid("72AD0CA5-F455-4CDB-B435-7DFBDD0BD682"),
            Title = "xASYNC UNIT TESTx",
            FileName = "Test.txt",
            Extension = "txt",
            Size = 69,
            Type = "text/plain",
            Description = "Test Asset voor UNIT ASYNC TEST",
            Created = DateTime.Now,
            IsOldStyle = false,
            IsNewStyle = true,
            SortOrder = 1,
            IsArchived = false,
            IsImage = false,
            IsActive = false,
            RemoteLocation = null,
            ParentID = null,
            GalleryID = 1,

            AssetTypeID = 2
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj(int galleryID = 0, int assetTypeID = 0, bool? isImage = null, bool? isActive = null, bool? isArchived = null)
        {
            if (galleryID > 0)
                _TestObj.GalleryID = galleryID; 
            
            if (assetTypeID > 0)
                _TestObj.AssetTypeID = assetTypeID;

            if (isImage != null)
                _TestObj.IsImage = (bool)isImage;

            if (isActive != null)
                _TestObj.IsActive = (bool)isActive;

            if (isArchived != null)
                _TestObj.IsArchived = (bool)isArchived;

            _TestObj.Save();

            Assert.IsTrue(_TestObj.ID > 0);

            var dbAsset = Asset.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.ID, dbAsset.ID);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED Asset: {_TestObj.ID}");
            }
        }
            public async Task A_Create_TestObjAsync(int galleryID = 0, int assetTypeID = 0, bool? isImage = null, bool? isActive = null, bool? isArchived = null)
        {
            if (galleryID > 0)          
                _TestObjAsync.GalleryID = galleryID;

            if (assetTypeID > 0)
                _TestObjAsync.AssetTypeID = assetTypeID;

            if (isImage != null)
                _TestObjAsync.IsImage = (bool)isImage;

            if (isActive != null)
                _TestObjAsync.IsActive = (bool)isActive;

            if (isArchived != null)
                _TestObjAsync.IsArchived = (bool)isArchived;

            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync.ID > 0);

            var dbAsset = Asset.SelectOne(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.ID, dbAsset.ID);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED Asset: {_TestObjAsync.ID}");
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
                var asset = Asset.SelectOne(_TestObj.ID);

                Assert.IsFalse(asset.ID == 0);

                if (asset?.ID > 0)
                    Trace.WriteLine($"FOUND Asset: {asset.ID}");
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
                var asset = await Asset.SelectOneAsync(_TestObjAsync.ID);

                Assert.IsFalse(asset.ID == 0);

                if (asset?.ID > 0)
                    Trace.WriteLine($"FOUND Asset: {asset.ID}");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByIDWithAssetType()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var asset = Asset.SelectOne(_TestObj.GalleryID, (int)_TestObj.AssetTypeID);

                Assert.IsFalse(asset.ID == 0);

                if (asset?.ID > 0)
                    Trace.WriteLine($"FOUND Asset: {asset.ID}");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByIDWithAssetTypeAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var asset = await Asset.SelectOneAsync(_TestObjAsync.GalleryID, (int)_TestObjAsync.AssetTypeID);

                Assert.IsFalse(asset.ID == 0);

                if (asset?.ID > 0)
                    Trace.WriteLine($"FOUND Asset: {asset.ID}");
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
        public void B_SelectAll()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var asset = Asset.SelectAll_Local();

                if (asset?.Count > 0)
                    Trace.WriteLine($"FOUND Asset: {asset.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Asset NOT FOUND...");

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
                var asset = await Asset.SelectAll_LocalAsync();

                if (asset?.Count > 0)
                    Trace.WriteLine($"FOUND Asset: {asset.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Asset NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        #endregion Select All

        #region Update

        [TestMethod]
        public void C_Update()
        {
            string fileName = "Verandering.txt";
            try
            {
                A_Create_TestObj();

                var asset = Asset.SelectOne(_TestObj.ID);
                if (asset?.ID > 0)
                {
                    asset.FileName = fileName;
                    // Function that we are testing BELOW...
                    asset.Save();

                    asset = Asset.SelectOne(_TestObj.ID);
                    Assert.AreEqual(fileName, asset.FileName);
                }
                else
                {
                    Assert.Fail($"Asset with ID {_TestObj.ID} not found");
                }
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }
        [TestMethod]
        public async Task C_UpdateAsync()
        {
            string fileName = "ASYNC_Verandering.txt";
            try
            {
                await A_Create_TestObjAsync();

                var asset = await Asset.SelectOneAsync(_TestObjAsync.ID);
                if (asset?.ID > 0)
                {
                    asset.FileName = fileName;
                    // Function that we are testing BELOW...
                    await asset.SaveAsync();

                    asset = await Asset.SelectOneAsync(_TestObjAsync.ID);
                    Assert.AreEqual(fileName, asset.FileName);
                }
                else
                {
                    Assert.Fail($"Asset with ID {_TestObjAsync.ID} not found");
                }
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }
        #endregion Update

        #region Delete
        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = Asset.SelectAll_Local();
            var ass = allAssert.Where(x => x.Title == _TestObj.Title);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Asset wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Asset Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                if (a?.ID > 0)
                {
                    // Asset does not contain a Delete(), Replace code below when Delete() is added.
                    var connector = ConnectorFactory.CreateConnector(new Asset.AssetMap(true));
                    connector.Delete(a);
                    Trace.WriteLine($"DELETE Asset: {a?.ID}");

                    // Check if delete in the DB is succesfull, for this record
                    var testDelete = Asset.SelectOne((int)a?.ID);
                    if (testDelete?.ID > 0) // ERROR
                        errorList.Add($"{a?.ID}");
                }
            }

            // Return error if they exist
            if (errorList.Count > 0)
                Assert.Fail($"Test Asset not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all posible test records, we will delete them ALL
            var allAssert = await Asset.SelectAll_LocalAsync();
            var ass = allAssert.Where(x => x.Title == _TestObjAsync.Title);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Asset wasn't found, you created it yet?");
            }
            Trace.WriteLine($"Delete Asset Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                if (a?.ID > 0)
                {
                    // Asset does not contain a DeleteAsync(), Replace code below when DeleteAsync() is added.
                    var connector = ConnectorFactory.CreateConnector(new Asset.AssetMap(true));
                    await connector.DeleteAsync(a);
                    Trace.WriteLine($"DELETE Asset: {a?.ID}");

                    // Check if delete in the DB is succesfull, for this record
                    var testDelete = await Asset.SelectOneAsync((int)a?.ID);
                    if (testDelete?.ID > 0) // ERROR
                        errorList.Add($"{a?.ID}");
                }
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Asset not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
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
            var connector = ConnectorFactory.CreateConnector<Asset>();
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
