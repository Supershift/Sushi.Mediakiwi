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
    public class AvailableTemplateTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_AvailableTemplates";
        private string _key = "AvailableTemplates_Key";

        private AvailableTemplate _TestObj = new AvailableTemplate()
        {
            GUID = Guid.NewGuid(),
            PageTemplateID = 1,
            ComponentTemplateID = 1,
            Target = "xUNIT TESTx",
            IsPossible = true,
            IsSecundary = false,
            IsPresent = false,
            SortOrder = 1,
            FixedFieldName = "UNIT_TEST",
        };

        private AvailableTemplate _TestObjAsync = new AvailableTemplate()
        {
            GUID = Guid.NewGuid(),
            PageTemplateID = 1,
            ComponentTemplateID = 2,
            Target = "xASYNC UNIT TESTx",
            IsPossible = true,
            IsSecundary = false,
            IsPresent = false,
            SortOrder = 1,
            FixedFieldName = "UNIT_ASYNC_TEST",
        };
        #endregion Test Data

        #region Create 
        public AvailableTemplate A_Create_TestObj(int pageTemplateID = 0, int componentTemplateID = 0)
        {
            if (pageTemplateID > 0)
                _TestObj.PageTemplateID = pageTemplateID;
            if (componentTemplateID > 0)
                _TestObj.ComponentTemplateID = componentTemplateID;

            _TestObj.Save();

            Assert.IsTrue(_TestObj?.ID > 0);

            var dbAsset = AvailableTemplate.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.ID, dbAsset.ID);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED AvailableTemplate: {_TestObj.ID}");
            }

            return _TestObj;
        }

        public async Task<AvailableTemplate> A_Create_TestObjAsync(int pageTemplateID = 0, int componentTemplateID = 0)
        {
            if (pageTemplateID > 0)
                _TestObjAsync.PageTemplateID = pageTemplateID;
            if (componentTemplateID > 0)
                _TestObjAsync.ComponentTemplateID = componentTemplateID;

            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            var dbAsset = AvailableTemplate.SelectOne(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.ID, dbAsset.ID);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED AvailableTemplate: {_TestObjAsync.ID}");
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

                var allAssert = AvailableTemplate.SelectAll();
                var ass = allAssert.Where(x => x.Target == _TestObj.Target).FirstOrDefault();

                // Function that we are testing BELOW...
                var template = AvailableTemplate.SelectOne(ass.ID);
                Assert.IsFalse(template == null || template.ID == 0);

                if (template?.ID > 0)
                    Trace.WriteLine($"FOUND AvailableTemplate: {template.ID}");
                else
                    Assert.Fail("AvailableTemplate NOT FOUND...");
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

                var allAssert = AvailableTemplate.SelectAll();
                var ass = allAssert.Where(x => x.Target == _TestObjAsync.Target).FirstOrDefault();

                // Function that we are testing BELOW...
                var template = await AvailableTemplate.SelectOneAsync(ass.ID);
                Assert.IsFalse(template == null || template.ID == 0);

                if (template?.ID > 0)
                    Trace.WriteLine($"FOUND AvailableTemplate: {template.ID}");
                else
                    Assert.Fail("AvailableTemplate NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByPageAndTag()
        {
            try
            {
                A_Create_TestObj();

                var allAssert = AvailableTemplate.SelectAll();
                var ass = allAssert.Where(x => x.Target == _TestObj.Target).FirstOrDefault();

                // Function that we are testing BELOW...
                var template = AvailableTemplate.SelectOne(ass.PageTemplateID, ass.FixedFieldName);
                Assert.IsFalse(template == null || template.ID == 0);

                if (template?.ID > 0)
                    Trace.WriteLine($"FOUND AvailableTemplate: {template.ID}");
                else
                    Assert.Fail("AvailableTemplate NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByPageAndTagAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                var allAssert = AvailableTemplate.SelectAll();
                var ass = allAssert.Where(x => x.Target == _TestObjAsync.Target).FirstOrDefault();

                // Function that we are testing BELOW...
                var template = await AvailableTemplate.SelectOneAsync(ass.PageTemplateID, ass.FixedFieldName);
                Assert.IsFalse(template == null || template.ID == 0);

                if (template?.ID > 0)
                    Trace.WriteLine($"FOUND AvailableTemplate: {template.ID}");
                else
                    Assert.Fail("AvailableTemplate NOT FOUND...");
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
            var templates = AvailableTemplate.SelectAll();

            if (templates?.Length > 0)
                Trace.WriteLine($"FOUND AvailableTemplate: {templates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("AvailableTemplate NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllAsync()
        {
            // Function that we are testing BELOW...
            var templates = await AvailableTemplate.SelectAllAsync();

            if (templates?.Length > 0)
                Trace.WriteLine($"FOUND AvailableTemplate: {templates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("AvailableTemplate NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectAllByPageTemplate()
        {
            // Function that we are testing BELOW...
            var templates = AvailableTemplate.SelectAll(1);

            if (templates?.Length > 0)
                Trace.WriteLine($"FOUND AvailableTemplate: {templates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("AvailableTemplate NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllByPageTemplateAsync()
        {
            // Function that we are testing BELOW...
            var templates = await AvailableTemplate.SelectAllAsync(1);

            if (templates?.Length > 0)
                Trace.WriteLine($"FOUND AvailableTemplate: {templates.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("AvailableTemplate NOT FOUND...");
        }

        #endregion Select ALL

        #region Delete

        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = AvailableTemplate.SelectAll();
            var ass = allAssert.Where(x => x.Target == _TestObj.Target);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test AvailableTemplate wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE AvailableTemplate Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                if (a?.ID > 0)
                {
                    ((AvailableTemplate)a).Delete();
                    Trace.WriteLine($"DELETE AvailableTemplate: {a?.ID}");

                    // Check if delete in the DB is succesfull, for this record
                    var testDelete = AvailableTemplate.SelectOne((int)a?.ID);
                    if (testDelete?.ID > 0) // ERROR
                        errorList.Add($"{a?.ID}");
                }
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test AvailableTemplate not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all posible test records, we will delete them ALL
            var allAssert = await AvailableTemplate.SelectAllAsync();
            var ass = allAssert.Where(x => x.Target == _TestObjAsync.Target);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test AvailableTemplate wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE AvailableTemplate Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                if (a?.ID > 0)
                {
                    await ((AvailableTemplate)a).DeleteAsync();
                    Trace.WriteLine($"DELETE AvailableTemplate: {a?.ID}");

                    // Check if delete in the DB is succesfull, for this record
                    var testDelete = await AvailableTemplate.SelectOneAsync((int)a?.ID);
                    if (testDelete?.ID > 0) // ERROR
                        errorList.Add($"{a?.ID}");
                }
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test AvailableTemplate not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>();
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
            var connector = ConnectorFactory.CreateConnector<AvailableTemplate>();
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
