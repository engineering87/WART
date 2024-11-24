// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Logging;

namespace WART_Core.Hubs
{
    /// <summary>
    /// The WART SignalR hub.
    /// </summary>
    public class WartHub : WartHubBase
    {
        public WartHub(ILogger<WartHub> logger) : base(logger) { }
    }
}
