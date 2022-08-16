using System;

namespace Sushi.Mediakiwi.Framework
{

    [AttributeUsage(AttributeTargets.Property)]
    public class OnlyRequiredWhenFalse : Attribute
    {
        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        /// <value>The property.</value>
        public string Property { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OnlyVisibleWhenTrue"/> is state.
        /// </summary>
        /// <value><c>true</c> if state; otherwise, <c>false</c>.</value>
        public bool State { get; set; }

        /// <summary>
        /// Only visible when the property state == true
        /// </summary>
        /// <param name="property"></param>
        public OnlyRequiredWhenFalse(string property) : this(property, true) { }

        /// <summary>
        /// Only visible when the property state == state
        /// </summary>
        /// <param name="property"></param>
        /// <param name="state"></param>
        public OnlyRequiredWhenFalse(string property, bool state)
        {
            Property = property;
            State = state;
        }
    }
}
