using System.Collections.Generic;
using System.Linq;

namespace Sushi.MailTemplate.Entities
{
    /// <summary>
    /// PlaceholderGroup class
    /// </summary>
    public class PlaceholderGroup
    {
        /// <summary>
        /// PlaceholderGroup ctor
        /// </summary>
        public PlaceholderGroup()
        {

        }

        /// <summary>
        /// PlaceholderGroup ctor
        /// </summary>
        /// <param name="name"></param>
        public PlaceholderGroup(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the placeholder group
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Adds a new row to the placeholder group
        /// </summary>
        public void AddNewRow()
        {
            PlaceholderRows.Add(new PlaceholderRow());
        }

        /// <summary>
        /// Adds a new item to the placeholder group row
        /// </summary>
        /// <param name="placeholder"></param>
        /// <param name="value"></param>
        public void AddNewRowItem(string placeholder, string value)
        {
            PlaceholderRows.Last().Placeholders.Add(new Placeholder { Name = placeholder, Value = value });
        }

        /// <summary>
        /// List of placeholder rows
        /// </summary>
        public List<PlaceholderRow> PlaceholderRows { get; set; } = [];

        /// <summary>
        /// List of placeholder tags
        /// </summary>
        public List<string> PlaceholderTags { get; set; } = [];
    }
}