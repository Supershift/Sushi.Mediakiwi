using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sushi.Mediakiwi.Data;
using Sushi.MicroORM;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Tests
{
    [TestClass]
    public class Data
    {
        [TestInitialize]
        public void Init()
        {
            string connectionString = "Server=testing-mediakiwi.database.windows.net,1433;Database=testing-mediakiwi;Uid=mediakiwi;Pwd=tMAX28KozCCXt0HUVDCg";
            DatabaseConfiguration.SetDefaultConnectionString(connectionString);
        }

        [TestMethod]
        public async Task Fetch_All_Countries()
        {
            var cc = await Country.SelectAllAsync();
            Assert.IsTrue(cc.Length > 0);
        }
    }
}
