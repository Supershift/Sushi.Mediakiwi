using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Tests.ORM
{
    [TestClass]
    public class EnvironmentTests : BaseTest
    {
        #region Select ONE

        [TestMethod]
        public void X_SelectOne()
        {
            // Function that we are testing BELOW...
            var environment = Mediakiwi.Data.Environment.SelectOne();

            if (environment?.ID > 0)
                Trace.WriteLine($"FOUND Environment: {environment.ID}");
            else
                Assert.Fail("Environment NOT FOUND...");
        }


        [TestMethod]
        public async Task X_SelectOneAsync()
        {
            // Function that we are testing BELOW...
            var environment = await Mediakiwi.Data.Environment.SelectOneAsync();

            if (environment?.ID > 0)
                Trace.WriteLine($"FOUND Environment: {environment.ID}");
            else
                Assert.Fail("Environment NOT FOUND...");
        }

        #endregion Select ONE

        #region Save

        [TestMethod]
        public void X_Save()
        {
            var environment = Mediakiwi.Data.Environment.SelectOne();
            Assert.IsFalse(environment == null || environment.ID == 0);

            if (environment?.ID > 0)
            {
                Trace.WriteLine($"FOUND Environment: {environment.ID}");

                string oldTitle = environment.Title;
                string test = "xUNIT TESTx";
                environment.Title = test;

                // Function that we are testing BELOW...
                environment.Save();

                var testDB = Mediakiwi.Data.Environment.SelectOne();

                // Revert
                environment.Title = oldTitle;
                environment.Save();

                Assert.AreEqual(testDB.Title, test);
            }
            else
                Assert.Fail("Environment NOT FOUND...");



        }

        [TestMethod]
        public async Task X_SaveAsync()
        {
            var environment = await Mediakiwi.Data.Environment.SelectOneAsync();
            Assert.IsFalse(environment == null || environment.ID == 0);
            if (environment?.ID > 0)
            {
                Trace.WriteLine($"FOUND Environment: {environment.ID}");

                string oldTitle = environment.Title;
                string test = "xASYNC UNIT TESTx";
                environment.Title = test;

                // Function that we are testing BELOW...
                await environment.SaveAsync();

                var testDB = await Mediakiwi.Data.Environment.SelectOneAsync();

                // Revert
                environment.Title = oldTitle;
                await environment.SaveAsync();

                Assert.AreEqual(testDB.Title, test);
            }
            else
                Assert.Fail("Environment NOT FOUND...");
        }

        #endregion Save

    }
}
