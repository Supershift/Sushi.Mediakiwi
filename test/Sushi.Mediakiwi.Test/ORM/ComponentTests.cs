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
    public class ComponentTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Components";
        private string _key = "Component_Key";

        private static DateTime _date = DateTime.Now;
        // Test object
        private Component _TestObj = new Component()
        {
            GUID = new Guid("2297DFC6-A938-462E-A971-D5494D107E1B"),
            PageID = 69420,
            ComponentTemplateID = 10,
            Target = "unitTest",
            FixedId = "C000_unitTest",
            Created = _date,
            Updated = _date,
            IsFixedOnTemplate = true,
            IsAlive = false,
            IsSecundaryContainerItem = true,
            Serialized_XML = @"<Content xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><Fields></Fields></Content>",
            SortDate = null,
            SortOrder = 666
        };
        // Async test object
        private Component _TestObjAsync = new Component()
        {
            GUID = new Guid("8A1BD71C-5225-48D6-B141-C4209473AF97"),
            PageID = 69420,
            ComponentTemplateID = 10,
            Target = "unitAsyncTest",
            FixedId = "C000_unitAsyncTest",
            Created = _date,
            Updated = _date,
            IsFixedOnTemplate = true,
            IsAlive = false,
            IsSecundaryContainerItem = true,
            Serialized_XML = @"<Content xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><Fields></Fields></Content>",
            SortDate = null,
            SortOrder = 777
        };
        #endregion Test Data

        #region  Create
        public void A_Create_TestObj(int ComponentTemplateID = 0)
        {
            if (ComponentTemplateID > 0)
                _TestObj.ComponentTemplateID = ComponentTemplateID;

            _TestObj.Save();

            Assert.IsTrue(_TestObj?.ID > 0);

            Component db = Component.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.ID, db.ID);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED Component: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync(int ComponentTemplateID = 0)
        {
            if (ComponentTemplateID > 0)
                _TestObjAsync.ComponentTemplateID = ComponentTemplateID;

            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            var db = await Component.SelectOneAsync(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.ID, db.ID);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED Component: {_TestObjAsync.ID}");
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
                var component = Component.SelectOne(_TestObj.ID);

                if (component?.ID > 0)
                    Trace.WriteLine($"FOUND Component: {component.ID}");
                else
                    Assert.Fail("Component NOT FOUND...");
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
                var component = await Component.SelectOneAsync(_TestObjAsync.ID);

                if (component?.ID > 0)
                    Trace.WriteLine($"FOUND Component: {component.ID}");
                else
                    Assert.Fail("Component NOT FOUND...");
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
                var component = Component.SelectOne(_TestObj.GUID);

                if (component?.ID > 0)
                    Trace.WriteLine($"FOUND Component: {component.ID}");
                else
                    Assert.Fail("Component NOT FOUND...");
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
                var component = await Component.SelectOneAsync(_TestObjAsync.GUID);

                if (component?.ID > 0)
                    Trace.WriteLine($"FOUND Component: {component.ID}");
                else
                    Assert.Fail("Component NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectOneByPageAndFixedID()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var component = Component.SelectOne((int)_TestObj.PageID, _TestObj.FixedId);

                if (component?.ID > 0)
                    Trace.WriteLine($"FOUND Component: {component.ID}");
                else
                    Assert.Fail("Component NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectOneByPageAndFixedIDAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var component = await Component.SelectOneAsync((int)_TestObjAsync.PageID, _TestObjAsync.FixedId);

                if (component?.ID > 0)
                    Trace.WriteLine($"FOUND Component: {component.ID}");
                else
                    Assert.Fail("Component NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }


        [TestMethod]
        public void B_SelectOneByPageAndType()
        {
            // ComponentTemplate with test on ComponentTemplate_Type
            ComponentTemplateTests componentTemplateTest = new ComponentTemplateTests();
            ComponentTemplate templ = componentTemplateTest.A_Create_TestObj();

            // JOIN wim_Components ON ComponentTemplate_Key = Component_ComponentTemplate_Key
            A_Create_TestObj(templ.ID);

            try
            {
                // Function that we are testing BELOW...
                var component = Component.SelectOne((int)_TestObj.PageID, componentTemplateTest.GetType());

                if (component?.ID > 0)
                    Trace.WriteLine($"FOUND Component: {component.ID}");
                else
                    Assert.Fail("Component NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                componentTemplateTest.D_Delete_TestObj();

                F_Reset_AutoIndent();
                componentTemplateTest.F_Reset_AutoIndent();
            }
        }


        [TestMethod]
        public async Task B_SelectOneByPageAndTypeAsync()
        {
            // ComponentTemplate with test on ComponentTemplate_Type
            ComponentTemplateTests componentTemplateTest = new ComponentTemplateTests();
            ComponentTemplate templ = await componentTemplateTest.A_Create_TestObjAsync();

            // JOIN wim_Components ON ComponentTemplate_Key = Component_ComponentTemplate_Key
            await A_Create_TestObjAsync(templ.ID);

            try
            {
                // Function that we are testing BELOW...
                var component = await Component.SelectOneAsync((int)_TestObjAsync.PageID, componentTemplateTest.GetType());

                if (component?.ID > 0)
                    Trace.WriteLine($"FOUND Component: {component.ID}");
                else
                    Assert.Fail("Component NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await componentTemplateTest.D_Delete_TestObjAsync();

                await F_Reset_AutoIndentAsync();
                await componentTemplateTest.F_Reset_AutoIndentAsync();
            }
        }

        #endregion

        #region Select ALL
        [TestMethod]
        public void X_SelectAll()
        {
            // Function that we are testing BELOW...
            var components = Component.SelectAll();

            if (components?.Length > 0)
                Trace.WriteLine($"FOUND Component: {components.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Component NOT FOUND...");
        }


        [TestMethod]
        public async Task X_SelectAllAsync()
        {
            // Function that we are testing BELOW...
            var components = await Component.SelectAllAsync();

            if (components?.Length > 0)
                Trace.WriteLine($"FOUND Component: {components.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Component NOT FOUND...");
        }

        [TestMethod]
        public void B_SelectAllForPageID()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var components = Component.SelectAll((int)_TestObj.PageID);

                if (components?.Length > 0)
                    Trace.WriteLine($"FOUND Component: {components.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Component NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }


        [TestMethod]
        public async Task B_SelectAllForPageIDAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var components = await Component.SelectAllAsync((int)_TestObjAsync.PageID);

                if (components?.Length > 0)
                    Trace.WriteLine($"FOUND Component: {components.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Component NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectAllShared()
        {
            // wim_ComponentTemplates (met IsShared = true) 
            ComponentTemplateTests componentTemplateTest = new ComponentTemplateTests();
            ComponentTemplate templ = componentTemplateTest.A_Create_TestObj(true);

            // wim_Components (ON ComponentTemplate_Key = Component_ComponentTemplate_Key)
            A_Create_TestObj(templ.ID);

            // wim_ComponentTargets (ON Component_GUID = ComponentTarget_Component_Source), ComponentTarget_Page_Key = pageId
            ComponentTargetTests componentTargetTest = new ComponentTargetTests();
            componentTargetTest.A_Create_TestObj(_TestObj.GUID, (int)_TestObj.PageID);

            try
            {
                // Function that we are testing BELOW...
                var components = Component.SelectAllShared((int)_TestObj.PageID);

                if (components?.Length > 0)
                    Trace.WriteLine($"FOUND Component: {components.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Component NOT FOUND...");
            }
            finally
            {
                componentTargetTest.D_Delete_TestObj();
                D_Delete_TestObj();
                componentTemplateTest.D_Delete_TestObj();

                componentTargetTest.F_Reset_AutoIndent();
                F_Reset_AutoIndent();
                componentTemplateTest.F_Reset_AutoIndent();
            }
        }


        [TestMethod]
        public async Task B_SelectAllSharedAsync()
        {
            // wim_ComponentTemplates (met IsShared = true) 
            ComponentTemplateTests componentTemplateTest = new ComponentTemplateTests();
            ComponentTemplate templ = await componentTemplateTest.A_Create_TestObjAsync(true);

            // wim_Components (ON ComponentTemplate_Key = Component_ComponentTemplate_Key)
            await A_Create_TestObjAsync(templ.ID);

            // wim_ComponentTargets (ON Component_GUID = ComponentTarget_Component_Source), ComponentTarget_Page_Key = pageId
            ComponentTargetTests componentTargetTest = new ComponentTargetTests();
            await componentTargetTest.A_Create_TestObjAsync(_TestObjAsync.GUID, (int)_TestObjAsync.PageID);

            try
            {
                // Function that we are testing BELOW...
                var components = await Component.SelectAllSharedAsync((int)_TestObjAsync.PageID);

                if (components?.Length > 0)
                    Trace.WriteLine($"FOUND Component: {components.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Component NOT FOUND...");
            }
            finally
            {
                await componentTargetTest.D_Delete_TestObjAsync();
                await D_Delete_TestObjAsync();
                await componentTemplateTest.D_Delete_TestObjAsync();

                await componentTargetTest.F_Reset_AutoIndentAsync();
                await F_Reset_AutoIndentAsync();
                await componentTemplateTest.F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectAllPrimaryForPage()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var components = Component.SelectAll((int)_TestObj.PageID, false);

                if (components?.Length > 0)
                    Trace.WriteLine($"FOUND Component: {components.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Component NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllPrimaryForPageAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var components = await Component.SelectAllAsync((int)_TestObjAsync.PageID, false);

                if (components?.Length > 0)
                    Trace.WriteLine($"FOUND Component: {components.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Component NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }


        [TestMethod]
        public void B_SelectAllSecundaryForPage()
        {
            // ComponentTemplate is needed for this test
            ComponentTemplateTests componentTemplateTest = new ComponentTemplateTests();
            ComponentTemplate template = componentTemplateTest.A_Create_TestObj();

            _TestObj.ComponentTemplateID = template.ID;
            _TestObj.IsSecundaryContainerItem = true;

            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var components = Component.SelectAll((int)_TestObj.PageID, false);

                if (components?.Length > 0)
                    Trace.WriteLine($"FOUND Component: {components.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Component NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                componentTemplateTest.D_Delete_TestObj();

                F_Reset_AutoIndent();
                componentTemplateTest.F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllSecundaryForPageAsync()
        {
            // ComponentTemplate is needed for this test
            ComponentTemplateTests componentTemplateTest = new ComponentTemplateTests();
            ComponentTemplate template = await componentTemplateTest.A_Create_TestObjAsync();

            _TestObjAsync.ComponentTemplateID = template.ID;
            _TestObjAsync.IsSecundaryContainerItem = true;

            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var components = await Component.SelectAllAsync((int)_TestObjAsync.PageID, false);

                if (components?.Length > 0)
                    Trace.WriteLine($"FOUND Component: {components.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Component NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await componentTemplateTest.D_Delete_TestObjAsync();

                await F_Reset_AutoIndentAsync();
                await componentTemplateTest.F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectAllForPageAndTemplateID()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var components = Component.SelectAll((int)_TestObj.PageID, (int)_TestObj.ComponentTemplateID);

                if (components?.Length > 0)
                    Trace.WriteLine($"FOUND Component: {components.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Component NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllForPageAndTemplateIDAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var components = await Component.SelectAllAsync((int)_TestObjAsync.PageID, (int)_TestObjAsync.ComponentTemplateID);

                if (components?.Length > 0)
                    Trace.WriteLine($"FOUND Component: {components.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Component NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        #endregion Select ALL

        #region Delete
        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = Component.SelectAll();
            var ass = allAssert.Where(x => x.Target == _TestObj.Target);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Component wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Component Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                a.Delete();
                Trace.WriteLine($"DELETE Component: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = Component.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Component not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await Component.SelectAllAsync();
            var ass = allAssert.Where(x => x.Target == _TestObjAsync.Target);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Component wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Component Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (var a in ass)
            {
                await a.DeleteAsync();
                Trace.WriteLine($"DELETE Component: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = await Component.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Component not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<Component>();
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
            var connector = ConnectorFactory.CreateConnector<Component>();
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
