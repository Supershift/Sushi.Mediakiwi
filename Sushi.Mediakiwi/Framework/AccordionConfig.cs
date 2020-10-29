using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// Config for a listsearch acting as a accordion UI element (example http://api.jqueryui.com/accordion/#entry-examples
    /// </summary>
    public class AccordionConfig
    {
        /// <summary>
        /// When set to true the other panels which maybe opened are closed
        /// </summary>
        public bool CloseOtherPanelsOnClick { get; set; } = true;
        /// <summary>
        /// The height in pixesl of the opened item which will show the item from the list search in a pannel below it
        /// </summary>
        public int PanelHeight { get; set; } = 350;
       
        /// <summary>
        /// The close label which recides right from the  PanelName
        /// </summary>
        public string CloseLabel { get; set; } = "Sluiten";
    }
}
