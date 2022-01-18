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
    public class ComponentTargetTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_ComponentTargets";
        private string _key = "ComponentTarget_Key";

        // Test data
        private ComponentTarget _TestObj = new ComponentTarget()
        {
            PageID = 69420,
            Source = new Guid("C217AA4C-3511-48B8-8623-BF0C7AA0692C"),
            Target = new Guid("48503759-BD5D-4506-B1F2-F309D71FD867")
        };

        // Async test data
        private ComponentTarget _TestObjAsync = new ComponentTarget()
        {
            PageID = 69421,
            Source = new Guid("FE866B84-C060-4AA4-B00B-9A21B804BB23"),
            Target = new Guid("71757812-580B-4285-9850-0CDBD99D916A")
        };
        #endregion Test Data

        #region  Create

        public void A_Create_TestObj(Guid source = new Guid(), int pageID = 0)
        {
            if (source != Guid.Empty)
                _TestObj.Source = source;

            if (pageID > 0)
                _TestObj.PageID = pageID;

            _TestObj.Save();

            Assert.IsTrue(_TestObj?.ID > 0);

            // MJ: check if saved db record is ok            
            var expected = new Likeness<ComponentTarget, ComponentTarget>(_TestObj);
            ComponentTarget db = (ComponentTarget)ComponentTarget.SelectOne(_TestObj.ID);
            Assert.AreEqual(expected, db);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED ComponentTarget: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync(Guid source = new Guid(), int pageID = 0)
        {
            if (source != Guid.Empty)
                _TestObjAsync.Source = source;

            if (pageID > 0)
                _TestObjAsync.PageID = pageID;

            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            // MJ: check if saved db record is ok            
            var expected = new Likeness<ComponentTarget, ComponentTarget>(_TestObjAsync);
            var db = await ComponentTarget.SelectOneAsync(_TestObjAsync.ID);
            Assert.AreEqual(expected, (ComponentTarget)db);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED ComponentTarget: {_TestObjAsync.ID}");
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

                var allAssert = ComponentTarget.SelectAll(_TestObj.PageID);
                var ass = allAssert.Where(x => x.Source == _TestObj.Source).FirstOrDefault();

                // Function that we are testing BELOW...
                var componentTarget = ComponentTarget.SelectOne(ass.ID);

                Assert.IsFalse(componentTarget == null || componentTarget.ID == 0);
                // Check if variables of the returned object are correct
                Assert.IsTrue(componentTarget.PageID == _TestObj.PageID, $"ComponentTarget PageID Failed, Expected: {_TestObj.PageID}, Actual: {componentTarget.PageID}");
                Assert.IsTrue(componentTarget.Source == _TestObj.Source, $"ComponentTarget Source Failed, Expected: {_TestObj.Source}, Actual: {componentTarget.Source}");
                Assert.IsTrue(componentTarget.Target == _TestObj.Target, $"ComponentTarget Target Failed, Expected: {_TestObj.Target}, Actual: {componentTarget.Target}");

                if (componentTarget?.Target == _TestObj.Target)
                    Trace.WriteLine($"FOUND ComponentTarget: ({componentTarget.ID})");
                else
                {
                    Assert.Fail("ComponentTarget NOT FOUND...");
                    Assert.IsFalse(true);
                }
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

                var allAssert = await ComponentTarget.SelectAllAsync(_TestObjAsync.PageID);
                var ass = allAssert.Where(x => x.Source == _TestObjAsync.Source).FirstOrDefault();

                // Function that we are testing BELOW...
                var componentTarget = await ComponentTarget.SelectOneAsync(ass.ID);

                Assert.IsFalse(componentTarget == null || componentTarget.ID == 0);
                // Check if variables of the return are correct
                Assert.IsTrue(componentTarget.PageID == _TestObjAsync.PageID, $"ComponentTarget PageID Failed, Expected: {_TestObjAsync.PageID}, Actual: {componentTarget.PageID}");
                Assert.IsTrue(componentTarget.Source == _TestObjAsync.Source, $"ComponentTarget Source Failed, Expected: {_TestObjAsync.Source}, Actual: {componentTarget.Source}");
                Assert.IsTrue(componentTarget.Target == _TestObjAsync.Target, $"ComponentTarget Target Failed, Expected: {_TestObjAsync.Target}, Actual: {componentTarget.Target}");

                if (componentTarget?.Target == _TestObjAsync.Target)
                    Trace.WriteLine($"FOUND ComponentTarget: ({componentTarget.ID})");
                else
                {
                    Assert.Fail("ComponentTarget NOT FOUND...");
                    Assert.IsFalse(true);
                }
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        #endregion

        #region Select ALL

        [TestMethod]
        public void B_SelectAllByComponentGuid()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentTarget = ComponentTarget.SelectAll(_TestObj.Source);

                if (componentTarget?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentTarget: (Count {componentTarget.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)})");
                else
                    Assert.Fail("ComponentTarget NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllByComponentGuidAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentTarget = await ComponentTarget.SelectAllAsync(_TestObjAsync.Source);

                if (componentTarget?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentTarget: (Count {componentTarget.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)})");
                else
                    Assert.Fail("ComponentTarget NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectAllByPageID()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var componentTarget = ComponentTarget.SelectAll(_TestObj.PageID);

                if (componentTarget?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentTarget: (Count {componentTarget.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)})");
                else
                    Assert.Fail("ComponentTarget NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllByPageIDAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var componentTarget = await ComponentTarget.SelectAllAsync(_TestObjAsync.PageID);

                if (componentTarget?.Length > 0)
                    Trace.WriteLine($"FOUND ComponentTarget: (Count {componentTarget.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)})");
                else
                    Assert.Fail("ComponentTarget NOT FOUND...");
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
            var allAssert = ComponentTarget.SelectAll(_TestObj.PageID);
            var ass = allAssert.Where(x => x.Source == _TestObj.Source);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test ComponentTarget wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE ComponentTarget Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (ComponentTarget a in ass)
            {
                if (a?.ID > 0)
                {
                    a.Delete();
                    Trace.WriteLine($"DELETE ComponentTarget: {a?.ID}");

                    // Check if delete in the DB is succesfull, for this record
                    var testDelete = ComponentTarget.SelectOne((int)a?.ID);
                    if (testDelete?.ID > 0) // ERROR
                        errorList.Add($"{a?.ID}");
                }
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test ComponentTarget not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync()
        {
            List<string> errorList = new List<string>();

            // Get all test records, we will delete them ALL
            var allAssert = await ComponentTarget.SelectAllAsync(_TestObjAsync.PageID);
            var ass = allAssert.Where(x => x.Source == _TestObjAsync.Source);
            if (ass.Count() == 0)
            {
                Assert.Fail("Test ComponentTarget wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE ComponentTarget Found: [{ass.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            foreach (ComponentTarget a in ass)
            {
                if (a?.ID > 0)
                {
                    await a.DeleteAsync();
                    Trace.WriteLine($"DELETE ComponentTarget: {a?.ID}");

                    // Check if delete in the DB is succesfull, for this record
                    var testDelete = await ComponentTarget.SelectOneAsync((int)a?.ID);
                    if (testDelete?.ID > 0) // ERROR
                        errorList.Add($"{a?.ID}");
                }
            }

            // Return errors, if they exist...
            if (errorList.Count > 0)
                Assert.Fail($"Test ComponentTarget not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        [TestMethod]
        public async Task E_DeleteCompleteTest()
        {
            _TestObj.Save();
            Assert.IsTrue(_TestObj.ID > 0);
            _TestObjAsync.PageID = _TestObj.PageID;
            // _TestObjAsync.Source is unchanged for this test, so we will delete this record.
            _TestObjAsync.Target = _TestObj.Target;
            _TestObjAsync.Save();
            Assert.IsTrue(_TestObjAsync.ID > 0);

            try
            {
                try
                {
                    // Function that we are testing BELOW...
                    _TestObj.DeleteComplete();

                    // Test _TestObj, it should still be there
                    var test = ComponentTarget.SelectOne(_TestObj.ID);
                    Assert.IsTrue(test?.ID > 0, "Error, _TestObj was deleted, This should not happen.");

                    // Test Delete _TestObjAsync
                    test = ComponentTarget.SelectOne(_TestObjAsync.ID);
                    if (test?.ID > 0)
                    {
                        await D_Delete_TestObjAsync();
                        Assert.Fail("Error, _TestObjAsync was NOT deleted.");
                    }
                }
                catch (Exception e)
                {
                    Assert.Fail($"Error {e.Message}");
                }
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task E_DeleteCompleteTestAsync()
        {
            await _TestObj.SaveAsync();
            Assert.IsTrue(_TestObj.ID > 0);
            _TestObjAsync.PageID = _TestObj.PageID;
            // _TestObjAsync.Source is unchanged for this test, so we will delete this record.
            _TestObjAsync.Target = _TestObj.Target;
            await _TestObjAsync.SaveAsync();
            Assert.IsTrue(_TestObjAsync.ID > 0);

            try
            {
                try
                {
                    // Function that we are testing BELOW...
                    await _TestObjAsync.DeleteCompleteAsync();

                    // Test _TestObjAsync, it should still be there
                    var test = await ComponentTarget.SelectOneAsync(_TestObjAsync.ID);
                    Assert.IsTrue(test?.ID > 0, "Error, _TestObj was deleted, This should not happen.");

                    // Test Delete _TestObj
                    test = await ComponentTarget.SelectOneAsync(_TestObj.ID);
                    if (test?.ID > 0)
                    {
                        D_Delete_TestObj();
                        Assert.Fail("Error, _TestObjAsync was NOT deleted.");
                    }
                }
                catch (Exception e)
                {
                    Assert.Fail($"Error {e.Message}");
                }
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }
        #endregion Delete

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTarget>();
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
            var connector = ConnectorFactory.CreateConnector<ComponentTarget>();
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
