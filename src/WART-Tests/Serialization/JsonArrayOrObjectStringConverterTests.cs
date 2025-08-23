// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Text.Json;
using WART_Core.Entity;
using WART_Core.Serialization;

namespace WART_Tests.Serialization
{
    public class JsonArrayOrObjectStringConverterTests
    {
        private readonly JsonSerializerOptions _opts = new()
        {
            Converters = { new JsonArrayOrObjectStringConverter() }
        };

        private class Wrapper { public string Payload { get; set; } }

        [Fact]
        public void Write_String_RemainsString()
        {
            var obj = new Wrapper { Payload = "hello" };
            var json = JsonSerializer.Serialize(obj, _opts);
            Assert.Contains("\"Payload\":\"hello\"", json);
        }

        [Fact]
        public void Write_ObjectString_WritesAsObject()
        {
            var obj = new Wrapper { Payload = "{\"a\":1}" };
            var json = JsonSerializer.Serialize(obj, _opts);
            Assert.Contains("\"Payload\":{\"a\":1}", json);
        }

        [Fact]
        public void Write_ArrayString_WritesAsArray()
        {
            var obj = new Wrapper { Payload = "[1,2,3]" };
            var json = JsonSerializer.Serialize(obj, _opts);
            Assert.Contains("\"Payload\":[1,2,3]", json);
        }

        [Fact]
        public void Write_InvalidJson_FallsBackToString()
        {
            var obj = new Wrapper { Payload = "{not json}" };
            var json = JsonSerializer.Serialize(obj, _opts);
            Assert.Contains("\"Payload\":\"{not json}\"", json);
        }

        [Fact]
        public void Read_ObjectToken_ReturnsRawJson()
        {
            var json = "{\"Payload\": {\"x\":42}}";
            var wrapper = JsonSerializer.Deserialize<Wrapper>(json, _opts);
            Assert.Equal("{\"x\":42}", wrapper.Payload);
        }

        [Fact]
        public void WartEvent_ToString_IsValidJson()
        {
            var e = new WartEvent("GET", "/api/test", "127.0.0.1") { ExtraInfo = "ok" };
            var json = e.ToString();
            using var _ = JsonDocument.Parse(json);
        }
    }
}
