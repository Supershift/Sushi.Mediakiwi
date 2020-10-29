using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data
{
    [DatabaseTable("wim_Installers")]
    public class Installer : IInstaller
    {
   
        static IInstallerParser _Parser;
        static IInstallerParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IInstallerParser>();
                return _Parser;
            }
        }

        #region COMMENTED, not used anywhere

        //IInstallable _Instance;
        //public IInstallable Instance()
        //{
        //    return Parser.Instance(this);
        //}

        #endregion COMMENTED, not used anywhere

        #region MOVED to EXTENSION / LOGIC

        //public virtual int ActualVersion
        //{
        //    get
        //    {
        //        var instance = (IInstallable)Wim.Utility.CreateInstance(Assembly, ClassName);
        //        var attributes = instance.GetType().GetCustomAttributes(typeof(InstallableAttribute), true);
        //        if (attributes.Length > 0)
        //        {
        //            return ((InstallableAttribute)attributes[0]).Version;
        //        }
        //        return 0;
        //    }
        //}


        //public bool Setup()
        //{
        //    if (Parser.Setup(this))
        //    {
        //        this.Version = 1;
        //        return Upgrade();
        //    }
        //    return false;
        //}

        //public bool Upgrade()
        //{
        //    return Parser.Upgrade(this);
        //}

        //public bool Uninstall()
        //{
        //    return Parser.Uninstall(this);
        //}

        //public InstallableVersion[] GetInformation()
        //{
        //    return Parser.GetInformation(this);
        //}

        #endregion MOVED to EXTENSION / LOGIC

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        #region Properties

        /// <summary>
        /// The primary key
        /// </summary>
        [DatabaseColumn("Installer_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public virtual int ID { get; set; }
        /// <summary>
        /// The migration key
        /// </summary>
        [DatabaseColumn("Installer_GUID", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public virtual Guid GUID { get; set; }

        [DatabaseColumn("Installer_Folder_Key", SqlDbType.Int, IsNullable = true)]
        public virtual int? FolderID { get; set; }
        /// <summary>
        /// The name of the installer
        /// </summary>
        [DatabaseColumn("Installer_Name", SqlDbType.NVarChar, Length = 50)]
        public virtual string Name { get; set; }
        /// <summary>
        /// The assembly of the installer
        /// </summary>
        [DatabaseColumn("Installer_Assembly", SqlDbType.VarChar, Length = 150, IsNullable = true)]
        public virtual string Assembly { get; set; }
        /// <summary>
        /// The classname of the installer
        /// </summary>
        [DatabaseColumn("Installer_ClassName", SqlDbType.VarChar, Length = 250, IsNullable = true)]
        public virtual string ClassName { get; set; }
        /// <summary>
        /// The description of the installer
        /// </summary>
        [DatabaseColumn("Installer_Description", SqlDbType.NVarChar, IsNullable = true)]
        public virtual string Description { get; set; }
        /// <summary>
        /// The custom settings of the installer
        /// </summary>
        [DatabaseColumn("Installer_Settings", SqlDbType.Xml, IsNullable = true)]
        public virtual Sushi.Mediakiwi.Data.CustomData Settings { get; set; }
        /// <summary>
        /// The current version of the installer
        /// </summary>
        [DatabaseColumn("Installer_Version", SqlDbType.Int)]
        public virtual int Version { get; set; }
        /// <summary>
        /// The timestamp when this installer was created
        /// </summary>
        [DatabaseColumn("Installer_Installed", SqlDbType.DateTime, IsNullable = true)]
        public virtual DateTime? Installed { get; set; }
        public virtual bool HasUpdate
        {
            get
            {
                if (this.ClassName != null)
                    return this.GetActualVersion() > this.Version;
                return true;
            }
        }
        #endregion Properties

        public static IInstaller SelectOne(Guid guid)
        {
            return Parser.SelectOne(guid);
        }

        public static IInstaller SelectOne(int id)
        {
            return Parser.SelectOne(id);
        }

        public static IInstaller[] SelectAll()
        {
            return Parser.SelectAll();
        }

        public bool Save()
        {
            return Parser.Save(this);
        }

        public bool Delete()
        {
            return Parser.Delete(this);
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard

    }
}
