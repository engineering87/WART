// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WART_Core.Hubs
{
    /// <summary>
    /// The WART SignalR hub with JWT authentication.
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WartHubJwt : Hub
    {
        private readonly ILogger<WartHubJwt> _logger;

        public WartHubJwt(ILogger<WartHubJwt> logger)
        {
            this._logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            _logger?.LogInformation($"OnConnect {Context.ConnectionId}");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _logger?.LogInformation($"OnDisconnect {Context.ConnectionId}");

            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Broadcast the WartEvent to all clients.
        /// </summary>
        /// <param name="jsonWartEvent"></param>
        /// <returns></returns>
        public Task Send(string jsonWartEvent)
        {
            _logger?.LogInformation($"Send {jsonWartEvent}");

            return Clients.All.SendAsync("Send", jsonWartEvent);
        }
    }
}
