namespace Sushi.Mediakiwi.Data
{
    public interface IMenu
    {
        int ID { get; set; }
        bool IsActive { get; set; }
        string Name { get; set; }
        int? RoleID { get; set; }
        int? SiteID { get; set; }

        bool Save();
        void Delete();
    }
}