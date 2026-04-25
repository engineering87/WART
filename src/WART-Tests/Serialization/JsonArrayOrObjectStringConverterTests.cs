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

        private class Wrapper { public string? Payload { get; set; } }

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

        [Fact]
        public void Read_NullToken_ReturnsNull()
        {
            var json = "{\"Payload\": null}";
            var wrapper = JsonSerializer.Deserialize<Wrapper>(json, _opts);
            Assert.Null(wrapper!.Payload);
        }

        [Fact]
        public void Read_NumberToken_ReturnsStringRepresentation()
        {
            var json = "{\"Payload\": 42}";
            var wrapper = JsonSerializer.Deserialize<Wrapper>(json, _opts);
            Assert.Equal("42", wrapper!.Payload);
        }

        [Fact]
        public void Read_DecimalNumberToken_ReturnsStringRepresentation()
        {
            var json = "{\"Payload\": 3.14}";
            var wrapper = JsonSerializer.Deserialize<Wrapper>(json, _opts);
            Assert.Equal("3.14", wrapper!.Payload);
        }

        [Fact]
        public void Read_BooleanTrueToken_ReturnsString()
        {
            var json = "{\"Payload\": true}";
            var wrapper = JsonSerializer.Deserialize<Wrapper>(json, _opts);
            Assert.Equal("True", wrapper!.Payload);
        }

        [Fact]
        public void Read_BooleanFalseToken_ReturnsString()
        {
            var json = "{\"Payload\": false}";
            var wrapper = JsonSerializer.Deserialize<Wrapper>(json, _opts);
            Assert.Equal("False", wrapper!.Payload);
        }

        [Fact]
        public void Read_ArrayToken_ReturnsRawJson()
        {
            var json = "{\"Payload\": [1,2,3]}";
            var wrapper = JsonSerializer.Deserialize<Wrapper>(json, _opts);
            Assert.Equal("[1,2,3]", wrapper!.Payload);
        }

        [Fact]
        public void Write_NullValue_WritesNull()
        {
            var obj = new Wrapper { Payload = null! };
            var json = JsonSerializer.Serialize(obj, _opts);
            Assert.Contains("\"Payload\":null", json);
        }

        [Fact]
        public void Write_EmptyString_WritesEmptyString()
        {
            var obj = new Wrapper { Payload = string.Empty };
            var json = JsonSerializer.Serialize(obj, _opts);
            Assert.Contains("\"Payload\":\"\"", json);
        }

        [Fact]
        public void Read_StringToken_ReturnsString()
        {
            var json = "{\"Payload\": \"simple text\"}";
            var wrapper = JsonSerializer.Deserialize<Wrapper>(json, _opts);
            Assert.Equal("simple text", wrapper!.Payload);
        }

        [Fact]
        public void Roundtrip_ObjectPayload_PreservesStructure()
        {
            var obj = new Wrapper { Payload = "{\"key\":\"value\"}" };
            var json = JsonSerializer.Serialize(obj, _opts);
            var deserialized = JsonSerializer.Deserialize<Wrapper>(json, _opts);
            Assert.Equal("{\"key\":\"value\"}", deserialized!.Payload);
        }
    }
}
