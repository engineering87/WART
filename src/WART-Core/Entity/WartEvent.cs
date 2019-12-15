// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Newtonsoft.Json;
using System;

namespace WART_Core.Entity
{
    [Serializable]
    public class WartEvent
    {
        public Guid EventId { get; }
        public DateTime TimeStamp { get; }
        public string HttpMethod { get; set; }
        public string JsonRequestPayload { get; set; }
        public string JsonResponsePayload { get; set; }

        /// <summary>
        /// Create a new WartEvent
        /// <param name="httpMethod"></param>
        /// </summary>
        public WartEvent(string httpMethod)
        {
            this.EventId = Guid.NewGuid();
            this.TimeStamp = DateTime.Now;
            this.HttpMethod = httpMethod;
        }

        /// <summary>
        /// Create a new WartEvent
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="httpMethod"></param>
        public WartEvent(object request, object response, string httpMethod)
        {
            this.EventId = Guid.NewGuid();
            this.TimeStamp = DateTime.Now;
            this.HttpMethod = httpMethod;
            this.JsonRequestPayload = request != null ? JsonConvert.SerializeObject(request) : string.Empty;
            this.JsonResponsePayload = response != null ? JsonConvert.SerializeObject(response) : string.Empty;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
