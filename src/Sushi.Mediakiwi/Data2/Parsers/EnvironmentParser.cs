using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;
using System.Web;
using Sushi.Mediakiwi.Data.Parsers;
using Sushi.Mediakiwi.Data.Identity.Parsers;
using Sushi.Mediakiwi.Data.Identity;
using Sushi.Mediakiwi.Data.Statistics;
using Sushi.Mediakiwi.Data.Statistics.Parsers;
using System.Collections.Concurrent;
using Sushi.Mediakiwi.Framework.UI;

namespace Sushi.Mediakiwi.Data.Parsers
{
    /// <summary>
    /// Wim environment settings.
    /// </summary>
    public class EnvironmentParser : IEnvironmentParser
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

        public virtual bool Save(IEnvironment entity)
        {
            DataParser.Save<IEnvironment>(entity);
            return true;
        }

        static bool _ContainerIsVerified;
        static ConcurrentDictionary<Type, string> _ContainerLookup = new ConcurrentDictionary<Type, string>();
         
        static SimpleInjector.Container _Container;
        static SimpleInjector.Container DependencyContainer
        {
            get
            {
                if (_Container == null)
                    _Container = new SimpleInjector.Container();

                return _Container;
            }
            set
            {
                _Container = value;
            }
        }

        public virtual bool AddDependencyIfNotExist<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            //  Do not remove this, it looks extra, but circomvents that RegisterDependencyContainer is called to early!
            //if (_ContainerLookup == null)
            //    _ContainerLookup = new Hashtable();

            //if (!ContainsDependency<TService>())
            //{
                if (_ContainerLookup.TryAdd(typeof(TService), "1"))
                    DependencyContainer.Register<TService, TImplementation>();
            //}
            return false;
        }

        public virtual bool ContainsDependency<TService>()
        {
            if (_ContainerLookup.Count == 0)
                RegisterDependencyContainer();

            return _ContainerLookup.ContainsKey(typeof(TService));
        }

        public virtual ICollection<System.Type> RegisteredDependencies()
        {
            return _ContainerLookup.Keys;
        }

        private static Object thisLock = new Object();

        public virtual void RegisterDependencyContainer()
        {
            if (_ContainerIsVerified)
                return;

            lock (thisLock)
            {
                if (_ContainerIsVerified)
                    return;

                AddDependencyIfNotExist<IPagePreview, PagePreview>();
                AddDependencyIfNotExist<IPagePublication, PagePublication>();
                AddDependencyIfNotExist<IMultiFieldParser, MultiFieldParser>();
                AddDependencyIfNotExist<iPreMonitorHook, PreMonitorHook>();
                AddDependencyIfNotExist<IRequestRouter, RequestRouter>();
                AddDependencyIfNotExist<iMappedUrlCreator, Common.MappedUrlCreator>();
                AddDependencyIfNotExist<IHtmlTableParser, HtmlTableParser>();
                AddDependencyIfNotExist<IRichTextDataCleaner, RichTextDataCleaner>();
                AddDependencyIfNotExist<IOutputCompressor, OutputCompressor>();

                AddDependencyIfNotExist<ISqlEntityParser, SqlEntityParser>();
                // Entity parsers
                AddDependencyIfNotExist<IVisitorParser, VisitorParser>();
                AddDependencyIfNotExist<IVisitor, Visitor>();
                AddDependencyIfNotExist<IProfileParser, ProfileParser>();
                AddDependencyIfNotExist<IProfile, Profile>();
                // Data
                AddDependencyIfNotExist<IAssetType, AssetType>();
                AddDependencyIfNotExist<IAssetTypeParser, AssetTypeParser>();

                AddDependencyIfNotExist<ICacheItem, CacheItem>();
                AddDependencyIfNotExist<ICacheItemParser, CacheItemParser>();

                AddDependencyIfNotExist<IComponentList, ComponentList>();
                AddDependencyIfNotExist<IComponentListParser, ComponentListParser>();

                AddDependencyIfNotExist<IComponentTarget, ComponentTarget>();
                AddDependencyIfNotExist<IComponentTargetParser, ComponentTargetParser>();

                AddDependencyIfNotExist<IComponentTargetPage, ComponentTargetPage>();
                AddDependencyIfNotExist<IComponentTargetPageParser, ComponentTargetPageParser>();

                AddDependencyIfNotExist<IEnvironment, Environment>();
                AddDependencyIfNotExist<IEnvironmentParser, EnvironmentParser>();

                AddDependencyIfNotExist<IEnvironmentVersionParser, EnvironmentVersionParser>();
                AddDependencyIfNotExist<IEnvironmentVersion, EnvironmentVersion>();

                AddDependencyIfNotExist<IInstallerParser, InstallerParser>();
                AddDependencyIfNotExist<IInstaller, Installer>();

                AddDependencyIfNotExist<IMenu, Menu>();
                AddDependencyIfNotExist<IMenuParser, MenuParser>();

                AddDependencyIfNotExist<IMenuItem, MenuItem>();
                AddDependencyIfNotExist<IMenuItemParser, MenuItemParser>();

                AddDependencyIfNotExist<IMenuItemView, MenuItemView>();
                AddDependencyIfNotExist<IMenuItemViewParser, MenuItemViewParser>();

                AddDependencyIfNotExist<INotification, Notification>();
                AddDependencyIfNotExist<INotificationParser, NotificationParser>();

                AddDependencyIfNotExist<IPageMapping, PageMapping>();
                AddDependencyIfNotExist<IPageMappingParser, PageMappingParser>();

                AddDependencyIfNotExist<IPageVersion, PageVersion>();
                AddDependencyIfNotExist<IPageVersionParser, PageVersionParser>();

                AddDependencyIfNotExist<IPortalRight, PortalRight>();
                AddDependencyIfNotExist<IPortalRightParser, PortalRightParser>();

                AddDependencyIfNotExist<IRegistry, Registry>();
                AddDependencyIfNotExist<IRegistryParser, RegistryParser>();

                AddDependencyIfNotExist<IRoleRightAccessItem, RoleRightAccessItem>();
                AddDependencyIfNotExist<IRoleRightAccessItemParser, RoleRightAccessItemParser>();

                AddDependencyIfNotExist<ISearchView, SearchView>();
                AddDependencyIfNotExist<ISearchViewParser, SearchViewParser>();

                // [MR:23-01-2020] Will not be used again
                //AddDependencyIfNotExist<IDataFilter, DataFilter>();
                //AddDependencyIfNotExist<IDataFilterParser, DataFilterParser>();

                AddDependencyIfNotExist<IPortal, Portal>();
                AddDependencyIfNotExist<IPortalParser, PortalParser>();

                AddDependencyIfNotExist<ISubscription, Subscription>();
                AddDependencyIfNotExist<ISubscriptionParser, SubscriptionParser>();

                AddDependencyIfNotExist<IApplicationRole, ApplicationRole>();
                AddDependencyIfNotExist<IApplicationRoleParser, ApplicationRoleParser>();

                AddDependencyIfNotExist<IApplicationUser, ApplicationUser>();
                AddDependencyIfNotExist<IApplicationUserParser, ApplicationUserParser>();

                AddDependencyIfNotExist<IAvailableTemplate, AvailableTemplate>();
                AddDependencyIfNotExist<IAvailableTemplateParser, AvailableTemplateParser>();

                AddDependencyIfNotExist<iPresentationMonitor, Sushi.Mediakiwi.Framework.Presentation.Monitor>();
                AddDependencyIfNotExist<iPresentationNavigation, Sushi.Mediakiwi.Framework.Presentation.Logic.Navigation>();

                //  Statistics
                AddDependencyIfNotExist<IVisitorClick, VisitorClick>();
                AddDependencyIfNotExist<IVisitorClickParser, VisitorClickParser>();

                _ContainerIsVerified = true;
                DependencyContainer.Verify();
            }
        }

        public virtual TService GetInstance<TService>() where TService : class
        {
            RegisterDependencyContainer();
            return DependencyContainer.GetInstance<TService>();
        }

        public virtual object GetInstance(Type type) 
        {
            RegisterDependencyContainer();
            return DependencyContainer.GetInstance(type);
        }

        public virtual IEnvironment SelectOne()
        {
            return DataParser.SelectOne<IEnvironment>(true);
        }


        /// <summary>
        /// Returns a SMTP client, always a new instance
        /// </summary>
        /// <param name="entity">The environment itself.</param>
        /// <returns></returns>
        public virtual System.Net.Mail.SmtpClient SmtpClient(IEnvironment entity)
        {
            string[] split = entity.SmtpServer.Split(':');
            System.Net.Mail.SmtpClient m_Client = new System.Net.Mail.SmtpClient();

            if (split.Length > 1)
                m_Client.Port = Convert.ToInt32(split[1]);
            m_Client.Host = split[0];

            if (!string.IsNullOrEmpty(entity.SmtpServerUser))
                m_Client.Credentials = new System.Net.NetworkCredential(entity.SmtpServerUser, entity.SmtpServerPass);

            m_Client.EnableSsl = entity.SmtpEnableSSL;

            return m_Client;
        }

        #region registry
        private Object registerLock = new Object();

        /// <summary>
        /// Gets the registry value.
        /// </summary>
        /// <param name="registry">The registry.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public virtual string GetRegistryValue(IEnvironment entity, string registry, string defaultValue)
        {
            string candidate = entity[registry];
            if (string.IsNullOrEmpty(candidate))
                return defaultValue;
            return candidate;
        }
        #endregion registry

        static ConcurrentBag<IPageModule> _registeredPageModules = new ConcurrentBag<IPageModule>();

        public void AddPageModule<TPageModule>(TPageModule pageModule) where TPageModule : class, IPageModule
        {
            if (_registeredPageModules.Contains(pageModule) == false)
                _registeredPageModules.Add(pageModule);
        }

        public ICollection<IPageModule> GetPageModules()
        {
            return _registeredPageModules.ToList();
        }
    }
}