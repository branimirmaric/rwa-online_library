using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Drawing;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class ImageController : Controller
    {
        private readonly OnlineLibraryContext _context;

        public ImageController(OnlineLibraryContext context)
        {
            _context = context;
        }

        // GET: ImageController
        public ActionResult Index()
        {
            try
            {
                if (TempData.ContainsKey("newImage"))
                {
                    var newImage = ((string)TempData["newImage"]).FromJson<ImageVM>();
                }

                var imageVms = _context.Images.Select(x => new ImageVM
                {
                    Id = x.Id,
                    Content = x.Content,
                }).ToList();

                return View(imageVms);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: ImageController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var image = _context.Images.FirstOrDefault(x => x.Id == id);
                var imageVM = new ImageVM
                {
                    Id = image.Id,
                    Content = image.Content
                };

                return View(imageVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // GET: ImageController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ImageController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ImageVM image)
        {
            try
            {
                var newImage = new WebApp.Models.Image
                {
                    Content = image.Content
                };

                _context.Images.Add(newImage);

                _context.SaveChanges();

                TempData["newImage"] = newImage.ToJson();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ImageController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ImageController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ImageController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ImageController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
