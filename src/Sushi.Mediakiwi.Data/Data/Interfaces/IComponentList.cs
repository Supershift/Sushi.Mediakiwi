using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    public interface IComponentList
    {
        string AssemblyName { get; set; }
        bool CanSortOrder { get; set; }
        int CatalogID { get; set; }
        string Class { get; set; }
        string ClassName { get; set; }
        int? ComponentTemplateID { get; set; }
        CustomData Data { get; set; }
        string Description { get; set; }
        int? FolderID { get; set; }
        string Group { get; set; }
        Guid GUID { get; set; }
        bool HasGenericListFilter { get; set; }
        bool HasGenericSiteFilter { get; set; }
        bool HasOneChild { get; set; }
        string Icon { get; set; }
        int ID { get; set; }
        bool IsClassReference { get; }
        bool IsInherited { get; set; }
        bool IsNewInstance { get; }
        bool IsSingleInstance { get; set; }
        bool IsTemplate { get; set; }
        bool IsVisible { get; set; }
        string Label_NewRecord { get; set; }
        string Label_Save { get; set; }
        string Label_Saved { get; set; }
        string Label_Search { get; set; }
        string Name { get; set; }
        bool Option_AfterSaveListView { get; set; }
        bool Option_CanCreate { get; set; }
        bool Option_CanDelete { get; set; }
        bool Option_CanSave { get; set; }
        bool Option_CanSaveAndAddNew { get; set; }
        bool Option_ConvertUTCToLocalTime { get; set; }
        bool Option_FormAsync { get; set; }
        bool Option_HasDataReport { get; set; }
        bool Option_HasExportColumnTitlesXLS { get; set; }
        bool Option_HasExportXLS { get; set; }
        bool Option_HasShowAll { get; set; }
        bool Option_HasSubscribeOption { get; set; }
        bool Option_HideBreadCrumbs { get; set; }
        bool Option_HideNavigation { get; set; }
        bool Option_LayerResult { get; set; }
        bool Option_OpenInEditMode { get; set; }
        bool Option_PostBackSearch { get; set; }
        int Option_Search_MaxResult { get; }
        int Option_Search_MaxResultPerPage { get; set; }
        int Option_Search_MaxViews { get; set; }
        bool Option_SearchAsync { get; set; }
        int ReferenceID { get; set; }
        int? SenseInterval { get; set; }
        DateTime? SenseScheduled { get; set; }
        CustomData Settings { get; set; }

        string SingleItemName { get; set; }
        int? SiteID { get; set; }
        int SortOrder { get; set; }
        ComponentListTarget Target { get; set; }
        ComponentListType Type { get; set; }
        DateTime? Updated { get; }

        string CompletePath();

        bool Delete();

        Task<bool> DeleteAsync();

        bool HasRoleAccess(IApplicationUser user);
        bool HasRoleAccess(IApplicationRole role);

        Task<bool> HasRoleAccessAsync(IApplicationUser user);
        Task<bool> HasRoleAccessAsync(IApplicationRole role);

        int Save();

        Task<int> SaveAsync();
    }
}