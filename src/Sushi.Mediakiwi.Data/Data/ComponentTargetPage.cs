using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(ComponentTargetPageMap))]
    public class ComponentTargetPage : IComponentTargetPage
    {
        public class ComponentTargetPageMap : DataMap<ComponentTargetPage>
        {
            public ComponentTargetPageMap()
            {
                Table("wim_ComponentVersions");

                Id(x => x.ID, "ComponentVersion_Key").Identity();

                Map(x => x.PageID, "Page_Key").ReadOnly();
                Map(x => x.Path, "Page_CompletePath").ReadOnly();
                Map(x => x.IsActivePage, "Page_IsPublished").ReadOnly();
                Map(x => x.IsActive, "ComponentVersion_IsActive").ReadOnly();
                Map(x => x.Version_GUID, "ComponentVersion_GUID").ReadOnly();
                Map(x => x.Component_Source, "ComponentTarget_Component_Source").ReadOnly();
                Map(x => x.AssignedComponent, "Name").ReadOnly();
                Map(x => x.PublishedCount, "PublishedCount").ReadOnly();
            }
        }

        public static IComponentTargetPage[] SelectAll(int templateID, int siteID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTargetPage>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@templateId", templateID);
            filter.AddParameter("@siteId", siteID);

            var result = connector.FetchAll(@"
    SELECT
        (SELECT COUNT(*) FROM [wim_Components] WHERE [ComponentVersion_GUID] = [component_GUID]) PublishedCount,
        ISNull((SELECT TOP 1 [componentVersion_Name] FROM [wim_ComponentVersions] WHERE [ComponentVersion_GUID] = [ComponentTarget_Component_Source]), '?') Name,
        *
    FROM
        [wim_ComponentVersions]
        JOIN [wim_Pages] ON [ComponentVersion_Page_Key] = [Page_Key]
        LEFT JOIN [wim_ComponentTargets] ON [ComponentTarget_Component_Target] = [ComponentVersion_GUID]
        JOIN [dbo].[wim_Folders] ON [Folder_Key] = [Page_Folder_Key]
    WHERE
        [ComponentVersion_ComponentTemplate_Key] = @templateId AND [Folder_Site_Key] = @siteId
    ORDER BY
        [Page_CompletePath], [ComponentVersion_SortOrder]", filter);

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

            return result.ToArray();
        }

        public static async Task<IComponentTargetPage[]> SelectAllAsync(int templateID, int siteID)
        {
            var connector = ConnectorFactory.CreateConnector<ComponentTargetPage>();
            var filter = connector.CreateDataFilter();
            filter.AddParameter("@templateId", templateID);
            filter.AddParameter("@siteId", siteID);

            var result = await connector.FetchAllAsync(@"
    SELECT
        (SELECT COUNT(*) FROM [wim_Components] WHERE [ComponentVersion_GUID] = [component_GUID]) PublishedCount,
        ISNull((SELECT TOP 1 [componentVersion_Name] FROM [wim_ComponentVersions] WHERE [ComponentVersion_GUID] = [ComponentTarget_Component_Source]), '?') Name,
        *
    FROM
        [wim_ComponentVersions]
        JOIN [wim_Pages] ON [ComponentVersion_Page_Key] = [Page_Key]
        LEFT JOIN [wim_ComponentTargets] ON [ComponentTarget_Component_Target] = [ComponentVersion_GUID]
        JOIN [dbo].[wim_Folders] ON [Folder_Key] = [Page_Folder_Key]
    WHERE
        [ComponentVersion_ComponentTemplate_Key] = @templateId AND [Folder_Site_Key] = @siteId
    ORDER BY
        [Page_CompletePath], [ComponentVersion_SortOrder]", filter);

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

            return result.ToArray();
        }

        #region Properties

        public virtual int ID { get; set; }

        public virtual int PageID { get; set; }

        public virtual string Path { get; set; }

        public virtual bool IsActivePage { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual int Position { get; set; }

        public virtual Guid Version_GUID { get; set; }

        public virtual Guid Component_Source { get; set; }

        public virtual string AssignedComponent { get; set; }

        public virtual int PublishedCount { get; set; }

        public virtual bool IsPublished
        {
            get { return PublishedCount > 0; }
        }

        public virtual bool IsNewInstance
        {
            get { return ID == 0; }
        }

        #endregion Properties
    }
}