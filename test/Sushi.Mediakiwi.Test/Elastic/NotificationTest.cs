using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using static Nest.Infer;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.Elastic.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sushi.Mediakiwi.Test.Elastic
{
    [TestClass]
    public class NotificationTest : BaseTest
    {
        private static NotificationRepository _repository = new NotificationRepository(ElasticClient, "notifications");

        private Notification GetNotificationInstance()
        {
            var result = new Data.Notification()
            {
                Group = "Elastic notification test",
                Text = "Notifcation for xUNIT TESTx",
                Selection = NotificationType.Information,
                UserID = null,
                VisitorID = 1,
                PageID = 69420,
                Created = DateTime.UtcNow
            };
            return result;
        }

        [TestMethod]
        public async Task SaveAsync()
        {
            var notification = GetNotificationInstance();
            var result = await _repository.SaveAsync(notification);
            WriteResult(result);
            Assert.IsInstanceOfType(result, typeof(Data.Elastic.Notification));
            Assert.IsNotNull(((Data.Elastic.Notification)result).ElasticId);            
        }

        [TestMethod]
        public async Task GetIdMessage()
        {
            var notification = GetNotificationInstance();
            var result = await _repository.SaveAsync(notification);
            WriteResult(notification.GetIdMessage());
            WriteResult(result.GetIdMessage());
            Assert.AreNotEqual(notification.GetIdMessage(), result.GetIdMessage());
        }

        [TestMethod]
        public async Task GetAll()
        {
            var result = await _repository.GetAllAsync(null, "Elastic notification test", null, null, 25, null);
            WriteResult(result);
        }

        [TestMethod]
        public async Task GetAggregated()
        {
            var result = await _repository.GetAggregatedGroupsAsync(null, null, null);
            WriteResult(result);
            Assert.AreNotEqual(0, result.Count);
        }

        public class BuiltIn
        {
            [Text(Name = "naam")]
            public string Name { get; set; }
        }

        [TestMethod]
        public async Task TimestampNameInference()
        {
            var fieldExpression = Field<Data.Elastic.Notification>(p => p.Timestamp);
            
            var fieldResolver = new FieldResolver(ElasticClient.ConnectionSettings);
            
            var fieldName = fieldResolver.Resolve(fieldExpression);
            Console.WriteLine(fieldName);
            Assert.AreEqual("@timestamp", fieldName);
        }
    }
}
