using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemanticComparison;
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
    public class PortalRightTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_PortalRights";
        private string _key = "PortalRight_Key";

        private PortalRight _TestObj = new PortalRight()
        {
            RoleID = 69,
            PortalID = 69420
        };

        private PortalRight _TestObjAsync = new PortalRight()
        {
            RoleID = 71,
            PortalID = 69421
        };
        #endregion Test Data

        #region Create
        public void A_Create_TestObj()
        {
            // PortalRight does not contain a Save(), Replace code below when Save() is added.
            var connector = ConnectorFactory.CreateConnector<PortalRight>();
            connector.Save(_TestObj);

            Assert.IsTrue(_TestObj?.ID > 0);

            // MJ: check if saved db record is ok            
            var expected = new Likeness<PortalRight, PortalRight>(_TestObj);
            PortalRight db = (PortalRight)PortalRight.SelectOne(_TestObj.ID);
            Assert.AreEqual(expected, db);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED PortalRight: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            // PortalRight does not contain a SaveAsync(), Replace code below when SaveAsync() is added.
            var connector = ConnectorFactory.CreateConnector<PortalRight>();
            await connector.SaveAsync(_TestObjAsync);

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            // MJ: check if saved db record is ok            
            var expected = new Likeness<PortalRight, PortalRight>(_TestObjAsync);
            PortalRight db = (PortalRight)PortalRight.SelectOne(_TestObjAsync.ID);
            Assert.AreEqual(expected, db);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED PortalRight: {_TestObjAsync.ID}");
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

                var allAssert = PortalRight.SelectAll(_TestObj.RoleID);
                var ass = (PortalRight)allAssert.Where(x => x.PortalID == _TestObj.PortalID).FirstOrDefault();

                // Function that we are testing BELOW...
                var portalRight = PortalRight.SelectOne(ass.ID);

                if (portalRight?.ID > 0)
                    Trace.WriteLine($"FOUND PortalRight: {portalRight.ID}");
                else
                    Assert.Fail("PortalRight NOT FOUND...");
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

                var allAssert = await PortalRight.SelectAllAsync(_TestObjAsync.RoleID);
                var ass = (PortalRight)allAssert.Where(x => x.PortalID == _TestObjAsync.PortalID).FirstOrDefault();

                // Function that we are testing BELOW...
                var portalRight = await PortalRight.SelectOneAsync(ass.ID);

                if (portalRight?.ID > 0)
                    Trace.WriteLine($"FOUND PortalRight: {portalRight.ID}");
                else
                    Assert.Fail("PortalRight NOT FOUND...");
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
        public void B_SelectAllForRoleID()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var portalRights = PortalRight.SelectAll(_TestObj.RoleID);

                if (portalRights?.Length > 0)
                    Trace.WriteLine($"FOUND PortalRight: {portalRights.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("PortalRight NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllForRoleIDAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var portalRights = await PortalRight.SelectAllAsync(_TestObjAsync.RoleID);

                if (portalRights?.Length > 0)
                    Trace.WriteLine($"FOUND PortalRight: {portalRights.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("PortalRight NOT FOUND...");
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
            var allAssert = PortalRight.SelectAll(_TestObj.RoleID);
            var ass = allAssert.Where(x => x.PortalID == _TestObj.PortalID);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test PortalRight wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE PortalRight Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (PortalRight a in ass)
            {
                // PortalRight does not contain a Delete(), Replace code below when Delete() is added.
                var connector = ConnectorFactory.CreateConnector<PortalRight>();
                connector.Delete(a);
                Trace.WriteLine($"DELETE PortalRight: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                connector = ConnectorFactory.CreateConnector<PortalRight>();
                var testDelete = PortalRight.SelectOne((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test PortalRight not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await PortalRight.SelectAllAsync(_TestObjAsync.RoleID);
            var ass = allAssert.Where(x => x.PortalID == _TestObjAsync.PortalID);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test PortalRight wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE PortalRight Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (PortalRight a in ass)
            {
                // PortalRight does not contain a DeleteAsync(), Replace code below when DeleteAsync() is added.
                var connector = ConnectorFactory.CreateConnector<PortalRight>();
                await connector.DeleteAsync(a);
                Trace.WriteLine($"DELETE PortalRight: {a?.ID}");

                // Check if delete in the DB is succesfull, for this record
                connector = ConnectorFactory.CreateConnector<PortalRight>();
                var testDelete = await PortalRight.SelectOneAsync((int)a?.ID);
                if (testDelete?.ID > 0) // ERROR
                    errorList.Add($"{a?.ID}");
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test PortalRight not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<PortalRight>();
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
            var connector = ConnectorFactory.CreateConnector<PortalRight>();
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
