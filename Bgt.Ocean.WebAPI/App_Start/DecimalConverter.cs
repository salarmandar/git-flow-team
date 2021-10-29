using Newtonsoft.Json;
using System;

namespace Bgt.Ocean.WebAPI.App_Start
{
    public class DecimalConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value.ToString() == "")
                return null;

            if (reader.Value != null)
                return decimal.Parse(reader.Value.ToString());
            return reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal) || objectType == typeof(decimal?);
        }
    }
}