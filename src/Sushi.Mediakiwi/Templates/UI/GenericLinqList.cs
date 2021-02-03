using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wim.Framework;

namespace Wim.Templates.UI
{
    public interface iGenericLinqList
    {
    }
    public class GenericLinqList : ComponentListTemplate, iGenericLinqList
    {
        

        public GenericLinqList()
        {
            this.ListSearch += new Framework.ComponentSearchEventHandler(GenericLinqList_ListSearch);
        }

        void GenericLinqList_ListSearch(object sender, Framework.ComponentListSearchEventArgs e)
        {
        }
    }
}
