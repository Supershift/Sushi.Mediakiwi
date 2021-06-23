using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System;
using System.Linq;

namespace Sushi.Mediakiwi.AppCentre.UI.Forms
{
    public class FieldPropertiesForm : FormMap<Property>
    {
        public FieldPropertiesForm()
        {

        }

        string _culture;

        public ListItemCollection TypeOptions { get; set; }
        public ListItemCollection ListOptions { get; set; }

        
        public FieldPropertiesForm(WimComponentListRoot wim, Property implement)
        {
            ListOptions = new ListItemCollection();

            foreach (var list in ComponentList.SelectAll())
            {
                ListOptions.Add(new ListItem(list.Name, list.GUID.ToString()));
            }

            TypeOptions = new ListItemCollection();
            foreach (var value in Enum.GetValues(typeof(ContentType)))
            {
                TypeOptions.Add(new ListItem(((ContentType)value).ToString(), ((int)value).ToString()));
            }

            bool isChoiceType =
                implement.ContentTypeID.Equals(ContentType.Choice_Dropdown) ||
                implement.ContentTypeID.Equals(ContentType.ListItemSelect) ||
                implement.ContentTypeID.Equals(ContentType.Choice_Radio);

            if (isChoiceType)
            {
                var propertydate = PropertyOption.SelectAll(implement.ID);
                implement.Data = string.Join(",", propertydate.Select(x => x.Value));
            }

            bool isSublist = (implement.ContentTypeID.Equals(ContentType.SubListSelect));

            //// Check if we're dealing with a shared field
            if (implement.IsSharedField && string.IsNullOrWhiteSpace(implement.FieldName) == false)
            {
                // Retrieve the shared field (if any)
                var _sharedField = SharedField.FetchSingle(implement.FieldName, implement.ContentTypeID);
                if (_sharedField?.ID > 0)
                {
                    // Retrieve the translated values for this shared Field (if any)
                    var _sharedFieldValues = SharedFieldTranslation.FetchAllForField(_sharedField.ID);
                    var sharedFieldInstance = _sharedFieldValues.FirstOrDefault();
                    if (sharedFieldInstance?.ID > 0)
                    {
                        SharedValue = sharedFieldInstance.Value;
                    }
                }
            }


            Load(implement);

            _culture = wim.CurrentApplicationUser.LanguageCulture;

            Map(x => x.Section).Section("Additional settings");
            Map(x => x.Title).TextField("Title", 255, false).Expression(OutputExpression.FullWidth);
            Map(x => x.ContentTypeID).Dropdown("Type", nameof(TypeOptions), true, true).Expression(OutputExpression.FullWidth);
            Map(x => x.IsMandatory).Checkbox("Required").Expression(OutputExpression.FullWidth);

            Map(x => x.ListCollection).Dropdown("Module", nameof(ListOptions), true).Expression(OutputExpression.FullWidth).Show(isSublist);
            Map(x => x.CanContainOneItem).Checkbox("Only one item").Expression(OutputExpression.FullWidth).Show(isSublist);

            Map(x => x.DefaultValue).TextField("Default value").Expression(OutputExpression.FullWidth).Hide(isSublist);
            Map(x => x.MaxValueLength).TextField("Max length", 4, false).Expression(OutputExpression.FullWidth).Hide(isSublist);
            Map(x => x.Data).Tagging("Options").Expression(OutputExpression.FullWidth).Show(isChoiceType);

            Map(x => x.SortOrder).TextField("SortOrder").Expression(OutputExpression.FullWidth).Hide();
            Map(x => x.InteractiveHelp).TextArea("Help", 512, false).Expression(OutputExpression.FullWidth);
            Map(x => x.FieldName).TextField("Field", 50, false).Expression(OutputExpression.FullWidth).ReadOnly();
            Map(x => x.IsSharedField).Checkbox("Is shared field", true).Expression(OutputExpression.Left).Hide(isSublist);
            Map(x => x.SharedValue, this).TextField("Shared value").Expression(OutputExpression.Right).Hide(isSublist).ReadOnly();
        }

        public string SharedValue { get; set; }

        public string Translate(string nl, string en)
        {
            if (_culture.StartsWith("nl"))
                return nl;
            return en;
        }

        public string Section { get; set; }
    }
}
