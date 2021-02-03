using System;

namespace Sushi.Mediakiwi.Data
{
    public interface IInstaller
    {
        string Assembly { get; set; }
        string ClassName { get; set; }
        DateTime? Installed { get; set; }
        string Description { get; set; }
        Guid GUID { get; set; }
        int ID { get; set; }
        int? FolderID { get; set; }
        string Name { get; set; }
        CustomData Settings { get; set; }
        int Version { get; set; }

        bool Save();

        bool Delete();
    }
}