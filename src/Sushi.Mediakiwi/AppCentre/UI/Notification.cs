using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System.IO;
using System.Text;
using System.Threading.Tasks;
//using ColorCode;
using System.Xml;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{


    /// <summary>
    /// 
    /// </summary>
    public class Notification : BaseImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Notification"/> class.
        /// </summary>
        public Notification()
        {
            this.FilterSelection = 1;
            //ShowInFullWidthMode = true;

            this.ListSearch += Notification_ListSearch;
            this.ListLoad += Notification_ListLoad;
            this.ListAction += Notification_ListAction;
        }

        async Task Notification_ListAction(ComponentActionEventArgs e)
        {
            await Mediakiwi.Data.Notification.DeleteAllAsync(this.FilterGroup);
            Response.Redirect(wim.Console.GetSafeUrl());
        }

        /// <summary>
        /// Gets or sets a value indicating whether [clean notification].
        /// </summary>
        /// <value><c>true</c> if [clean notification]; otherwise, <c>false</c>.</value>
        [OnlyVisibleWhenTrue("CanClear")]
        [Framework.ContentListSearchItem.Button("Clear log", false, false, 0)]
        public bool CleanNotification { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can clear.
        /// </summary>
        /// <value><c>true</c> if this instance can clear; otherwise, <c>false</c>.</value>
        public bool CanClear {
            get {
                if (!string.IsNullOrEmpty(Request.Query["q"]) || !(Request.HasFormContentType && string.IsNullOrEmpty(Request.Form["FilterGroup"])))
                    return true;
                return false; 
            }
        }

        INotification _implement;

        async Task Notification_ListLoad(ComponentListEventArgs e)
        {
            if (e.SelectedKey == 0)
                return;

            _implement = await Mediakiwi.Data.Notification.SelectOneAsync(e.SelectedKey);
            m_Date = _implement.Created.ToString("dd-MM-yy hh:mm tt");
            m_Note = _implement.Text;
            m_Type = _implement.Group;

            if (_implement.XML == null)
                return;
        }

        /// <summary>
        /// Formats the XML.
        /// </summary>
        /// <param name="inputXml">The input XML.</param>
        /// <returns></returns>
        string FormatXml(string inputXml)
        {
            var document = new XmlDocument();
            document.Load(new StringReader(inputXml));

            var builder = new StringBuilder();

            using (var textWriter = new StringWriter(builder))
            using (var xmlWriter = new XmlTextWriter(textWriter))
            {
                xmlWriter.Formatting = Formatting.Indented;
                document.Save(xmlWriter);
            }

            return builder.ToString();
        }

        async Task Notification_ListSearch(ComponentListSearchEventArgs e)
        {
            //HighlightColumn = 0;
            wim.CanAddNewItem = false;
            wim.ForceLoad = true;
            wim.SearchViewDashboardMaxLength = 250;
            //ListColumns = new string[] { "Notification", "Date" };

            if (string.IsNullOrEmpty(m_SearchTemplateSite))
            {
                wim.ListDataColumns.Add("Type", "Type", ListDataColumnType.HighlightPresent);
                wim.ListDataColumns.Add("", "Count", 50);
                wim.ListDataColumns.Add("Last notification", "Last", 80, Align.Right);
                wim.ListDataAdd(NotificationOverview.SelectAll(FilterSelection));
                wim.SearchResultItemPassthroughParameterProperty = "Deeplink";
            }
            else
            {
                wim.ListDataColumns.Add("ID", "ID", ListDataColumnType.UniqueIdentifier);
                wim.ListDataColumns.Add("Notification", "Text", ListDataColumnType.HighlightPresent);
                wim.ListDataColumns.Add("Created", "Created", ListDataColumnType.Default, ListDataContentType.ItemSelect);
                
                int page = Utility.ConvertToInt(Context.Request.Query["set"], 0) - 1;
                int step = wim.CurrentList.Option_Search_MaxResultPerPage;
                wim.ListDataAdd(await Mediakiwi.Data.Notification.SelectAllAsync(this.FilterGroup, this.FilterSelection, null));
            }
        }



        private ListItemCollection m_Collection;
        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <value>The collection.</value>
        public ListItemCollection Collection
        {
            get
            {
                if (m_Collection != null) return m_Collection;

                m_Collection = new ListItemCollection();
                m_Collection.Add(new ListItem(""));

                foreach (var group in NotificationOverview.SelectAll(FilterSelection))
                {
                    m_Collection.Add(new ListItem(group.Type));
                }
                return m_Collection;
            }
        }

        ListItemCollection m_TypeCollection;
        /// <summary>
        /// Gets the type collection.
        /// </summary>
        /// <value>The type collection.</value>
        public ListItemCollection TypeCollection
        {
            get
            {
                if (m_TypeCollection != null) return m_TypeCollection;

                m_TypeCollection = new ListItemCollection();
                m_TypeCollection.Add(new ListItem("Error", "1"));
                m_TypeCollection.Add(new ListItem("Warning", "2"));
                m_TypeCollection.Add(new ListItem("Information", "3"));
                return m_TypeCollection;
            }
        }

        /// <summary>
        /// Gets or sets the filter selection.
        /// </summary>
        /// <value>The filter selection.</value>
        [Framework.ContentListSearchItem.Choice_Radio("Selection", "TypeCollection", "Sel", false, true)]
        public int FilterSelection { get; set; }

        private string m_SearchTemplateSite;
        /// <summary>
        /// Gets or sets the search template site.
        /// </summary>
        /// <value>The search template site.</value>
        [Framework.ContentListSearchItem.Choice_Dropdown("Group", "Collection", false, true)]
        public string FilterGroup
        {
            get {
                if (!IsPostBack && !string.IsNullOrEmpty(Request.Query["q"]))
                    m_SearchTemplateSite = Request.Query["q"];
                return m_SearchTemplateSite; 
            }
            set { m_SearchTemplateSite = value; }
        }


        #region List attributes
        private string m_Date;
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        [Framework.ContentListItem.TextLine("Date")]
        public string Date
        {
            get { return m_Date; }
            set { m_Date = value; }
        }

        private string m_Type;
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [Framework.ContentListItem.TextLine("Type")]
        public string Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }


        private string m_Note;
        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        /// <value>The note.</value>
        [Framework.ContentListItem.TextLine("Notification")]
        public string Note
        {
            get { return m_Note; }
            set { m_Note = value; }
        }

        /// <summary>
        /// Gets or sets the XML.
        /// </summary>
        /// <value>
        /// The XML.
        /// </value>
        [Framework.ContentListItem.HtmlContainer(true)]
        public string XML { get; set; }
        #endregion List attributes
    }
}
