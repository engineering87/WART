// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace WART_Core.Helpers
{
    public class SerializationHelper
    {
        // Default JSON serializer options to be used for serialization and deserialization.
        private static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Serializes the given object to a JSON string using default serialization options.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A JSON string representing the serialized object, or an empty string if serialization fails.</returns>
        public static string Serialize<T>(T obj)
        {
            try
            {
                return JsonSerializer.Serialize(obj, DefaultOptions);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Deserializes the given JSON string to an object of type T using default deserialization options.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize into.</typeparam>
        /// <param name="jsonString">The JSON string to deserialize.</param>
        /// <returns>An object of type T representing the deserialized data, or the default value of T if deserialization fails.</returns>
        public static T Deserialize<T>(string jsonString)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(jsonString, DefaultOptions);
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
