using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    public class ContentCreatorLogic
    {
        public static async Task<Dictionary<string, ContentItem>> GetContentAsync(CustomData data, string ContentDeliveryPrefix)
        {
            var dict = new Dictionary<string, ContentItem>();

            if (data == null || data.Items == null || !data.Items.Any())
            {
                return dict;
            }

            foreach (var element in data.Items)
            {
                var node = await GetContentItemFromFieldAsync(element.ToField(), ContentDeliveryPrefix).ConfigureAwait(false);
                dict.Add(element.Property, node.contentItem);
            }

            return dict;
        }

        private static async Task<Dictionary<string, ContentItem>> GetMultiFieldContentAsync(string value, string ContentDeliveryPrefix)
        {
            Dictionary<string, ContentItem> lst = new Dictionary<string, ContentItem>();

            if (string.IsNullOrWhiteSpace(value))
                return lst;

            var mfs = MultiField.GetDeserialized(value);
            if (mfs.Length > 0)
            {
                int idx = 0;
                foreach (var item in mfs)
                {
                    idx++;
                    (ContentItem contentItem, bool isFilled) result = await GetContentItemFromFieldAsync(item, ContentDeliveryPrefix).ConfigureAwait(false);
                    if (result.isFilled)
                        lst.Add($"Multifield_{idx}", result.contentItem);
                }
            }

            return lst;
        }

        public static async Task<(ContentItem contentItem, bool isFilled)> GetContentItemFromFieldAsync(Field inField, string ContentDeliveryPrefix)
        {
            ContentItem content = new ContentItem();
            bool isFilled = false;

            switch ((ContentType)inField.Type)
            {
                default:
                    {
                        isFilled = true;
                        content.Text = inField.Value;
                    }
                    break;
                case ContentType.MultiField:
                    {
                        isFilled = true;
                        content.MultiFieldContent = await GetMultiFieldContentAsync(inField.Value, ContentDeliveryPrefix).ConfigureAwait(false);
                    }
                    break;
                case ContentType.PageSelect:
                    {
                        if (inField?.Page?.ID > 0)
                        {
                            isFilled = true;
                            content.Text = inField.Page?.LinkText;
                            content.Href = ConvertPath(inField.Page?.InternalPath);
                        }
                    }
                    break;
                case ContentType.Binary_Image:
                    {
                        if (inField?.Image?.ID > 0)
                        {
                            isFilled = true;
                            content.Text = inField.Image?.Description;
                            if (inField?.Image.Width > 0)
                                content.Width = inField?.Image.Width;
                            if (inField?.Image.Height > 0)
                                content.Height = inField?.Image.Height;

                            content.Url = inField.Image?.RemoteLocation;
                            if (!string.IsNullOrWhiteSpace(ContentDeliveryPrefix) && !string.IsNullOrWhiteSpace(content.Url))
                            {
                                Uri path = new Uri(inField.Image.RemoteLocation, UriKind.Absolute);
                                content.Url = $"{ContentDeliveryPrefix}{path.AbsolutePath}";
                            }
                        }
                    }
                    break;
                case ContentType.Binary_Document:
                    {
                        if (inField?.Asset?.ID > 0)
                        {
                            isFilled = true;
                            content.Text = inField.Asset?.Description;
                            content.Url = inField.Asset?.RemoteLocation;
                            if (!string.IsNullOrWhiteSpace(ContentDeliveryPrefix) && !string.IsNullOrWhiteSpace(content.Url))
                            {
                                Uri path = new Uri(inField.Asset.RemoteLocation, UriKind.Absolute);
                                content.Url = $"{ContentDeliveryPrefix}{path.AbsolutePath}";
                            }
                        }
                    }
                    break;
                case ContentType.Hyperlink:
                    {
                        if (inField.Link?.ID > 0)
                        {
                            isFilled = true;
                            content.Text = inField.Link?.Text;
                            if (inField.Link != null)
                            {
                                if (inField.Link.IsInternal)
                                {
                                    var pagelink = await Page.SelectOneAsync(inField.Link.PageID.GetValueOrDefault()).ConfigureAwait(false);
                                    content.Href = ConvertPath(pagelink?.InternalPath);
                                }
                                else
                                    content.Href = inField.Link.ExternalUrl;
                            }
                        }
                    }
                    break;
          
            }

            return (content, isFilled);
        }

        public static string ConvertPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            return path
                .ToLowerInvariant()
                .Replace(" ", "-");
        }
    }
}
