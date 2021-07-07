using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;

namespace Sushi.Mediakiwi.AppCentre.UI.Forms
{
    public class PageMappingForm : FormMap<IPageMapping>
    {
        public PageMappingForm(IPageMapping implement)
        {
            Load(implement);

            Map(x => x.Path).TextField("URL", 150, true, false, "relative path that can end with [*] for a wildcard");
            Map(x => x.TargetTypeID).Radio("Page or document", nameof(TargetTypes), "uxTargetType", true, true);
            Map(x => x.MappingTypeID).Dropdown("Type", nameof(MappingTypes), true).Show(implement.TargetTypeID == (int)PageMappingTargetType.PAGE);
            Map(x => x.PageID).PageSelect("Internal page", true).Show(implement.TargetTypeID == (int)PageMappingTargetType.PAGE);
            Map(x => x.Query).TextField("Querystring", 50, false, false, "Use this for a specific item like ?productID=1").Show(implement.TargetTypeID == (int)PageMappingTargetType.PAGE);
            Map(x => x.Expression).TextField("Expression", 200, false, false, "Can be used to replace a file extension<br/>(if URL starts with a '.' this expression will be used as a replacement for that extension)").Show(implement.TargetTypeID == (int)PageMappingTargetType.PAGE);
            Map(x => x.AssetID).Document("Internal document", true).Show(implement.TargetTypeID == (int)PageMappingTargetType.FILE);
            Map(x => x.Title).TextField("Browser title", 150, false);
            Map(x => x.IsActive).Checkbox("Active");
        }

    
        private static ListItemCollection m_MappingTypes;
        public static ListItemCollection MappingTypes
        {
            get
            {
                if (m_MappingTypes == null)
                {
                    m_MappingTypes = new ListItemCollection();
                    m_MappingTypes.Add(new ListItem("Rewrite (200)", $"{(int)PageMappingType.Rewrite200}"));
                    m_MappingTypes.Add(new ListItem("Temporary redirect (302)", $"{(int)PageMappingType.Redirect302}"));
                    m_MappingTypes.Add(new ListItem("Permanent redirect (301)", $"{(int)PageMappingType.Redirect301}"));
                    m_MappingTypes.Add(new ListItem("Not found (404)", $"{(int)PageMappingType.NotFound404}"));
                }

                return m_MappingTypes;
            }
        }

        private static ListItemCollection m_TargetTypes;
        public static ListItemCollection TargetTypes
        {
            get
            {
                if (m_TargetTypes == null)
                {
                    m_TargetTypes = new ListItemCollection();
                    m_TargetTypes.Add(new ListItem("Page", $"{(int)PageMappingTargetType.PAGE}"));
                    m_TargetTypes.Add(new ListItem("File", $"{(int)PageMappingTargetType.FILE}"));
                }

                return m_TargetTypes;
            }
        }
    }
}
