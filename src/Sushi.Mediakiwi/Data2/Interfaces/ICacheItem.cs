using System;

namespace Sushi.Mediakiwi.Data
{
    public interface ICacheItem
    {
        DateTime Created { get; set; }
        int ID { get; set; }
        bool IsIndex { get; set; }
        string Name { get; set; }

        void Save();
    }
}