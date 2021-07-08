using Sushi.Mediakiwi.Data;
using System.Text;
namespace Sushi.Mediakiwi.Framework
{
    public class MultiFieldParser : IMultiFieldParser
    {
        public virtual string WriteHTML(string serialized, Page page = null)
        {
            return WriteHTML(MultiField.GetDeserialized(serialized), page);
        }

        /// <summary>
        /// Write the HTML output
        /// </summary>
        /// <returns></returns>
        public virtual string WriteHTML(MultiField[] fields, Page page = null)
        {
            if (fields == null || fields.Length == 0)
                return null;

            StringBuilder build = new StringBuilder();
            foreach (var multiField in fields)
            {
                if (multiField.Type == (int)ContentType.Binary_Image)
                {
                    if (multiField.Image != null && multiField.Image.ID != 0)
                    {
                        build.AppendFormat("<img src=\"{0}\">", multiField.Image.Url);
                    }
                }
                else
                    build.Append(multiField.Value);
            }
            return build.ToString();
        }
    }

}
