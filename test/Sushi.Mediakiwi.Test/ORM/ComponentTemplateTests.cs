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
    public class ComponentTemplateTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_ComponentTemplates";
        private string _key = "ComponentTemplate_Key";

        private static DateTime _date = DateTime.Now;
        // Test object
        private ComponentTemplate _TestObj = new ComponentTemplate()
        {
            Name = "xUNIT TESTx",
            Location = "",
            TypeDefinition = "", // Set in Create
            SourceTag = "testcomponent",
            Source = "",
            ReferenceID = 666,
            IsSearchable = true,
            SiteID = 0,
            IsFixedOnPage = false,
            Description = "Test voor UNIT TEST",
            CanReplicate = true,
            CacheLevel = 0,
            OutputCacheParams = null,
            CanDeactivate = false,
            AjaxType = 0,
            CanMoveUpDown = true,
            IsHeader = false,
            IsFooter = false,
            IsSecundaryContainerItem = false,
            IsShared = false,
            GUID = new Guid("031D6E53-4B5A-496C-A84F-97ABB454D014"),
            IsListTemplate = false,
            MetaData = @"<ArrayOfMetaData xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""> <MetaData></MetaData> </ArrayOfMetaData>",
            LastWriteTimeUtc = _date
        };
        // Async test object        
        private ComponentTemplate _TestObjAsync = new ComponentTemplate()
        {
            Name = "xASYNC UNIT TESTx",
            Location = "",
            TypeDefinition = "", // Set in Create
            SourceTag = "testcomponent",
            Source = "",
            ReferenceID = 777,
            IsSearchable = true,
            SiteID = 0,
            IsFixedOnPage = false,
            Description = "Test voor ASYNC UNIT TEST",
            CanReplicate = true,
            CacheLevel = 0,
            OutputCacheParams = null,
            CanDeactivate = false,
            AjaxType = 0,
            CanMoveUpDown = true,
            IsHeader = false,
            IsFooter = false,
            IsSecundaryContainerItem = false,
            IsShared = false,
            GUID = new Guid("494769E2-7EAD-4285-BE01-DF91A7ED6385"),
            IsListTemplate = false,
            MetaData = @"<ArrayOfMetaData xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""> <MetaData></MetaData> </ArrayOfMetaData>",
            LastWriteTimeUtc = _date,
        };
        #endregion Test Data

        #region  Create
        public ComponentTemplate A_Create_TestObj(bool isShared = false)
        {
            if (isShared)
                _TestObj.IsShared = isShared;

            _TestObj.TypeDefinition = this.GetType().BaseType.ToString();
            _TestObj.Save();

            Assert.IsTrue(_TestObj?.ID > 0);

            ComponentTemplate db = ComponentTemplate.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.ID, db.ID);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED ComponentTemplate: {_TestObj.ID}");
            }

            return _TestObj;
        }

        public async Task<ComponentTemplate> A_Create_TestObjAsync(bool isShared = false)
        {
            if (isShared)
                _TestObjAsync.IsShared = isShared;

            _TestObjAsync.TypeDefinition = this.GetType().BaseType.ToString();
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            var db = await ComponentTemplate.SelectOneAsync(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.ID, db.ID);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED ComponentTemplate: {_TestObjAsync.ID}");
            }

            return _TestObjAsync;
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
                var componentTemplate = ComponentTemplate.SelectOne(_TestObj.ID);

                if (componentTemplate?.ID > 0)
                    Trace.WriteLine($"FOUN DComponentTemplate: {componentTemplate.ID}");
                else
                    Assert.Fail("ComponentTemplate NOT FOUND...");
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
                var componentTemplate = await ComponentTemplate.SelectOneAsync(_TestObjAsync.ID);

                if (componentTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentTemplate: {componentTemplate.ID}");
                else
                    Assert.Fail("ComponentTemplate NOT FOUND...");
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
                var componentTemplate = ComponentTemplate.SelectOne(_TestObj.GUID);

                if (componentTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentTemplate: {componentTemplate.ID}");
                else
                    Assert.Fail("ComponentTemplate NOT FOUND...");
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
                var componentTemplate = await ComponentTemplate.SelectOneAsync(_TestObjAsync.GUID);

                if (componentTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentTemplate: {componentTemplate.ID}");
                else
                    Assert.Fail("ComponentTemplate NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByReferenceID()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentTemplate = ComponentTemplate.SelectOneByReference((int)_TestObj.ReferenceID);

                if (componentTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentTemplate: {componentTemplate.ID}");
                else
                    Assert.Fail("ComponentTemplate NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByReferenceIDAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentTemplate = await ComponentTemplate.SelectOneByReferenceAsync((int)_TestObjAsync.ReferenceID);

                if (componentTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentTemplate: {componentTemplate.ID}");
                else
                    Assert.Fail("ComponentTemplate NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneBySourceTag()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentTemplate = ComponentTemplate.SelectOneBySourceTag(_TestObj.SourceTag);

                if (componentTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentTemplate: {componentTemplate.ID}");
                else
                    Assert.Fail("ComponentTemplate NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneBySourceTagAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentTemplate = await ComponentTemplate.SelectOneBySourceTagAsync(_TestObjAsync.SourceTag);

                if (componentTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentTemplate: {componentTemplate.ID}");
                else
                    Assert.Fail("ComponentTemplate NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByType()
        {
            try
            {
                A_Create_TestObj();

                // Get saved record from DB
                var allAssert = ComponentTemplate.SelectAll();
                var ass = allAssert.Where(x => x.Name == _TestObj.Name).FirstOrDefault();
                // Set TypeDefinition and Save
                var type = this.GetType();
                ass.TypeDefinition = type.ToString();
                ass.Save();

                // Function that we are testing BELOW...
                var componentTemplate = ComponentTemplate.SelectOne_BasedOnType(type);

                if (componentTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentTemplate: {componentTemplate.ID}");
                else
                    Assert.Fail("ComponentTemplate NOT FOUND...");

                ass.TypeDefinition = "";
                ass.Save();
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByTypeAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Get saved record from DB
                var allAssert = await ComponentTemplate.SelectAllAsync();
                var ass = allAssert.Where(x => x.Name == _TestObjAsync.Name).FirstOrDefault();
                // Set TypeDefinition and Save
                var type = this.GetType();
                ass.TypeDefinition = type.ToString();
                await ass.SaveAsync();

                // Function that we are testing BELOW...
                var componentTemplate = await ComponentTemplate.SelectOne_BasedOnTypeAsync(type);

                if (componentTemplate?.ID > 0)
                    Trace.WriteLine($"FOUND ComponentTemplate: {componentTemplate.ID}");
                else
                    Assert.Fail("ComponentTemplate NOT FOUND...");

                ass.TypeDefinition = "";
                await ass.SaveAsync();
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
        public void B_SelectAll()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentTemplates = ComponentTemplate.SelectAll();

                if (componentTemplates?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentTemplate: (Count {componentTemplates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)})");
                else
                    Assert.Fail("ComponentTemplate NOT FOUND...");
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
                var componentTemplates = await ComponentTemplate.SelectAllAsync();

                if (componentTemplates?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentTemplate: (Count {componentTemplates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)})");
                else
                    Assert.Fail("ComponentTemplate NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void X_SelectAllForPageTemplateAndPage()
        {
            int pageId = 1;
            int pageTemplateId = 1;

            // Function that we are testing BELOW...
            var componentTemplates = ComponentTemplate.SelectAll(pageTemplateId, pageId);

            if (componentTemplates?.Length > 0)
                Trace.WriteLine($"FOUND ComponentTemplate: (Count {componentTemplates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)})");
            else
                Assert.Fail("ComponentTemplate NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllForPageTemplateAndPageAsync()
        {
            int pageId = 1;
            int pageTemplateId = 1;

            // Function that we are testing BELOW...
            var componentTemplates = await ComponentTemplate.SelectAllAsync(pageTemplateId, pageId);

            if (componentTemplates?.Length > 0)
                Trace.WriteLine($"FOUND ComponentTemplate: (Count {componentTemplates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)})");
            else
                Assert.Fail("ComponentTemplate NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectAllAvailableForPage()
        {
            int pageId = 2;

            // Function that we are testing BELOW...
            var componentTemplates = ComponentTemplate.SelectAllAvailable(pageId);

            if (componentTemplates?.Length > 0)
                Trace.WriteLine($"FOUND ComponentTemplate: (Count {componentTemplates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)})");
            else
                Assert.Fail("ComponentTemplate NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllAvailableForPageAsync()
        {
            int pageId = 2;

            // Function that we are testing BELOW...
            var componentTemplates = await ComponentTemplate.SelectAllAvailableAsync(pageId);

            if (componentTemplates?.Length > 0)
                Trace.WriteLine($"FOUND ComponentTemplate: (Count {componentTemplates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)})");
            else
                Assert.Fail("ComponentTemplate NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectAllAvailableForPageSecundary()
        {
            int pageTemplateId = 4;

            // Function that we are testing BELOW...
            var componentTemplates = ComponentTemplate.SelectAllAvailable(pageTemplateId, true);

            if (componentTemplates?.Length > 0)
                Trace.WriteLine($"FOUND ComponentTemplate: (Count {componentTemplates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)})");
            else
                Assert.Fail("ComponentTemplate NOT FOUND...");
        }


        [TestMethod]
        public async Task X_SelectAllAvailableForPageSecundaryAsync()
        {
            int pageTemplateId = 4;

            // Function that we are testing BELOW...
            var componentTemplates = await ComponentTemplate.SelectAllAvailableAsync(pageTemplateId, true);

            if (componentTemplates?.Length > 0)
                Trace.WriteLine($"FOUND ComponentTemplate: (Count {componentTemplates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)})");
            else
                Assert.Fail("ComponentTemplate NOT FOUND...");
        }

        [TestMethod]
        public void B_SelectAllAvailableForPageSecundary2()
        {
            int pageTemplateID = 132456;
            A_Create_TestObj();

            // AvailableTemplate is also needed for this test
            AvailableTemplateTests availableTemplateTests = new AvailableTemplateTests();
            availableTemplateTests.A_Create_TestObj(pageTemplateID, _TestObj.ID);

            try
            {
                // Function that we are testing BELOW...
                var componentTemplates = ComponentTemplate.SelectAllAvailable(pageTemplateID, false, false);

                if (componentTemplates?.Length > 0 && _TestObj.Name == componentTemplates.First().Name)
                    Trace.WriteLine($"FOUND ComponentTemplate: (Count {componentTemplates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)})");
                else
                    Assert.Fail("ComponentTemplate NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                availableTemplateTests.D_Delete_TestObj();

                F_Reset_AutoIndent();
                availableTemplateTests.F_Reset_AutoIndent();
            }
        }


        [TestMethod]
        public async Task B_SelectAllAvailableForPageSecundary2Async()
        {
            int pageTemplateID = 123456;
            await A_Create_TestObjAsync();

            // AvailableTemplate is also needed for this test
            AvailableTemplateTests availableTemplateTests = new AvailableTemplateTests();
            await availableTemplateTests.A_Create_TestObjAsync(pageTemplateID, _TestObjAsync.ID);

            try
            {
                // Function that we are testing BELOW...
                var componentTemplates = await ComponentTemplate.SelectAllAvailableAsync(pageTemplateID, false, false);

                if (componentTemplates?.Length > 0 && _TestObjAsync.Name == componentTemplates.First().Name)
                    Trace.WriteLine($"FOUND ComponentTemplate: (Count {componentTemplates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)})");
                else
                    Assert.Fail("ComponentTemplate NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await availableTemplateTests.D_Delete_TestObjAsync();

                await F_Reset_AutoIndentAsync();
                await availableTemplateTests.F_Reset_AutoIndentAsync();
            }
        }


        [TestMethod]
        public void X_SelectAllShared()
        {
            // Function that we are testing BELOW...
            var componentTemplates = ComponentTemplate.SelectAllShared();

            if (componentTemplates?.Length > 0)
                Trace.WriteLine($"FOUND ComponentTemplate: (Count {componentTemplates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)})");
            else
                Assert.Fail("ComponentTemplate NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllSharedAsync()
        {
            // Function that we are testing BELOW...
            var componentTemplates = await ComponentTemplate.SelectAllSharedAsync();

            if (componentTemplates?.Length > 0)
                Trace.WriteLine($"FOUND ComponentTemplate: (Count {componentTemplates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)})");
            else
                Assert.Fail("ComponentTemplate NOT FOUND...");
        }

        #endregion Select ALL

        #region Delete
        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = ComponentTemplate.SelectAll();
            var ass = allAssert.Where(x => x.Name == _TestObj.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test ComponentTemplate wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE ComponentTemplate Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                a.Delete();
                Trace.WriteLine($"DELETE ComponentTemplate: {a.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = ComponentTemplate.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test ComponentTemplate not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await ComponentTemplate.SelectAllAsync();
            var ass = allAssert.Where(x => x.Name == _TestObjAsync.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test ComponentTemplate wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE ComponentTemplate Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                await a.DeleteAsync();
                Trace.WriteLine($"DELETE ComponentTemplate: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = await ComponentTemplate.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test ComponentTemplate not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
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
            var connector = ConnectorFactory.CreateConnector<ComponentTemplate>();
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
