using Sushi.Mediakiwi.AppCentre.UI.Forms;
using Sushi.Mediakiwi.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.UI
{
    public class Demonstration : ComponentListTemplate
    {
        [Sushi.Mediakiwi.Framework.ContentSettingItem.TextField("Setting", 50)]
        public string Backend { get; set; }

        DemonstrationForm Implement { get; set; }

        public Demonstration()
        {
            wim.CanContainSingleInstancePerDefinedList = true;

            this.ListLoad += Demonstration_ListLoad;
            this.ListSave += Demonstration_ListSave;
            this.ListAction += Demonstration_ListAction;
        }

        Task Demonstration_ListAction(ComponentActionEventArgs e)
        {
            Response.Redirect("https://www.google.com");
            return Task.CompletedTask;
        }

        Task Demonstration_ListSave(ComponentListEventArgs e)
        {
            if (Implement.Upload != null)
            {
                var file = Implement.Upload.ReadToEnd();
                wim.Notification.AddNotificationAlert(file, true);
            }
            wim.FlushCache();

            if (!string.IsNullOrWhiteSpace(TitleTest))
                wim.Notification.AddNotificationAlert($"Saved [Title]: {this.TitleTest}", true);

            return Task.CompletedTask;
        }

        Task Demonstration_ListLoad(ComponentListEventArgs e)
        {
            Implement = new DemonstrationForm(wim.IsLayerMode);

            Map<Demonstration>(x => x.TitleTest, this).TextField("Title");
            Map<DemonstrationForm>(x => x.OuterTextField, Implement).TextField("Title (outer)");

            this.FormMaps.Add(this);
            this.FormMaps.Add(Implement);

            return Task.CompletedTask;
        }

        public string TitleTest { get; set; }
    }
}
