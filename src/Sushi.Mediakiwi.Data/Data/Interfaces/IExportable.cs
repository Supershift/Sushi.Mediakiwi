using System;

namespace Sushi.Mediakiwi.Data
{
    public interface IExportable
    {
        int ID { get; set; }
        Guid GUID { get; set; }
        DateTime? Updated { get; }
    }
}