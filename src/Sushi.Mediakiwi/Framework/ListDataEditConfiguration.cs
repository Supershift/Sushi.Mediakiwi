namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public enum ListDataEditConfigurationType
    {
        /// <summary>
        /// 
        /// </summary>
        TextField,
        /// <summary>
        /// 
        /// </summary>
        Dropdown,
        /// <summary>
        /// 
        /// </summary>
        Hyperlink,
        /// <summary>
        /// 
        /// </summary>
        Button,
        /// <summary>
        /// 
        /// </summary>
        Checkbox
    }

    /// <summary>
    /// 
    /// </summary>
    public class ListDataEditConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListDataEditConfiguration"/> class.
        /// </summary>
        public ListDataEditConfiguration()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListDataEditConfiguration"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyToSet">The property to set.</param>
        /// <param name="width">The width.</param>
        /// <param name="collectionProperty">The collection property.</param>
        public ListDataEditConfiguration(ListDataEditConfigurationType type, string propertyToSet, int width, string collectionProperty)
        {
            this.Type = type;
            this.Width = width;
            this.PropertyToSet = propertyToSet;
            this.CollectionProperty = collectionProperty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListDataEditConfiguration"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public ListDataEditConfiguration(ListDataEditConfigurationType type)
            : this(type, null, 0, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListDataEditConfiguration"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyToSet">The property to set.</param>
        public ListDataEditConfiguration(ListDataEditConfigurationType type, string propertyToSet)
            : this(type, propertyToSet, 0, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListDataEditConfiguration"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyToSet">The property to set.</param>
        /// <param name="width">The width.</param>
        public ListDataEditConfiguration(ListDataEditConfigurationType type, string propertyToSet, int width)
            : this(type, propertyToSet, width, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListDataEditConfiguration"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyToSet">The property to set.</param>
        /// <param name="collectionProperty">The collection property.</param>
        public ListDataEditConfiguration(ListDataEditConfigurationType type, string propertyToSet, string collectionProperty)
            : this(type, propertyToSet, 0, collectionProperty) { }

        ListDataEditConfigurationType m_Type;
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public ListDataEditConfigurationType Type 
        { 
            get { return m_Type; }
            set { m_Type = value; }
        }

        public string InteractiveHelp { get; set; }

        int m_Width;
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }

        string m_PropertyToSet;
        /// <summary>
        /// Gets or sets the property to set.
        /// </summary>
        /// <value>The property to set.</value>
        public string PropertyToSet
        {
            get { return m_PropertyToSet; }
            set { m_PropertyToSet = value; }
        }

        /// <summary>
        /// Hide the table row visualy when the property values changes
        /// </summary>
        public bool HideTableRowIfChanged { get; set; }
        /// <summary>
        /// Gets or sets the state of the null value in case of a bool?.
        /// </summary>
        /// <value>
        /// The state of the is nullable checked.
        /// </value>
        public bool NullableCheckedState { get; set; }
        /// <summary>
        /// Gets or sets the property that defines the enabled state of the instance.
        /// </summary>
        /// <value>
        /// The enabled property.
        /// </value>
        public string IsEnabledProperty { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the IsEnabledProperty property should be reverted (true becomes false and visa versa) 
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [reverse enabled property]; otherwise, <c>false</c>.
        /// </value>
        public bool ReverseEnabledProperty { get; set; }
        /// <summary>
        /// Gets or sets the property that defines the visible state of the instance.
        /// </summary>
        /// <value>
        /// The is invisible property.
        /// </value>
        public string IsVisibleProperty { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the IsVisibleProperty property should be reverted (true becomes false and visa versa) 
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [reverse visible property]; otherwise, <c>false</c>.
        /// </value>
        public bool ReverseVisibleProperty { get; set; }

        string m_CollectionProperty;
        /// <summary>
        /// Gets or sets the collection property.
        /// </summary>
        /// <value>The collection property.</value>
        public string CollectionProperty
        {
            get { return m_CollectionProperty; }
            set { m_CollectionProperty = value; }
        }
    }
}
