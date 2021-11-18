using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sushi.Mediakiwi.Data;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Test.ORM
{
    [TestClass]
    public class EnvironmentVersionTests : BaseTest
    {

        #region Select ONE

        [TestMethod]
        public void X_Select()
        {
            // Function that we are testing BELOW...
            var environmentVersion = EnvironmentVersion.Select();

            if (environmentVersion?.ID > 0)
                Trace.WriteLine($"FOUND EnvironmentVersion: {environmentVersion.ID}");
            else
                Assert.Fail("EnvironmentVersion NOT FOUND...");
        }



        [TestMethod]
        public async Task X_SelectAsync()
        {
            // Function that we are testing BELOW...
            var environmentVersion = await EnvironmentVersion.SelectAsync();

            if (environmentVersion?.ID > 0)
                Trace.WriteLine($"FOUND EnvironmentVersion: {environmentVersion.ID}");
            else
                Assert.Fail("EnvironmentVersion NOT FOUND...");
        }

        #endregion Select ONE

    }
}
