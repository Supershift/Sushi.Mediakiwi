﻿using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class WimServerMapCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Gets or sets the <see cref="Sushi.Mediakiwi.Framework.WimServerMap"/> at the specified index.
        /// </summary>
        /// <value></value>
        public WimServerMap this[int index]
        {
            get
            {
                return base.BaseGet(index) as WimServerMap;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new WimServerMap();
        }

        /// <summary>
        /// Gets the element key.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WimServerMap)element).Name;
        }
    }
}
