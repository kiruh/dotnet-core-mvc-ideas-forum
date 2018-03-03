using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ideas.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Ideas.Controllers
{
    public class BaseController : Controller
    {
        private readonly IdeasContext _context;

        public BaseController(IdeasContext context)
        {
            _context = context;
        }

        [Microsoft.AspNetCore.Mvc.NonAction]
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            IRequestCookieCollection cookies = context.HttpContext.Request.Cookies;
            if (cookies.ContainsKey("Token"))
            {
                string value = cookies["Token"];
                Token token = _context.Token.Include(t => t.User).FirstOrDefault(t => t.Value == value);
                if (token != null)
                {
                    User user = token.User;
                    context.RouteData.Values.Add("User", user);
                    ViewBag.User = user;

                    return;
                }
            }

            context.RouteData.Values.Add("User", null);
            ViewBag.User = null;
        }
    }
}