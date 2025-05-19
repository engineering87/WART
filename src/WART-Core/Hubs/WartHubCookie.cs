// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace WART_Core.Hubs
{
    /// <summary>
    /// The WART SignalR hub with Cookie-based authentication.
    /// </summary>
    [Authorize]
    public class WartHubCookie : WartHubBase
    {
        public WartHubCookie(ILogger<WartHubCookie> logger) : base(logger) { }
    }
}