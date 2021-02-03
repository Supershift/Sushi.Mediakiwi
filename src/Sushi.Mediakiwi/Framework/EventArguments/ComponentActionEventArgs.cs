using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class ComponentActionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentActionEventArgs"/> class.
        /// </summary>
        /// <param name="selectedKey">The selected key.</param>
        /// <param name="componentVersionKey">The component version key.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="selectedGroupKey">The selected group key.</param>
        /// <param name="selectedGroupItemKey">The selected group item key.</param>
        public ComponentActionEventArgs(int selectedKey, int componentVersionKey, string propertyName, int selectedGroupKey, int selectedGroupItemKey, bool? isValidForm)
        {
            SelectedKey = selectedKey;
            VersionKey = componentVersionKey;
            PropertyName = propertyName;
            SelectedGroupKey = selectedGroupKey;
            SelectedGroupItemKey = selectedGroupItemKey;
            IsValidForm = isValidForm;
        }

        public bool? IsValidForm { get; set; }

        private int m_SelectedKey;
        /// <summary>
        /// Gets or sets the selected key.
        /// </summary>
        /// <value>The selected key.</value>
        public int SelectedKey
        {
            get { return m_SelectedKey; }
            set { m_SelectedKey = value; }
        }

        private int m_VersionKey;
        /// <summary>
        /// Gets or sets the version key.
        /// </summary>
        /// <value>The version key.</value>
        public int VersionKey
        {
            get { return m_VersionKey; }
            set { m_VersionKey = value; }
        }

        private string m_PropertyName;
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName
        {
            get { return m_PropertyName; }
            set { m_PropertyName = value; }
        }

        private int m_SelectedGroupKey;
        /// <summary>
        /// Gets or sets the selected group key.
        /// </summary>
        /// <value>The selected group key.</value>
        public int SelectedGroupKey
        {
            get { return m_SelectedGroupKey; }
            set { m_SelectedGroupKey = value; }
        }

        private int m_SelectedGroupItemKey;
        /// <summary>
        /// Gets or sets the selected group item key.
        /// </summary>
        /// <value>The selected group item key.</value>
        public int SelectedGroupItemKey
        {
            get { return m_SelectedGroupItemKey; }
            set { m_SelectedGroupItemKey = value; }
        }
    }
}
