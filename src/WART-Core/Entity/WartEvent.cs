// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Newtonsoft.Json;
using System;

namespace WART_Core.Entity
{
    [Serializable]
    public class WartEvent
    {
        public Guid EventId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string HttpMethod { get; set; }
        public string HttpPath { get; set; }
        public string RemoteAddress { get; set; }
        public string JsonRequestPayload { get; set; }
        public string JsonResponsePayload { get; set; }
        public string ExtraInfo { get; set; }

        /// <summary>
        /// Empty private constructor for JSON deserialization
        /// </summary>
        private WartEvent() { }

        /// <summary>
        /// Create a new WartEvent
        /// <param name="httpMethod"></param>
        /// <param name="httpPath"></param>
        /// <param name="remoteAddress"></param>
        /// </summary>
        public WartEvent(string httpMethod, string httpPath, string remoteAddress)
        {
            this.EventId = Guid.NewGuid();
            this.TimeStamp = DateTime.Now;
            this.HttpMethod = httpMethod;
            this.HttpPath = httpPath;
            this.RemoteAddress = remoteAddress;
        }

        /// <summary>
        /// Create a new WartEvent
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="httpMethod"></param>
        /// <param name="httpPath"></param>
        /// <param name="remoteAddress"></param>
        public WartEvent(object request, object response, string httpMethod, string httpPath, string remoteAddress)
        {
            this.EventId = Guid.NewGuid();
            this.TimeStamp = DateTime.Now;
            this.HttpMethod = httpMethod;
            this.HttpPath = httpPath;
            this.RemoteAddress = remoteAddress;
            this.JsonRequestPayload = request != null ? JsonConvert.SerializeObject(request) : string.Empty;
            this.JsonResponsePayload = response != null ? JsonConvert.SerializeObject(response) : string.Empty;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Deserialize the JSON request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetRequestObject<T>() where T : class
        {
            return string.IsNullOrEmpty(JsonRequestPayload) ?
                JsonConvert.DeserializeObject<T>(JsonRequestPayload) : null;
        }

        /// <summary>
        /// Deserialize the JSON response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetResponseObject<T>() where T : class
        {
            return string.IsNullOrEmpty(JsonResponsePayload) ?
                JsonConvert.DeserializeObject<T>(JsonResponsePayload) : null;
        }
    }
}
