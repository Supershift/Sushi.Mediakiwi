using System;

namespace Sushi.Mediakiwi.Data
{
    public interface IAssetType
    {
        DateTime Created { get; set; }
        Guid Guid { get; set; }
        int ID { get; set; }
        bool IsNewInstance { get; }
        bool IsVariant { get; set; }
        string Name { get; set; }
        string Tag { get; set; }

        bool Save();
    }
}