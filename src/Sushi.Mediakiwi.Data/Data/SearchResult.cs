using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data
{
    public class SearchResult
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public List<string> Data { get; set; }
        public DateTime LastModified { get; set; }
    }
}
