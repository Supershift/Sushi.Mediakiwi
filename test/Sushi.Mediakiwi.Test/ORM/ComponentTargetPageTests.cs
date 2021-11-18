using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sushi.Mediakiwi.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Test.ORM
{
    [TestClass]
    public class ComponentTargetPageTests : BaseTest
    {
        #region Select ALL

        [TestMethod]
        public void X_SelectAllByTemplateAndSite()
        {
            int templateId = 6;
            int siteId = 2;

            // Function that we are testing BELOW...
            var componentTargetPages = ComponentTargetPage.SelectAll(templateId, siteId);

            if (componentTargetPages?.Length > 0)
                Trace.WriteLine($"FOUND ComponentTargetPage: {componentTargetPages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ComponentTargetPage NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAllByTemplateAndSiteAsync()
        {
            int templateId = 6;
            int siteId = 2;

            // Function that we are testing BELOW...
            var componentTargetPages = await ComponentTargetPage.SelectAllAsync(templateId, siteId);

            if (componentTargetPages?.Length > 0)
                Trace.WriteLine($"FOUND ComponentTargetPage: {componentTargetPages.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("ComponentTargetPage NOT FOUND...");
        }

        #endregion Select ALL
    }
}
