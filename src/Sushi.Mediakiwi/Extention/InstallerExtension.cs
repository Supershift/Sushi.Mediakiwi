using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;

public static class InstallerExtension
{
    static IInstallerParser _Parser;
    static IInstallerParser Parser
    {
        get
        {
            if (_Parser == null)
                _Parser = Sushi.Mediakiwi.Data.Environment.GetInstance<IInstallerParser>();
            return _Parser;
        }
    }


    /// <summary>
    /// Gets the actual version 
    /// </summary>
    /// <param name="inInstaller"></param>
    /// <returns></returns>
    public static int GetActualVersion(this IInstaller inInstaller)
    {
        var instance = (Sushi.Mediakiwi.Framework.IInstallable)Wim.Utility.CreateInstance(inInstaller.Assembly, inInstaller.ClassName);
        var attributes = instance.GetType().GetCustomAttributes(typeof(Sushi.Mediakiwi.Framework.InstallableAttribute), true);
        if (attributes.Length > 0)
        {
            return ((Sushi.Mediakiwi.Framework.InstallableAttribute)attributes[0]).Version;
        }
        return 0;
    }

    public static bool Setup(this IInstaller inInstaller)
    {
        if (Parser.Setup(inInstaller))
        {
            inInstaller.Version = 1;
            return Upgrade(inInstaller);
        }
        return false;
    }

    public static bool Upgrade(this IInstaller inInstaller)
    {
        return Parser.Upgrade(inInstaller);
    }

    public static bool Uninstall(this IInstaller inInstaller)
    {
        return Parser.Uninstall(inInstaller);
    }

    public static Sushi.Mediakiwi.Framework.InstallableVersion[] GetInformation(this IInstaller inInstaller)
    {
        return Parser.GetInformation(inInstaller);
    }
}
