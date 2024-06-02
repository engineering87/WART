// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace WART_Core.Entity.Tests
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
            Assert.NotEqual(default(DateTime), wartEvent.TimeStamp);
            Assert.NotEqual(default(DateTime), wartEvent.UtcTimeStamp);
            Assert.Equal(httpMethod, wartEvent.HttpMethod);
            Assert.Equal(httpPath, wartEvent.HttpPath);
            Assert.Equal(remoteAddress, wartEvent.RemoteAddress);
            Assert.Null(wartEvent.JsonRequestPayload);
            Assert.Null(wartEvent.JsonResponsePayload);
            Assert.Null(wartEvent.ExtraInfo);
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
            Assert.NotEqual(default(DateTime), wartEvent.TimeStamp);
            Assert.NotEqual(default(DateTime), wartEvent.UtcTimeStamp);
            Assert.Equal(httpMethod, wartEvent.HttpMethod);
            Assert.Equal(httpPath, wartEvent.HttpPath);
            Assert.Equal(remoteAddress, wartEvent.RemoteAddress);
            Assert.NotNull(wartEvent.JsonRequestPayload);
            Assert.NotNull(wartEvent.JsonResponsePayload);
            Assert.Null(wartEvent.ExtraInfo);
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
    }
}
