// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WART_Core.Hubs;

namespace WART_Core.Controllers
{
    /// <summary>
    /// The WART Controller with Cookie authentication
    /// </summary>
    public class WartControllerCookie : WartBaseController<WartHubCookie>
    {
        public WartControllerCookie(IHubContext<WartHubCookie> hubContext, ILogger<WartControllerCookie> logger)
            : base(hubContext, logger)
        {
        }
    }
}
