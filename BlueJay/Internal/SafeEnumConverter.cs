using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueJay
{
    // Helper to convert Enums as string.
    // Unlike StringEnumConverter, if the string value is missing, then use the 0 value. 
    public class SafeEnumConverter<TEnum> : JsonConverter
        where TEnum : struct
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
        public override bool CanRead { get { return true; } }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var x = (string)reader.Value;

            TEnum result;
            if (!Enum.TryParse<TEnum>(x, true, out result)) // ignore case
            {
                // This is the difference from StringEnumConverter.
                // Instead of an error, use 0. 
                return default(TEnum); // will pick item 0
            }
            return result;
        }
        public override bool CanWrite { get { return true; } }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteToken(JsonToken.String, value.ToString());
        }
    }
}
