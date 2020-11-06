using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Sushi.Mediakiwi.AppCentre.UI.Forms
{
    public class DemonstrationForm : FormMap<DemonstrationForm>
    {
        public DemonstrationForm()
        {
            Load(this);

            Map(x => x.TextField).TextField("TextField", 50, true, true, "Interactive help");
            Map(x => x.TextArea).TextArea("TextArea", 500, true, "Interactive help");
            Map(x => x.Richtext).RichText("Richtext", 500, true, "Interactive help");

            Map(x => x.Sourcecode).TextArea("Sourcecode", 500, true, "Interactive help", null, true);
            Map(x => x.TextLine).TextLine("TextLine", false, "Interactive help");
            Map(x => x.Section).Section("Section", false, true, true);

            Map(x => x.Date).Date("Date", true, "Interactive help").Expression(OutputExpression.Alternating);
            Map(x => x.DateTime).DateTime("DateTime", true, false, "Interactive help").Expression(OutputExpression.Alternating);

            List = new ListItemCollection();
            List.Add(new ListItem("A"));
            List.Add(new ListItem("B"));
            List.Add(new ListItem("C"));

            Map(x => x.Radio).Radio("Radio", "List", "Radio", true, true, "Interactive help").Expression(OutputExpression.Alternating); ;
            Map(x => x.Checkbox).Checkbox("Checkbox", true, "Interactive help").Expression(OutputExpression.Alternating);
            Map(x => x.Dropdown1).Dropdown("Dropdown", "List", true, true, false, "Interactive help");
            Map(x => x.Dropdown2).Dropdown("Dropdown-multi", "List", true, false, true, "Interactive help");
            Map(x => x.Tagging).Tagging("Tagging", "List", true, false, "Interactive help");
            Map(x => x.SubListSelect).SubListSelect("SubListSelect", typeof(Sushi.Mediakiwi.AppCentre.Data.Implementation.ComponentList), true, false, "Interactive help");

            Map(x => x.MultiField).MultiField("MultiField", "Interactive help");

            Map(x => x.Image).Image("Image", true, null, "Interactive help");
            Map(x => x.Document).Document("Document", true, null, "Interactive help");
            Map(x => x.PageSelect).PageSelect("PageSelect", true, "Interactive help");
            Map(x => x.FolderSelect).FolderSelect("FolderSelect", true, Mediakiwi.Data.FolderType.Administration_Or_List, "Interactive help");

            Map(x => x.ListItemSelect).ListItemSelect("ListItemSelect", "List", true, "Interactive help");

            Map(x => x.Hyperlink).Hyperlink("Hyperlink", true, "Interactive help");
            Map(x => x.HtmlContainer).HtmlContainer(true);
            HtmlContainer = "<b>HtmlContainer</b>";

            Map(x => x.PageContainer).PageContainer();

            Map(x => x.Button1).Button("Button BL", true, true, true, ButtonTarget.BottomLeft, false);
            Map(x => x.Button1).Button("Button BR", true, true, true, ButtonTarget.BottomRight, false);
            Map(x => x.Button1).Button("Button TL", true, true, true, ButtonTarget.TopLeft, false);
            Map(x => x.Button1).Button("Button TR", true, true, true, ButtonTarget.TopRight, false);

        }
        public Mediakiwi.Data.Page PageContainer { get; set; }
        public string HtmlContainer { get; set; }
        public int Hyperlink { get; set; }
        public int Image { get; set; }
        public int Document { get; set; }
        public int PageSelect { get; set; }
        public int FolderSelect { get; set; }
        public string MultiField { get; set; }
        public string ListItemSelect { get; set; }
        public ListItemCollection List { get; set; }
        public bool Checkbox { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateTime { get; set; }
        public string Radio { get; set; }
        public string Dropdown1 { get; set; }
        public string Dropdown2 { get; set; }
        public string Tagging { get; set; }

        public Mediakiwi.Data.SubList SubListSelect { get; set; }

        public string TextField { get; set; }
        public string TextArea { get; set; }
        public string Richtext { get; set; }
        public string Sourcecode { get; set; }
        public string TextLine { get; set; } = "Information assigned to textline";
        public string Section { get; set; }
        public bool Button1 { get; set; }
        public bool Button2 { get; set; }
        public bool Button3 { get; set; }
        public bool Button4 { get; set; }
    }
}
