namespace Sushi.Mediakiwi.Data
{
    public interface ISubList
    {
        ISubListitem GetListItemValue();

        ISubList SetListItemValue(ISubListitem[] values);
    }
}