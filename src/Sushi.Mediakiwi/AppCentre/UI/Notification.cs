using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// Represents the UI to display Notifications stored using SQL.
    /// </summary>
    public class Notification : BaseImplementation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Notification"/> class.
        /// </summary>
        public Notification()
        {
            FilterSelection = 1;            

            ListSearch += Notification_ListSearch;
            ListLoad += Notification_ListLoad;
            ListAction += Notification_ListAction;
        }

        private readonly Mediakiwi.Data.Repositories.Sql.NotificationRepository _repository = new Mediakiwi.Data.Repositories.Sql.NotificationRepository();

        async Task Notification_ListAction(ComponentActionEventArgs e)
        {
            await _repository.DeleteAllAsync(FilterGroup);
            Response.Redirect(wim.Console.WimPagePath);
        }

        /// <summary>
        /// Gets or sets a value indicating whether [clean notification].
        /// </summary>
        /// <value><c>true</c> if [clean notification]; otherwise, <c>false</c>.</value>
        [OnlyVisibleWhenTrue(nameof(CanClear))]
        [Framework.ContentListSearchItem.Button("Clear log", false, false, 0)]
        public bool CleanNotification { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can clear.
        /// </summary>
        /// <value><c>true</c> if this instance can clear; otherwise, <c>false</c>.</value>
        public bool CanClear
        {
            get
            {
                if (!string.IsNullOrEmpty(Request.Query["q"]) || !(Request.HasFormContentType && string.IsNullOrEmpty(Request.Form["FilterGroup"])))
                {
                    return true;
                }
                return false;
            }
        }

        async Task Notification_ListLoad(ComponentListEventArgs e)
        {
            if (e.SelectedKey == 0)
            {
                return;
            }

            Mediakiwi.Data.Notification _implement = await _repository.SelectOneAsync(e.SelectedKey).ConfigureAwait(false);
            Date = _implement.Created.ToString("dd-MM-yy hh:mm tt");
            Note = _implement.Text;
            Type = _implement.Group;

            if (_implement.XML == null)
            {
                return;
            }
        }

        async Task Notification_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = false;
            wim.ForceLoad = true;
            wim.SearchViewDashboardMaxLength = 250;

            if (string.IsNullOrEmpty(m_SearchTemplateSite))
            {
                wim.ListDataColumns.Add(new ListDataColumn("Type", nameof(NotificationOverview.Type), ListDataColumnType.HighlightPresent));
                wim.ListDataColumns.Add(new ListDataColumn("", nameof(NotificationOverview.Count)) { ColumnWidth = 50 });
                wim.ListDataColumns.Add(new ListDataColumn("Last notification", nameof(NotificationOverview.Last)) { ColumnWidth = 80, Alignment = Align.Right });

                var results = await NotificationOverview.SelectAllAsync(FilterSelection).ConfigureAwait(false);
                wim.ListDataAdd(results);

                wim.SearchResultItemPassthroughParameterProperty = "Deeplink";
            }
            else
            {
                wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(Mediakiwi.Data.Sql.Notification.ID), ListDataColumnType.UniqueIdentifier));
                wim.ListDataColumns.Add(new ListDataColumn("Notification", nameof(Mediakiwi.Data.Sql.Notification.Text), ListDataColumnType.HighlightPresent));
                wim.ListDataColumns.Add(new ListDataColumn("Created", nameof(Mediakiwi.Data.Sql.Notification.Created), ListDataColumnType.Default) { ContentType = ListDataContentType.ItemSelect });

                var results = await _repository.SelectAllAsync(FilterGroup, FilterSelection, null).ConfigureAwait(false);
                wim.ListDataAdd(results);
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
                if (m_Collection == null)
                {
                    m_Collection = new ListItemCollection();
                    m_Collection.Add(new ListItem(""));

                    foreach (var group in NotificationOverview.SelectAll(FilterSelection))
                    {
                        m_Collection.Add(new ListItem(group.Type));
                    }
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
                if (m_TypeCollection == null)
                {
                    m_TypeCollection = new ListItemCollection();
                    m_TypeCollection.Add(new ListItem("Error", "1"));
                    m_TypeCollection.Add(new ListItem("Warning", "2"));
                    m_TypeCollection.Add(new ListItem("Information", "3"));
                }
                return m_TypeCollection;
            }
        }

        /// <summary>
        /// Gets or sets the filter selection.
        /// </summary>
        /// <value>The filter selection.</value>
        [Framework.ContentListSearchItem.Choice_Radio("Selection", nameof(TypeCollection), "Sel", false, true)]
        public int FilterSelection { get; set; }

        private string m_SearchTemplateSite;
        /// <summary>
        /// Gets or sets the search template site.
        /// </summary>
        /// <value>The search template site.</value>
        [Framework.ContentListSearchItem.Choice_Dropdown("Group", nameof(Collection), false, true)]
        public string FilterGroup
        {
            get
            {
                if (!IsPostBack && !string.IsNullOrEmpty(Request.Query["q"]))
                {
                    m_SearchTemplateSite = Request.Query["q"];
                }
                return m_SearchTemplateSite;
            }
            set { m_SearchTemplateSite = value; }
        }


        #region List attributes

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        [Framework.ContentListItem.TextLine("Date")]
        public string Date { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [Framework.ContentListItem.TextLine("Type")]
        public string Type { get; set; }


        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        /// <value>The note.</value>
        [Framework.ContentListItem.TextLine("Notification")]
        public string Note { get; set; }

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
