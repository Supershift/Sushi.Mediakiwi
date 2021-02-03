using System;
using System.Data;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Parsers
{
    public partial class ComponentTargetPageParser : IComponentTargetPageParser
    {
        static ISqlEntityParser _DataParser;
        static ISqlEntityParser DataParser
        {
            get
            {
                if (_DataParser == null)
                    _DataParser = Environment.GetInstance<ISqlEntityParser>();
                return _DataParser;
            }
        }

        public virtual IComponentTargetPage[] SelectAll(int templateID, int siteID)
        {
            #region Query
            // [CB: 3-1-2016] aan de name een isNull toegevoegd... anders crashde het. Je kan namelijk niet van uit gaan dat
            //                  er een wim_componentVersion is en de name niet nullable zetten

            string sql = @"
    select
        (select COUNT(*) from wim_Components where ComponentVersion_GUID = component_GUID) PublishedCount, 
        isNull((select top 1 componentVersion_Name from wim_ComponentVersions where ComponentVersion_GUID = ComponentTarget_Component_Source), '?') Name, 
        *
    from
        wim_ComponentVersions 
        join wim_Pages on ComponentVersion_Page_Key = Page_Key 
        left join wim_ComponentTargets on ComponentTarget_Component_Target = ComponentVersion_GUID 
        join dbo.wim_Folders on Folder_Key = Page_Folder_Key
    where 
        ComponentVersion_ComponentTemplate_Key = @TEMPLATE and Folder_Site_Key = @SITE
    order by 
        Page_CompletePath, ComponentVersion_SortOrder
";
            #endregion Query

            DataRequest data = new DataRequest();
            data.AddParam("TEMPLATE", templateID, SqlDbType.Int);
            data.AddParam("SITE", siteID, SqlDbType.Int);
            var result = DataParser.ExecuteList<IComponentTargetPage>(sql, data).ToArray();

            int p = 0;
            int x = 1;
            foreach (var item in result)
            {
                if (p != item.PageID)
                {
                    x = 1;
                    p = item.PageID;
                }
                else
                    x++;

                item.Position = x;
            }

            return result;
        }
    }
}