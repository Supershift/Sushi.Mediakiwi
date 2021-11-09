using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sushi.Mediakiwi.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Test.ORM
{
    [TestClass]
    public class RoleRightAccessItemTests : BaseTest
    {
        [TestMethod]
        public void X_Select()
        {
            int roleID = 5;
            int typeID = 1;
            int childTypeID = 1;

            // Function that we are testing BELOW...
            var roleRights = RoleRightAccessItem.Select(roleID, typeID, childTypeID);

            if (roleRights?.Length > 0)
                Trace.WriteLine($"FOUND RoleRightAccessItem: {roleRights.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("RoleRightAccessItem NOT FOUND...");
        }

        [TestMethod]
        public async Task X_SelectAsync()
        {
            int roleID = 5;
            int typeID = 1;
            int childTypeID = 1;

            // Function that we are testing BELOW...
            var roleRights = await RoleRightAccessItem.SelectAsync(roleID, typeID, childTypeID);

            if (roleRights?.Length > 0)
                Trace.WriteLine($"FOUND RoleRightAccessItem: {roleRights.Select(x => x.ID.ToString()).Aggregate((a, b) => a + ", " + b)}");
            else
                Assert.Fail("RoleRightAccessItem NOT FOUND...");
        }
    }
}
