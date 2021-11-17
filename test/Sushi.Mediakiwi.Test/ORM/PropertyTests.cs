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
    public class PropertyTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Properties";
        private string _key = "Property_Key";

        private Property _TestObj = new Property()
        {
            GUID = new Guid("D6538313-94D3-4309-B8CD-03FBE7DD25A7"),
            ListID = 69420,
            ListTypeID = null,
            Title = "xUNIT TESTx",
            IsPresentProperty = true,
            FieldName = "Test",
            ContentTypeID = ContentType.TextField,
            FilterType = null,
            OptionListSelect = null,
            IsShort = false,
            ListSelect = null,
            ListCollection = null,
            PropertyType = 0,
            Filter = null,
            CanFilter = false,
            CanOnlyCreate = false,
            Data = @"<MetaData xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><title>Test</title></MetaData>",
            IsFixed = true,
            InheritedID = null,
            SortOrder = 1
        };

        private Property _TestObjAsync = new Property()
        {
            GUID = new Guid("33C0CA97-3D40-4386-A80D-5BF05347FADC"),
            ListID = 69421,
            ListTypeID = null,
            Title = "xASYNC UNIT TESTx",
            IsPresentProperty = true,
            FieldName = "ASYNC Test",
            ContentTypeID = ContentType.TextField,
            FilterType = null,
            OptionListSelect = null,
            IsShort = false,
            ListSelect = null,
            ListCollection = null,
            PropertyType = 0,
            Filter = null,
            CanFilter = false,
            CanOnlyCreate = false,
            Data = @"<MetaData xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><title>ASYNC Test</title></MetaData>",
            IsFixed = true,
            InheritedID = null,
            SortOrder = 1
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj.ID > 0);

            Property db = Property.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.GUID, db.GUID);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED Property: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync.ID > 0);

            Property db = Property.SelectOne(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.GUID, db.GUID);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED Property: {_TestObjAsync.ID}");
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
                var property = Property.SelectOne(_TestObj.ID);

                if (property?.ID > 0)
                    Trace.WriteLine($"FOUND Property: {property.ID}");
                else
                    Assert.Fail("Property NOT FOUND...");
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
                var property = await Property.SelectOneAsync(_TestObjAsync.ID);

                if (property?.ID > 0)
                    Trace.WriteLine($"FOUND Property: {property.ID}");
                else
                    Assert.Fail("Property NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }


        [TestMethod]
        public void B_SelectOneByListAndFieldName()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var property = Property.SelectOne(_TestObj.ListID, _TestObj.FieldName, null);

                if (property?.ID > 0)
                    Trace.WriteLine($"FOUND Property: {property.ID}");
                else
                    Assert.Fail("Property NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByListAndFieldNameAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var property = await Property.SelectOneAsync(_TestObjAsync.ListID, _TestObjAsync.FieldName, null);

                if (property?.ID > 0)
                    Trace.WriteLine($"FOUND Property: {property.ID}");
                else
                    Assert.Fail("Property NOT FOUND...");
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
                var properties = Property.SelectAll();

                if (properties?.Count > 0)
                    Trace.WriteLine($"FOUND Property: {properties.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Property NOT FOUND...");
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
                var properties = await Property.SelectAllAsync();

                if (properties?.Count > 0)
                    Trace.WriteLine($"FOUND Property: {properties.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Property NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void X_SelectAllForIDs()
        {
            List<int> IDs = new List<int>() { 1, 2, 54 };

            // Function that we are testing BELOW...
            var properties = Property.SelectAll(IDs.ToArray());

            if (properties?.Length > 0)
                Trace.WriteLine($"FOUND Property: {properties.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Property NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllForIDsAsync()
        {
            List<int> IDs = new List<int>() { 1, 2, 54 };

            // Function that we are testing BELOW...
            var properties = await Property.SelectAllAsync(IDs.ToArray());

            if (properties?.Length > 0)
                Trace.WriteLine($"FOUND Property: {properties.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Property NOT FOUND...");
        }
        #endregion Select All

        #region Delete
        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = Property.SelectAll();
            var ass = allAssert.Where(x => x.Title == _TestObj.Title);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Property wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Property Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (Property a in ass)
            {
                a.Delete();
                Trace.WriteLine($"DELETE Property: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = PortalRight.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Property not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await Property.SelectAllAsync();
            var ass = allAssert.Where(x => x.Title == _TestObjAsync.Title);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Property wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Property Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (Property a in ass)
            {
                await a.DeleteAsync();
                Trace.WriteLine($"DELETE Property: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = await Property.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Property not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
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
