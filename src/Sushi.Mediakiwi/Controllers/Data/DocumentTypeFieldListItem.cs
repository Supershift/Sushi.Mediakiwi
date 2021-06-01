using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Controllers.Data
{
    public class DocumentTypeFieldListItem
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int TypeID { get; set; }
        public bool IsMandatory { get; set; }

        /// <summary>
        /// Zero based order
        /// </summary>
        public int SortOrder { get; set; }
    }
}
