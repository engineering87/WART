// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace WART_Core.Serialization
{
    public class JsonArrayOrObjectStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (string.IsNullOrEmpty(value))
            {
                writer.WriteStringValue(value);
                return;
            }

            using (JsonDocument doc = JsonDocument.Parse(value))
            {
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    writer.WriteStartArray();
                    foreach (var element in doc.RootElement.EnumerateArray())
                    {
                        element.WriteTo(writer);
                    }
                    writer.WriteEndArray();
                }
                else if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    writer.WriteStartObject();
                    foreach (var property in doc.RootElement.EnumerateObject())
                    {
                        property.WriteTo(writer);
                    }
                    writer.WriteEndObject();
                }
                else
                {
                    writer.WriteStringValue(value);
                }
            }
        }
    }
}
