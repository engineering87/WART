// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WART_Core.Helpers;
using WART_Core.Serialization;

namespace WART_Core.Entity
{
    /// <summary>
    /// Represents an event in the WART system, typically capturing HTTP request and response data, 
    /// along with additional metadata such as timestamps and remote addresses.
    /// This class is serializable and designed to be used for logging or transmitting event data.
    /// </summary>
    [Serializable]
    public class WartEvent
    {
        /// <summary>
        /// Unique identifier for this event instance.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Local timestamp when the event was created. Intended to mirror
        /// <see cref="UtcTimeStamp"/> converted to the local time zone.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// UTC timestamp when the event was created. Preferred for storage and comparisons.
        /// </summary>
        public DateTime UtcTimeStamp { get; set; }

        /// <summary>
        /// HTTP verb used by the request (e.g., GET, POST, PUT, DELETE).
        /// </summary>
        public string HttpMethod { get; set; } = string.Empty;

        /// <summary>
        /// HTTP path or route template of the request (e.g., "/api/orders/42").
        /// </summary>
        public string HttpPath { get; set; } = string.Empty;

        /// <summary>
        /// Remote client address (e.g., IP or "host:port") that originated the request.
        /// </summary>
        public string RemoteAddress { get; set; } = string.Empty;

        /// <summary>
        /// Request payload serialized as JSON text. May contain either a JSON object or array.
        /// Left as raw string to preserve the original body.
        /// </summary>
        [JsonConverter(typeof(JsonArrayOrObjectStringConverter))]
        public string JsonRequestPayload { get; set; } = string.Empty;

        /// <summary>
        /// Response payload serialized as JSON text. May contain either a JSON object or array.
        /// Left as raw string to preserve the original body.
        /// </summary>
        [JsonConverter(typeof(JsonArrayOrObjectStringConverter))]
        public string JsonResponsePayload { get; set; } = string.Empty;

        /// <summary>
        /// Optional, free-form additional information (tags, notes, etc.).
        /// </summary>
        public string ExtraInfo { get; set; } = string.Empty;

        /// <summary>
        /// Private constructor used for JSON deserialization.
        /// This constructor is necessary for deserializing a `WartEvent` object from JSON.
        /// </summary>
        private WartEvent() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WartEvent"/> class with the specified HTTP method, path, and remote address.
        /// </summary>
        /// <param name="httpMethod">The HTTP method (e.g., GET, POST).</param>
        /// <param name="httpPath">The path of the HTTP request.</param>
        /// <param name="remoteAddress">The remote address (IP) from which the request originated.</param>
        /// <summary>
        /// Initializes a new instance with method, path and remote address.
        /// </summary>
        public WartEvent(string httpMethod, string httpPath, string remoteAddress)
        {
            EventId = Guid.NewGuid();

            var utcNow = DateTime.UtcNow;
            UtcTimeStamp = utcNow;
            TimeStamp = utcNow.ToLocalTime();

            HttpMethod = httpMethod ?? string.Empty;
            HttpPath = httpPath ?? string.Empty;
            RemoteAddress = remoteAddress ?? string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WartEvent"/> class with the specified HTTP method, path, remote address,
        /// request payload, and response payload. This constructor is typically used when logging both the request and response data.
        /// </summary>
        /// <param name="request">The request object to be serialized into JSON format.</param>
        /// <param name="response">The response object to be serialized into JSON format.</param>
        /// <param name="httpMethod">The HTTP method (e.g., GET, POST).</param>
        /// <param name="httpPath">The path of the HTTP request.</param>
        /// <param name="remoteAddress">The remote address (IP) from which the request originated.</param>
        public WartEvent(object request, object response, string httpMethod, string httpPath, string remoteAddress)
        {
            EventId = Guid.NewGuid();

            var utcNow = DateTime.UtcNow;
            UtcTimeStamp = utcNow;
            TimeStamp = utcNow.ToLocalTime();

            HttpMethod = httpMethod ?? string.Empty;
            HttpPath = httpPath ?? string.Empty;
            RemoteAddress = remoteAddress ?? string.Empty;

            JsonRequestPayload = SerializationHelper.Serialize(request);
            JsonResponsePayload = SerializationHelper.Serialize(response);
        }

        /// <summary>
        /// Returns the string representation of the <see cref="WartEvent"/> object as a JSON string.
        /// The entire event is serialized to JSON using the <see cref="SerializationHelper"/>.
        /// </summary>
        /// <returns>A JSON string representation of the <see cref="WartEvent"/>.</returns>
        public override string ToString()
        {
            return SerializationHelper.Serialize(this);
        }

        /// <summary>
        /// Deserializes the request payload (JSON) into an object of the specified type.
        /// This method is useful for retrieving the original request object from the stored payload.
        /// </summary>
        /// <typeparam name="T">The type to which the request payload will be deserialized.</typeparam>
        /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
        public T GetRequestObject<T>() where T : class
        {
            return SerializationHelper.Deserialize<T>(JsonRequestPayload);
        }

        /// <summary>
        /// Deserializes the response payload (JSON) into an object of the specified type.
        /// This method is useful for retrieving the original response object from the stored payload.
        /// </summary>
        /// <typeparam name="T">The type to which the response payload will be deserialized.</typeparam>
        /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
        public T GetResponseObject<T>() where T : class
        {
            return SerializationHelper.Deserialize<T>(JsonResponsePayload);
        }

        /// <summary>
        /// Converts the WartEvent instance into a dictionary for flexible logging or data analysis.
        /// </summary>
        /// <returns>A dictionary representation of the event.</returns>
        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                { "EventId", EventId },
                { "TimeStamp", TimeStamp },
                { "UtcTimeStamp", UtcTimeStamp },
                { "HttpMethod", HttpMethod },
                { "HttpPath", HttpPath },
                { "RemoteAddress", RemoteAddress },
                { "JsonRequestPayload", JsonRequestPayload },
                { "JsonResponsePayload", JsonResponsePayload },
                { "ExtraInfo", ExtraInfo }
            };
        }
    }
}
