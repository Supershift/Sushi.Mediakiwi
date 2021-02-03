using System;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data
{
    public interface IInstallerParser
    {
        bool Delete(IInstaller entity);
        InstallableVersion[] GetInformation(IInstaller entity);
        IInstallable Instance(IInstaller entity);
        bool Save(IInstaller entity);
        IInstaller[] SelectAll();
        IInstaller SelectOne(int id);
        IInstaller SelectOne(Guid guid);
        bool Setup(IInstaller entity);
        bool Uninstall(IInstaller entity);
        bool Upgrade(IInstaller entity);
    }
}