using System;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    ///
    /// </summary>
    public class AssetInfo
    {
        /// <summary>
        /// Gets or sets the asset ID.
        /// </summary>
        /// <value>The asset ID.</value>
        public int AssetID { get; set; }

        internal int? m_GalleryID;
        internal int? m_RootGalleryID;
        internal bool m_CanOnlyCreate;

        /// <summary>
        /// Sets the gallery.
        /// </summary>
        /// <param name="galleryID">The gallery ID.</param>
        /// <param name="canOnlyCreate">if set to <c>true</c> [can only create].</param>
        public void SetGallery(int galleryID, bool canOnlyCreate)
        {
            m_GalleryID = galleryID;
            m_RootGalleryID = galleryID;
            m_CanOnlyCreate = canOnlyCreate;
        }

        /// <summary>
        /// Sets the gallery.
        /// </summary>
        /// <param name="galleryGUID">The gallery GUID.</param>
        /// <param name="canOnlyCreate">if set to <c>true</c> [can only create].</param>
        public void SetGallery(Guid galleryGUID, bool canOnlyCreate)
        {
            m_GalleryID = Gallery.SelectOne(galleryGUID).ID;

            m_RootGalleryID = m_GalleryID;
            m_CanOnlyCreate = canOnlyCreate;
        }
    }
}