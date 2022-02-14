using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Persisters;

public static class AssetExtension
{
    /// <summary>
    /// Deletes the Cloud Asset, the DB record and updates the gallery count
    /// </summary>
    /// <returns></returns>
    public static async Task<bool> CompleteDeleteAsync(this Asset inAsset)
    {
        // Get the parent gallery for the asset
        var gallery = await Gallery.SelectOneAsync(inAsset.GalleryID).ConfigureAwait(false);

        // Delete the file from the cloud
        var result = await CloudDeleteAsync(inAsset).ConfigureAwait(false);

        // Delete the DB record
        await inAsset.DeleteAsync().ConfigureAwait(false);

        if (gallery?.ID > 0)
        {
            await gallery.UpdateCountAsync().ConfigureAwait(false);
        }

        return result;
    }

    internal static async Task<bool> CloudDeleteAsync(this Asset inAsset)
    {
        var success = false;
        var successThumb = false;
        BlobPersister persister = new BlobPersister();
        var blobRef = await persister.GetBlockBlobReferenceAsync(inAsset.FileName).ConfigureAwait(false);
        if (blobRef != null)
        {
            success = await blobRef.DeleteIfExistsAsync().ConfigureAwait(false);
        }

        if (string.IsNullOrWhiteSpace(inAsset.RemoteLocation_Thumb) == false)
        {
            try
            {
                System.Uri thumbUri = new System.Uri(inAsset.RemoteLocation_Thumb);
                string thumbFileName = thumbUri?.Segments?.GetValue(thumbUri.Segments.Length - 1).ToString();

                var blobRefThumb = await persister.GetBlockBlobReferenceAsync(thumbFileName).ConfigureAwait(false);
                if (blobRefThumb != null)
                {
                    successThumb = await blobRefThumb.DeleteIfExistsAsync().ConfigureAwait(false);
                }
            }
            catch (System.Exception ex)
            {
                await Notification.InsertOneAsync("Thumb Delete", ex);
            }
        }

        return success && successThumb;
    }

}

