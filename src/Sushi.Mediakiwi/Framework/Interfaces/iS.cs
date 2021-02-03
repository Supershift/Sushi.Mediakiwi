using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    public interface ISaveble
    {
        bool Save();
        int ID { get; set; }
    }
}
