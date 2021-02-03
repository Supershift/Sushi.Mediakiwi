using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Sushi.Mediakiwi.Data
{
    public class AssetLogic
    {

        /// <summary>
        /// Selects the in active.
        /// </summary>
        /// <returns></returns>
        internal static int SelectInActive()
        {
            Asset implement = new Asset();
            return Utility.ConvertToInt(implement.Execute("select count(*) from wim_Assets where Asset_IsActive = 0"));
        }

        internal static void UpdateActive()
        {
            Asset implement = new Asset();
            implement.Execute("update wim_Assets set Asset_IsActive = 0");
        }

        internal static void UpdateActive(int assetID)
        {
            Asset implement = new Asset();
            implement.Execute(string.Concat("update wim_Assets set Asset_IsActive = 0 where Asset_Key = ", assetID));
        }


        internal static bool CloudUpload(Asset document, Gallery gallery, HttpPostedFile file)
        {
            bool result = false;
            document.ValidateImage(file.InputStream, file.FileName);
            if (Asset.HasCloudSetting)
            {
                Asset.m_CloundInstance.Upload(document, gallery, file);
                result = true;
            }
            return result;
        }

        internal static bool CloudUpload(Asset document, Gallery gallery, System.IO.Stream fileStream, string fileName, string contentType)
        {
            bool result = false;
            document.ValidateImage(fileStream, fileName);
            if (Asset.HasCloudSetting)
            {
                Asset.m_CloundInstance.Upload(document, gallery, fileStream, fileName, contentType);
                result = true;
            }
            return result;
        }

    }
}
