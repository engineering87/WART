using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace WART_Core.Filters
{
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
