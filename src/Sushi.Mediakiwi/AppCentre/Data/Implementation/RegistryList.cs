using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class RegistryList : BaseImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryList"/> class.
        /// </summary>
        public RegistryList()
        {
            //wim.CanAddNewItem = false;

            this.ListDelete += RegistryList_ListDelete;
            this.ListLoad += new ComponentListEventHandler(RegistryList_ListLoad);
            this.ListSearch += new ComponentSearchEventHandler(RegistryList_ListSearch);
            this.ListSave += new ComponentListEventHandler(RegistryList_ListSave);
        }

        void RegistryList_ListDelete(object sender, ComponentListEventArgs e)
        {
            Implement.Delete();
        }

        void RegistryList_ListLoad(object sender, ComponentListEventArgs e)
        {
            Implement = Sushi.Mediakiwi.Data.Registry.SelectOne(e.SelectedKey);
        }

        void RegistryList_ListSave(object sender, ComponentListEventArgs e)
        {
            Implement.Save();
            
            if (Implement.Name == "SPACE_REPLACEMENT")
            {
                Sushi.Mediakiwi.Framework.Functions.FolderPath.UpdateCompletePath();
            }
            wim.FlushCache();
        }

        void RegistryList_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
            wim.ListDataColumns.Add("Name", "NameDescription");
            wim.ListDataColumns.Add("Value", "Value");

            wim.ListData = Sushi.Mediakiwi.Data.Registry.SelectAll();
        }

        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.DataExtend()]
        public Sushi.Mediakiwi.Data.IRegistry Implement { get; set; }
    }
}
