using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Requests
{
    /// <summary>
    /// The request object for updating/creating an asset
    /// </summary>
    [DataContract]
    public class SaveAssetRequest
    {
        /// <summary>
        /// The Identifier for the asset to update, or 0 when it's a new asset
        /// </summary>
        [DataMember(Name = "id")]
        public int? ID { get; set; }

        /// <summary>
        /// The title for the asset
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// The gallery to which the asset should be saved
        /// </summary>
        [DataMember(Name = "galleryId")]
        public int? GalleryID { get; set; }

        /// <summary>
        /// The description for the asset
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        /// The actual posted data which contains the asset data.
        /// </summary>
        [DataMember(Name ="data")]
        public IFormFile Data { get; set; }
    }
}
