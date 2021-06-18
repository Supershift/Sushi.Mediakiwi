namespace Sushi.Mediakiwi.Framework
{
    public interface ISaveble
    {
        bool Save();
        int ID { get; set; }
    }
}
