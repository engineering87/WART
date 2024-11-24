// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WART_Core.Hubs;

namespace WART_Core.Controllers
{
    /// <summary>
    /// The WART Controller with JWT authentication
    /// </summary>
    public class WartControllerJwt : WartBaseController<WartHubJwt>
    {
        public WartControllerJwt(IHubContext<WartHubJwt> hubContext, ILogger<WartControllerJwt> logger)
            : base(hubContext, logger)
        {
        }
    }
}
