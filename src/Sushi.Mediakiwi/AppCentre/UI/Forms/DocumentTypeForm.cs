using Sushi.Mediakiwi.Data.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.AppCentre.UI.Forms
{
    public class DocumentTypeForm : FormMap<DocumentType>
    {
        public DocumentTypeForm()
        {

        }

        string _culture;
        public ListItemCollection TypeOptions { get; set; }

        public DocumentTypeForm(WimComponentListRoot wim, DocumentType implement)
        {
            TypeOptions = new ListItemCollection();
            TypeOptions.Add(new ListItem("Page", "1"));
            TypeOptions.Add(new ListItem("Component", "2"));

            Load(implement);
            _culture = wim.CurrentApplicationUser.LanguageCulture;

            Map(x => x.ID).TextLine(""); // need to know the ID in JS
            Map(x => x.Name).TextField("Name", 2048, false).Expression(OutputExpression.FullWidth);
            Map(x => x.Type).Dropdown("Type", nameof(TypeOptions)).Expression(OutputExpression.FullWidth);
        }

        public string Translate(string nl, string en)
        {
            if (_culture.StartsWith("nl"))
                return nl;
            return en;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public int DocumentType { get; set; }
    }
}
