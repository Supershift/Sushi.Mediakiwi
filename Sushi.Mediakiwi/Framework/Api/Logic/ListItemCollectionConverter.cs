using Newtonsoft.Json;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework2.Api.Logic
{
    public class ListItemCollectionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var collection = (ListItemCollection)value;

            writer.WriteStartArray();
            foreach (ListItem item in collection)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("text");
                writer.WriteValue(item.Text);

                writer.WritePropertyName("value");
                writer.WriteValue(item.Value);

                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ListItemCollection);
        }
    }
}
