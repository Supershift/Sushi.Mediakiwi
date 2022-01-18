using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(InstallerMap))]
    public class Installer : IInstaller
    {
        public class InstallerMap : DataMap<Installer>
        {
            public InstallerMap()
            {
                Table("wim_Installers");
                Id(x => x.ID, "Installer_Key").Identity();
                Map(x => x.GUID, "Installer_GUID");
                Map(x => x.FolderID, "Installer_Folder_Key");
                Map(x => x.Name, "Installer_Name").Length(50);
                Map(x => x.Assembly, "Installer_Assembly").Length(150);
                Map(x => x.ClassName, "Installer_ClassName").Length(250);
                Map(x => x.Description, "Installer_Description");
                Map(x => x.SettingsString, "Installer_Settings").SqlType(System.Data.SqlDbType.Xml);
                Map(x => x.Version, "Installer_Version");
                Map(x => x.Installed, "Installer_Installed");
            }
        }

        #region Properties

        /// <summary>
        /// The primary key
        /// </summary>
        public virtual int ID { get; set; }

        /// <summary>
        /// The migration key
        /// </summary>
        public virtual Guid GUID { get; set; }

        /// <summary>
        /// The Folder where the install should take place
        /// </summary>
        public virtual int? FolderID { get; set; }

        /// <summary>
        /// The name of the installer
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// The assembly of the installer
        /// </summary>
        public virtual string Assembly { get; set; }

        /// <summary>
        /// The classname of the installer
        /// </summary>
        public virtual string ClassName { get; set; }

        /// <summary>
        /// The description of the installer
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// The custom settings of the installer (String)
        /// </summary>
        public virtual string SettingsString { get; set; }

        private CustomData m_Settings;

        /// <summary>
        /// The custom settings of the installer
        /// </summary>
        public CustomData Settings
        {
            get
            {
                if (m_Settings == null)
                    m_Settings = new CustomData(SettingsString);

                return m_Settings;
            }
            set
            {
                m_Settings = value;
                SettingsString = m_Settings.Serialized;
            }
        }

        /// <summary>
        /// The current version of the installer
        /// </summary>
        public virtual int Version { get; set; }

        /// <summary>
        /// The timestamp when this installer was created
        /// </summary>
        public virtual DateTime? Installed { get; set; }

        #endregion Properties

        /// <summary>
        /// Selects a single installer by GUID
        /// </summary>
        /// <param name="guid">The matching GUID</param>
        /// <returns></returns>
        public static IInstaller SelectOne(Guid guid)
        {
            var connector = ConnectorFactory.CreateConnector<Installer>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.GUID, guid);
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects a single installer by GUID Async
        /// </summary>
        /// <param name="guid">The matching GUID</param>
        /// <returns></returns>
        public static async Task<IInstaller> SelectOneAsync(Guid guid)
        {
            var connector = ConnectorFactory.CreateConnector<Installer>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.GUID, guid);
            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects a single installer by Identifier
        /// </summary>
        /// <param name="guid">The matching GUID</param>
        /// <returns></returns>
        public static IInstaller SelectOne(int id)
        {
            var connector = ConnectorFactory.CreateConnector<Installer>();
            return connector.FetchSingle(id);
        }

        /// <summary>
        /// Selects a single installer by Identifier
        /// </summary>
        /// <param name="guid">The matching GUID</param>
        /// <returns></returns>
        public static async Task<IInstaller> SelectOneAsync(int id)
        {
            var connector = ConnectorFactory.CreateConnector<Installer>();
            return await connector.FetchSingleAsync(id);
        }

        /// <summary>
        /// Selects all Installers
        /// </summary>
        /// <returns></returns>
        public static IInstaller[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<Installer>();
            var filter = connector.CreateQuery();
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Selects all Installers Async
        /// </summary>
        /// <returns></returns>
        public static async Task<IInstaller[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Installer>();
            var filter = connector.CreateQuery();

            var result = await connector.FetchAllAsync(filter);
            return result.ToArray();
        }

        /// <summary>
        /// Saves the current Installer
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            var connector = ConnectorFactory.CreateConnector<Installer>();
            try
            {
                connector.Save(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Saves the current Installer
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Installer>();
            try
            {
                await connector.SaveAsync(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Deletes the current Installer
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            var connector = ConnectorFactory.CreateConnector<Installer>();
            try
            {
                connector.Delete(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Deletes the current Installer Async
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Installer>();
            try
            {
                await connector.DeleteAsync(this);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}