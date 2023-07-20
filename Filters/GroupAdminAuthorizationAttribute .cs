using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hatebook.Filters
{
    public class GroupAdminAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // Retrieve the group ID from the route parameters or query string
            var groupName = context.HttpContext.Request.RouteValues["groupName"]?.ToString()
            ?? context.HttpContext.Request.Query["groupName"].ToString();
            var groupId = GetGroupNameById(groupName, context);

            if (groupId == Guid.Empty)
            {
                // Group ID is missing or invalid, return a bad request response
                context.Result = new BadRequestObjectResult("Invalid group ID");
                return;
            }

            var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                if (!IsUserGroupAdmin(userId, groupId, context))
                {
                    context.Result = new ForbidResult();
                }
            }
        }

        private static bool IsUserGroupAdmin(string userId, Guid groupId, ActionExecutedContext context)
        {
            var _dependency = context.HttpContext.RequestServices.GetService<IControllerConstructor>();
            if (_dependency != null)
            {
                var isAdmin = _dependency.Context.GroupAdmins.Any(ga => ga.UserId == userId && ga.GroupId == groupId);
                return isAdmin;
            }
            return false;
        }

        public Guid GetGroupNameById(string groupName, ActionExecutedContext context)
        {
            var _dependency = context.HttpContext.RequestServices.GetService<IControllerConstructor>();
            if (_dependency != null)
            {
                var groupId = _dependency.Context.groups.FirstOrDefault(g => g.Name == groupName);
                if (groupId != null)
                {
                    return groupId.Id;
                }
            }
            return Guid.Empty;
        }
    }
}
