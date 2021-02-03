using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sushi.Mediakiwi.Data
{
    public interface iDataUpload
    {
        bool Upload(Sushi.Mediakiwi.Data.Asset asset, System.Web.HttpPostedFile file);
        bool Upload(Sushi.Mediakiwi.Data.Asset asset, Sushi.Mediakiwi.Data.Gallery gallery, System.Web.HttpPostedFile file);
        bool Upload(Sushi.Mediakiwi.Data.Asset asset, Sushi.Mediakiwi.Data.Gallery gallery, System.IO.Stream fileStream, string fileName, string contentType);
        bool Upload(Sushi.Mediakiwi.Data.Asset asset, System.IO.Stream fileStream, string fileName, string contentType);
        bool UploadGeneratedContent(System.IO.Stream fileStream, string fileName, string contentType, string folder);
        bool Delete(Sushi.Mediakiwi.Data.Asset asset);
        bool Exists(string filePath);
        bool ExistsGeneratedContent(string filePath);
        
        string ContentDeliveryUrl { get; }
        string Container { get; }
        string ContainerGeneratedContent { get; }
        string LocalUploadFolder { get; }
        Stream GetFileStream(string filePath);
        bool IsPartOfCDN(string uri);
    }
}
