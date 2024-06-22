// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace WART_Core.Helpers
{
    public class SerializationHelper
    {
        private static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Serialize the object to JSON string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
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
        /// Deserialize the JSON string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
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
