using Sushi.Mediakiwi.API.Transport.Responses;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.API.Services
{
    public interface IContentService
    {
        public Task<GetListResponse> GetListResponseAsync(UrlResolver resolver);
    }
}
