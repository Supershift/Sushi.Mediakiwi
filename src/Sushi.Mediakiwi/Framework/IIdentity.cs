using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework
{
    public interface IIdentity
    {
        bool IsExistingInstance { get; set; }
    }
}
