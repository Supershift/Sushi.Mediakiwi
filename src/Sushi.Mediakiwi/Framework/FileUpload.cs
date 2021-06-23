using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework
{
    public class FileUpload
    {
        public IFormFile File { get; set; }

        public FileUpload(IFormFile upload)
        {
            File = upload;
        }

        public async Task<string> ReadToEndAsync()
        {
            if (File == null || File.Length == 0)
                return null;

            using (var reader = new StreamReader(File.OpenReadStream()))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public string ReadToEnd()
        {
            if (File == null || File.Length == 0)
                return null;

            using (var reader = new StreamReader(File.OpenReadStream()))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
