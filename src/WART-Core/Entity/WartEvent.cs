// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
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
        public Guid EventId { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime UtcTimeStamp { get; set; }
        public string HttpMethod { get; set; }
        public string HttpPath { get; set; }
        public string RemoteAddress { get; set; }
        [JsonConverter(typeof(JsonArrayOrObjectStringConverter))]
        public string JsonRequestPayload { get; set; }
        [JsonConverter(typeof(JsonArrayOrObjectStringConverter))]
        public string JsonResponsePayload { get; set; }
        public string ExtraInfo { get; set; }

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
        public WartEvent(string httpMethod, string httpPath, string remoteAddress)
        {
            this.EventId = Guid.NewGuid();
            this.TimeStamp = DateTime.Now;
            this.UtcTimeStamp = DateTime.UtcNow;
            this.HttpMethod = httpMethod;
            this.HttpPath = httpPath;
            this.RemoteAddress = remoteAddress;
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
            this.EventId = Guid.NewGuid();
            this.TimeStamp = DateTime.Now;
            this.UtcTimeStamp = DateTime.UtcNow;
            this.HttpMethod = httpMethod;
            this.HttpPath = httpPath;
            this.RemoteAddress = remoteAddress;
            this.JsonRequestPayload = SerializationHelper.Serialize(request);
            this.JsonResponsePayload = SerializationHelper.Serialize(response);
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
    }    
}
