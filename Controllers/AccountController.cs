using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ideas.Models;
using Microsoft.AspNetCore.Http;
using FiltersSample.Filters;

namespace Ideas.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IdeasContext _context;

        public AccountController(IdeasContext context) : base(context)
        {
            _context = context;
        }

        [HttpGet]
        [CustomAnonymousOnly]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [CustomAnonymousOnly]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User _user)
        {
            User user = await _context.GetUserByCredentials(_user.Login, _user.Password);
            if (user != null)
            {
                SaveUserToCookies(user);
                return RedirectHome();
            }
            ModelState.AddModelError(string.Empty, "Invalid credentials.");
            return View(user);
        }

        [HttpGet]
        [CustomAnonymousOnly]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [CustomAnonymousOnly]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                user.Login = user.Login.ToLowerInvariant();

                User existing = await _context.User.FirstOrDefaultAsync(
                    u => u.Login == user.Login
                );

                if (existing == null)
                {
                    _context.User.Add(user);
                    await _context.SaveChangesAsync();
                    SaveUserToCookies(user);
                    return RedirectHome();
                }

                ModelState.AddModelError(
                    "Login",
                    "User with this login already exists."
                );
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("Token");
            return RedirectHome();
        }

        private void SaveUserToCookies(User user)
        {
            string token = _context.CreateTokenForUser(user);

            CookieOptions cookies = new CookieOptions();
            cookies.Expires = DateTime.Now.AddDays(30);
            Response.Cookies.Append("Token", token);
        }

        private IActionResult RedirectHome()
        {
            return RedirectToAction("Index", "Idea");
        }
    }
}
