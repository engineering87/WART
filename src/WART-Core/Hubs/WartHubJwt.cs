// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace WART_Core.Hubs
{
    /// <summary>
    /// The WART SignalR hub with JWT authentication.
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WartHubJwt : WartHubBase
    {
        public WartHubJwt(ILogger<WartHubJwt> logger) : base(logger) { }
    }
}
