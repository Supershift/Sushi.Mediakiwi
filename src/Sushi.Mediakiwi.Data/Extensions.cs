using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    public static class Extensions
    {
        /// <summary>
        /// Gets the Href from a link, for both an internal and external link
        /// </summary>
        /// <param name="inLink">The link to get the Href for</param>
        /// <returns></returns>
        public static async Task<string> GetHrefAsync(this Link inLink)
        {
            var result = "";

            if (inLink?.ID > 0)
            {
                if (inLink.IsInternal && inLink.PageID.GetValueOrDefault(0) > 0)
                {
                    var p = await Page.SelectOneAsync(inLink.PageID.Value).ConfigureAwait(false);
                    if (p?.ID > 0)
                    {
                        result = p.InternalPath;
                    }
                }
                else if (inLink.IsInternal == false && string.IsNullOrWhiteSpace(inLink.ExternalUrl) == false)
                {
                    result = inLink.ExternalUrl;
                }
            }

            return result;
        }
    }
}
