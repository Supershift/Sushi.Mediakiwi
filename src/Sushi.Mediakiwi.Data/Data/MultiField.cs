namespace Sushi.Mediakiwi.Data
{
    public class MultiField : Field
    {
        /// <summary>
        /// Gets the deserialized.
        /// </summary>
        /// <param name="serialized">The serialized.</param>
        /// <param name="ID">The identifier.</param>
        /// <returns></returns>
        public static MultiField[] GetDeserialized(string serialized, string ID = null)
        {
            // Default return value is NULL, which is returned when the supplied serialized string is empty.
            MultiField[] candidate = null;

            //  If the supplied serialized text is not empty
            if (!string.IsNullOrEmpty(serialized))
            {
                if (serialized.StartsWith("<?xml") || serialized.StartsWith("<ArrayOfMultiField")) //  If the text is of type XML, then try to Deserialize
                {
                    // It is expected to be an array of MultiField classes
                    candidate = Utility.GetDeserialized(typeof(MultiField[]), serialized) as MultiField[];

                    // Return an empty array of Multifield when this resolves to NULL
                    if (candidate == null)
                        candidate = new MultiField[] { };
                }
                else // If text is NOT of type XML, then add a richtext Field
                {
                    if (string.IsNullOrEmpty(ID))
                        ID = "MF";

                    //  It is expected to be XML
                    candidate = new MultiField[1];
                    candidate[0] = new MultiField()
                    {
                        Property = string.Concat(ID, "__", ((int)ContentType.RichText).ToString(), "__", 1),
                        Type = (int)ContentType.RichText,
                        Value = serialized
                    };
                }
            }

            return candidate;
        }

        public MultiField()
        {
        }

        public MultiField(string property, ContentType type, string value)
        {
            this.Type = (int)type;
            this.Property = property;
            this.Value = value;
        }
    }
}
