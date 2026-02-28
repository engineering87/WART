// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace WART_Core.Serialization
{
    /// <summary>
    /// A custom JSON converter that allows a string property to accept JSON objects or arrays
    /// as raw JSON text. This is useful when the schema is flexible, and a value could be a
    /// primitive string or an entire JSON structure (array or object).
    /// </summary>
    public class JsonArrayOrObjectStringConverter : JsonConverter<string>
    {
        /// <summary>
        /// Reads JSON tokens and returns them as a raw JSON string.
        /// If the token is a string, returns its value.
        /// If it's an object or array, serializes it to a JSON string.
        /// </summary>
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString(),
                JsonTokenType.StartObject or JsonTokenType.StartArray => JsonDocument.ParseValue(ref reader).RootElement.GetRawText(),
                JsonTokenType.Null => null,
                JsonTokenType.Number => JsonDocument.ParseValue(ref reader).RootElement.GetRawText(),
                JsonTokenType.True or JsonTokenType.False => reader.GetBoolean().ToString(),
                _ => throw new JsonException($"Unexpected token type: {reader.TokenType}")
            };
        }

        /// <summary>
        /// Writes a string to the JSON output.
        /// If the string contains valid JSON (object or array), it writes it as raw JSON.
        /// Otherwise, it writes it as a simple string value.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                writer.WriteStringValue(value);
                return;
            }

            try
            {
                using var doc = JsonDocument.Parse(value);
                doc.RootElement.WriteTo(writer);
            }
            catch
            {
                writer.WriteStringValue(value);
            }
        }
    }
}
