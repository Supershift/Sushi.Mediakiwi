using Microsoft.Extensions.DependencyInjection;
using Sushi.Mediakiwi.AppCentre.Data.Implementation;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data.Elastic.UI
{
    public class NotificationList : BaseImplementation
    {
        private readonly Repositories.NotificationRepository _repository;

        public NotificationList(Repositories.NotificationRepository repository)
        {
            _repository = repository;

            ListSearch += NotificationList_ListSearch;
            ListLoad += NotificationList_ListLoad;            
        }

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

        private async Task NotificationList_ListLoad(ComponentListEventArgs arg)
        {
            // todo: get index from url
            var notification = await _repository.GetOneAsync(Request.Query["index"], Request.Query["id"]);
            Date = notification.Timestamp.ToString();
            Type = notification.Group;
            Note = notification.Text;
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
                    m_TypeCollection.Add(new ListItem("", "0"));
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
        [Framework.ContentListSearchItem.Choice_Dropdown("Selection", nameof(TypeCollection), false, true, Expression = OutputExpression.Left)]
        public int FilterSelection { get; set; }
        
        [Framework.ContentListSearchItem.Date("From", false, Expression = OutputExpression.Left)]
        public DateTime? FilterFrom { get; set; }
        
        [Framework.ContentListSearchItem.Date("To", false, Expression = OutputExpression.Right)]
        public DateTime? FilterTo { get; set; }

        async Task NotificationList_ListSearch(ComponentListSearchEventArgs e)
        {
            wim.CanAddNewItem = false;
            wim.ForceLoad = true;
            wim.SearchViewDashboardMaxLength = 250;

            NotificationType? selection = null;
            if (FilterSelection > 0)
                selection = (NotificationType)FilterSelection;

            if (Request.Query.ContainsKey("groupName"))
            {
                wim.ListDataColumns.Add(new ListDataColumn("ID", "ElasticId.Id", ListDataColumnType.Default));
                wim.ListDataColumns.Add(new ListDataColumn("Index", "ElasticId.Index", ListDataColumnType.Default));
                
                wim.ListDataColumns.Add(new ListDataColumn("Notification", nameof(Notification.Text), ListDataColumnType.HighlightPresent));
                wim.ListDataColumns.Add(new ListDataColumn("Created", nameof(Notification.Timestamp), ListDataColumnType.Default) { ContentType = ListDataContentType.ItemSelect });

                var results = await _repository.GetAllAsync(selection, Request.Query["groupName"], FilterFrom, FilterTo, wim.CurrentList.Option_Search_MaxResultPerPage, null);
                wim.ListDataAdd(results);

                
            }
            else
            {
                wim.ListDataColumns.Add(new ListDataColumn("ID", "Count", ListDataColumnType.UniqueIdentifier));
                wim.ListDataColumns.Add(new ListDataColumn("Type", nameof(NotificationGroup.GroupName), ListDataColumnType.HighlightPresent));
                wim.ListDataColumns.Add(new ListDataColumn("Count", nameof(NotificationGroup.Count)) { ColumnWidth = 50 });
                wim.ListDataColumns.Add(new ListDataColumn("Last notification", nameof(NotificationGroup.LastTimestamp)) { ColumnWidth = 80, Alignment = Align.Right });

                var results = await _repository.GetAggregatedGroupsAsync(selection, FilterFrom, FilterTo);


                foreach (var result in results)
                {
                    result.Deeplink = "?groupName=" + WebUtility.UrlEncode(result.GroupName) + "&id"; 
                }

                wim.ListDataAdd(results);

                wim.SearchResultItemPassthroughParameterProperty = "Deeplink";
            }
        }
    }
}
