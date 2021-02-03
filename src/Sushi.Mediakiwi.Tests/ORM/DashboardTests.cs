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
    public class DashboardTests : BaseTest
    {

        #region Select ONE

        [TestMethod]
        public void X_SelectOneByID()
        {
            int Id = 1;

            // Function that we are testing BELOW...
            var dashboard = Dashboard.SelectOne(Id);

            if (dashboard?.ID > 0)
                Trace.WriteLine($"FOUND Dashboard: {dashboard.ID}");
            else
                Assert.Fail("Dashboard NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectOneByIDAsync()
        {
            int Id = 1;

            // Function that we are testing BELOW...
            var dashboard = await Dashboard.SelectOneAsync(Id);

            if (dashboard?.ID > 0)
                Trace.WriteLine($"FOUND Dashboard: {dashboard.ID}");
            else
                Assert.Fail("Dashboard NOT FOUND...");
        }

        #endregion Select ONE

        #region Select ALL


        [TestMethod]
        public void X_SelectAll()
        {
            // Function that we are testing BELOW...
            var dashboards = Dashboard.SelectAll();

            if (dashboards?.Length > 0)
                Trace.WriteLine($"FOUND Dashboard: {dashboards.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Dashboard NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllAsync()
        {
            // Function that we are testing BELOW...
            var dashboards = await Dashboard.SelectAllAsync();

            if (dashboards?.Length > 0)
                Trace.WriteLine($"FOUND Dashboard: {dashboards.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("Dashboard NOT FOUND...");
        }

        #endregion Select ALL
    }
}
