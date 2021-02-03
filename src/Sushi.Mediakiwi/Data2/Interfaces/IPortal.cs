using System;

namespace Sushi.Mediakiwi.Data
{
    public interface IPortal
    {
        void Delete();
        string Authentication { get; set; }
        string Authenticode { get; set; }
        DateTime Created { get; set; }
        CustomData Data { get; set; }
        string Domain { get; set; }
        Guid GUID { get; set; }
        int ID { get; set; }
        bool IsActive { get; set; }
        string Name { get; set; }
        int UserID { get; set; }

        void Save();
    }
}