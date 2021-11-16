using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data.Elastic
{
    public class Notification : Data.Notification
    {
        public Notification(Data.Notification notification)
        {
            Utility.ReflectProperty(notification, this);
            this.Timestamp = notification.Created;
        }
        
        [Ignore]
        public ElasticId ElasticId { get; set; }
        
        [PropertyName("@timestamp")]
        public DateTime Timestamp { get; set; }

        public override string GetIdMessage()
        {
            return $"{ElasticId.Index} - {ElasticId.Id}";
        }
    }
}
