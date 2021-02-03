using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sushi.Mediakiwi.Data
{
    public interface iExportable
    {
        int ID { get; set; }
        Guid GUID { get; set; }
        DateTime? Updated { get; }
    }
}
