using Microsoft.AspNetCore.Http;
using Sushi.Mediakiwi.Data;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Controllers
{
    public interface IController
    {
        IApplicationUser CurrentApplicationUser { get; }
        IVisitor CurrentVisitor { get; }
    }
}
