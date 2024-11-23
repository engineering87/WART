// (c) 2019 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Mvc.Filters;

namespace WART_Core.Filters
{
    /// <summary>
    /// A custom action filter that prevents the propagation of any SignalR events related to the action.
    /// When applied to an action, this filter ensures that no SignalR events are triggered or broadcasted
    /// as a result of the execution of the action.
    /// </summary>
    public class ExcludeWartAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }
    }
}
