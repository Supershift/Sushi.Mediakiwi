using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Persistors;

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
        BlobPersister persister = new BlobPersister();
        var blobRef = await persister.GetBlockBlobReferenceAsync(inAsset.FileName).ConfigureAwait(false);
        return await blobRef.DeleteIfExistsAsync().ConfigureAwait(false);
    }

}

