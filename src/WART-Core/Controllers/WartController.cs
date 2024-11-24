// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WART_Core.Hubs;

namespace WART_Core.Controllers
{
    /// <summary>
    /// The WART Controller
    /// </summary>
    public class WartController : WartBaseController<WartHub>
    {
        public WartController(IHubContext<WartHub> hubContext, ILogger<WartController> logger)
            : base(hubContext, logger)
        {
        }
    }
}
