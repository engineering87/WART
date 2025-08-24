// (c) 2024 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WART_Core.Filters
{
    /// <summary>
    /// A custom action filter attribute used to direct SignalR events to a specific SignalR group or multiple groups.
    /// This attribute allows specifying a list of group names, which can be used to target SignalR events
    /// to one or more SignalR groups during the execution of an action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class GroupWartAttribute : ActionFilterAttribute
    {
        public IReadOnlyList<string> GroupNames { get; }

        // Constructor accepting a list of group names
        public GroupWartAttribute(params string[] groupNames)
        {
            var cleaned = (groupNames ?? [])
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            GroupNames = new ReadOnlyCollection<string>(cleaned);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (GroupNames.Count > 0)
            {
                context.HttpContext.Items["WartGroups"] = GroupNames;
            }

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }
    }
}
