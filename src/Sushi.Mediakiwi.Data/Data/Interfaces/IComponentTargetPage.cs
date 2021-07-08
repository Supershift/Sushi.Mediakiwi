using System;

namespace Sushi.Mediakiwi.Data
{
    public interface IComponentTargetPage
    {
        string AssignedComponent { get; set; }
        Guid Component_Source { get; set; }
        int ID { get; set; }
        bool IsActive { get; set; }
        bool IsActivePage { get; set; }
        bool IsNewInstance { get; }
        bool IsPublished { get; }
        int PageID { get; set; }
        string Path { get; set; }
        int Position { get; set; }
        int PublishedCount { get; set; }
        Guid Version_GUID { get; set; }
    }
}