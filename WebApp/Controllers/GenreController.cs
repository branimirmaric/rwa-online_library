using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class GenreController : Controller
    {
        private readonly OnlineLibraryContext _context;

        public GenreController(OnlineLibraryContext context)
        {
            _context = context;
        }

        // GET: GenreController
        public ActionResult Index()
        {
            try
            {
                if (TempData.ContainsKey("newGenre"))
                {
                    var newGenre = ((string)TempData["newGenre"]).FromJson<GenreVM>();
                }

                var genreVms = _context.Genres
                    .Select(x => new GenreVM
                    {
                        Id = x.Id,
                        Name = x.Name,
                    }).ToList();

                return View(genreVms);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: GenreController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var genre = _context.Genres.FirstOrDefault(x => x.Id == id);
                var genreVM = new GenreVM
                {
                    Id = genre.Id,
                    Name = genre.Name
                };

                return View(genreVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // GET: GenreController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GenreController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GenreVM genre)
        {
            try
            {
                var newGenre = new Genre
                {
                    Name = genre.Name
                };

                _context.Genres.Add(newGenre);

                _context.SaveChanges();

                TempData["newGenre"] = newGenre.ToJson();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GenreController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var genre = _context.Genres.FirstOrDefault(x => x.Id == id);
                var genreVM = new GenreVM
                {
                    Id = genre.Id,
                    Name = genre.Name
                };

                return View(genreVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // POST: GenreController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, GenreVM genre)
        {
            try
            {
                var dbGenre = _context.Genres.FirstOrDefault(x => x.Id == id);
                dbGenre.Name = genre.Name;

                _context.SaveChanges();

                TempData["EditMessage"] = $"Genre '{dbGenre.Name}' has been successfully updated.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the genre.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: GenreController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var genre = _context.Genres.FirstOrDefault(x => x.Id == id);
                var genreVM = new GenreVM
                {
                    Id = genre.Id,
                    Name = genre.Name,
                };

                return View(genreVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // POST: GenreController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, GenreVM genre)
        {
            try
            {
                var dbGenre = _context.Genres.FirstOrDefault(x => x.Id == id);

                // Provjera je li žanr koristen u nekim knjigama
                if (_context.Books.Any(b => b.GenreId == id))
                {
                    // Ako je zanr referenciran, prikazat ce se poruka o gresci
                    TempData["ErrorMessage"] = "This genre is being used by one or more books. You cannot delete it.";
                    return RedirectToAction(nameof(Index));
                }

                if (dbGenre != null)
                {
                    _context.Genres.Remove(dbGenre);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = $"Genre '{dbGenre.Name}' has been successfully deleted.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the genre.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
