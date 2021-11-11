using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data.Elastic
{
    public struct ElasticId
    {
        public ElasticId(string id, string index)
        {
            Id = id;
            Index = index;
        }

        public string Id { get; }
        public string Index { get; }
    }
}
