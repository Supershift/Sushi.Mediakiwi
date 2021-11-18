using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemanticComparison;
using Sushi.Mediakiwi.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Test.ORM
{
    [TestClass]
    public class RoleRightTests : BaseTest
    {
        int[] _TestList = { 12345, 12346 };

        private RoleRight _TestObj = new RoleRight()
        {
            RoleID = 69420,
            ItemID = 12345,
            TypeID = 1,
            AccessType = Access.Granted,
            SortOrder = 1,
        };
        private SubList _TestSubList = new SubList(12345, "Test List voor UNIT TEST");

        private RoleRight _TestObjAsync = new RoleRight()
        {
            RoleID = 69421,
            ItemID = 12346,
            TypeID = 1,
            AccessType = Access.Granted,
            SortOrder = 1,
        };
        private SubList _AsyncTestSubList = new SubList(12346, "Test List voor UNIT ASYNC TEST");

        #region CreateAndDelete

        [TestMethod]
        public void A_CreateAndDelete()
        {
            // Function that we are testing BELOW...
            RoleRight.Update(_TestSubList, RoleRightType.List, _TestObj.RoleID);

            var allAssert = RoleRight.SelectAll(_TestObj.RoleID, RoleRightType.List);
            // MJ: check if saved db record is ok            
            var expected = new Likeness<RoleRight, RoleRight>(_TestObj);
            Assert.AreEqual(expected, allAssert[0]);

            // Delete
            RoleRight.Update(new SubList(), RoleRightType.List, _TestObj.RoleID);

            // Check if delete is succesfull
            allAssert = RoleRight.SelectAll(_TestObj.RoleID, RoleRightType.List);
            Assert.IsTrue(allAssert.Length == 0);

        }

        [TestMethod]
        public async Task A_CreateAndDeleteAsync()
        {
            // Function that we are testing BELOW...
            await RoleRight.UpdateAsync(_AsyncTestSubList, RoleRightType.List, _TestObjAsync.RoleID);

            var allAssert = await RoleRight.SelectAllAsync(_TestObjAsync.RoleID, RoleRightType.List);
            // MJ: check if saved db record is ok            
            var expected = new Likeness<RoleRight, RoleRight>(_TestObjAsync);
            Assert.AreEqual(expected, allAssert[0]);

            // Delete
            await RoleRight.UpdateAsync(new SubList(), RoleRightType.List, _TestObjAsync.RoleID);

            // Check if delete is succesfull
            allAssert = await RoleRight.SelectAllAsync(_TestObjAsync.RoleID, RoleRightType.List);
            Assert.IsTrue(allAssert.Length == 0);
        }

        /*
        [TestMethod]
        public void A_FolderCreateAndDelete()
        {
            //  NOTE: Function for this test is possibly become obsolete.

            _TestObj.TypeID = 6; // Folder type
            // Create
            RoleRight.UpdateFolder(_TestSubList, _TestObj.RoleID);

            var allAssert = RoleRight.SelectAll(_TestObj.RoleID);
            // MJ: check if saved db record is ok            
            var expected = new Likeness<RoleRight, RoleRight>(_TestObj);
            Assert.AreEqual(expected, allAssert[0]);

            // Cleanup before check
            RoleRight.UpdateFolder(new SubList(), _TestObj.RoleID);

            // Check if delete is succesfull
            allAssert = RoleRight.SelectAll(_TestObj.RoleID);
            Assert.IsTrue(allAssert.Length == 0);

        }

        [TestMethod]
        public async Task A_FolderCreateAndDeleteAsync()
        {
            //  NOTE: Function for this test is possibly become obsolete.

            _TestObjAsync.TypeID = 6; // Folder type
            // Create
            await RoleRight.UpdateFolderAsync(_AsyncTestSubList, _TestObjAsync.RoleID);

            var allAssert = await RoleRight.SelectAllAsync(_TestObjAsync.RoleID);
            // MJ: check if saved db record is ok            
            var expected = new Likeness<RoleRight, RoleRight>(_TestObjAsync);
            Assert.AreEqual(expected, allAssert[0]);

            // Cleanup before check
            await RoleRight.UpdateFolderAsync(new SubList(), _TestObjAsync.RoleID);

            // Check if delete is succesfull
            allAssert = await RoleRight.SelectAllAsync(_TestObjAsync.RoleID);
            Assert.IsTrue(allAssert.Length == 0);
        }
        */
        [TestMethod]
        public void A_ListCreateAndDelete()
        {
            _TestObjAsync.RoleID = 69420;
            _TestObjAsync.SortOrder = 2;

            // Function that we are testing BELOW...
            RoleRight.Update(_TestList, RoleRightType.List, _TestObj.RoleID);

            var allAssert = RoleRight.SelectAll(_TestObj.RoleID);

            // Check if both record are there
            Assert.IsTrue(allAssert.Length == 2);
            // MJ: check if saved db records are ok            
            var expected = new Likeness<RoleRight, RoleRight>(_TestObj);            
            Assert.AreEqual(expected, allAssert[0]);
            expected = new Likeness<RoleRight, RoleRight>(_TestObjAsync);
            Assert.AreEqual(expected, allAssert[1]);

            // Delete
            RoleRight.Update(new SubList(), RoleRightType.List, _TestObj.RoleID);

            // Check if delete is succesfull
            allAssert = RoleRight.SelectAll(_TestObj.RoleID);
            Assert.IsTrue(allAssert.Length == 0);

        }

        [TestMethod]
        public async Task A_ListCreateAndDeleteAsync()
        {
            _TestObjAsync.RoleID = 69420;
            _TestObjAsync.SortOrder = 2;

            // Function that we are testing BELOW...
            await RoleRight.UpdateAsync(_TestList, RoleRightType.List, _TestObjAsync.RoleID);

            var allAssert = await RoleRight.SelectAllAsync(_TestObjAsync.RoleID);

            // Check if both record are there
            Assert.IsTrue(allAssert.Length == 2);
            // check if saved db records are ok            
            var expected = new Likeness<RoleRight, RoleRight>(_TestObj);
            Assert.AreEqual(expected, allAssert[0]);
            expected = new Likeness<RoleRight, RoleRight>(_TestObjAsync);
            Assert.AreEqual(expected, allAssert[1]);

            // Delete
            await RoleRight.UpdateAsync(new SubList(), RoleRightType.List, _TestObjAsync.RoleID);

            // Check if delete is succesfull
            allAssert = await RoleRight.SelectAllAsync(_TestObjAsync.RoleID);
            Assert.IsTrue(allAssert.Length == 0);
        }

        #endregion CreateAndDelete

        #region Select All

        [TestMethod]
        public void X_SelectAll()
        {
            // Function that we are testing BELOW...
            var roleRights = RoleRight.SelectAll();

            if (roleRights?.Length > 0)
                Trace.WriteLine($"FOUND RoleRight[Role_Key, Child_Key]: {roleRights.Select(x => "[" + x.RoleID.ToString() +", "+ x.ItemID.ToString() + "]").Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("RoleRight NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllAsync()
        {
            // Function that we are testing BELOW...
            var roleRights = await RoleRight.SelectAllAsync();

            if (roleRights?.Length > 0)
                Trace.WriteLine($"FOUND RoleRight[Role_Key, Child_Key]: {roleRights.Select(x => "[" + x.RoleID.ToString() + ", " + x.ItemID.ToString() + "]").Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("RoleRight NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectAllForRoleID()
        {
            int roleId = 2;

            // Function that we are testing BELOW...
            var roleRights = RoleRight.SelectAll(roleId);
            
            if (roleRights?.Length > 0)
                Trace.WriteLine($"FOUND RoleRight[Role_Key, Child_Key]: {roleRights.Select(x => "[" + x.RoleID.ToString() + ", " + x.ItemID.ToString() + "]").Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("RoleRight NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllForRoleIDAsync()
        {
            int roleId = 2;

            // Function that we are testing BELOW...
            var roleRights = await RoleRight.SelectAllAsync(roleId);

            if (roleRights?.Length > 0)
                Trace.WriteLine($"FOUND RoleRight[Role_Key, Child_Key]: {roleRights.Select(x => "[" + x.RoleID.ToString() + ", " + x.ItemID.ToString() + "]").Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("RoleRight NOT FOUND...");
        }


        [TestMethod]
        public void X_SelectAllForRoleIDAndType()
        {
            int roleId = 2;

            // Function that we are testing BELOW...
            var roleRights = RoleRight.SelectAll(roleId, RoleRightType.List);

            if (roleRights?.Length > 0)
                Trace.WriteLine($"FOUND RoleRight[Role_Key, Child_Key]: {roleRights.Select(x => "[" + x.RoleID.ToString() + ", " + x.ItemID.ToString() + "]").Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("RoleRight NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllForRoleIDAndTypeAsync()
        {
            int roleId = 2;

            // Function that we are testing BELOW...
            var roleRights = await RoleRight.SelectAllAsync(roleId, RoleRightType.List);

            if (roleRights?.Length > 0)
                Trace.WriteLine($"FOUND RoleRight[Role_Key, Child_Key]: {roleRights.Select(x => "[" + x.RoleID.ToString() + ", " + x.ItemID.ToString() + "]").Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("RoleRight NOT FOUND...");
        }

        #endregion Select All
    }
}
