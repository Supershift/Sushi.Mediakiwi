using System;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data
{
    public class InstallerParser : IInstallerParser
    {
        static ISqlEntityParser _DataParser;
        static ISqlEntityParser DataParser
        {
            get
            {
                if (_DataParser == null)
                    _DataParser = Environment.GetInstance<ISqlEntityParser>();
                return _DataParser;
            }
        }


        public virtual IInstaller SelectOne(Guid guid)
        {
            return DataParser.SelectOne<IInstaller>(guid);
        }

        public virtual IInstaller SelectOne(int id)
        {
            return DataParser.SelectOne<IInstaller>(id, true);
        }

        public virtual IInstaller[] SelectAll()
        {
            return DataParser.SelectAll<IInstaller>(null, "all").ToArray();
        }

        public virtual bool Save(IInstaller entity)
        {
            return DataParser.Save<IInstaller>(entity) > 0;
        }

        public virtual bool Delete(IInstaller entity)
        {
            return DataParser.Delete<IInstaller>(entity);
        }

        IInstallable _Instance;
        public virtual IInstallable Instance(IInstaller entity)
        {
            if (_Instance == null)
                _Instance = (IInstallable)Wim.Utility.CreateInstance(entity.Assembly, entity.ClassName);
            return _Instance;
        }


        public virtual bool Setup(IInstaller entity)
        {
            return Instance(entity).Setup(entity);
        }

        public virtual bool Upgrade(IInstaller entity)
        {
            return Instance(entity).Upgrade(entity);
        }

        public virtual bool Uninstall(IInstaller entity)
        {
            return Instance(entity).Uninstall(entity);
        }

        public virtual InstallableVersion[] GetInformation(IInstaller entity)
        {
            return Instance(entity).GetInformation(entity);
        }

    }
}
