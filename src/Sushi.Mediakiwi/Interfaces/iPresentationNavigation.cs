using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Interfaces
{
    public interface iPresentationNavigation
    {
        Task<string> NewLeftNavigationAsync(Beta.GeneratedCms.Console container, Framework.ContentListItem.ButtonAttribute[] buttonList);
        string NewBottomNavigation(Beta.GeneratedCms.Console container, Framework.ContentListItem.ButtonAttribute[] buttonList, bool hasFilters);
        string NewBottomNavigation(Beta.GeneratedCms.Console container, Framework.ContentListItem.ButtonAttribute[] buttonList, bool hasFilters, WimControlBuilder builder);
        string RightSideNavigation(Beta.GeneratedCms.Console container, Framework.ContentListItem.ButtonAttribute[] buttonList);
        string RightSideNavigation(Beta.GeneratedCms.Console container, Framework.ContentListItem.ButtonAttribute[] buttonList, WimControlBuilder builder);
        string TopNavigation(Beta.GeneratedCms.Console container);
        string GetUrl(Beta.GeneratedCms.Console container, IMenuItemView entity, int channel);
    }
}
