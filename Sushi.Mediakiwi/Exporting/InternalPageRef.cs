using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sushi.Mediakiwi.Data;

namespace Sushi.Mediakiwi.Framework.Exporting
{
    /// <summary>
    /// 
    /// </summary>
    public class InternalPageRef : iExportable
    {
        public int ID { get; set; }
        public Guid GUID { get; set; }
        public DateTime? Updated { get; set; }
    }
}
