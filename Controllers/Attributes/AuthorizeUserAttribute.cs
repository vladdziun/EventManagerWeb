using EventManagerWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace EventManagerWeb.Controllers.Attributes
{
    public class AuthorizeUserAttribute: ActionFilterAttribute
    {
        public AuthorizeUserAttribute() 
        {

        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new RedirectResult("/");
            }
        }
    }

}
