using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// All logic concerning site inheritance
    /// </summary>
    internal class Inheritance
    {
        /// <summary>
        /// Creates the inherited site.
        /// </summary>
        /// <param name="currentUserKey">The current user key.</param>
        /// <param name="parentSiteKey">The parent site key.</param>
        /// <param name="childSiteKey">The child site key.</param>
        internal static void CreateInheritedSite(int currentUserKey, int parentSiteKey, int childSiteKey)
        {
            Sushi.Mediakiwi.Data.Site childSite = Sushi.Mediakiwi.Data.Site.SelectOne(childSiteKey, true);

            foreach (Sushi.Mediakiwi.Data.Page page in Sushi.Mediakiwi.Data.Page.SelectAllBySite(parentSiteKey))
            {
                //Sushi.Mediakiwi.Data.Page.Inheritance.CreatePageInChild(childSite, currentUserKey, parentSiteKey, page, page.IsFixed, false);
            }
        }
    }
}
