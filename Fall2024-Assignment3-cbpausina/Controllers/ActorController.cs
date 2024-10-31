using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_cbpausina.Data;
using Fall2024_Assignment3_cbpausina.Models.Entities;
using Fall2024_Assignment3_cbpausina.Models.ViewModels;
using Fall2024_Assignment3_cbpausina.Controllers.API;

namespace Fall2024_Assignment3_cbpausina.Controllers
{
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OpenAIAPIController _openAi;
        private readonly VaderSharpTool _vaderSharp;

        public ActorController(ApplicationDbContext context, OpenAIAPIController openAi, VaderSharpTool vaderSharp)
        {
            _context = context;
            _openAi = openAi;
            _vaderSharp = vaderSharp;
        }

        // GET: Actors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Actors.ToListAsync());
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (actor == null)
            {
                return NotFound();
            }

            // Get list of movies associated with actor
            var movies = actor.MovieActors.Select(ma => ma.Movie).ToList();

            // Check if tweets already exist, no need to generate if they do
            if (actor.Tweets == null || !actor.Tweets.Any())
            {
                var generatedTweets = await _openAi.TwitterApiSim(actor.Name);
                actor.Tweets = generatedTweets;

                actor.TweetSentiment = _vaderSharp.AnalyzeTweets(generatedTweets);

                // Save generated reviews to the database
                _context.Actors.Update(actor);
                await _context.SaveChangesAsync();
            }

            var vm = new ActorDetailsViewModel(actor, movies, actor.Tweets, actor.TweetSentiment);

            return View(vm);
        }

        // GET: Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Gender,Age,IMDbLink")] Actor actor, IFormFile? photo)
        {
            if (ModelState.IsValid)
            {
                if (photo != null && photo.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await photo.CopyToAsync(memoryStream); // asynchronous
                    actor.Photo = memoryStream.ToArray();
                }

                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        // POST: Actors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Age,IMDbLink")] Actor actor, IFormFile? photo)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (photo != null && photo.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await photo.CopyToAsync(memoryStream); // asynchronous
                    actor.Photo = memoryStream.ToArray();
                }

                try
                {
                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor != null)
            {
                _context.Actors.Remove(actor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return _context.Actors.Any(e => e.Id == id);
        }
    }
}
