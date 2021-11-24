using Sushi.Mediakiwi.API.Transport;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.API.Services
{
    public interface INavigationService
    {
        public string GetLogoURL(Beta.GeneratedCms.Console container);

        public string GetHomepageURL(Beta.GeneratedCms.Console container);

        public bool IsRequestPartOfNavigation(Data.IMenuItemView item, UrlResolver urlResolver);

        public string GetUrl(Beta.GeneratedCms.Console container, Data.IMenuItemView entity, int channel);

        public Task<bool> HasRoleAccessAsync(Data.IMenuItemView item, Data.IApplicationRole role);

        public Task<bool> HasRoleAccessAsync(Data.Interfaces.ISearchView item, Data.IApplicationRole role);

        public Task<(bool isCurrent, bool addEmpty)> AddSubSubNavigationAsync(Beta.GeneratedCms.Console container, NavigationItem topnav, Data.IMenuItemView item, string className, Data.IApplicationRole role);
    }
}
