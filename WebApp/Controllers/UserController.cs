using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.Models;
using WebApp.ViewModels;
using WebApp.Security;
using Microsoft.AspNetCore.Authorization;

/*
 * Podaci za Log In: [username] [password]
 * [admin] [12345678]     
 * [user1] [12345678]
 */

namespace WebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly OnlineLibraryContext _context;

        public UserController(OnlineLibraryContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string returnUrl)
        {
            var loginVm = new LoginVM
            {
                ReturnUrl = returnUrl
            };

            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginVM loginVm)
        {
            // Try to get a user from database
            var existingUser =
                _context
                    .Users
                    .Include(x => x.Role)
                    .FirstOrDefault(x => x.Username == loginVm.Username);

            if (existingUser == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            // Check is password hash matches
            var b64hash = PasswordHashProvider.GetHash(loginVm.Password, existingUser.PwdSalt);
            if (b64hash != existingUser.PwdHash)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, loginVm.Username),
                new Claim(ClaimTypes.Role, existingUser.Role.Name)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties();

            // We need to wrap async code here into synchronous since we don't use async methods
            Task.Run(async () =>
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties)
            ).GetAwaiter().GetResult();

            if (loginVm.ReturnUrl != null)
                return LocalRedirect(loginVm.ReturnUrl);
            else if (existingUser.Role.Name == "Admin")
                return RedirectToAction("Index", "Book");
            else if (existingUser.Role.Name == "User")
                return RedirectToAction("Search", "Book");
            else
                return View();
        }

        public IActionResult Logout()
        {
            Task.Run(async () =>
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme)
            ).GetAwaiter().GetResult();

            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserVM userVm)
        {
            try
            {
                // Check if there is such a username in the database already
                var trimmedUsername = userVm.Username.Trim();
                if (_context.Users.Any(x => x.Username.Equals(trimmedUsername)))
                    return BadRequest($"Username {trimmedUsername} already exists");

                // Hash the password
                var b64salt = PasswordHashProvider.GetSalt();
                var b64hash = PasswordHashProvider.GetHash(userVm.Password, b64salt);

                // Create user from DTO and hashed password
                var user = new User
                {
                    Id = userVm.Id,
                    Username = userVm.Username,
                    PwdHash = b64hash,
                    PwdSalt = b64salt,
                    FirstName = userVm.FirstName,
                    LastName = userVm.LastName,
                    Email = userVm.Email,
                    Phone = userVm.Phone,
                    RoleId = 2, // regular user
                };

                // Add user and save changes to database
                _context.Add(user);
                _context.SaveChanges();

                // Update DTO Id to return it to the client
                userVm.Id = user.Id;

                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Vjezba 14 > dodano ovo ispod ------------------------------

        [Authorize]
        public IActionResult ProfileDetails()
        {
            var username = HttpContext.User.Identity.Name;

            UserVM userVm = GetUserVm(username);

            return View(userVm);
        }

        [Authorize]
        public IActionResult ProfileDetailsPartial()
        {
            var username = HttpContext.User.Identity.Name;

            UserVM userVm = GetUserVm(username);

            return PartialView("_ProfileDetailsPartial", userVm);
        }

        private UserVM GetUserVm(string? username)
        {
            var userDb = _context.Users.First(x => x.Username == username);
            var userVm = new UserVM
            {
                Id = userDb.Id,
                Username = userDb.Username,
                FirstName = userDb.FirstName,
                LastName = userDb.LastName,
                Email = userDb.Email,
                Phone = userDb.Phone,
            };
            return userVm;
        }

        [Authorize]
        public IActionResult ProfileEdit(int id)
        {
            var userDb = _context.Users.First(x => x.Id == id);
            var userVm = new UserVM
            {
                Id = userDb.Id,
                Username = userDb.Username,
                FirstName = userDb.FirstName,
                LastName = userDb.LastName,
                Email = userDb.Email,
                Phone = userDb.Phone
            };

            return View(userVm);
        }

        [Authorize]
        [HttpPost]
        public IActionResult ProfileEdit(int id, UserVM userVm)
        {
            var userDb = _context.Users.First(x => x.Id == id);

            userDb.Username = userVm.Username;
            userDb.FirstName = userVm.FirstName;
            userDb.LastName = userVm.LastName;
            userDb.Email = userVm.Email;
            userDb.Phone = userVm.Phone;

            _context.SaveChanges();

            return RedirectToAction("ProfileDetails");
        }

        public JsonResult GetProfileData(int id)
        {
            var userDb = _context.Users.First(x => x.Id == id);
            return Json(new
            {
                userDb.Username,
                userDb.FirstName,
                userDb.LastName,
                userDb.Email,
                userDb.Phone,
            });
        }

        [HttpPut]
        public ActionResult SetProfileData(int id, [FromBody] UserVM userVm)
        {
            var userDb = _context.Users.First(x => x.Id == id);
            if (userDb == null)
            {
                return NotFound("User not found.");
            }

            userDb.Username = userVm.Username;
            userDb.FirstName = userVm.FirstName;
            userDb.LastName = userVm.LastName;
            userDb.Email = userVm.Email;
            userDb.Phone = userVm.Phone;

            _context.SaveChanges();

            return Ok();
        }
    }
}
