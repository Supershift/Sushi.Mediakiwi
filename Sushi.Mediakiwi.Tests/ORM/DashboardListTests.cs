using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.MicroORM;

namespace Sushi.Mediakiwi.Tests.ORM
{
    [TestClass]
    public class DashboardListTests : BaseTest
    {
        #region Select ALL


        [TestMethod]
        public void X_SelectAll()
        {
            // Function that we are testing BELOW...
            var dashboardItems = DashboardListItem.SelectAll();

            if (dashboardItems?.Length > 0)
                Trace.WriteLine($"FOUND DashboardListItem[Dashboard_Key, List_Key]: {dashboardItems.Select(x => "[" + x.DashboardID.ToString() + ", " + x.ListID.ToString() + "]").Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("DashboardListItem NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllAsync()
        {
            // Function that we are testing BELOW...
            var dashboardItems = await DashboardListItem.SelectAllAsync();

            if (dashboardItems?.Length > 0)
                Trace.WriteLine($"FOUND DashboardListItem[Dashboard_Key, List_Key]: {dashboardItems.Select(x => "[" + x.DashboardID.ToString() + ", " + x.ListID.ToString() + "]").Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("DashboardListItem NOT FOUND...");
        }

        [TestMethod]
        public void X_SelectAllForDashboard()
        {
            int dashBoardId = 1;

            // Function that we are testing BELOW...
            var dashboardItems = DashboardListItem.SelectAll(dashBoardId);

            if (dashboardItems?.Length > 0)
                Trace.WriteLine($"FOUND DashboardListItem[Dashboard_Key, List_Key]: {dashboardItems.Select(x => "[" + x.DashboardID.ToString() + ", " + x.ListID.ToString() + "]").Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("DashboardListItem NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllForDashboardAsync()
        {
            int dashBoardId = 1;

            // Function that we are testing BELOW...
            var dashboardItems = await DashboardListItem.SelectAllAsync(dashBoardId);

            if (dashboardItems?.Length > 0)
                Trace.WriteLine($"FOUND DashboardListItem[Dashboard_Key, List_Key]: {dashboardItems.Select(x => "[" + x.DashboardID.ToString() + ", " + x.ListID.ToString() + "]").Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("DashboardListItem NOT FOUND...");
        }


        #endregion Select ALL
    }
}
