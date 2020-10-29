using System;

namespace Sushi.Mediakiwi.Data
{
    public interface IComponentTarget
    {
        int ID { get; set; }
        bool IsNewInstance { get; }
        int PageID { get; set; }
        Guid Source { get; set; }
        Guid Target { get; set; }

        void Delete();
        void DeleteCompetion();
        bool Save();
    }
}