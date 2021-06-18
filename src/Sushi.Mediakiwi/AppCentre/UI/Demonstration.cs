using Sushi.Mediakiwi.AppCentre.UI.Forms;
using Sushi.Mediakiwi.Framework;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.UI
{
    public class Demonstration : ComponentListTemplate
    {
        [Framework.ContentSettingItem.TextField("Setting", 50)]
        public string Backend { get; set; }

        public string Backend2 { get; set; }

        DemonstrationForm Implement { get; set; }

        public Demonstration()
        {
            wim.CanContainSingleInstancePerDefinedList = true;

            this.ListLoad += Demonstration_ListLoad;
            this.ListSave += Demonstration_ListSave;
            this.ListAction += Demonstration_ListAction;
            this.ListConfigure += Demonstration_ListConfigure;
        }

        private Task Demonstration_ListConfigure()
        {
            this.SenderInstance = this;

            Map(x => x.Backend, this).TextField("Backend detais");
            Map(x => x.Backend2, this).TextField("Backend2");
            this.FormMaps.Add(this);
            return Task.CompletedTask;
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

            Map(x => x.TitleTest, this).TextField("Title");
            Map(x => x.OuterTextField, Implement).TextField("Title (outer)");
            

            this.FormMaps.Add(this);
            this.FormMaps.Add(Implement);

            return Task.CompletedTask;
        }

        public string TitleTest { get; set; }
    }
}
