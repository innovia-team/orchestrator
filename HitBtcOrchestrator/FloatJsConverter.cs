using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HitBtcOrchestrator
{
    class FloatJsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(double) || objectType == typeof(double?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
            {
                return Convert.ToDouble(token.ToString(),
                    System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            }
            if (token.Type == JTokenType.String)
            {
                // customize this to suit your needs
                return Convert.ToDouble(token.ToString(),
                    System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            }
            if (token.Type == JTokenType.Null && objectType == typeof(double?))
            {
                return null;
            }
            throw new JsonSerializationException("Unexpected token type: " +
                                                 token.Type.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}