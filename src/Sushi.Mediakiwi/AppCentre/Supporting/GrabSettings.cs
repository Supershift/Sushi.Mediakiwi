using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.AppCentre.Data.Supporting
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class GrabSettings : Attribute
    {
        //  Grab a hashtable to fill in internal parameters
        private string m_Property;
        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        /// <value>The property.</value>
        public string Property
        {
            get { return m_Property; }
            set { m_Property = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GrabSettings"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public GrabSettings(string property)
        {
            this.Property = property;
        }
    }
}
