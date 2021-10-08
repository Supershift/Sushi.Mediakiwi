using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    public interface iHeadlessListTemplate
    {
        /// <summary>
        /// Get the element for headless use
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DoHeadLessFetchAsync(HeadlessRequest request);
    }
}
