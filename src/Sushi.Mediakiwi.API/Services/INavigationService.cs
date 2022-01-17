using Sushi.Mediakiwi.API.Transport;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.API.Services
{
    public interface INavigationService
    {
        public string GetQueryStringRecording(UrlResolver resolver);
        
        public Task ApplyTabularUrlAsync(UrlResolver resolver, Framework.WimComponentListRoot.Tabular tab, int levelEntry, int? currentListID);

        public string GetLogoURL(UrlResolver resolver);

        public Task<string> GetHomepageURLAsync(UrlResolver resolver);

        public bool IsRequestPartOfNavigation(Data.IMenuItemView item, UrlResolver urlResolver);

        public Task<string> GetUrlAsync(UrlResolver resolver, Data.IMenuItemView entity, int? siteId);

        public Task<bool> HasRoleAccessAsync(Data.IMenuItemView item, Data.IApplicationRole role);

        public Task<bool> HasRoleAccessAsync(Data.Interfaces.ISearchView item, Data.IApplicationRole role);

        public Task<(bool isCurrent, bool addEmpty)> AddSubSubNavigationAsync(UrlResolver resolver, NavigationItem topnav, Data.IMenuItemView item, string className, Data.IApplicationRole role);
    }
}
