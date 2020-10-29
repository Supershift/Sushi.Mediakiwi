using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface IEnvironmentParser
    {
        ICollection<System.Type> RegisteredDependencies();
        void RegisterDependencyContainer();
        bool AddDependencyIfNotExist<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;
        bool ContainsDependency<TService>();
        object GetInstance(Type type);
        TService GetInstance<TService>() where TService : class;
        string GetRegistryValue(IEnvironment entity, string registry, string defaultValue);
        IEnvironment SelectOne();
        SmtpClient SmtpClient(IEnvironment entity);
        bool Save(IEnvironment entity);

        void AddPageModule<TPageModule>(TPageModule pageModule) where TPageModule : class, IPageModule;

        ICollection<IPageModule> GetPageModules();
    }
}