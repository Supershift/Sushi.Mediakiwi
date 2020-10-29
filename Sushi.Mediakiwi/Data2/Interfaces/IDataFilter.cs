using System;

namespace Sushi.Mediakiwi.Data
{
    public interface IDataFilter
    {
        void Delete();
        bool IsNewInstance { get; }
        bool? FilterB { get; set; }
        string FilterC { get; set; }
        decimal? FilterD { get; set; }
        int? FilterI { get; set; }
        DateTime? FilterT { get; set; }
        int ID { get; set; }
        int ItemID { get; set; }
        int PropertyID { get; set; }
        string Type { get; set; }

        void Save();
    }
}