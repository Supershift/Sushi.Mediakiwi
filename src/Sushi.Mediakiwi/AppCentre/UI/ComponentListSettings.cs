﻿using System;
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

                ComponentListInstance.Settings[item.Name].Apply(item.SenderInstance);
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
                ComponentListInstance.Settings.ApplyObject(item.Name, value);
            }
            await ComponentListInstance.SaveAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Handles the ListLoad event of the ComponentListSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sushi.Mediakiwi.Framework.ComponentListEventArgs"/> instance containing the event data.</param>
        async Task ComponentListSettings_ListLoad(ComponentListEventArgs e)
        {
            ComponentListInstance = await Sushi.Mediakiwi.Data.ComponentList.SelectOneAsync(e.SelectedGroupItemKey).ConfigureAwait(false);
            Implement = Utils.CreateInstance(ComponentListInstance) as ComponentListTemplate;
            Implement.SenderInstance = Implement;

            await Implement.OnListConfigure().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets or sets the implement.
        /// </summary>
        /// <value>The implement.</value>
        [Sushi.Mediakiwi.Framework.ContentSettingItem.DataExtend()]
        public ComponentListTemplate Implement { get; set; }

        /// <summary>
        /// Gets or sets the componenent list instance.
        /// </summary>
        /// <value>The componenent list instance.</value>
        public Sushi.Mediakiwi.Data.IComponentList ComponentListInstance { get; set; }

    }
}