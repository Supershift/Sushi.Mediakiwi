using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data.Elastic
{
    public class NotificationGroup
    {
        public string GroupName { get; set; }
        public int Count { get; set; }
        public DateTime LastTimestamp { get; set; }
        public string Deeplink { get; set; }
    }
}
