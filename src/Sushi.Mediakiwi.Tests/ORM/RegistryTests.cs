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
    public class RegistryTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Registry";
        private string _key = "Registry_Key";

        private Registry _TestObj = new Registry()
        {
            GUID = new Guid("7D0289C4-0348-4FFA-954C-D0EEFE861953"),
            Name = "xUNIT_TESTx",
            Type = 1,
            Value = "Test Value",
            Description = "Test Asset voor UNIT TEST"
        };

        private Registry _TestObjAsync = new Registry()
        {
            GUID = new Guid("A2B1B4BE-7D35-43F5-B861-842DEFFBEFC9"),
            Name = "xUNIT_ASYNC_TESTx",
            Type = 1,
            Value = "Test Value",
            Description = "Test Asset voor UNIT ASYNC TEST"
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj.ID > 0);

            // MJ: check if saved db record is ok            
            var expected = new Likeness<Registry, Registry>(_TestObj);
            Registry db = (Registry)Registry.SelectOne(_TestObj.ID);
            Assert.AreEqual(expected, db);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED Registry: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync.ID > 0);

            // MJ: check if saved db record is ok            
            var expected = new Likeness<Registry, Registry>(_TestObjAsync);
            Registry db = (Registry)Registry.SelectOne(_TestObjAsync.ID);
            Assert.AreEqual(expected, db);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED Registry: {_TestObjAsync.ID}");
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
                var registry = Registry.SelectOne(_TestObj.ID);

                if (registry?.ID > 0)
                    Trace.WriteLine($"FOUND: {registry.ID}");
                else
                    Assert.Fail("NOT FOUND...");
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
                var registry = await Registry.SelectOneAsync(_TestObjAsync.ID);

                if (registry?.ID > 0)
                    Trace.WriteLine($"FOUND: {registry.ID}");
                else
                    Assert.Fail("NOT FOUND...");
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
                var registry = Registry.SelectOneByName(_TestObj.Name);

                if (registry?.ID > 0)
                    Trace.WriteLine($"FOUND: {registry.ID}");
                else
                    Assert.Fail("NOT FOUND...");
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
                var registry = await Registry.SelectOneByNameAsync(_TestObjAsync.Name);

                if (registry?.ID > 0)
                    Trace.WriteLine($"FOUND: {registry.ID}");
                else
                    Assert.Fail("NOT FOUND...");
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
                var registrys = Registry.SelectAll();

                if (registrys?.Length > 0)
                    Trace.WriteLine($"FOUND: (Count {registrys.Length})");
                else
                    Assert.Fail("NOT FOUND...");
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
                var registrys = await Registry.SelectAllAsync();

                if (registrys?.Length > 0)
                    Trace.WriteLine($"FOUND: (Count {registrys.Length})");
                else
                    Assert.Fail("NOT FOUND...");
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
            var allAssert = Registry.SelectAll();
            var ass = allAssert.Where(x => x.Name == _TestObj.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Registry wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Registry Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (Registry a in ass)
            {
                a.Delete();
                Trace.WriteLine($"DELETE Registry: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = Registry.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Registry not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await Registry.SelectAllAsync();
            var ass = allAssert.Where(x => x.Name == _TestObjAsync.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Registry wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Registry Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (Registry a in ass)
            {
                await a.DeleteAsync();
                Trace.WriteLine($"DELETE Registry: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = await Registry.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Registry not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
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
