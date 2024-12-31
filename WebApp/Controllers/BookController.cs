using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using NuGet.Protocol;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class BookController : Controller
    {
        private readonly OnlineLibraryContext _context;
        private readonly IConfiguration _configuration;

        public BookController(OnlineLibraryContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private List<SelectListItem> GetGenreListItems()
        {
            var genreListItemsJson = HttpContext.Session.GetString("GenreListItems");

            List<SelectListItem> genreListItems;
            if (genreListItemsJson == null)
            {
                genreListItems = _context.Genres
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList();

                HttpContext.Session.SetString("GenreListItems", genreListItems.ToJson());
            }
            else
            {
                genreListItems = genreListItemsJson.FromJson<List<SelectListItem>>();
            }

            return genreListItems;
        }

        private List<SelectListItem> GetImageListItems()
        {
            var imageListItemsJson = HttpContext.Session.GetString("ImageListItems");

            List<SelectListItem> imageListItems;
            if (imageListItemsJson == null)
            {
                imageListItems = _context.Images
                    .Select(x => new SelectListItem
                    {
                        Text = x.Content,
                        Value = x.Id.ToString()
                    }).ToList();

                HttpContext.Session.SetString("ImageListItems", imageListItems.ToJson());
            }
            else
            {
                imageListItems = imageListItemsJson.FromJson<List<SelectListItem>>();
            }

            return imageListItems;
        }

        // GET: BookController
        public ActionResult Index()
        {
            try
            {
                var bookVms = _context.Books
                        .Where(x => x.DeletedAt == null)
                        .Include(x => x.Genre)
                        .Include(x => x.Image)
                        .Select(x => new BookVM
                        {
                            Id = x.Id,
                            Title = x.Title,
                            Author = x.Author,
                            GenreId = x.GenreId,
                            GenreName = x.Genre.Name,
                            Description = x.Description,
                            Isbn = x.Isbn,
                            Availability = x.Availability,
                            ImageId = x.ImageId
                        })
                        .ToList();

                return View(bookVms);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Search(SearchVM searchVm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchVm.Q) && string.IsNullOrEmpty(searchVm.Submit))
                {
                    searchVm.Q = Request.Cookies["query"];
                }

                HandleSearchVm(searchVm);

                var option = new CookieOptions { Expires = DateTime.Now.AddMinutes(15) };
                Response.Cookies.Append("query", searchVm.Q ?? "", option);

                return View(searchVm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult SearchPartial(SearchVM searchVm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchVm.Q) && string.IsNullOrEmpty(searchVm.Submit))
                {
                    searchVm.Q = Request.Cookies["query"];
                }

                HandleSearchVm(searchVm);

                var option = new CookieOptions { Expires = DateTime.Now.AddMinutes(15) };
                Response.Cookies.Append("query", searchVm.Q ?? "", option);

                return PartialView("_SearchPartial", searchVm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void HandleSearchVm(SearchVM searchVm)
        {
            IQueryable<Book> books = _context.Books
                .Where(x => x.DeletedAt == null)
                .Include(x => x.Genre)
                .Include(x => x.Image);

            if (!string.IsNullOrEmpty(searchVm.Q))
            {
                books = books.Where(x => x.Title.Contains(searchVm.Q));
            }

            // We need this for pager
            var filteredCount = books.Count();

            switch (searchVm.OrderBy.ToLower())
            {
                case "id":
                    books = books.OrderBy(x => x.Id);
                    break;
                case "title":
                    books = books.OrderBy(x => x.Title);
                    break;
                case "author":
                    books = books.OrderBy(x => x.Author);
                    break;
                case "genre":
                    books = books.OrderBy(x => x.Genre.Name);
                    break;
                case "isbn":
                    books = books.OrderBy(x => x.Isbn);
                    break;
                case "availability":
                    books = books.OrderBy(x => x.Availability);
                    break;
            }

            books = books.Skip((searchVm.Page - 1) * searchVm.Size).Take(searchVm.Size); // if pages start from 1

            searchVm.Books =
                books.Select(x => new BookVM
                {
                    Id = x.Id,
                    Title = x.Title,
                    Author = x.Author,
                    GenreId = x.GenreId,
                    GenreName = x.Genre.Name,
                    Description = x.Description,
                    Isbn = x.Isbn,
                    Availability = x.Availability,
                    ImageId = x.ImageId
                })
                .ToList();

            // BEGIN PAGER
            var expandPages = _configuration.GetValue<int>("Paging:ExpandPages");
            searchVm.LastPage = (int)Math.Ceiling(1.0 * filteredCount / searchVm.Size);
            searchVm.FromPager = searchVm.Page > expandPages ?
                searchVm.Page - expandPages :
                1;
            searchVm.ToPager = (searchVm.Page + expandPages) < searchVm.LastPage ?
                searchVm.Page + expandPages :
                searchVm.LastPage;
            // END PAGER
        }

        // GET: BookController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var book = _context.Books
                    .Include(x => x.Genre)
                    .Include(x => x.Image)
                    .FirstOrDefault(x => x.Id == id);

                var bookVM = new BookVM
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    GenreId = book.GenreId,
                    GenreName = book.Genre.Name,
                    Description = book.Description,
                    Isbn = book.Isbn,
                    Availability = book.Availability,
                    ImageId = book.ImageId
                };

                return View(bookVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // GET: BookController/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.GenreDdlItems = GetGenreListItems();
                ViewBag.ImageDdlItems = GetImageListItems();

                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        // POST: BookController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BookVM book)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.GenreDdlItems = GetGenreListItems();
                    ViewBag.ImageDdlItems = GetImageListItems();

                    ModelState.AddModelError("", "Failed to create book");

                    return View();
                }

                var newBook = new Book
                {
                    CreatedAt = DateTime.UtcNow,
                    Title = book.Title,
                    Author = book.Author,
                    GenreId = book.GenreId,
                    Description = book.Description,
                    Isbn = book.Isbn,
                    Availability = book.Availability,
                    ImageId = book.ImageId
                };

                _context.Books.Add(newBook);

                _context.SaveChanges();

                var option = new CookieOptions { Expires = DateTime.Now.AddDays(14) };

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BookController/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.GenreDdlItems = GetGenreListItems();
            ViewBag.ImageDdlItems = GetImageListItems();

            var book = _context.Books.Include(x => x.BookLocations).FirstOrDefault(x => x.Id == id);
            var bookVM = new BookVM
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                GenreId = book.GenreId,
                Description = book.Description,
                Isbn = book.Isbn,
                Availability = book.Availability,
                ImageId = book.ImageId
            };

            return View(bookVM);
        }

        // POST: BookController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, BookVM book)
        {
            try
            {
                var dbBook = _context.Books.Include(x => x.BookLocations).FirstOrDefault(x => x.Id == id);
                dbBook.Title = book.Title;
                dbBook.Author = book.Author;
                dbBook.GenreId = book.GenreId;
                dbBook.Description = book.Description;
                dbBook.Isbn = book.Isbn;
                dbBook.Availability = book.Availability;
                dbBook.ImageId = book.ImageId;

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: BookController/Delete/5
        public ActionResult Delete(int id)
        {
            var book = _context.Books
                .Include(x => x.Genre)
                .Include(x => x.Image)
                .FirstOrDefault(x => x.Id == id);
            var bookVM = new BookVM
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                GenreId = book.GenreId,
                GenreName = book.Genre.Name,
                Description = book.Description,
                Isbn = book.Isbn,
                Availability = book.Availability,
                ImageId = book.ImageId
            };

            return View(bookVM);
        }

        // POST: BookController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Book book)
        {
            try
            {
                var dbBook = _context.Books.FirstOrDefault(x => x.Id == id);

                if (dbBook == null)
                {
                    return NotFound("Book not found!");
                }

                dbBook.DeletedAt = DateTime.UtcNow;

                // _context.Books.Remove(dbBook);

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
