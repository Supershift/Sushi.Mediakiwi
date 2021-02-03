using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Interfaces
{
    public interface iPresentationNavigation
    {
        string NewLeftNavigation(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Framework.ContentListItem.ButtonAttribute[] buttonList);
        string NewBottomNavigation(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Framework.ContentListItem.ButtonAttribute[] buttonList, bool hasFilters);
        string NewBottomNavigation(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Framework.ContentListItem.ButtonAttribute[] buttonList, bool hasFilters, WimControlBuilder builder);
        string RightSideNavigation(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Framework.ContentListItem.ButtonAttribute[] buttonList);
        string RightSideNavigation(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, Framework.ContentListItem.ButtonAttribute[] buttonList, WimControlBuilder builder);
        string TopNavigation(Sushi.Mediakiwi.Beta.GeneratedCms.Console container);
        string GetUrl(Sushi.Mediakiwi.Beta.GeneratedCms.Console container, IMenuItemView entity, int channel);
    }
}
