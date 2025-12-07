using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab6.Data;
using Lab6.Models;

namespace Lab6.Controllers
{
    public class HomeController : Controller
    {
        private readonly MoviesDbContext _context;

        public HomeController(MoviesDbContext context)
        {
            _context = context;
        }

        // GET: Home
        public async Task<IActionResult> Index()
        {
            return _context.Movies != null
                ? View(await _context.Movies.Include(x => x.Genre).ToListAsync())
                : Problem("Entity set 'MoviesDbContext.Movies'  is null.");
        }

        // GET: Home/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return View("NotFound");
            }

            var movie = await _context.Movies
                .Include(x => x.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return View("NotFound");
            }

            return View(movie);
        }

        // GET: Home/Create
        public IActionResult Create()
        {
            var all = _context.Genres
                .Select(g => g.Name)
                .AsEnumerable()
                .Select(n => (n ?? string.Empty).Trim())
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n)
                .ToList();

            var m = new MovieDto { AllGenres = all };
            return View(m);
        }

        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Title,Description,Rating,TrailerLink,Genre")] MovieDto movie
        )
        {
            // Normalize and validate genre on server side
            var genreName = (movie.Genre ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(genreName))
            {
                ModelState.AddModelError("Genre", "Gatunek jest wymagany");
            }

            if (ModelState.IsValid)
            {
                // Case-insensitive lookup for existing genre
                var genre = _context.Genres
                    .FirstOrDefault(x => x.Name.ToLower() == genreName.ToLower());

                if (genre == null)
                {
                    genre = new Genre { Name = genreName };
                    _context.Genres.Add(genre);
                }

                Movie m = new Movie
                {
                    Id = 0,
                    Title = movie.Title,
                    Description = movie.Description ?? string.Empty,
                    Rating = movie.Rating,
                    TrailerLink = movie.TrailerLink ?? string.Empty,
                    Genre = genre
                };

                _context.Add(m);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            movie.AllGenres = _context.Genres
                .Select(g => g.Name)
                .AsEnumerable()
                .Select(n => (n ?? string.Empty).Trim())
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n)
                .ToList();

            return View(movie);
        }

        // GET: Home/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return View("NotFound");
            }

            var movie = await _context.Movies.Include(x => x.Genre).FirstOrDefaultAsync(x => x.Id == id);
            if (movie == null)
            {
                return View("NotFound");
            }

            var dto = new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Rating = movie.Rating,
                TrailerLink = movie.TrailerLink,
                Genre = movie.Genre?.Name ?? string.Empty,
                AllGenres = _context.Genres.Select(x => x.Name).ToList()
            };

            return View(dto);
        }

        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Rating,TrailerLink,Genre")] MovieDto movie)
        {
            if (id != movie.Id)
            {
                return View("NotFound");
            }

            // Normalize and validate genre on server side
            var genreName = (movie.Genre ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(genreName))
            {
                ModelState.AddModelError("Genre", "Gatunek jest wymagany");
            }

            if (ModelState.IsValid)
            {
                var existing = await _context.Movies.Include(x => x.Genre).FirstOrDefaultAsync(x => x.Id == id);
                if (existing == null)
                {
                    return View("NotFound");
                }

                var genre = _context.Genres
                    .FirstOrDefault(x => x.Name.ToLower() == genreName.ToLower());
                if (genre == null)
                {
                    genre = new Genre { Name = genreName };
                    _context.Genres.Add(genre);
                }

                existing.Title = movie.Title;
                existing.Description = movie.Description ?? string.Empty;
                existing.Rating = movie.Rating;
                existing.TrailerLink = movie.TrailerLink ?? string.Empty;
                existing.Genre = genre;

                try
                {
                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(existing.Id))
                    {
                        return View("NotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            movie.AllGenres = _context.Genres
                .Select(g => g.Name)
                .AsEnumerable()
                .Select(n => (n ?? string.Empty).Trim())
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n)
                .ToList();

            return View(movie);
        }

        // GET: Home/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Movies == null)
            {
                return Problem("Entity set 'MoviesDbContext.Movies'  is null.");
            }
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return (_context.Movies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
