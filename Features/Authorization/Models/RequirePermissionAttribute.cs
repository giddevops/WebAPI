using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates this simple attribute [RequirePermission("PermissionName")]
    /// This will make sure that people are in groups with that permission before they can perform that method
    /// </summary>
    public class RequirePermissionAttribute : TypeFilterAttribute
    {
        public RequirePermissionAttribute(string permissionType) : base(typeof(RequirePermissionFilter))
        {
            Arguments = new object[] { permissionType };
        }
    }

    public class RequirePermissionFilter : IAuthorizationFilter
    {
        readonly String requestedPermission;
        AppDBContext dbContext;

        public RequirePermissionFilter(string permission, AppDBContext appDBContext)
        {
            requestedPermission = permission;
            this.dbContext = appDBContext;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {   
            var permission = this.dbContext.Permissions.AsNoTracking().FirstOrDefault(p => p.Name == this.requestedPermission);
            List<string> allowedGroups = JsonConvert.DeserializeObject<List<string>>(permission.AllowedGroups);
            List<string> userGroups = context.HttpContext.User.Claims.Where(c => c.Type == "groups").Select(c => c.Value).ToList<string>();

            var matchingGroups = allowedGroups.Intersect(userGroups);

            if (matchingGroups.Count() == 0)
            {
                var result = new ContentResult();
                result.Content = "You do not have the required permission to access that resource";
                result.StatusCode = 403;
                context.Result = result;
                return;
            }
        }
    }
}
