using Nest;
using Sushi.Mediakiwi.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data.Elastic.Repositories
{
    /// <summary>
    /// Provides methods to store and retrieve instances of <see cref="Notification"/> from an Elastic server. 
    /// It is advised to use one instance of <see cref="NotificationRepository"/> per application or at least per Elastic connection.
    /// </summary>
    public class NotificationRepository : INotificationRepository
    {
        private Nest.IElasticClient _client;
        private string _dataStreamName;
        public NotificationRepository(Nest.IElasticClient client, string dataStreamName)
        {
            // create client
            _client = client;
            _dataStreamName = dataStreamName;
        }
        
        public Data.Notification Save(Data.Notification notification)
        {
            var elasticNotification = new Notification(notification);
            var response = _client.Index(elasticNotification, i => i.Index(_dataStreamName));
            elasticNotification.ElasticId = new ElasticId(response.Id, response.Index);
            return elasticNotification;
        }

        public async Task<Data.Notification> SaveAsync(Data.Notification notification)
        {
            var elasticNotification = new Notification(notification);
            var response = await _client.IndexAsync(elasticNotification, i => i.Index(_dataStreamName));
            elasticNotification.ElasticId = new ElasticId(response.Id, response.Index);
            return elasticNotification;
        }

        public async Task<List<NotificationGroup>> GetAggregatedGroupsAsync(NotificationType? selectionID, DateTime? from, DateTime? to)
        {
            var response = await _client.SearchAsync<Notification>(s => s
                .Size(0)
                .Index(_dataStreamName)
                .Query(q =>
                    +q.Term(t => t.Selection, selectionID)
                    && +q.DateRange(d => d.GreaterThanOrEquals(from))
                    && +q.DateRange(d => d.LessThan(to))
                    )
                .Aggregations(a => a
                    .Terms("groups", t => t
                        .Size(100)
                        .Field(f => f.Group)
                        .Order(o => o.Descending("maxTimestamp"))
                        .Aggregations(subAgg => subAgg.Max("maxTimestamp", m => m.Field(f => f.Timestamp)))
                        )
                    )
                ).ConfigureAwait(false);

            var groupsAggregation = response.Aggregations.Terms("groups");
            var result = new List<NotificationGroup>();
            foreach(var group in groupsAggregation.Buckets)
            {
                var resultItem = new NotificationGroup()
                {
                    Count = (int)group.DocCount.GetValueOrDefault(),
                    GroupName = group.Key,
                    LastTimestamp = DateTime.Parse(group.Max("maxTimestamp").ValueAsString)
                };
                result.Add(resultItem);
            }

            return result;
        }

        public async Task<List<Notification>> GetAllAsync(NotificationType? selectionID, string groupName, DateTime? from, DateTime? to, int pageSize, object[] searchAfter)
        {
            var response = await _client.SearchAsync<Notification>(s => s
                .Size(pageSize)
                .Index(_dataStreamName)
                .Sort(srt => srt.Descending(f => f.Timestamp))
                .SearchAfter(searchAfter)
                .Query(q =>
                    +q.Term(t => t.Group, groupName)
                    && +q.Term(t => t.Selection, selectionID)
                    && +q.DateRange(d => d.GreaterThanOrEquals(from))
                    && +q.DateRange(d => d.LessThan(to))
                    )
                ).ConfigureAwait(false);

            var result = new List<Notification>();
            foreach(var hit in response.Hits)
            {
                // apply hit ID to item
                hit.Source.ElasticId = new ElasticId(hit.Id, hit.Index);
                result.Add(hit.Source);
            }

            return result;
        }

        public async Task<Notification> GetOneAsync(string index, string id)
        {
            var response = await _client.GetAsync<Notification>(id, s => s.Index(index));
            return response.Source;
        }

        
    }
}
