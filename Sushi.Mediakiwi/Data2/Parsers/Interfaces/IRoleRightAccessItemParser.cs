namespace Sushi.Mediakiwi.Data.Parsers
{
    internal interface IRoleRightAccessItemParser
    {
        IRoleRightAccessItem[] Select(int roleID, int typeID, int childTypeID, string portal = null);
    }
}