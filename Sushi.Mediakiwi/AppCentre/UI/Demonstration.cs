using Sushi.Mediakiwi.AppCentre.UI.Forms;
using Sushi.Mediakiwi.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.AppCentre.UI
{
    public class Demonstration : ComponentListTemplate
    {
        DemonstrationForm Implement { get; set; }

        public Demonstration()
        {
            wim.CanContainSingleInstancePerDefinedList = true;

            this.ListDelete += this.Demonstration_ListDelete;
            this.ListLoad += this.Demonstration_ListLoad;
            this.ListSave += this.Demonstration_ListSave;
        }

        private void Demonstration_ListDelete(IComponentListTemplate sender, ComponentListEventArgs e)
        {
        }

        private void Demonstration_ListSave(IComponentListTemplate sender, ComponentListEventArgs e)
        {
            if (Implement.Upload != null)
            {
                var file = Implement.Upload.ReadToEnd();
                wim.Notification.AddNotificationAlert(file, true);
            }
            wim.FlushCache();

            if (!string.IsNullOrWhiteSpace(TitleTest))
                wim.Notification.AddNotificationAlert($"Saved [Title]: {this.TitleTest}", true);
        }

        private void Demonstration_ListLoad(IComponentListTemplate sender, ComponentListEventArgs e)
        {
            Implement = new DemonstrationForm(wim.IsLayerMode);

            Map<Demonstration>(x => x.TitleTest, this).TextField("Title");
            Map<DemonstrationForm>(x => x.OuterTextField, Implement).TextField("Title (outer)");

            //SenderInstance = this;
            this.FormMaps.Add(this);
            this.FormMaps.Add(Implement);
        }

        public string TitleTest { get; set; }
    }
}
