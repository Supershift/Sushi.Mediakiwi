using Sushi.MailTemplate.Data;
using Sushi.Mediakiwi.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.MailTemplate.MediaKiwi.UI
{
    /// <summary>
    /// DefaultValues_List Component List
    /// </summary>
    public class DefaultValues_List : ComponentListTemplate
    {
        private readonly MailTemplateRepository _mailTemplateRepository;
        private readonly DefaultValuePlaceholderRepository _defaultValuePlaceholderRepository;

        /// <summary>
        /// DefaultValues_List ctor
        /// </summary>
        public DefaultValues_List(MailTemplateRepository mailTemplateRepository, Data.DefaultValuePlaceholderRepository defaultValuePlaceholderRepository)
        {
            // additional styles for border around the ListDataEditConfiguration textbox
            //wim.Page.Head.AddStyle("/css/mediakiwiStyles.css", true);

            wim.OpenInEditMode = true;
            wim.CanContainSingleInstancePerDefinedList = true;

            ListSave += DefaultValues_List_ListSave;
            ListSearch += DefaultValues_List_ListSearch;
            _mailTemplateRepository = mailTemplateRepository;
            _defaultValuePlaceholderRepository = defaultValuePlaceholderRepository;
        }

        private async Task DefaultValues_List_ListSearch(ComponentListSearchEventArgs e)
        {
            if (!wim.IsSearchListMode)
            {
                var idQueryString = Request.Query["item"].ToString();

                int.TryParse(idQueryString, out int id);

                var mailTemplate = await _mailTemplateRepository.FetchSingleAsync(id);

                var list = await _defaultValuePlaceholderRepository.FetchAllByMailTemplateAsync(mailTemplate.Identifier);

                var subjectPlaceholders = Logic.PlaceholderLogic.GetPlaceholderTags(mailTemplate.Subject);
                var bodyPlaceholders = Logic.PlaceholderLogic.GetPlaceholderTags(mailTemplate.Body);

                var tmpID = -1;

                list.AddRange(subjectPlaceholders.Where(x => list.All(defaultValue => defaultValue.Placeholder != x)).Select(x => new Data.DefaultValuePlaceholder { Placeholder = x, ID = tmpID-- }));
                list.AddRange(bodyPlaceholders.Where(x => list.All(defaultValue => defaultValue.Placeholder != x)).Select(x => new Data.DefaultValuePlaceholder { Placeholder = x, ID = tmpID-- }));

                wim.SearchResultItemPassthroughParameterProperty = nameof(Data.DefaultValuePlaceholder.ID);

                wim.ListDataColumns.Add(new ListDataColumn("ID", nameof(Data.DefaultValuePlaceholder.ID), ListDataColumnType.UniqueHighlightedIdentifier));
                wim.ListDataColumns.Add(new ListDataColumn("Placeholder", nameof(Data.DefaultValuePlaceholder.Placeholder)));
                wim.ListDataColumns.Add(new ListDataColumn("Value", nameof(Data.DefaultValuePlaceholder.Value))
                {
                    Alignment = Align.Left,
                    Type = ListDataColumnType.Default,
                    EditConfiguration = new ListDataEditConfiguration()
                    {
                        Type = ListDataEditConfigurationType.TextField,
                        PropertyToSet = nameof(Data.DefaultValuePlaceholder.Value),
                        Width = 580
                    },
                });

                wim.SearchListCanClickThrough = false;
                wim.ListDataAdd(list);
            }
        }

        private async Task DefaultValues_List_ListSave(ComponentListEventArgs e)
        {
            if (wim.ChangedSearchGridItem != null)
            {
                var idQueryString = Request.Query["item"].ToString();

                int.TryParse(idQueryString, out int id);

                Data.MailTemplate mailTemplate = null;

                foreach (var item in wim.ChangedSearchGridItem)
                {
                    var defaultValue = item as Data.DefaultValuePlaceholder;
                    if (defaultValue != null)
                    {
                        if (defaultValue.ID < 0 && !string.IsNullOrEmpty(defaultValue.Value))
                        {
                            mailTemplate ??= await _mailTemplateRepository.FetchSingleAsync(id);

                            // create
                            defaultValue.ID = 0;
                            defaultValue.MailTemplateID = mailTemplate.ID;
                            await _defaultValuePlaceholderRepository.SaveAsync(defaultValue);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(defaultValue.Value))
                            {
                                // delete
                                await _defaultValuePlaceholderRepository.DeleteAsync(defaultValue);
                            }
                            else
                            {
                                // edit
                                await _defaultValuePlaceholderRepository.SaveAsync(defaultValue);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Explanation of this window
        /// </summary>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("")]
        public string Title
        {
            get
            {
                return @"Use this page to fill the default values of placeholders in this template. 
A default placeholder value will be used when there is no value provided.";
            }
        }

        /// <summary>
        /// DataList with the default values
        /// </summary>
        [Mediakiwi.Framework.ContentListItem.DataList()]
        public Mediakiwi.Data.DataList DefaultValuesList { get; set; }
    }
}
