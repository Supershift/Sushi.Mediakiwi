using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.MicroORM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Sushi.Mediakiwi.Tests.ORM
{
    [TestClass]
    public class NotificationTests : BaseTest
    {
        #region Test Data
        private string _table = "wim_Notifications";
        private string _key = "Notification_Key";

        private static DateTime _date = DateTime.Now;
        // Test object
        private static string _xmlData = @"<Content xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><Fields></Fields></Content>";

        private Notification _TestObj = new Notification()
        {
            Group = "RewriteAssetPath",
            Text = "Notifcation for xUNIT TESTx",
            Selection = (int)NotificationType.Information,
            UserID = 0, // Set in test
            VisitorID = 1,
            PageID = 69420,
            Created = _date,
            XML = null // Set in test

        };
        // Async test object
        private Notification _TestObjAsync = new Notification()
        {
            Group = "RewriteAssetPathAsync",
            Text = "Notifcation for xASYNC UNIT TESTx",
            Selection = (int)NotificationType.Information,
            UserID = 0, // Set in test
            VisitorID = 1,
            PageID = 69421,
            Created = _date,
            XML = null // Set in test
        };
        #endregion Test Data

        #region  Create
        public void A_Create_TestObj()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(_xmlData));
            _TestObj.XML = doc;

            _TestObj.Save();

            Assert.IsTrue(_TestObj?.ID > 0);

            Notification db = (Notification)Notification.SelectOne(_TestObj.ID);
            Assert.AreEqual(_TestObj.ID, db.ID);

            if (_TestObj.ID > 0)
            {
                Trace.WriteLine($"CREATED Notification: {_TestObj.ID}");
            }
        }

        public async Task A_Create_TestObjAsync()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(_xmlData));
            _TestObjAsync.XML = doc;

            await _TestObjAsync.SaveAsync();

            Assert.IsTrue(_TestObjAsync?.ID > 0);

            var db = (Notification)await Notification.SelectOneAsync(_TestObjAsync.ID);
            Assert.AreEqual(_TestObjAsync.ID, db.ID);

            if (_TestObjAsync.ID > 0)
            {
                Trace.WriteLine($"CREATED Notification: {_TestObjAsync.ID}");
            }
        }
        #endregion Create

        #region INSERT
        [TestMethod]
        public void A_InsertOne()
        {
            try
            {
                // Function that we are testing BELOW...
                Notification.InsertOne(_TestObj.Group, _TestObj.Text);

                var notifications = Notification.SelectAll(_TestObj.Group, 1);
                Assert.IsTrue(notifications.Length > 0);
                var notification = notifications[0];
                Assert.IsTrue(notification?.ID > 0, $"CREATED: {notification.ID}");

                Assert.IsTrue(notification.Group == _TestObj.Group, $"Group Failed, Expected: {_TestObj.Group}, Actual: {notification.Group}");
                Assert.IsTrue(notification.Text == _TestObj.Text, $"Text Failed, Expected: {_TestObj.Text}, Actual: {notification.Text}");
                Assert.IsTrue(notification.Selection == 1, $"Selection Failed, Expected: 1, Actual: {notification.Selection}");
                Assert.IsTrue(notification.PageID == null, $"PageID Failed, Expected: <NULL>, Actual: {notification.PageID}");
                Assert.IsTrue(notification.VisitorID == null, $"VisitorID Failed, Expected: <NULL>, Actual: {notification.VisitorID}");
                Assert.IsTrue(notification.UserID == null, $"UserID Failed, Expected: <NULL>, Actual: {notification.UserID}");
            }
            finally
            {
                D_Delete_TestObj(1);

                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task A_InsertOneAsync()
        {
            try
            {
                // Function that we are testing BELOW...
                await Notification.InsertOneAsync(_TestObjAsync.Group, _TestObjAsync.Text);

                var notifications = Notification.SelectAll(_TestObjAsync.Group, 1);
                Assert.IsTrue(notifications.Length > 0);
                var notification = notifications[0];
                Assert.IsTrue(notification?.ID > 0, $"CREATED: {notification.ID}");

                Assert.IsTrue(notification.Group == _TestObjAsync.Group, $"Group Failed, Expected: {_TestObjAsync.Group}, Actual: {notification.Group}");
                Assert.IsTrue(notification.Text == _TestObjAsync.Text, $"Text Failed, Expected: {_TestObjAsync.Text}, Actual: {notification.Text}");
                Assert.IsTrue(notification.Selection == 1, $"Selection Failed, Expected: 1, Actual: {notification.Selection}");
                Assert.IsTrue(notification.PageID == null, $"PageID Failed, Expected: <NULL>, Actual: {notification.PageID}");
                Assert.IsTrue(notification.VisitorID == null, $"VisitorID Failed, Expected: <NULL>, Actual: {notification.VisitorID}");
                Assert.IsTrue(notification.UserID == null, $"UserID Failed, Expected: <NULL>, Actual: {notification.UserID}");
            }
            finally
            {
                await D_Delete_TestObjAsync(1);

                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void A_InsertOneWithType()
        {
            try
            {
                // Function that we are testing BELOW...
                Notification.InsertOne(_TestObj.Group, (NotificationType)_TestObj.Selection, _TestObj.Text);

                var notifications = Notification.SelectAll(_TestObj.Group, (int)_TestObj.Selection);
                Assert.IsTrue(notifications.Length > 0);
                var notification = notifications[0];
                Assert.IsTrue(notification?.ID > 0, $"CREATED: {notification.ID}");

                Assert.IsTrue(notification.Group == _TestObj.Group, $"Group Failed, Expected: {_TestObj.Group}, Actual: {notification.Group}");
                Assert.IsTrue(notification.Text == _TestObj.Text, $"Text Failed, Expected: {_TestObj.Text}, Actual: {notification.Text}");
                Assert.IsTrue(notification.Selection == _TestObj.Selection, $"Selection Failed, Expected: {_TestObj.Selection}, Actual: {notification.Selection}");
                Assert.IsTrue(notification.PageID == null, $"PageID Failed, Expected: <NULL>, Actual: {notification.PageID}");
                Assert.IsTrue(notification.VisitorID == null, $"VisitorID Failed, Expected: <NULL>, Actual: {notification.VisitorID}");
                Assert.IsTrue(notification.UserID == null, $"UserID Failed, Expected: <NULL>, Actual: {notification.UserID}");

            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task A_InsertOneWithTypeAsync()
        {
            try
            {
                // Function that we are testing BELOW...
                await Notification.InsertOneAsync(_TestObjAsync.Group, (NotificationType)_TestObj.Selection, _TestObjAsync.Text);

                var notifications = Notification.SelectAll(_TestObjAsync.Group, (int)_TestObj.Selection);
                Assert.IsTrue(notifications.Length > 0);
                var notification = notifications[0];
                Assert.IsTrue(notification?.ID > 0, $"CREATED: {notification.ID}");

                Assert.IsTrue(notification.Group == _TestObjAsync.Group, $"Group Failed, Expected: {_TestObjAsync.Group}, Actual: {notification.Group}");
                Assert.IsTrue(notification.Text == _TestObjAsync.Text, $"Text Failed, Expected: {_TestObjAsync.Text}, Actual: {notification.Text}");
                Assert.IsTrue(notification.Selection == _TestObjAsync.Selection, $"Selection Failed, Expected: {_TestObjAsync.Selection}, Actual: {notification.Selection}");
                Assert.IsTrue(notification.PageID == null, $"PageID Failed, Expected: <NULL>, Actual: {notification.PageID}");
                Assert.IsTrue(notification.VisitorID == null, $"VisitorID Failed, Expected: <NULL>, Actual: {notification.VisitorID}");
                Assert.IsTrue(notification.UserID == null, $"UserID Failed, Expected: <NULL>, Actual: {notification.UserID}");

            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void A_InsertOneWithUser()
        {
            // ApplicationUser is needed for this test
            ApplicationUserTests applicationUserTest = new ApplicationUserTests();
            IApplicationUser user = applicationUserTest.A_CreateTestUser();

            try
            {
                // Function that we are testing BELOW...
                Notification.InsertOne(_TestObj.Group, (NotificationType)_TestObj.Selection, user, _TestObj.Text);

                var notifications = Notification.SelectAll(_TestObj.Group, (int)_TestObj.Selection);
                Assert.IsTrue(notifications.Length > 0);
                var notification = notifications[0];
                Assert.IsTrue(notification?.ID > 0, $"CREATED: {notification.ID}");

                Assert.IsTrue(notification.Group == _TestObj.Group, $"Group Failed, Expected: {_TestObj.Group}, Actual: {notification.Group}");
                Assert.IsTrue(notification.Text == _TestObj.Text, $"Text Failed, Expected: {_TestObj.Text}, Actual: {notification.Text}");
                Assert.IsTrue(notification.Selection == _TestObj.Selection, $"Selection Failed, Expected: {_TestObj.Selection}, Actual: {notification.Selection}");
                Assert.IsTrue(notification.PageID == null, $"PageID Failed, Expected: <NULL>, Actual: {notification.PageID}");
                Assert.IsTrue(notification.VisitorID == null, $"VisitorID Failed, Expected: <NULL>, Actual: {notification.VisitorID}");

                Assert.IsTrue(notification.UserID > 0);
            }
            finally
            {
                D_Delete_TestObj();
                applicationUserTest.D_Delete_TestObj();

                F_Reset_AutoIndent();
                applicationUserTest.F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task A_InsertOneWithUserAsync()
        {
            // ApplicationUser is needed for this test
            ApplicationUserTests applicationUserTest = new ApplicationUserTests();
            IApplicationUser user = await applicationUserTest.A_CreateTestUserAsync();

            try
            {
                // Function that we are testing BELOW...
                await Notification.InsertOneAsync(_TestObjAsync.Group, (NotificationType)_TestObj.Selection, user, _TestObjAsync.Text);

                var notifications = Notification.SelectAll(_TestObjAsync.Group, (int)_TestObj.Selection);
                Assert.IsTrue(notifications.Length > 0);
                var notification = notifications[0];
                Assert.IsTrue(notification?.ID > 0, $"CREATED: {notification.ID}");

                Assert.IsTrue(notification.Group == _TestObjAsync.Group, $"Group Failed, Expected: {_TestObjAsync.Group}, Actual: {notification.Group}");
                Assert.IsTrue(notification.Text == _TestObjAsync.Text, $"Text Failed, Expected: {_TestObjAsync.Text}, Actual: {notification.Text}");
                Assert.IsTrue(notification.Selection == _TestObjAsync.Selection, $"Selection Failed, Expected: {_TestObj.Selection}, Actual: {notification.Selection}");
                Assert.IsTrue(notification.PageID == null, $"PageID Failed, Expected: <NULL>, Actual: {notification.PageID}");
                Assert.IsTrue(notification.VisitorID == null, $"VisitorID Failed, Expected: <NULL>, Actual: {notification.VisitorID}");

                Assert.IsTrue(notification.UserID > 0);
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await applicationUserTest.D_Delete_TestObjAsync();

                await F_Reset_AutoIndentAsync();
                await applicationUserTest.F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void A_InsertOne_Full()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(_xmlData));
            _TestObj.XML = doc;

            // ApplicationUser is needed for this test
            ApplicationUserTests applicationUserTest = new ApplicationUserTests();
            IApplicationUser user = applicationUserTest.A_CreateTestUser();

            try
            {
                // Function that we are testing BELOW...
                Notification.InsertOne(_TestObj.Group, (NotificationType)_TestObj.Selection, user, _TestObj.Text, _TestObj.PageID, _TestObj.VisitorID, _TestObj.XML);

                var notifications = Notification.SelectAll(_TestObj.Group, (int)_TestObj.Selection);
                Assert.IsTrue(notifications.Length > 0);
                var notification = notifications[0];
                Assert.IsTrue(notification?.ID > 0, $"CREATED: {notification.ID}");

                Assert.IsTrue(notification.Group == _TestObj.Group, $"Group Failed, Expected: {_TestObj.Group}, Actual: {notification.Group}");
                Assert.IsTrue(notification.Text == _TestObj.Text, $"Text Failed, Expected: {_TestObj.Text}, Actual: {notification.Text}");
                Assert.IsTrue(notification.Selection == _TestObj.Selection, $"Selection Failed, Expected: {_TestObj.Selection}, Actual: {notification.Selection}");
                Assert.IsTrue(notification.PageID == _TestObj.PageID, $"PageID Failed, Expected: {_TestObj.PageID}, Actual: {notification.PageID}");
                Assert.IsTrue(notification.VisitorID == _TestObj.VisitorID, $"VisitorID Failed, Expected: {_TestObj.VisitorID}, Actual: {notification.VisitorID}");

                Assert.IsTrue(notification.UserID > 0);
            }
            finally
            {
                try
                {
                    D_Delete_TestObj();
                }
                finally
                {
                    applicationUserTest.D_Delete_TestObj();
                }

                F_Reset_AutoIndent();
                applicationUserTest.F_Reset_AutoIndent();
                }
        }

        [TestMethod]
        public async Task A_InsertOne_FullAsync()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(_xmlData));
            _TestObjAsync.XML = doc;

            // ApplicationUser is needed for this test
            ApplicationUserTests applicationUserTest = new ApplicationUserTests();
            IApplicationUser user = await applicationUserTest.A_CreateTestUserAsync();

            try
            {
                // Function that we are testing BELOW...
                await Notification.InsertOneAsync(_TestObjAsync.Group, (NotificationType)_TestObj.Selection, user, _TestObjAsync.Text, _TestObjAsync.PageID, _TestObjAsync.VisitorID, _TestObjAsync.XML);

                var notifications = Notification.SelectAll(_TestObjAsync.Group, (int)_TestObj.Selection);
                Assert.IsTrue(notifications.Length > 0);
                var notification = notifications[0];
                Assert.IsTrue(notification?.ID > 0, $"CREATED: {notification.ID}");

                Assert.IsTrue(notification.Group == _TestObjAsync.Group, $"Group Failed, Expected: {_TestObjAsync.Group}, Actual: {notification.Group}");
                Assert.IsTrue(notification.Text == _TestObjAsync.Text, $"Text Failed, Expected: {_TestObjAsync.Text}, Actual: {notification.Text}");
                Assert.IsTrue(notification.Selection == _TestObjAsync.Selection, $"Selection Failed, Expected: {_TestObj.Selection}, Actual: {notification.Selection}");
                Assert.IsTrue(notification.PageID == _TestObjAsync.PageID, $"PageID Failed, Expected: {_TestObjAsync.PageID}, Actual: {notification.PageID}");
                Assert.IsTrue(notification.VisitorID == _TestObjAsync.VisitorID, $"VisitorID Failed, Expected: {_TestObjAsync.VisitorID}, Actual: {notification.VisitorID}");

                Assert.IsTrue(notification.UserID > 0);
            }
            finally
            {
                try
                {
                    await D_Delete_TestObjAsync();
                }
                finally
                {
                    await applicationUserTest.D_Delete_TestObjAsync();
                }

                await F_Reset_AutoIndentAsync();
                await applicationUserTest.F_Reset_AutoIndentAsync();
            }
        }
        #endregion INSERT

        #region Select ONE

        [TestMethod]
        public void B_SelectOneByID()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var notification = Notification.SelectOne(_TestObj.ID);

                if (notification?.ID > 0)
                    Trace.WriteLine($"FOUND Notification: {notification.ID}");
                else
                    Assert.Fail("Notification NOT FOUND...");
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
                var notification = await Notification.SelectOneAsync(_TestObjAsync.ID);

                if (notification?.ID > 0)
                    Trace.WriteLine($"FOUND Notification: {notification.ID}");
                else
                    Assert.Fail("Notification NOT FOUND...");
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
        public void B_SelectAllByGroupAndSelection()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var notifications = Notification.SelectAll(_TestObj.Group, (int)_TestObj.Selection);

                if (notifications?.Length > 0)
                    Trace.WriteLine($"FOUND Notification: {notifications.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Notification NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllByGroupAndSelectionAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var notifications = await Notification.SelectAllAsync(_TestObjAsync.Group, (int)_TestObjAsync.Selection);

                if (notifications?.Length > 0)
                    Trace.WriteLine($"FOUND Notification: {notifications.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Notification NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }


        [TestMethod]
        public void B_SelectAllByGroupAndSelectionMaxResult()
        {
            int maxResult = 20;
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var notifications = Notification.SelectAll(_TestObj.Group, (int)_TestObj.Selection, maxResult, out int pageCount);                

                if (notifications?.Length > 0)
                    Trace.WriteLine($"FOUND Notification: {notifications.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Notification NOT FOUND...");

                Assert.AreEqual(notifications.Count(), pageCount);
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllByGroupAndSelectionMaxResultAsync()
        {
            int maxResult = 20;
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var notifications = await Notification.SelectAllAsync(_TestObjAsync.Group, (int)_TestObjAsync.Selection, maxResult);

                if (notifications?.Length > 0)
                    Trace.WriteLine($"FOUND Notification: {notifications.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Notification NOT FOUND...");

                Assert.AreEqual(notifications.Count(), maxResult);
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        [TestMethod]
        public void B_SelectAllGroups()
        {
            try
            {
                A_Create_TestObj();

                // Function that we are testing BELOW...
                var notifications = Notification.SelectAll_Groups();

                if (notifications?.Length > 0)
                    Trace.WriteLine($"FOUND Notification: {notifications.Select(x => x).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Notification NOT FOUND...");
            }
            finally
            {
                D_Delete_TestObj();
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task B_SelectAllGroupsAsync()
        {
            try
            {
                await A_Create_TestObjAsync();

                // Function that we are testing BELOW...
                var notifications = await Notification.SelectAll_GroupsAsync();

                if (notifications?.Length > 0)
                    Trace.WriteLine($"FOUND Notification: {notifications.Select(x => x).Aggregate((a, b) => a + ", " + b)}");
                else
                    Assert.Fail("Notification NOT FOUND...");
            }
            finally
            {
                await D_Delete_TestObjAsync();
                await F_Reset_AutoIndentAsync();
            }
        }

        #endregion Select All

        #region DELETE
        [TestMethod]
        public void D_DeleteAll()
        {
            var preTest = Notification.SelectAll_Groups();
            if (preTest.Length == 0)
            {
                A_Create_TestObj();

                try
                {
                    var notifications = Notification.SelectAll(_TestObj.Group, (int)_TestObj.Selection);
                    Assert.IsTrue(notifications.Length == 1);

                    // Function that we are testing BELOW...
                    Notification.DeleteAll();
                    notifications = Notification.SelectAll(_TestObj.Group, (int)_TestObj.Selection);
                    Assert.IsTrue(notifications.Length == 0);
                }
                finally
                {
                    F_Reset_AutoIndent();
                }
            }
            else
            {
                Assert.Fail("Cannot perform test. There is data in the [wim_Notifications] table.");
            }
        }

        [TestMethod]
        public async Task D_DeleteAllAsync()
        {
            var preTest = await Notification.SelectAll_GroupsAsync();
            if (preTest.Length == 0)
            {
                await A_Create_TestObjAsync();

                try
                {
                    var notifications = await Notification.SelectAllAsync(_TestObjAsync.Group, (int)_TestObjAsync.Selection);
                    Assert.IsTrue(notifications.Length == 1);

                    // Function that we are testing BELOW...
                    await Notification.DeleteAllAsync();
                    notifications = await Notification.SelectAllAsync(_TestObjAsync.Group, (int)_TestObjAsync.Selection);
                    Assert.IsTrue(notifications.Length == 0);
                }
                finally
                {
                    await F_Reset_AutoIndentAsync();
                }
            }
            else
            {
                Assert.Fail("Cannot perform test. There is data in the [wim_Notifications] table.");
            }
        }


        [TestMethod]
        public void D_DeleteAllByGroup()
        {

            try
            {
                A_Create_TestObj();

                var notifications = Notification.SelectAll(_TestObj.Group, (int)_TestObj.Selection);
                Assert.IsTrue(notifications.Length == 1);

                // Function that we are testing BELOW...
                Notification.DeleteAll(_TestObj.Group);
                notifications = Notification.SelectAll(_TestObj.Group, (int)_TestObj.Selection);
                Assert.IsTrue(notifications.Length == 0);
            }
            finally
            {
                F_Reset_AutoIndent();
            }
        }

        [TestMethod]
        public async Task D_DeleteAllByGroupAsync()
        {

            try
            {
                await A_Create_TestObjAsync();

                var notifications = await Notification.SelectAllAsync(_TestObjAsync.Group, (int)_TestObjAsync.Selection);
                Assert.IsTrue(notifications.Length == 1);

                // Function that we are testing BELOW...
                await Notification.DeleteAllAsync(_TestObjAsync.Group);
                notifications = await Notification.SelectAllAsync(_TestObjAsync.Group, (int)_TestObjAsync.Selection);
                Assert.IsTrue(notifications.Length == 0);
            }
            finally
            {
                await F_Reset_AutoIndentAsync();
            }
        }

        public void D_Delete_TestObj(int selection = 0)
        {
            List<string> errorList = new List<string>();

            if (selection > 0)
                _TestObj.Selection = selection;

            var notifications = Notification.SelectAll(_TestObj.Group, (int)_TestObj.Selection);
            if (notifications.Length == 0)
            {
                Assert.Fail("Test Notification wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Notification Found: [{notifications.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            Notification.DeleteAll(_TestObj.Group);

            foreach (Notification a in notifications)
            {
                if (a?.ID > 0)
                {
                    var connector = ConnectorFactory.CreateConnector<Notification>();
                    var testDelete = Notification.SelectOne((int)a?.ID);
                    if (testDelete?.ID > 0) // ERROR
                        errorList.Add($"{a?.ID}");
                }
            }
            if (errorList.Count > 0)
                Assert.Fail($"Test Notification not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }

        public async Task D_Delete_TestObjAsync(int selection = 0)
        {
            List<string> errorList = new List<string>(); 
            
            if (selection > 0)
                _TestObjAsync.Selection = selection;

            var notifications = await Notification.SelectAllAsync(_TestObjAsync.Group, (int)_TestObjAsync.Selection);
            if (notifications.Length == 0)
            {
                Assert.Fail("Test Notification wasn't found, you created it yet?");
            }
            Trace.WriteLine($"DELETE Notification Found: [{notifications.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}]");

            await Notification.DeleteAllAsync(_TestObjAsync.Group);

            foreach (Notification a in notifications)
            {
                if (a?.ID > 0)
                {
                    var connector = ConnectorFactory.CreateConnector<Notification>();
                    var testDelete = await Notification.SelectOneAsync((int)a?.ID);
                    if (testDelete?.ID > 0) // ERROR
                        errorList.Add($"{a?.ID}");
                }
            }
            if (errorList.Count > 0)
                Assert.Fail($"Test Notification not deleted, found [{errorList.Aggregate((a, b) => a + ", " + b)}]");
        }
        #endregion DELETE      

        #region ResetAutoIndent
        public void F_Reset_AutoIndent()
        {
            var connector = ConnectorFactory.CreateConnector<Notification>();
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
            var connector = ConnectorFactory.CreateConnector<Notification>();
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
