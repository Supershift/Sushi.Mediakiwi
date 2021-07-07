using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sushi.Mediakiwi.Data;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Tests.ORM
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

        #region Save

        [TestMethod]
        public void X_Save()
        {
            var environmentVersion = EnvironmentVersion.Select();
            Assert.IsFalse(environmentVersion == null || environmentVersion.ID == 0);

            if (environmentVersion?.ID > 0)
            {
                Trace.WriteLine($"FOUND EnvironmentVersion: {environmentVersion.ID}");

                decimal oldVersion = environmentVersion.Version;
                decimal test = 420.69m;
                environmentVersion.Version = test;

                // Function that we are testing BELOW...
                environmentVersion.Save();

                var testDB = EnvironmentVersion.Select();

                // Revert
                environmentVersion.Version = oldVersion;
                environmentVersion.Save();

                Assert.AreEqual(testDB.Version, test);
            }
            else
                Assert.Fail("EnvironmentVersion NOT FOUND...");



        }

        [TestMethod]
        public async Task X_SaveAsync()
        {
            var environmentVersion = await EnvironmentVersion.SelectAsync();
            Assert.IsFalse(environmentVersion == null || environmentVersion.ID == 0);
            if (environmentVersion?.ID > 0)
            {
                Trace.WriteLine($"FOUND EnvironmentVersion: {environmentVersion.ID}");

                decimal oldVersion = environmentVersion.Version;
                decimal test = 421.69m;
                environmentVersion.Version = test;

                // Function that we are testing BELOW...
                await environmentVersion.SaveAsync();

                var testDB = await EnvironmentVersion.SelectAsync();

                // Revert
                environmentVersion.Version = oldVersion;
                await environmentVersion.SaveAsync();

                Assert.AreEqual(testDB.Version, test);
            }
            else
                Assert.Fail("EnvironmentVersion NOT FOUND...");
        }

        #endregion Save

    }
}
