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
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OpenAIAPIController _openAi;
        private readonly VaderSharpTool _vaderSharp;

        public MovieController(ApplicationDbContext context, OpenAIAPIController openAi, VaderSharpTool vaderSharp)
        {
            _context = context;
            _openAi = openAi;
            _vaderSharp = vaderSharp;
        }

        public async Task<IActionResult> GetMoviePoster(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null || movie.Poster == null)
            {
                return NotFound();
            }

            var data = movie.Poster;
            return File(data, "image/jpg");
        }

        // GET: Movie
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movie.ToListAsync());
        }

        // GET: Movie/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            // Get list of actors associated with movie
            var actors = movie.MovieActors.Select(ma => ma.Actor).ToList();

            // Check if reviews already exist, no need to generate if they do
            if (movie.Reviews == null || !movie.Reviews.Any())
            {
                var generatedReviews = await _openAi.GenerateTenMovieReviews(movie.Title, movie.YearOfRelease);
                movie.Reviews = generatedReviews;

                movie.ReviewSentiment = _vaderSharp.AnalyzeReviews(generatedReviews);

                // Save generated reviews to the database
                _context.Movie.Update(movie);
                await _context.SaveChangesAsync();
            }

            var vm = new MovieDetailsViewModel(movie, actors, movie.Reviews, movie.ReviewSentiment);

            return View(vm);
        }

        // GET: Movie/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movie/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,IMDbLink,Genre,YearOfRelease")] Movie movie, IFormFile? poster)
        {
            if (ModelState.IsValid)
            {
                if (poster != null && poster.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await poster.CopyToAsync(memoryStream); // asynchronous
                    movie.Poster = memoryStream.ToArray();
                }

                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movie/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movie/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,IMDbLink,Genre,YearOfRelease")] Movie movie, IFormFile? poster)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (poster != null && poster.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await poster.CopyToAsync(memoryStream); // asynchronous
                    movie.Poster = memoryStream.ToArray();
                }

                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
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
            return View(movie);
        }

        // GET: Movie/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
