using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExposedListCollection : Attribute
    {
        string m_Description;
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        string m_CollectionReferencingMethod;
        /// <summary>
        /// Gets or sets the collection referencing method.
        /// </summary>
        /// <value>The collection referencing method.</value>
        public string CollectionReferencingMethod
        {
            get { return m_CollectionReferencingMethod; }
            set { m_CollectionReferencingMethod = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExposedListCollection"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        public ExposedListCollection(string description)
        {
            m_Description = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExposedListCollection"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="collectionReferencingMethod">The collection referencing method. This method should have an INT as parameter</param>
        public ExposedListCollection(string description, string collectionReferencingMethod)
        {
            m_Description = description;
            m_CollectionReferencingMethod = collectionReferencingMethod;
        }
    }
    
    /// <summary>
    /// When used this attribute offers human readible desctive text when classes are listed in list selection dropdowns in WIM.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ListInformation : Attribute
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public ListInformation(string title)
        {
            this.Title = title;
        }

        public ListInformation(string title, string description)
        {
            this.Title = title;
            this.Description = description;
        }
    }
}
