using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.Data.Test
{
    /// <summary>
    /// Represents a Sublist entity.
    /// </summary>
    public class Sublist : ComponentListTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sublist"/> class.
        /// </summary>
        public Sublist()
        {
            this.ListSearch += new Sushi.Mediakiwi.Framework.ComponentSearchEventHandler(AllElement_ListSearch);
        }

        void AllElement_ListSearch(object sender, ComponentListSearchEventArgs e)
        {
            wim.ListDataColumns.Add("Value", "Value", ListDataColumnType.UniqueIdentifierPresent);
            wim.ListDataColumns.Add("Text", "Text", ListDataColumnType.HighlightPresent);

            ListItem li = new ListItem("TEXT", "1.2.3");
            List<ListItem> list = new List<ListItem>();

            list.Add(li);

            wim.ListData = list;
        }
    }
}
