using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface IApplicationRoleParser
    {
        void Clear();
        IApplicationRole[] SelectAll();
        IApplicationRole[] SelectAll(int folderID);
        bool Delete(int ID);
        IApplicationRole SelectOne(int ID);
        void Save(IApplicationRole entity);
    }
}