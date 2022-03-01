using System.Runtime.Serialization;

namespace Sushi.Mediakiwi.Controllers.Data
{
    [DataContract]
    public class InheritedPage
    {
        /// <summary>
        /// The absolute URL to the inherited page
        /// </summary>
        [DataMember(Name = "href")]
        public string Href { get; set; }

        /// <summary>
        /// The channel ID to which this page belongs
        /// </summary>
        [DataMember(Name = "channelId")]
        public int ChannelID { get; set; }

        /// <summary>
        /// The page ID 
        /// </summary>
        [DataMember(Name = "pageId")]
        public int PageID { get; set; }

        /// <summary>
        /// The language of the channel
        /// </summary>
        [DataMember(Name = "channelLanguage")]
        public string ChannelLanguage { get; set; }

        /// <summary>
        /// The culture of the channel
        /// </summary>
        [DataMember(Name = "channelCulture")]
        public string ChannelCulture { get; set; }
    }
}
