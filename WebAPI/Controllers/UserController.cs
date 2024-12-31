using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos;
using WebAPI.Models;
using WebAPI.Security;

namespace WebAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly OnlineLibraryContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;

        public UserController(IConfiguration configuration, OnlineLibraryContext context, IMapper mapper, ILogger<UserController> logger)
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        private void SaveLogToDatabase(int level, string message)
        {
            var log = new Log
            {
                Timestamp = DateTime.UtcNow,
                Level = level,      // Debug(1),Information(2),Warning(3),Error(4),Critical(5)
                Message = message
            };

            _context.Logs.Add(log);
            _context.SaveChanges();
        }

        // GET: api/user
        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> GetAll()
        {
            try
            {
                var users = _context.Users.ToList();
                var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
                _logger.LogInformation("Retrieved all users.");
                SaveLogToDatabase(2, "Retrieved all users.");
                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving all users: {}", ex.Message);
                SaveLogToDatabase(4, $"An error occurred while retrieving all users: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/user/5
        [HttpGet("{id}")]
        public ActionResult<UserDto> Get(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {} not found.", id);
                    SaveLogToDatabase(3, $"User with ID {id} not found.");
                    return NotFound($"User with ID {id} not found.");
                }

                var userDto = _mapper.Map<UserDto>(user);
                _logger.LogInformation("Retrieved User with ID {}.", id);
                SaveLogToDatabase(2, $"Retrieved User with ID {id}.");
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving User with ID {}: {}", id, ex.Message);
                SaveLogToDatabase(4, $"An error occurred while retrieving User with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE api/user/5
        [HttpDelete("{id}")]
        public ActionResult<UserDto> Delete(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {} not found.", id);
                    SaveLogToDatabase(3, $"User with ID {id} not found.");
                    return NotFound($"User with ID {id} not found.");
                }

                _context.Users.Remove(user);
                _context.SaveChanges();

                var deletedUserDto = _mapper.Map<UserDto>(user);
                _logger.LogInformation("Deleted User with ID {}.", id);
                SaveLogToDatabase(2, $"Deleted User with ID {id}.");
                return Ok(deletedUserDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while deleting User with ID {}: {}", id, ex.Message);
                SaveLogToDatabase(4, $"An error occurred while deleting User with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("gettoken")]
        public ActionResult GetToken()
        {
            try
            {
                // The same secure key must be used here to create JWT,
                // as the one that is used by middleware to verify JWT
                var secureKey = _configuration["JWT:SecureKey"];
                var serializedToken = JwtTokenProvider.CreateToken(secureKey, 10);

                _logger.LogInformation("Generated a new token.");
                SaveLogToDatabase(2, "Generated a new token.");
                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while generating a token: {}", ex.Message);
                SaveLogToDatabase(4, $"An error occurred while generating a token: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("register")]
        public ActionResult<UserDto> Register(UserDto userDto)
        {
            try
            {
                // Check if there is such a username in the database already
                var trimmedUsername = userDto.Username.Trim();
                if (_context.Users.Any(x => x.Username.Equals(trimmedUsername)))
                {
                    _logger.LogWarning("Username {} already exists.", trimmedUsername);
                    SaveLogToDatabase(3, $"Username {trimmedUsername} already exists.");
                    return BadRequest($"Username {trimmedUsername} already exists");
                }
                if (userDto.Password.Length < 8)
                {
                    ModelState.AddModelError("Password", "Password should be at least 8 characters long.");
                    return BadRequest(ModelState);
                }

                // Hash the password
                var b64salt = PasswordHashProvider.GetSalt();
                var b64hash = PasswordHashProvider.GetHash(userDto.Password, b64salt);

                // Create user from DTO and hashed password
                var user = new User
                {
                    Id = userDto.Id,
                    Username = userDto.Username,
                    PwdHash = b64hash,
                    PwdSalt = b64salt,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Email = userDto.Email,
                    Phone = userDto.Phone,
                };

                // Add user and save changes to database
                _context.Add(user);
                _context.SaveChanges();

                // Update DTO Id to return it to the client
                userDto.Id = user.Id;
                _logger.LogInformation("Registered User with ID {}.", userDto.Id);
                SaveLogToDatabase(2, $"Registered User with ID {userDto.Id}.");
                return Ok(userDto);

            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while registering the user: {}", ex.Message);
                SaveLogToDatabase(4, $"An error occurred while registering the user: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public ActionResult Login(UserLoginDto userDto)
        {
            try
            {
                var genericLoginFail = "Incorrect username or password";

                // Try to get a user from database
                var existingUser = _context.Users.FirstOrDefault(x => x.Username == userDto.Username);
                if (existingUser == null)
                {
                    _logger.LogWarning("Login attempt failed: User not found.");
                    SaveLogToDatabase(3, "Login attempt failed: User not found.");
                    return BadRequest(genericLoginFail);
                }
                // Check is password hash matches
                var b64hash = PasswordHashProvider.GetHash(userDto.Password, existingUser.PwdSalt);
                if (b64hash != existingUser.PwdHash)
                {
                    _logger.LogWarning("Login attempt failed: Incorrect password.");
                    SaveLogToDatabase(3, "Login attempt failed: Incorrect password.");
                    return BadRequest(genericLoginFail);
                }
                // Create and return JWT token
                var secureKey = _configuration["JWT:SecureKey"];
                var serializedToken =
                    JwtTokenProvider.CreateToken(
                        secureKey,
                        120,
                        userDto.Username,
                        existingUser.Username == "Admin" ? "Admin" : "User"); // Hardcoded example!

                _logger.LogInformation("User {} logged in successfully.", userDto.Username);
                SaveLogToDatabase(2, $"User {userDto.Username} logged in successfully.");
                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while logging in: {}", ex.Message);
                SaveLogToDatabase(4, $"An error occurred while logging in: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("changepassword")]
        public ActionResult ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                var existingUser = _context.Users.FirstOrDefault(x => x.Username == changePasswordDto.Username);
                if (existingUser == null)
                {
                    _logger.LogWarning("Change password attempt failed: User not found.");
                    SaveLogToDatabase(3, "Change password attempt failed: User not found.");
                    return BadRequest("User not found");
                }

                var b64hash = PasswordHashProvider.GetHash(changePasswordDto.CurrentPassword, existingUser.PwdSalt);
                if (b64hash != existingUser.PwdHash)
                {
                    _logger.LogWarning("Change password attempt failed: Incorrect current password.");
                    SaveLogToDatabase(3, "Change password attempt failed: Incorrect current password.");
                    return BadRequest("Incorrect current password");
                }

                if (changePasswordDto.NewPassword.Length < 8)
                {
                    ModelState.AddModelError("Password", "New password should be at least 8 characters long.");
                    return BadRequest(ModelState);
                }

                var b64salt = PasswordHashProvider.GetSalt();
                var b64newhash = PasswordHashProvider.GetHash(changePasswordDto.NewPassword, b64salt);

                existingUser.PwdHash = b64newhash;
                existingUser.PwdSalt = b64salt;
                _context.SaveChanges();

                _logger.LogInformation("User {} changed their password successfully.", existingUser.Username);
                SaveLogToDatabase(2, $"User {existingUser.Username} changed their password successfully.");
                return Ok("Password changed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while changing password: {}", ex.Message);
                SaveLogToDatabase(4, $"An error occurred while changing password: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
