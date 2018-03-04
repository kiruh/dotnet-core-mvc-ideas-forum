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
    public class IdeaController : BaseController
    {
        private readonly IdeasContext _context;

        public IdeaController(IdeasContext context) : base(context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            int PageSize = 5;
            IQueryable<Idea> ideas =
                _context
                .Idea
                .Include(m => m.User)
                .Where(i => i.Approved)
                .OrderByDescending(i => i.CreatedAt);

            return View(await PaginatedList<Idea>.CreateAsync(ideas.AsNoTracking(), page ?? 1, PageSize));
        }

        [HttpGet]
        [CustomAuthorize]
        public IActionResult MyIdeas()
        {
            TempData["Success"] = TempData.ContainsKey("Success")
                ? TempData["Success"]
                : false;

            User user = (User)RouteData.Values["User"];

            List<Idea> ideas =
                _context
                    .Idea
                    .Where(u => u.UserId == user.Id)
                    .Include(m => m.User)
                    .ToList();

            return View(ideas);
        }

        [HttpGet]
        [AdminAuthorize]
        public IActionResult AllIdeas()
        {
            List<Idea> ideas =
                _context
                    .Idea
                    .Include(m => m.User)
                    .ToList();

            return View(ideas);
        }

        [HttpGet]
        [CustomAuthorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Text")] Idea idea)
        {
            User user = (User)RouteData.Values["User"];
            idea.UserId = user.Id;
            if (idea.Text != null && idea.Text.Length > 0)
            {
                _context.Add(idea);
                await _context.SaveChangesAsync();
                TempData["Success"] = true;
                return RedirectToAction(nameof(MyIdeas));
            }
            ModelState.AddModelError(string.Empty, "Specify your idea.");
            return View(idea);
        }

        [CustomAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            User user = (User)RouteData.Values["User"];
            Idea idea =
                await _context
                    .Idea
                    .Where(i => i.UserId == user.Id)
                    .Where(i => !i.Approved)
                    .FirstOrDefaultAsync(i => i.Id == id);

            if (idea != null)
            {
                _context.Idea.Remove(idea);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(MyIdeas));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var idea = await _context.Idea
                .Include(i => i.User)
                .Include(i => i.Comments)
                    .ThenInclude(c => c.User)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (idea == null || !idea.Approved)
            {
                return NotFound();
            }

            User user = (User)RouteData.Values["User"];
            if (user != null)
            {
                ViewBag.UserId = user.Id;
            }
            ViewBag.Editable = user != null && user.Id == idea.UserId && !idea.Approved;

            DetailsViewModel model = new DetailsViewModel();
            model.Idea = idea;

            ViewBag.ShowCommentForm = user != null;

            return View(model);
        }

        [HttpGet]
        [CustomAuthorize]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            User user = (User)RouteData.Values["User"];
            Idea idea =
                _context
                    .Idea
                    .Include(m => m.User)
                    .Where(u => u.UserId == user.Id)
                    .SingleOrDefault(m => m.Id == id);

            if (idea == null || idea.Approved)
            {
                return NotFound();
            }

            return View(idea);
        }

        [CustomAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Text")] Idea idea)
        {
            if (ModelState.IsValid)
            {
                User user = (User)RouteData.Values["User"];
                Idea dbIdea =
                    _context
                        .Idea
                        .Include(m => m.User)
                        .Where(u => u.UserId == user.Id)
                        .SingleOrDefault(m => m.Id == id);

                if (dbIdea == null || dbIdea.Approved)
                {
                    return NotFound();
                }

                try
                {
                    dbIdea.Text = idea.Text;
                    _context.Update(dbIdea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(MyIdeas));
            }
            return View(idea);
        }

        [AdminAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            Idea idea =
                await _context
                    .Idea
                    .FirstOrDefaultAsync(i => i.Id == id);

            if (idea == null)
            {
                return NotFound();
            }

            idea.Approved = true;
            _context.Update(idea);
            _context.SaveChanges();

            return RedirectToAction(nameof(AllIdeas));
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
