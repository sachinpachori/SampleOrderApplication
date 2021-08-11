using System.Text.Json;
using System;
using System.Text.Json.Serialization;

namespace Sample.Order.BE.Data.Helper
{
    /// <summary>
    /// Unleashed required format for UTC dates
    /// </summary>
    public class UnleashedDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly string _format = "yyyy-MM-dd'T'HH:mm:ss.fff";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString(), _format, System.Globalization.CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToUniversalTime().ToString(_format));
    }
}
