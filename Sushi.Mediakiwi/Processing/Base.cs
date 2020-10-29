using System;
using System.Collections.Generic;
using System.Text;

namespace Wim.Processing
{
    public class Base
    {
        Sushi.Mediakiwi.Framework.WimComponentListRoot m_wim;
        /// <summary>
        /// </summary>
        /// <value></value>
        public Sushi.Mediakiwi.Framework.WimComponentListRoot wim
        {
            get { return m_wim; }
            set { m_wim = value; }
        }
    }
}
