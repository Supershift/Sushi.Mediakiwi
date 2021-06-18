namespace Sushi.Mediakiwi.Framework
{
    public interface ICallback
    {
        string UID { get; set; }
        bool Run(Beta.GeneratedCms.Console console);
    }
}
