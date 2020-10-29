using System;

namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface ICacheItemParser
    {
        void ApplyLoadBalancedCacheCheckItem(string key, bool isIndexKey);
        void Clear();
        void Save(ICacheItem entity);
        ICacheItem[] SelectAll(DateTime dt);
        ICacheItem SelectOne(int Key);
    }
}