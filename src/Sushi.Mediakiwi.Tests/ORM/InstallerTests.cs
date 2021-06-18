using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Tests.ORM
{
    [TestClass]
    public class InstallerTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Installers";
        private string _key = "Installer_Key";

        private static DateTime _date = DateTime.Now;
        // Test object
        private Installer _TestObj = new Installer()
        {
            GUID = new Guid("A083E41B-0D6C-4280-8EC3-52B3318AB415"),
            FolderID = 69,
            Name = "xUNIT TESTx",
            Assembly = "unit.test.dll",
            ClassName = "unit.test.installer",
            Description = "Test voor UNIT TEST",
            SettingsString = "<settings></settings>",
            Version = 420,
            Installed = _date
        };
        // Async Test object
        private Installer _TestObjAsync = new Installer()
        {
            GUID = new Guid("A6EF7CB7-2A97-476C-A236-71621E8CA328"),
            FolderID = 70,
            Name = "xASYNC UNIT TESTx",
            Assembly = "unit.async.test.dll",
            ClassName = "unit.async.test.installer",
            Description = "Test voor UNIT ASYNC TEST",
            SettingsString = "<settings></settings>",
            Version = 421,
            Installed = _date
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj()
        {
            _TestObj.Save();

            Assert.IsTrue(_TestObj?.ID > 0);

            Installer db = (Installer)Installer.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.Name, db.Name);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED Installer: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            Installer db = (Installer)Installer.SelectOne(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.Name, db.Name);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED Installer: {_TestObjAsync.ID}");
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
                var installer = Installer.SelectOne(_TestObj.ID);

                if (installer?.ID > 0)
                    Trace.WriteLine($"FOUND Installer: {installer.ID}");
                else
                    Assert.Fail("Installer NOT FOUND...");
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
                var installer = await Installer.SelectOneAsync(_TestObjAsync.ID);

                if (installer?.ID > 0)
                    Trace.WriteLine($"FOUND Installer: {installer.ID}");
                else
                    Assert.Fail("Installer NOT FOUND...");
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
                var installer = Installer.SelectOne(_TestObj.GUID);

                if (installer?.ID > 0)
                    Trace.WriteLine($"FOUND Installer: {installer.ID}");
                else
                    Assert.Fail("Installer NOT FOUND...");
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
                var installer = await Installer.SelectOneAsync(_TestObjAsync.GUID);

                if (installer?.ID > 0)
                    Trace.WriteLine($"FOUND Installer: {installer.ID}");
                else
                    Assert.Fail("Installer NOT FOUND...");
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
            var installer = Installer.SelectAll();

            if (installer?.Length > 0)
                Trace.WriteLine($"FOUND Installer: {installer.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Installer NOT FOUND...");
        }


        [TestMethod]
        public async Task X_SelectAllAsync()
        {
            // Function that we are testing BELOW...
            var installer = await Installer.SelectAllAsync();

            if (installer?.Length > 0)
                Trace.WriteLine($"FOUND Installer: {installer.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Installer NOT FOUND...");
        }

        #endregion Select ALL

        #region Delete
        public void D_Delete_TestObj()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = Installer.SelectAll();
            var ass = allAssert.Where(x => x.Name == _TestObj.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Installer wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Installer Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (Installer a in ass)
            {
                a.Delete();
                Trace.WriteLine($"DELETE Installer: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = Installer.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Installer not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await Installer.SelectAllAsync();
            var ass = allAssert.Where(x => x.Name == _TestObjAsync.Name);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test Installer wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Installer Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (Installer a in ass)
            {
                await a.DeleteAsync();
                Trace.WriteLine($"DELETE Installer: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                var testDelete = await Installer.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test Installer not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<Installer>();
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
            var connector = ConnectorFactory.CreateConnector<Installer>();
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
