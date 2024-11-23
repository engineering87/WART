// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace WART_Core.Filters
{
    /// <summary>
    /// A custom action filter attribute used to direct SignalR events to a specific SignalR group or multiple groups.
    /// This attribute allows specifying a list of group names, which can be used to target SignalR events
    /// to one or more SignalR groups during the execution of an action.
    /// </summary>
    public class GroupWartAttribute : ActionFilterAttribute
    {
        public IReadOnlyList<string> GroupNames { get; }

        // Constructor accepting a list of group names
        public GroupWartAttribute(params string[] groupNames)
        {
            GroupNames = new List<string>(groupNames);
        }

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
