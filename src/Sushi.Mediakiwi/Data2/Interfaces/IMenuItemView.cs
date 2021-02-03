{
    public interface IMenuItemView
    {
        int DashboardID { get; set; }
        int ID { get; set; }
        int ItemID { get; set; }
        int MenuID { get; set; }
        string Name { get; set; }
        int Position { get; set; }
        int Section { get; set; }
        int SiteID { get; set; }
        int Sort { get; set; }
        string Tag { get; }
        int TypeID { get; set; }

        string Url(int currentChannelID);
    }
}