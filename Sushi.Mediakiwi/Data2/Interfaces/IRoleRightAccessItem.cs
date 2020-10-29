namespace Sushi.Mediakiwi.Data
{
    public interface IRoleRightAccessItem
    {
        int ChildTypeID { get; set; }
        int ID { get; set; }
        int RoleID { get; set; }
        int TypeID { get; set; }
    }
}