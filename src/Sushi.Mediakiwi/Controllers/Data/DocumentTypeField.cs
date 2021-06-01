using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Controllers.Data
{
    public class DocumentTypeField
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }
        public bool Required { get; set; }
        public string ExtraField { get; set; }
    }
}
