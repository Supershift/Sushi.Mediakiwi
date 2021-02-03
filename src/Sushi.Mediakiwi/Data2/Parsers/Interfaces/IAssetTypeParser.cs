namespace Sushi.Mediakiwi.Data
{
    public interface IAssetTypeParser
    {
        bool Save(IAssetType entity);
        IAssetType[] SelectAll(bool onlyReturnVariant = false);
        IAssetType SelectOne(string Tag);
        IAssetType SelectOne(int ID);
    }
}