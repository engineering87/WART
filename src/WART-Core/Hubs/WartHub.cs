// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Logging;

namespace WART_Core.Hubs
{
    /// <summary>
    /// The WART SignalR hub (unauthenticated).
    /// Group subscriptions are not permitted on this hub because connections
    /// carry no verified identity. Use <see cref="WartHubJwt"/> or
    /// <see cref="WartHubCookie"/> when group-scoped event delivery is required.
    /// </summary>
    public class WartHub : WartHubBase
    {
        public WartHub(ILogger<WartHub> logger) : base(logger) { }

        /// <inheritdoc />
        protected override bool IsGroupSubscriptionAllowed() => false;
    }
}
