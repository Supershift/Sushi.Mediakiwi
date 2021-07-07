using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.IO;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Headless
{
    /// <summary>
    /// Reads the given File from the Href attribute and emits its output inside a style tag
    /// </summary>
    public class InlineStyleTagHelper : TagHelper
    {
        public InlineStyleTagHelper(IWebHostEnvironment webHostEnvironment, IMemoryCache cache)
        {
            WebHostEnvironment = webHostEnvironment;
            Cache = cache;
        }

        [HtmlAttributeName("href")]
        public string Href { get; set; }

        private IWebHostEnvironment WebHostEnvironment { get; }
        private IMemoryCache Cache { get; }

        private async Task<string> createCacheEntryAsync(string path, ICacheEntry entry)
        {
            IFileProvider fileProvider = WebHostEnvironment.WebRootFileProvider;
            IChangeToken changeToken = fileProvider.Watch(path);

            entry.SetPriority(CacheItemPriority.NeverRemove);
            entry.AddExpirationToken(changeToken);

            IFileInfo file = fileProvider.GetFileInfo(path);
            if (file == null || !file.Exists)
                return null;

            return await ReadFileContentAsync(file).ConfigureAwait(false);
        }

        public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var path = Href;
            var version = string.Empty;

            if (Href.IndexOf('?') > -1)
            {
                version = Href.Substring(Href.IndexOf('?'));
                path = path.Remove(Href.IndexOf('?'));
            }

            // Get the value from the cache, or compute the value and add it to the cache
            var fileContent = await Cache.GetOrCreateAsync($"InlineStyleTagHelper-{path}-{version}", async entry => await createCacheEntryAsync(path, entry)).ConfigureAwait(false);
            if (fileContent == null)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "style";
            output.Attributes.RemoveAll("href");
            output.PostContent.AppendHtml(fileContent);
        }


        private static async Task<string> ReadFileContentAsync(IFileInfo file)
        {
            using var stream = file.CreateReadStream();
            using var textReader = new StreamReader(stream);

            return await textReader.ReadToEndAsync().ConfigureAwait(false);
        }
    }
}
