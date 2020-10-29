using System;

namespace Sushi.Mediakiwi.Data
{
    public interface IRegistry
    {
        string Description { get; set; }
        Guid GUID { get; set; }
        int ID { get; set; }
        string Name { get; set; }
        string NameDescription { get; }
        int Type { get; set; }
        string Value { get; set; }

        void Save();
        void Delete();
    }
}