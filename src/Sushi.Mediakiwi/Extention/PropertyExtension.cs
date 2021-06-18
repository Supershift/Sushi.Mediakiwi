using Sushi.Mediakiwi.Data;

public static class PropertyExtension
{

    /// <summary>
    /// Lists the select options.
    /// </summary>
    /// <param name="currentSite">The current site.</param>
    /// <returns></returns>
    //public static ListItemCollection ListSelectOptions(this Property inProperty, Site currentSite)
    //{
    //    ListItemCollection m_ListItemCollection = new ListItemCollection();


    //    if (inProperty.ListSelect.HasValue)
    //    {
    //        IComponentList list = ComponentList.SelectOne(inProperty.ListSelect.Value);
    //        m_ListItemCollection = Wim.Utility.GetInstanceListCollection(list, inProperty.ListCollection, currentSite, list.GetInstance());
    //    }
    //    else if (inProperty.OptionListSelect.HasValue)
    //    {
    //        Sushi.Mediakiwi.Framework.iOption options = Wim.Utility.GetInstanceOptions("Wim.Module.FormGenerator.dll", "Wim.Module.FormGenerator.Data.FormElementOptionList");
    //        if (options != null)
    //        {
    //            m_ListItemCollection = new ListItemCollection();
    //            foreach (Sushi.Mediakiwi.Framework.iNameValue nv in options.Options(inProperty.OptionListSelect.Value))
    //            {
    //                m_ListItemCollection.Add(new ListItem(nv.Name, nv.Value));
    //            }
    //        }
    //    }
    //    else
    //    {
    //        m_ListItemCollection = new ListItemCollection();
    //        foreach (PropertyOption option in inProperty.Options)
    //            m_ListItemCollection.Add(new ListItem(option.Name, option.Value.ToString()));
    //    }

    //    return m_ListItemCollection;
    //}



    /// <summary>
    /// Loads the data.
    /// </summary>
    //internal static void LoadData(this Property inProperty)
    //{
    //    if (inProperty.MetaData == null)
    //    {
    //        inProperty.MetaData = Wim.Utility.GetDeserialized(typeof(Sushi.Mediakiwi.Framework.MetaData), inProperty.Data) as Sushi.Mediakiwi.Framework.MetaData;

    //        if (inProperty.MetaData != null)
    //            Wim.Utility.ReflectProperty(inProperty.MetaData, inProperty);
    //    }

    //    if (inProperty.MetaData == null)
    //        inProperty.MetaData = new Sushi.Mediakiwi.Framework.MetaData();
    //}

    ///// <summary>
    ///// Creates the filter.
    ///// </summary>
    //internal static void CreateFilter(this Property inProperty)
    //{
    //    IComponentList list = ComponentList.SelectOne(inProperty.ListID);
    //    try
    //    {
    //        inProperty.Execute(string.Format("select top 1 {0}_{1} from {2}", list.Catalog().ColumnPrefix, inProperty.FieldName, list.Catalog().Table));
    //    }
    //    catch (Exception)
    //    {
    //        //  Set also in CustomDataItem: ParseSqlParameterValue
    //        //  Set also in CreateFilter
    //        //  IsNotFilterOrType

    //        string type = null;
    //        switch ((ContentType)inProperty.TypeID)
    //        {

    //            case ContentType.Date:
    //            case ContentType.DateTime:
    //                type = "DateTime null";
    //                break;
    //            case ContentType.Choice_Checkbox:
    //                type = "bit null";
    //                break;
    //            case ContentType.Binary_Image:
    //            case ContentType.Choice_Radio:
    //            case ContentType.FolderSelect:
    //            case ContentType.PageSelect:
    //            case ContentType.Binary_Document:
    //            case ContentType.Hyperlink:
    //            case ContentType.Choice_Dropdown:
    //                type = "int null";
    //                break;
    //            case ContentType.TextArea:
    //            case ContentType.RichText:
    //            case ContentType.TextField:
    //                int value = Wim.Utility.ConvertToInt(inProperty.MaxValueLength);
    //                if (value > 0 && value < 4000)
    //                    type = string.Format("nvarchar({0}) null", value);
    //                else
    //                    type = "ntext null";
    //                break;
    //            default: return;
    //        }
    //        if (list.Catalog() != null)
    //            inProperty.Execute(string.Format("alter table {0} add {1}_{2} {3}", list.Catalog().Table, list.Catalog().ColumnPrefix, inProperty.FieldName, type));
    //    }
    //}



    //System.Reflection.PropertyInfo info, Sushi.Mediakiwi.Beta.GeneratedCms.Console container, object senderInstance, ref bool isEditable, out bool isVisible
    public static Sushi.Mediakiwi.Framework.IContentInfo GetContentInfo(this Property inProperty)
    {
        Sushi.Mediakiwi.Framework.IContentInfo contentAttribute = null;
        switch (inProperty.ContentTypeID)
        {
            default:
            case ContentType.Binary_Document:
            case ContentType.Binary_Image:
            case ContentType.Choice_Dropdown:
            case ContentType.Choice_Radio:
            case ContentType.FolderSelect:
            case ContentType.PageSelect:
            case ContentType.Choice_Checkbox:
            case ContentType.DateTime:
            case ContentType.Date:
                break;
            case ContentType.TextLine:
                return new Sushi.Mediakiwi.Framework.ContentInfoItem.TextLineAttribute(inProperty.Title);
            case ContentType.TextField:
                return new Sushi.Mediakiwi.Framework.ContentInfoItem.TextFieldAttribute(inProperty.Title,Utility.ConvertToInt(inProperty.MaxValueLength), inProperty.IsMandatory, inProperty.InteractiveHelp);
            case ContentType.TextArea:
                return new Sushi.Mediakiwi.Framework.ContentInfoItem.TextAreaAttribute(inProperty.Title, Utility.ConvertToInt(inProperty.MaxValueLength), inProperty.IsMandatory, inProperty.InteractiveHelp);
            case ContentType.RichText:
                return new Sushi.Mediakiwi.Framework.ContentInfoItem.RichTextAttribute(inProperty.Title, Utility.ConvertToInt(inProperty.MaxValueLength), inProperty.IsMandatory, inProperty.InteractiveHelp);
        }
        return contentAttribute;
    }

    /// <summary>
    /// Applies the filter.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="customData">The custom data.</param>
    /// <param name="itemID">The item ID.</param>
    public static void ApplyFilter(this Property inProperty, IDataFilter filter, Sushi.Mediakiwi.Data.CustomData customData, int itemID)
    {
        switch (inProperty.ContentTypeID)
        {
            default:
                return;

            case ContentType.Binary_Document:
            case ContentType.Binary_Image:
            case ContentType.Choice_Dropdown:
            case ContentType.Choice_Radio:
            case ContentType.FolderSelect:
            case ContentType.PageSelect:
                filter.FilterI = customData[inProperty.FieldName].ParseInt();

                if (!filter.FilterI.HasValue && !filter.IsNewInstance)
                {
                    filter.Delete();
                    return;
                }

                break;

            case ContentType.Choice_Checkbox:
                filter.FilterB = customData[inProperty.FieldName].ParseBoolean();
                break;

            case ContentType.DateTime:
            case ContentType.Date:
                filter.FilterT = customData[inProperty.FieldName].ParseDateTime();

                if (!filter.FilterT.HasValue && !filter.IsNewInstance)
                {
                    filter.Delete();
                    return;
                }

                break;

            case ContentType.TextLine:
            case ContentType.TextField:
            case ContentType.TextArea:
            case ContentType.RichText:

                //if (prop.ContentType == "Decimal")
                //{
                //    filter.FilterD = m_Generic.Data[prop.FieldName].ParseDecimal();
                //}
                //else
                //{
                string tmp = customData[inProperty.FieldName].Value;

                if (!string.IsNullOrEmpty(tmp) && tmp.Length > 255)
                    tmp = tmp.Substring(0, 255);

                filter.FilterC = tmp;
                //}

                if (string.IsNullOrEmpty(filter.FilterC) && !filter.IsNewInstance)
                {
                    filter.Delete();
                    return;
                }

                break;
        }

        filter.PropertyID = inProperty.ID;
        filter.ItemID = itemID;
        filter.Save();
    }



}

