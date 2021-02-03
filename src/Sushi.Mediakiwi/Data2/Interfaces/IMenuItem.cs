namespace Sushi.Mediakiwi.Data
{
    public interface IMenuItem
    {
        int DashboardID { get; set; }
        int ID { get; set; }
        int ItemID { get; set; }
        int MenuID { get; set; }
        string Name { get; }
        int Position { get; set; }
        int Sort { get; set; }
        string Tag { get; }
        int TypeID { get; set; }
        string Url { get; }

        void Delete();
        bool Save();
    }
}