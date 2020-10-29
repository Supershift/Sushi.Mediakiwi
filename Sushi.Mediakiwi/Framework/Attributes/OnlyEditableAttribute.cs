using System;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)] 
    public class OnlyEditableWhenTrue : Attribute 
    {
        private string m_Property;
        /// <summary>
        /// 
        /// </summary>
        public string Property
        {
            get { return m_Property; }
            set { m_Property = value; }
        }

        private bool m_State;
        /// <summary>
        /// 
        /// </summary>
        public bool State
        {
            get { return m_State; }
            set { m_State = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        public OnlyEditableWhenTrue(string property)
        : this(property, true) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlyEditableWhenTrue"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        public OnlyEditableWhenTrue(string property, bool state)
        {
            this.Property = property;
            this.State = state;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OnlyEditableWhenFalse : Attribute
    {
        private string m_Property;
        /// <summary>
        /// 
        /// </summary>
        public string Property
        {
            get { return m_Property; }
            set { m_Property = value; }
        }

        private bool m_State;
        /// <summary>
        /// 
        /// </summary>
        public bool State
        {
            get { return m_State; }
            set { m_State = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        public OnlyEditableWhenFalse(string property)
            : this(property, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlyEditableWhenTrue"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        public OnlyEditableWhenFalse(string property, bool state)
        {
            this.Property = property;
            this.State = state;
        }

    }
}
