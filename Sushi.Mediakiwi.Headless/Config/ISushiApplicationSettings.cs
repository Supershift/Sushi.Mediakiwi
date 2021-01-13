using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Headless.Config
{
    public interface ISushiApplicationSettings
    {
        /// <summary>
        /// Contains all MediaKiwi related application settings
        /// </summary>
        public MediaKiwiConfig MediaKiwi { get; set; }
    }
}
