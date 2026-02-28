// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using WART_Core.Entity;

namespace WART_Tests.Entity
{
    public class WartEventTests
    {
        [Fact]
        public void WartEvent_ConstructorWithParameters_ShouldSetProperties()
        {
            // Arrange
            string httpMethod = "GET";
            string httpPath = "/api/users";
            string remoteAddress = "127.0.0.1";

            // Act
            var wartEvent = new WartEvent(httpMethod, httpPath, remoteAddress);

            // Assert
            Assert.NotEqual(Guid.Empty, wartEvent.EventId);
            Assert.NotEqual(default, wartEvent.TimeStamp);
            Assert.NotEqual(default, wartEvent.UtcTimeStamp);
            Assert.Equal(httpMethod, wartEvent.HttpMethod);
            Assert.Equal(httpPath, wartEvent.HttpPath);
            Assert.Equal(remoteAddress, wartEvent.RemoteAddress);
            Assert.True(string.IsNullOrEmpty(wartEvent.JsonRequestPayload));
            Assert.True(string.IsNullOrEmpty(wartEvent.JsonResponsePayload));
            Assert.True(string.IsNullOrEmpty(wartEvent.ExtraInfo));
        }

        [Fact]
        public void WartEvent_ConstructorWithRequestAndResponse_ShouldSetProperties()
        {
            // Arrange
            string httpMethod = "POST";
            string httpPath = "/api/users";
            string remoteAddress = "127.0.0.1";
            var request = new { Name = "John", Age = 30 };
            var response = new { Success = true };

            // Act
            var wartEvent = new WartEvent(request, response, httpMethod, httpPath, remoteAddress);

            // Assert
            Assert.NotEqual(Guid.Empty, wartEvent.EventId);
            Assert.NotEqual(default, wartEvent.TimeStamp);
            Assert.NotEqual(default, wartEvent.UtcTimeStamp);
            Assert.Equal(httpMethod, wartEvent.HttpMethod);
            Assert.Equal(httpPath, wartEvent.HttpPath);
            Assert.Equal(remoteAddress, wartEvent.RemoteAddress);
            Assert.False(string.IsNullOrEmpty(wartEvent.JsonRequestPayload));
            Assert.False(string.IsNullOrEmpty(wartEvent.JsonResponsePayload));
            Assert.True(string.IsNullOrEmpty(wartEvent.ExtraInfo));
        }

        [Fact]
        public void WartEvent_ToString_ShouldReturnJsonSerialization()
        {
            // Arrange
            string httpMethod = "GET";
            string httpPath = "/api/users";
            string remoteAddress = "127.0.0.1";
            var wartEvent = new WartEvent(httpMethod, httpPath, remoteAddress);

            // Act
            var jsonString = wartEvent.ToString();

            // Assert
            Assert.NotNull(jsonString);
            Assert.Contains(httpMethod, jsonString);
            Assert.Contains(httpPath, jsonString);
            Assert.Contains(remoteAddress, jsonString);
        }

        [Fact]
        public void WartEvent_GetRequestObject_ShouldDeserializeJsonRequest()
        {
            // Arrange
            string httpMethod = "POST";
            string httpPath = "/api/users";
            string remoteAddress = "127.0.0.1";
            var request = new { Name = "John", Age = 30 };
            var response = new { Success = true };
            var wartEvent = new WartEvent(request, response, httpMethod, httpPath, remoteAddress);

            // Act
            var deserializedRequest = wartEvent.GetRequestObject<object>();

            // Assert
            Assert.NotNull(deserializedRequest);
        }

        [Fact]
        public void WartEvent_GetResponseObject_ShouldDeserializeJsonResponse()
        {
            // Arrange
            string httpMethod = "POST";
            string httpPath = "/api/users";
            string remoteAddress = "127.0.0.1";
            var request = new { Name = "John", Age = 30 };
            var response = new { Success = true };
            var wartEvent = new WartEvent(request, response, httpMethod, httpPath, remoteAddress);

            // Act
            var deserializedResponse = wartEvent.GetResponseObject<object>();

            // Assert
            Assert.NotNull(deserializedResponse);
        }

        [Fact]
        public void WartEvent_Constructor_NullParameters_DefaultsToEmpty()
        {
            // Act
            var wartEvent = new WartEvent(null!, null!, null!);

            // Assert
            Assert.Equal(string.Empty, wartEvent.HttpMethod);
            Assert.Equal(string.Empty, wartEvent.HttpPath);
            Assert.Equal(string.Empty, wartEvent.RemoteAddress);
        }

        [Fact]
        public void WartEvent_FullConstructor_NullRequestResponse_DoesNotThrow()
        {
            // Act
            var wartEvent = new WartEvent(null, null, "PUT", "/api/items", "10.0.0.1");

            // Assert
            Assert.NotEqual(Guid.Empty, wartEvent.EventId);
            Assert.Equal("PUT", wartEvent.HttpMethod);
        }

        [Fact]
        public void WartEvent_ToDictionary_ContainsAllKeys()
        {
            // Arrange
            var wartEvent = new WartEvent("DELETE", "/api/items/1", "192.168.0.1")
            {
                ExtraInfo = "test-info"
            };

            // Act
            var dict = wartEvent.ToDictionary();

            // Assert
            Assert.Equal(9, dict.Count);
            Assert.Equal(wartEvent.EventId, dict["EventId"]);
            Assert.Equal("DELETE", dict["HttpMethod"]);
            Assert.Equal("/api/items/1", dict["HttpPath"]);
            Assert.Equal("192.168.0.1", dict["RemoteAddress"]);
            Assert.Equal("test-info", dict["ExtraInfo"]);
            Assert.True(dict.ContainsKey("TimeStamp"));
            Assert.True(dict.ContainsKey("UtcTimeStamp"));
            Assert.True(dict.ContainsKey("JsonRequestPayload"));
            Assert.True(dict.ContainsKey("JsonResponsePayload"));
        }

        [Fact]
        public void WartEvent_Timestamps_AreConsistent()
        {
            // Arrange & Act
            var before = DateTime.UtcNow;
            var wartEvent = new WartEvent("GET", "/", "::1");
            var after = DateTime.UtcNow;

            // Assert
            Assert.InRange(wartEvent.UtcTimeStamp, before, after);
            Assert.Equal(wartEvent.UtcTimeStamp.ToLocalTime(), wartEvent.TimeStamp);
        }

        [Fact]
        public void WartEvent_EventId_IsUnique()
        {
            var a = new WartEvent("GET", "/", "::1");
            var b = new WartEvent("GET", "/", "::1");

            Assert.NotEqual(a.EventId, b.EventId);
        }
    }
}
