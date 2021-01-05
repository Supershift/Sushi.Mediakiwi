using System;
using System.Data;
using System.Configuration;
using System.Web;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class ComponentListSettings : ComponentListTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentListSettings"/> class.
        /// </summary>
        public ComponentListSettings()
        {
            //wim.ShowInFullWidthMode = true;
            wim.CanAddNewItem = false;
            
            this.ListLoad += ComponentListSettings_ListLoad;
            this.ListSave += ComponentListSettings_ListSave;
            this.ListPreRender += ComponentListSettings_ListPreRender;
        }

        Task ComponentListSettings_ListPreRender(ComponentListEventArgs e)
        {
            if (wim.Form.ListItemElementList.Count < 2)
            {
                wim.Notification.AddNotification("This list has no custom settings.");    
            }
            if (wim.Form.ListItemElementList == null || IsPostBack) 
                return Task.CompletedTask;

            foreach (var item in wim.Form.ListItemElementList)
            {
                if (item.ContentAttribute.ContentTypeSelection == ContentType.DataExtend || item.ContentAttribute.ContentTypeSelection == ContentType.ContentContainer)
                    continue;

                ComponenentListInstance.Settings[item.Name].Apply(item.SenderInstance);
            }
            return Task.CompletedTask;
        }

        async Task ComponentListSettings_ListSave(ComponentListEventArgs e)
        {
            if (wim.Form.ListItemElementList.Count < 2) return;
            if (wim.Form.ListItemElementList == null) return;
            
            foreach (var item in wim.Form.ListItemElementList)
            {
                if (item.ContentAttribute.ContentTypeSelection == ContentType.DataExtend || item.ContentAttribute.ContentTypeSelection == ContentType.ContentContainer) 
                    continue;

                object value = item.GetValue();
                ComponenentListInstance.Settings.ApplyObject(item.Name, value);
            }
            await ComponenentListInstance.SaveAsync();
        }

        /// <summary>
        /// Handles the ListLoad event of the ComponentListSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        async Task ComponentListSettings_ListLoad(ComponentListEventArgs e)
        {
            this.ComponenentListInstance = await Sushi.Mediakiwi.Data.ComponentList.SelectOneAsync(e.SelectedGroupItemKey);
            this.Implement = Utils.CreateInstance(ComponenentListInstance);
        }

        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        [Sushi.Mediakiwi.Framework.ContentSettingItem.DataExtend()]
        public object Implement { get; set; }

        /// <summary>
        /// Gets or sets the componenent list instance.
        /// </summary>
        /// <value>The componenent list instance.</value>
        public Sushi.Mediakiwi.Data.IComponentList ComponenentListInstance { get; set; }
        //[Sushi.Mediakiwi.Framework.ContentSettingItem.d.DataList()]
        //public Sushi.Mediakiwi.Data.DataList DataJobList { get; set; }

    }
}
