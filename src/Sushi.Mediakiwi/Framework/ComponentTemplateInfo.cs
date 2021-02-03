using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{
    public enum CacheLevel
    {
        None = 0,
        ComponentBased = 1,
        PageBased = 2
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentTemplateInfo : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentTemplateInfo"/> class.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="title">The title.</param>
        /// <param name="isSecundaryContainerItem">if set to <c>true</c> [is secundary container item].</param>
        /// <param name="isSearchable">if set to <c>true</c> [is searchable].</param>
        /// <param name="canMoveUpDown">if set to <c>true</c> [can move up down].</param>
        /// <param name="canDeactivate">if set to <c>true</c> [can deactivate].</param>
        /// <param name="canReplicate">if set to <c>true</c> [can replicate].</param>
        /// <param name="isHeader">if set to <c>true</c> [is header].</param>
        /// <param name="isFooter">if set to <c>true</c> [is footer].</param>
        /// <param name="cacheLevel">The cache level.</param>
        public ComponentTemplateInfo(string reference, string title, bool isSecundaryContainerItem, bool isSearchable, bool canMoveUpDown, bool canDeactivate, bool canReplicate, bool isHeader, bool isFooter, CacheLevel cacheLevel)
        {
        }

        public decimal Reference { get; set; }
        public string Title { get; set; }
        public bool HasCustomDate { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PageTemplateInfo : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentTemplateInfo"/> class.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="title">The title.</param>
        /// <param name="onlyOneInstance">if set to <c>true</c> [only one instance].</param>
        /// <param name="hasCustomDate">if set to <c>true</c> [has custom date].</param>
        /// <param name="hasSecundaryContentContainer">if set to <c>true</c> [has secundary content container].</param>
        public PageTemplateInfo(string reference, string title, bool onlyOneInstance, bool hasCustomDate, bool hasSecundaryContentContainer)
        {
            this.Reference = Wim.Utility.ConvertToInt(reference);
            this.Title = title;
            this.OnlyOneInstance = onlyOneInstance;
            this.HasCustomDate = hasCustomDate;
        }

        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        /// <value>The reference.</value>
        public decimal Reference { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [only one instance].
        /// </summary>
        /// <value><c>true</c> if [only one instance]; otherwise, <c>false</c>.</value>
        public bool OnlyOneInstance { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance has custom date.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has custom date; otherwise, <c>false</c>.
        /// </value>
        public bool HasCustomDate { get; set; }
    }
}
