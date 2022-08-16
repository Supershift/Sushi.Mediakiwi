using System;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)] 
    public class OnlyEditableWhenTrue : Attribute 
    {
        /// <summary>
        /// 
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        public OnlyEditableWhenTrue(string property) : this(property, true) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlyEditableWhenTrue"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        public OnlyEditableWhenTrue(string property, bool state)
        {
            Property = property;
            State = state;
        }
    }
}
