using System;
using System.Diagnostics.CodeAnalysis;

namespace Sushi.Mediakiwi.Data.Elastic
{
    public struct ElasticId : IEquatable<ElasticId>
    {
        public ElasticId(string id, string index)
        {
            Id = id;
            Index = index;
        }

        public string Id { get; }
        public string Index { get; }

        public bool Equals([AllowNull] ElasticId other)
        {
            return other.Id == Id && other.Index == Index;
        }

        public override int GetHashCode()
        {
            return $"{Index}.{Id}".GetHashCode(StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ElasticId))
                return false;

            return Equals((ElasticId)obj);
        }

        public static bool operator == (ElasticId elasticId1, ElasticId elasticId2)
        {
            return elasticId1.Equals(elasticId2);
        }

        public static bool operator != (ElasticId elasticId1, ElasticId elasticId2)
        {
            return !elasticId1.Equals(elasticId2);
        }
    }
}
