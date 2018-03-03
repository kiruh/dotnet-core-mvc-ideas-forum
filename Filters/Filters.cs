using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Ideas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace FiltersSample.Filters
{
    // public class CustomAuthorizationFilterFactory : IFilterFactory
    // {
    //     public bool IsReusable => false;

    //     public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    //     {
    //         // manually find and inject necessary dependencies.
    //         var context = (IdeasContext)serviceProvider.GetService(typeof(IdeasContext));
    //         return new CustomAuthorization(context);
    //     }
    // }

    // public class CustomAuthorization : ActionFilterAttribute
    // {
    //     private readonly IdeasContext _context;

    //     public CustomAuthorization(IdeasContext context)
    //     {
    //         _context = context;
    //     }

    //     public override void OnActionExecuting(ActionExecutingContext context)
    //     {
    //         Console.WriteLine("CustomAuthorization");
    //         base.OnActionExecuting(context);

    //         IRequestCookieCollection cookies = context.HttpContext.Request.Cookies;
    //         if (cookies.ContainsKey("Token"))
    //         {
    //             string value = cookies["Token"];
    //             Token token = _context.Token.FirstOrDefault(t => t.Value == value);
    //             if (token != null)
    //             {
    //                 User user = token.User;
    //                 if (!context.RouteData.Values.ContainsKey("User"))
    //                     context.RouteData.Values.Add("User", user);
    //                 return;
    //             }
    //         }

    //         if (!context.RouteData.Values.ContainsKey("User"))
    //             context.RouteData.Values.Add("User", null);
    //     }
    // }

    public class CustomAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            User user = (User)context.RouteData.Values["User"];

            if (user == null)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                            { "controller", "Account" },
                            { "action", "Login" }
                    });
            }
        }
    }

    public class CustomAnonymousOnly : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            User user = (User)context.RouteData.Values["User"];

            if (user != null)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                    { "controller", "Idea" },
                    { "action", "Index" }
                });
            }
        }
    }

    public class AdminAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            User user = (User)context.RouteData.Values["User"];

            if (!user.IsAdmin)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                    { "controller", "Idea" },
                    { "action", "Index" }
                });
            }
        }
    }
}