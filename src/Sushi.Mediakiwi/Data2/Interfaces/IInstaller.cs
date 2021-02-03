using System;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;

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
    //  bool Setup();
    //  bool Upgrade();
    //  bool Uninstall();
    //  InstallableVersion[] GetInformation();
    //  int ActualVersion { get; }
        bool HasUpdate { get; }
    }
}