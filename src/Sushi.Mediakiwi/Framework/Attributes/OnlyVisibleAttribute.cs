using System;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// Represents a OnlyVisibleWhenTrue entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)] 
    public class OnlyVisibleWhenTrue : Attribute 
    {
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

        private bool m_State;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OnlyVisibleWhenTrue"/> is state.
        /// </summary>
        /// <value><c>true</c> if state; otherwise, <c>false</c>.</value>
        public bool State
        {
            get { return m_State; }
            set { m_State = value; }
        }

        /// <summary>
        /// Only visible when the property state == true
        /// </summary>
        /// <param name="property"></param>
        public OnlyVisibleWhenTrue( string property ) 
        : this(property, true) {}

        /// <summary>
        /// Only visible when the property state == state
        /// </summary>
        /// <param name="property"></param>
        /// <param name="state"></param>
        public OnlyVisibleWhenTrue(string property, bool state)
        {
            this.Property = property;
            this.State = state;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class OnlyVisibleWhenFalse : Attribute
    {
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

        private bool m_State;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OnlyVisibleWhenTrue"/> is state.
        /// </summary>
        /// <value><c>true</c> if state; otherwise, <c>false</c>.</value>
        public bool State
        {
            get { return m_State; }
            set { m_State = value; }
        }

        /// <summary>
        /// Only visible when the property state == true
        /// </summary>
        /// <param name="property"></param>
        public OnlyVisibleWhenFalse(string property)
            : this(property, true) { }

        /// <summary>
        /// Only visible when the property state == state
        /// </summary>
        /// <param name="property"></param>
        /// <param name="state"></param>
        public OnlyVisibleWhenFalse(string property, bool state)
        {
            this.Property = property;
            this.State = state;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class OnlyRequiredWhenTrue : Attribute
    {
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

        private bool m_State;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OnlyVisibleWhenTrue"/> is state.
        /// </summary>
        /// <value><c>true</c> if state; otherwise, <c>false</c>.</value>
        public bool State
        {
            get { return m_State; }
            set { m_State = value; }
        }

        /// <summary>
        /// Only visible when the property state == true
        /// </summary>
        /// <param name="property"></param>
        public OnlyRequiredWhenTrue(string property)
            : this(property, true) { }

        /// <summary>
        /// Only visible when the property state == state
        /// </summary>
        /// <param name="property"></param>
        /// <param name="state"></param>
        public OnlyRequiredWhenTrue(string property, bool state)
        {
            this.Property = property;
            this.State = state;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class OnlyRequiredWhenFalse : Attribute
    {
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

        private bool m_State;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OnlyVisibleWhenTrue"/> is state.
        /// </summary>
        /// <value><c>true</c> if state; otherwise, <c>false</c>.</value>
        public bool State
        {
            get { return m_State; }
            set { m_State = value; }
        }

        /// <summary>
        /// Only visible when the property state == true
        /// </summary>
        /// <param name="property"></param>
        public OnlyRequiredWhenFalse(string property)
            : this(property, true) { }

        /// <summary>
        /// Only visible when the property state == state
        /// </summary>
        /// <param name="property"></param>
        /// <param name="state"></param>
        public OnlyRequiredWhenFalse(string property, bool state)
        {
            this.Property = property;
            this.State = state;
        }
    }
}
