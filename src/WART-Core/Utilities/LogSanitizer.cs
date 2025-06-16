// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Linq;

namespace WART_Core.Utilities
{
    public static class LogSanitizer
    {
        /// <summary>
        /// Sanitizes a string to make it safe for logging by removing control characters.
        /// </summary>
        /// <param name="input">The input string to sanitize.</param>
        /// <returns>The sanitized string with control characters removed.</returns>
        public static string Sanitize(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return new string(input
                .Where(c => !char.IsControl(c))
                .ToArray());
        }
    }
}
