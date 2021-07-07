using Sushi.Mediakiwi.Data;

namespace Sushi.Mediakiwi.Framework
{
    public interface IInstallable
    {
        bool Setup(IInstaller installed);
        bool Upgrade(IInstaller installed);
        bool Uninstall(IInstaller installed);
        InstallableVersion[] GetInformation(IInstaller installed);
    }
}
