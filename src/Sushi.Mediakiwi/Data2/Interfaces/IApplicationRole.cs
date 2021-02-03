using System;

namespace Sushi.Mediakiwi.Data
{
    public interface IApplicationRole
    {
        bool All_Folders { get; set; }
        bool All_Galleries { get; set; }
        bool All_Lists { get; set; }
        bool All_Sites { get; set; }
        bool CanChangeList { get; set; }
        bool CanChangePage { get; set; }
        bool CanCreateList { get; set; }
        bool CanCreatePage { get; set; }
        bool CanDeletePage { get; set; }
        bool CanPublishPage { get; set; }
        bool CanSeeAdmin { get; set; }
        bool CanSeeGallery { get; set; }
        bool CanSeeList { get; set; }
        bool CanSeePage { get; set; }
        bool CanSeeFolder { get; set; }
        int Dashboard { get; set; }
        string Description { get; set; }
        int? GalleryRoot { get; set; }
        Guid GUID { get; set; }
        int ID { get; set; }
        bool IsAccessFolder { get; set; }
        bool IsAccessGallery { get; set; }
        bool IsAccessList { get; set; }
        bool IsAccessSite { get; set; }
        string Name { get; set; }

        void Save();
        bool Delete();

        //Folder[] Folders(IApplicationUser user);
        //Gallery[] Galleries(IApplicationUser user);
        //IComponentList[] Lists(IApplicationUser user);
        //Task[] OutboundTasks { get; }
        //Task[] SubscribedTasks { get; }
        //Site[] Sites(IApplicationUser user);

        bool IsNewInstance { get; }
    }
}