using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework
{
    public interface iMappedUrlCreator
    {
        string CreateMappedUrl(string urlMappingName, bool UseSpaceReplacement = false, params object[] args);
    }

 
}
