using Framework.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Framework.WebUtilities
{
    /// <summary>
	/// Assign an API controller/action a set of permissions that must all be held by this user.
	/// Multiple attributes can be applied to require one of many sets.
	/// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequireApiPermissionAttribute : Attribute, IActionFilter
    {
        /// <summary>
		/// The required permission set of this controller/action.
		/// </summary>
		public Permission[] RequiredPermissions { get; private set; }

        public RequireApiPermissionAttribute(params Permission[] permissions)
        {
            RequiredPermissions = permissions;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var claim = context.HttpContext.User.FindFirst(Constants.PermissionClaimType);

            if (claim != null)
            {
                var userPermissions = JsonSerializer.Deserialize<Permission[]>(claim.Value);

                if (!hasPermissions(userPermissions))
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                }
            }    
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        private bool hasPermissions(Permission[] userPermissions)
        {
            if (userPermissions == null)
            {
                return false;
            }

            var required = RequiredPermissions.Sum(p => (int)p);
            var userHas = userPermissions.Sum(p => (int)p);

            // Use bitwise & operator to check for permissions more efficient than nested loops
            return (required & userHas) == required;
        }
    }
}
