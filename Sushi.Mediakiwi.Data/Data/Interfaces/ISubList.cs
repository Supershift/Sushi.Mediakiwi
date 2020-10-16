namespace Wim.Data.Interfaces
{
    public interface ISubList
    {
        ISubListitem GetListItemValue();

        ISubList SetListItemValue(ISubListitem[] values);
    }
}