using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ideas.Models;
using Ideas.Controllers;
using Ideas.Models;
using Microsoft.AspNetCore.Http;
using FiltersSample.Filters;
using Microsoft.EntityFrameworkCore;

namespace ideas.Controllers
{
    public class CommentController : BaseController
    {
        private readonly IdeasContext _context;

        public CommentController(IdeasContext context) : base(context)
        {
            _context = context;
        }

        [HttpPost]
        [CustomAuthorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DetailsViewModel model, string returnUrl)
        {
            Comment comment = model.NewComment;
            User user = (User)RouteData.Values["User"];

            comment.UserId = user.Id;

            if (comment.Text != null && comment.Text.Length > 0)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return Redirect(returnUrl);
            }
            return Redirect(returnUrl);
        }

        // [CustomAuthorize]
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(int id)
        // {
        //     User user = (User)RouteData.Values["User"];
        //     Idea idea =
        //         await _context
        //             .Idea
        //             .Where(u => u.UserId == user.Id)
        //             .FirstOrDefaultAsync(i => i.Id == id);
        //     return RedirectToAction(nameof(MyIdeas));
    }
}
