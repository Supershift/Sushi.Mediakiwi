namespace Sushi.Mediakiwi.Data
{
    public interface IComponent
    {
        int ID { get; set; }
        string Target { get; set; }
        bool IsSecundary { get; set; }

        ComponentTemplate Template { get;  }

        string FixedFieldName { get; set; }
    }
}