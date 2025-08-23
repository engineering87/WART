// (c) 2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using WART_Core.Controllers;
using WART_Core.Filters;
using WART_Core.Services;

namespace WART_Tests.Controllers
{
    public class WartBaseControllerTests
    {
        public class TestHub : Hub { }

        private class SutController : WartBaseController<TestHub>
        {
            public SutController(IHubContext<TestHub> hubContext, ILogger logger) : base(hubContext, logger) { }
            public IActionResult OkResult() => new OkObjectResult(new { ok = true });
        }

        private (SutController ctrl, ActionExecutingContext aex, ActionExecutedContext aed, WartEventQueueService queue) Arrange(ObjectResult result, IList<IFilterMetadata> filters)
        {
            var services = new ServiceCollection();
            var queue = new WartEventQueueService();
            services.AddSingleton(queue);
            var sp = services.BuildServiceProvider();

            var http = new DefaultHttpContext { RequestServices = sp };
            http.Request.Method = "GET";
            http.Request.Path = "/api/test";

            var actionCtx = new ActionContext(http, new Microsoft.AspNetCore.Routing.RouteData(),
                new ControllerActionDescriptor(), modelState: new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary());

            var aex = new ActionExecutingContext(actionCtx, new List<IFilterMetadata>(), new Dictionary<string, object> { { "id", 1 } }, controller: null);
            var aed = new ActionExecutedContext(actionCtx, filters, controller: null) { Result = result };

            var hubCtx = new Mock<IHubContext<TestHub>>().Object;
            var logger = new Mock<ILogger>().Object;
            var ctrl = new SutController(hubCtx, logger);

            return (ctrl, aex, aed, queue);
        }

        [Fact]
        public void Enqueues_On_ObjectResult_Without_Exclude()
        {
            var filters = new List<IFilterMetadata>();
            var (ctrl, aex, aed, queue) = Arrange(new OkObjectResult(new { x = 1 }), filters);

            ctrl.OnActionExecuting(aex);
            ctrl.OnActionExecuted(aed);

            Assert.Equal(1, queue.Count);
            Assert.True(queue.TryPeek(out var item));
            Assert.NotNull(item.WartEvent);
            Assert.Equal("GET", item.WartEvent.HttpMethod);
            Assert.Equal("/api/test", item.WartEvent.HttpPath);
        }

        [Fact]
        public void DoesNotEnqueue_When_Excluded()
        {
            var filters = new List<IFilterMetadata> { new ExcludeWartAttribute() };
            var (ctrl, aex, aed, queue) = Arrange(new OkObjectResult(new { x = 1 }), filters);

            ctrl.OnActionExecuting(aex);
            ctrl.OnActionExecuted(aed);

            Assert.True(queue.IsEmpty);
        }

        [Fact]
        public void Preserves_Filters_For_Worker_GroupUsage()
        {
            var filters = new List<IFilterMetadata> { new GroupWartAttribute("g1", "g2") };
            var (ctrl, aex, aed, queue) = Arrange(new OkObjectResult(new { x = 1 }), filters);

            ctrl.OnActionExecuting(aex);
            ctrl.OnActionExecuted(aed);

            Assert.True(queue.TryPeek(out var item));
            Assert.Contains(item.Filters, f => f is GroupWartAttribute);
        }
    }
}
