using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Headless.Config
{
    public class ContentServiceConfig
    {    
        /// <summary>
        /// Where does the service reside ?
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// If the url contains one of these string,
        /// it will be excluded for the Content Service call
        /// </summary>
        public List<string> ExcludePaths { get; set; } = new List<string>();

        /// <summary>
        /// Does this website get its content from a CMS containing multiple websites ?
        /// Then set this folder prefix, so every call to the content service will be prefixed
        /// with this folder name. if this setting is 'subsite' and you're requesting the page '\system\contact'
        /// then the requested content will be '\subsite\system\contact'
        /// </summary>
        public string SiteFolderPrefix { get; set; }

        /// <summary>
        /// What is the timeout for the Content API in milliseconds ?
        /// </summary>
        public int TimeOut { get; set; } = 10000;

        /// <summary>
        /// Should we ping the Content server, before fetching content ?
        /// a safer, but slower option
        /// </summary>
        public bool PingFirst { get; set; } = false;
    }
}
