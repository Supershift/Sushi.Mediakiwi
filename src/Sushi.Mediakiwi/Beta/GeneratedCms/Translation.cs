using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Beta.GeneratedCms
{
    /// <summary>
    /// Represents a Translation entity.
    /// </summary>
    public class Translation
    {
        /// <summary>
        /// Gets the list transation.
        /// </summary>
        /// <param name="listType">Type of the list.</param>
        /// <returns></returns>
        public string GetListTransation(Sushi.Mediakiwi.Data.ComponentListType listType)
        {
            return GetEnglishListTransation((int)listType);

        }

        /// <summary>
        /// Gets the english list transation.
        /// </summary>
        /// <param name="listType">Type of the list.</param>
        /// <returns></returns>
        string GetEnglishListTransation(int listType)
        {
            switch (listType)
            {
                case 1: return "Users";
                case 2: return "Roles";
                case 3: return "Channels";
                case 4: return "Images";
                case 5: return "Links";
                case 6: return "Documents";
                case 7: return "??";
                case 8: return "Component templates";
                case 9: return "Component lists";
                case 10: return "Page templates";
                case 11: return "Notifications";
                case 12: return "Galleries";
                case 13: return "Portals";
                case 14: return "Tasks";
                case 15: return "???";
                case 16: return "Task inbox";
                case 17: return "Task outbox";
                case 18: return "Catalogs (obsolete)";
                case 19: return "Products (obsolete)";
                case 20: return "Structures (obsolete)";
                case 21: return "Folder";
                case 22: return "Browsing";
            }
            return "Unkown list";
        }

    }
}
