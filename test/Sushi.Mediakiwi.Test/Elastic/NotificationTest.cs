using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Data.Elastic.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Test.Elastic
{
    [TestClass]
    public class NotificationTest : BaseTest
    {
        private static NotificationRepository _repository = new NotificationRepository(ElasticConnectionSettings, "notifications");

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
        public async Task GetAllAsync()
        {
            //var result = await ElasticConnectionSettings.SearchAsync<Notification>(d => d
            //    .Index("notifications")
            //    .Query(q => q.MatchAll())
            //    );
            //WriteResult(result);
        }
    }
}
