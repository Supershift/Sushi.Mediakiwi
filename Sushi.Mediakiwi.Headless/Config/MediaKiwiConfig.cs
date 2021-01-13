using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Headless.Config
{
    public class MediaKiwiConfig
    {
        /// <summary>
        /// ContentService Settings
        /// </summary>
        public  ContentServiceConfig ContentService { get; set; }

        /// <summary>
        /// Is our application in Development mode ?
        /// </summary>
        public bool IsDevelopmentMode { get; set; }

        /// <summary>
        /// What are the assemblies where to retrieve components from ?
        /// </summary>
        public List<string> ComponentAssemblies { get; set; }

        /// <summary>
        /// Which namespaces should be excluded to retrieve components from ?
        /// </summary>
        public List<string> ExcludeComponentNameSpaces { get; set; }

        /// <summary>
        /// Internal URL mappings
        /// </summary>
        public List<URLMappingConfig> URLMappings { get; set; }

        /// <summary>
        /// The Locale (language) for the application
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Fileversion settings, will prevent prolongued cache issues
        /// </summary>
        public FileVersionConfig FileVersion { get; set; }
    }
}
