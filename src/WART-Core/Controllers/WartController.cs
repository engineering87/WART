// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WART_Core.Entity;
using WART_Core.Hubs;

namespace WART_Core.Controllers
{
    /// <summary>
    /// The WART Controller
    /// </summary>
    public class WartController : Controller
    {
        private readonly ILogger<WartController> _logger;
        private readonly IHubContext<WartHub> _hubContext;
        private const string RouteDataKey = "REQUEST";

        public WartController(IHubContext<WartHub> hubContext, ILogger<WartController> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        /// <summary>
        /// WART OnActionExecuting override
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // add the request objects to RouteData
            context?.RouteData.Values.Add(RouteDataKey, context.ActionArguments);

            base.OnActionExecuting(context);
        }

        /// <summary>
        /// WART OnActionExecuted override
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context?.Result is ObjectResult objectResult)
            {
                // get the request objects from RouteData
                var request = context.RouteData.Values[RouteDataKey];
                var httpMethod = context.HttpContext?.Request.Method;
                var httpPath = context.HttpContext?.Request.Path;
                var remoteAddress = context.HttpContext?.Connection.RemoteIpAddress.ToString();
                // get the object response
                var response = objectResult.Value;
                // create the new WartEvent and broadcast to all clients
                var wartEvent = new WartEvent(request, response, httpMethod, httpPath, remoteAddress);
                _hubContext?.Clients.All.SendAsync("Send", wartEvent.ToString());
                _logger?.LogInformation("WartEvent", wartEvent.ToString());
            }

            base.OnActionExecuted(context);
        }
    }
}
