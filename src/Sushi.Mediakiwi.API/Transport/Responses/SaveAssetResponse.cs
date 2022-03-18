using System.Runtime.Serialization;

namespace Sushi.Mediakiwi.API.Transport.Responses
{
    /// <summary>
    /// The response object which is returned after updating or creating an asset
    /// </summary>
    [DataContract]
    public class SaveAssetResponse
    {
        /// <summary>
        /// The ID for the updated asset
        /// </summary>
        [DataMember(Name = "id")]
        public int ID { get; set; }

        /// <summary>
        /// The title for the updated asset
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// The Gallery to which the asset has been saved
        /// </summary>
        [DataMember(Name = "galleryId")]
        public int GalleryID { get; set; }

        /// <summary>
        /// The description for the asset
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        /// The remote location (Azure URL) for the asset
        /// </summary>
        [DataMember(Name = "remoteLocation")]
        public string RemoteLocation { get; set; }

        /// <summary>
        /// The remove location (Azure URL) for the asset thumbnail (if any)
        /// </summary>
        [DataMember(Name = "remoteLocationThumb")]
        public string RemoteLocationThumb { get; set; }
    }
}
