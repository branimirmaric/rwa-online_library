using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class LocationController : Controller
    {
        private readonly OnlineLibraryContext _context;

        public LocationController(OnlineLibraryContext context)
        {
            _context = context;
        }

        // GET: LocationController
        public ActionResult Index()
        {
            try
            {
                if (TempData.ContainsKey("newLocation"))
                {
                    var newLocation = ((string)TempData["newLocation"]).FromJson<GenreVM>();
                }

                var locations = _context.Locations
                    .Select(l => new LocationVM
                    {
                        Id = l.Id,
                        State = l.State,
                        City = l.City,
                        Address = l.Address
                    }).ToList();

                return View(locations);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: LocationController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var location = _context.Locations
                    .Where(l => l.Id == id)
                    .Select(l => new LocationVM
                    {
                        Id = l.Id,
                        State = l.State,
                        City = l.City,
                        Address = l.Address
                    }).FirstOrDefault();

                if (location == null)
                {
                    return NotFound();
                }

                return View(location);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // GET: LocationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LocationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LocationVM location)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newLocation = new Location
                    {
                        State = location.State,
                        City = location.City,
                        Address = location.Address
                    };

                    _context.Locations.Add(newLocation);
                    _context.SaveChanges();

                    TempData["newLocation"] = newLocation.ToJson();

                    return RedirectToAction(nameof(Index));
                }

                return View(location);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: LocationController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var location = _context.Locations
                    .Where(l => l.Id == id)
                    .Select(l => new LocationVM
                    {
                        Id = l.Id,
                        State = l.State,
                        City = l.City,
                        Address = l.Address
                    }).FirstOrDefault();

                if (location == null)
                {
                    return NotFound();
                }

                return View(location);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // POST: LocationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, LocationVM locationVm)
        {
            try
            {
                if (id != locationVm.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var location = _context.Locations.Find(id);

                    if (location == null)
                    {
                        return NotFound();
                    }

                    location.State = locationVm.State;
                    location.City = locationVm.City;
                    location.Address = locationVm.Address;

                    _context.Update(location);
                    _context.SaveChanges();

                    TempData["EditMessage"] = $"Location has been successfully updated.";

                    return RedirectToAction(nameof(Index));
                }

                return View(locationVm);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the location.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: LocationController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var location = _context.Locations
                    .Where(l => l.Id == id)
                    .Select(l => new LocationVM
                    {
                        Id = l.Id,
                        State = l.State,
                        City = l.City,
                        Address = l.Address
                    }).FirstOrDefault();

                if (location == null)
                {
                    return NotFound();
                }

                return View(location);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // POST: LocationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, LocationVM locationVm)
        {
            try
            {
                var location = _context.Locations.Find(id);

                // Provjera je li lokacija koristena u nekim knjigama
                if (_context.BookLocations.Any(bl => bl.LocationId == id))
                {
                    // Ako je lokacija referencirana, prikazat ce se poruka o gresci
                    TempData["ErrorMessage"] = "This location is being used by one or more books. You cannot delete it.";
                    return RedirectToAction(nameof(Index));
                }

                if (location != null)
                {
                    _context.Locations.Remove(location);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Location has been successfully deleted.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the location.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
