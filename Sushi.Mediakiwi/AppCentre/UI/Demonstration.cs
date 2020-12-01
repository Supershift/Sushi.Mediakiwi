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
            var file = Implement.Upload.ReadToEnd();
            Console.WriteLine(file);

            //var size = file.OpenReadStream


        }

        private void Demonstration_ListLoad(IComponentListTemplate sender, ComponentListEventArgs e)
        {
            Implement = new DemonstrationForm();
            this.FormMaps.Add(Implement);
        }
    }
}
