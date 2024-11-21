using System.Collections.Generic;

namespace Sushi.MailTemplate.Entities
{
    /// <summary>
    /// Placeholder row, used in placeholder groups
    /// </summary>
    public class PlaceholderRow
    {
        /// <summary>
        /// List of placeholders in this placeholder row
        /// </summary>
        public List<Placeholder> Placeholders { get; set; } = [];
    }
}
