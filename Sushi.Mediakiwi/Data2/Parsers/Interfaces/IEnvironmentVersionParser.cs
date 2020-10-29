using System.Web;

namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface IEnvironmentVersionParser
    {
        void Flush(bool setChacheVersion = true, HttpContext context = null);
        IEnvironmentVersion Select();
        bool Save(IEnvironmentVersion entity);
    }
}