using System.Collections.Generic;

namespace Sushi.Mediakiwi.Controllers.Data
{
    public class CheckSharedFieldResponse
    {
        /// <summary>
        /// All the pages that have this shared field implemented
        /// </summary>
        public ICollection<SharedFieldUsagePage> Pages { get; set; } = new List<SharedFieldUsagePage>();
    }
}
