using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Headless.Config
{
    public class FileVersionConfig
    {   
        /// <summary>
        /// What is the fileVersion for CSS files ?
        /// </summary>
        public string CSS { get; set; }

        /// <summary>
        /// What is the fileversion for JS files ?
        /// </summary>
        public string JS { get; set; }

        /// <summary>
        /// What is the fileversion for External files ?
        /// </summary>
        public string External { get; set; }
    }
}
