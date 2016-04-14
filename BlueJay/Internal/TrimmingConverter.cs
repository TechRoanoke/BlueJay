using Newtonsoft.Json;
using System;

namespace BlueJay
{
    // http://stackoverflow.com/questions/19271470/deserialize-json-with-auto-trimming-strings
    internal class TrimmingConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
        public override bool CanRead { get { return true; } }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ((string)reader.Value).Trim();
        }
        public override bool CanWrite { get { return false; } }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}