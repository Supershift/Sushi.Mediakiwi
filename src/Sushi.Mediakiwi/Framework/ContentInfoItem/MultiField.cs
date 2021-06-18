using System;
using System.Linq;
using System.Collections.Generic;
using Sushi.Mediakiwi.Data;
using Microsoft.AspNetCore.Http;

namespace Sushi.Mediakiwi.Framework
{
    public class MultiField : Field
    {
        internal static string GetSerialized(HttpRequest request, string[] multiFound)
        {
            string serialized = null;
            var fields = new List<MultiField>();
            foreach (var instance in multiFound)
            {
                //  Split into 0 = [ID]  1 = [CONTENTTYPE_INT] 2 = [COUNT-1-BASED]
                var split = instance.Split(new string[] { "__" }, StringSplitOptions.RemoveEmptyEntries);
                var name = string.Concat(split[0], split[2].Split('$')[0]);
                var type = (ContentType)Convert.ToInt32(split[1]);
                string value = request.HasFormContentType ? request.Form[instance].ToString() : null;
                //  For a multi select field there is always a placeholder idenfitying the field
                bool isMultiSelection = (value == "_MK$PH_");
                //  When the post key contain a $ it is a instance of a field
                if (instance.Contains("$"))
                    continue;

                if (isMultiSelection)
                {
                    value = null;
                    //  Find other keys that are an extention of this one!
                    var others = (from x in multiFound where x.StartsWith(string.Concat(instance, "$")) select x).ToList();
                    if (others.Count > 0)
                    {
                        List<string> arr = new List<string>();
                        others.ForEach( x => 
                        { 
                            var result = request.HasFormContentType ? request.Form[x].ToString().Split('|')[0] : null;
                            if (!string.IsNullOrEmpty(result))
                                arr.Add(result); 
                        } );

                        value = Data.Utility.ConvertToCsvString(arr.ToArray(), true);
                    }
                }

                fields.Add(new MultiField(name, type, value));
            }
            //  Serialize and add as a value (fields in field.value)
            serialized = Data.Utility.GetSerialized(fields.ToArray());
            return serialized;
        }
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
                    candidate = Data.Utility.GetDeserialized(typeof(MultiField[]), serialized) as MultiField[];

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
