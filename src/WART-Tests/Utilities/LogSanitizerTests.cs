// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using WART_Core.Utilities;

namespace WART_Tests.Utilities
{
    public class LogSanitizerTests
    {
        [Fact]
        public void Sanitize_Null_ReturnsNull()
        {
            Assert.Null(LogSanitizer.Sanitize(null!));
        }

        [Fact]
        public void Sanitize_Empty_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, LogSanitizer.Sanitize(string.Empty));
        }

        [Fact]
        public void Sanitize_NormalString_ReturnsUnchanged()
        {
            Assert.Equal("hello world", LogSanitizer.Sanitize("hello world"));
        }

        [Fact]
        public void Sanitize_RemovesNewlines()
        {
            Assert.Equal("ab", LogSanitizer.Sanitize("a\nb"));
            Assert.Equal("ab", LogSanitizer.Sanitize("a\r\nb"));
        }

        [Fact]
        public void Sanitize_RemovesTabsAndControlChars()
        {
            Assert.Equal("ab", LogSanitizer.Sanitize("a\tb"));
            Assert.Equal("ab", LogSanitizer.Sanitize("a\0b"));
        }

        [Fact]
        public void Sanitize_PreservesUnicodeCharacters()
        {
            Assert.Equal("café ñ 日本語", LogSanitizer.Sanitize("café ñ 日本語"));
        }
    }
}
