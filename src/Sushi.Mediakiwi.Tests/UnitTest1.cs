using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Logic;
using Sushi.Mediakiwi.Utilities;
using Sushi.MicroORM;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
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

        void ExtractText(List<string> collection, ContentItem item)
        {
            if (item == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(item.Text))
            {
                collection.Add(item.Text);
            }

            if (item.MultiFieldContent != null && item.MultiFieldContent.Any())
            {
                foreach (var nested in item.MultiFieldContent)
                {
                    ExtractText(collection, nested.Value);
                }
            }
        }


        [TestMethod]
        public async Task Fetch_User()
        {
            var visitor = Visitor.Select(new Guid("708150D2-9292-4C43-81C7-F92D25D0E954"));
            Assert.IsTrue(visitor.Data.Items.Length > 0);
        }



        [TestMethod]
        public async Task Create_Instance()
        {
            var instance = Utils.CreateInstance("Sushi.Mediakiwi.dll", "Sushi.Mediakiwi.AppCentre.Data.Implementation.Browsing");
            Assert.IsTrue(instance != null);
        }

        [TestMethod]
        public async Task Authentication()
        {
            var mockHttpContext = new Mock<HttpContext>();

            var mockRequest = new Mock<HttpRequest>();
            var mockResponse = new Mock<HttpResponse>();
            var mockHttpContextAccessor = new Mock<HttpContextAccessor>();

            mockHttpContext.SetupGet(x => x.Request).Returns(mockRequest.Object);
            mockHttpContext.SetupGet(x => x.Response).Returns(mockResponse.Object);

            //mockRequest.SetupGet(x => x.Headers).Returns(
            //    new HeaderDictionary  {
            //        { "x-session-id", SessionID },
            //        //{ "accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8" }
            //    });

            // tell the mock to return "GET" when HttpMethod is called
            mockRequest.SetupGet(x => x.Method).Returns("GET");

            var value = "test";
            using (Authentication auth = new Authentication(mockHttpContext.Object))
            {
                auth.Password = "pwd";
                var encrypted = auth.Encrypt(value);
                Assert.IsTrue(auth.Decrypt(encrypted).Equals(value));
            }
        }
    }
}
