using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Dtos;
using WebAPI.Models;


namespace WebAPI.Controllers
{
    [Route("api/userreservation")]
    [ApiController]
    public class UserReservationController : ControllerBase
    {
        private readonly OnlineLibraryContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UserReservationController> _logger;

        public UserReservationController(OnlineLibraryContext context, IMapper mapper, ILogger<UserReservationController> logger)
        {
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

        // GET: api/userreservation
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<UserReservationDto>> GetAll()
        {
            try
            {
                var userReservations = _context.UserReservations.ToList();
                var userReservationDtos = _mapper.Map<IEnumerable<UserReservationDto>>(userReservations);
                _logger.LogInformation("Retrieved all user reservations.");
                SaveLogToDatabase(2, "Retrieved all user reservations.");
                return Ok(userReservationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving all user reservations: {}", ex.Message);
                SaveLogToDatabase(4, $"An error occurred while retrieving all user reservations: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/userreservation/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<UserReservationDto> Get(int id)
        {
            try
            {
                var userReservation = _context.UserReservations.FirstOrDefault(ur => ur.Id == id);
                if (userReservation == null)
                {
                    _logger.LogWarning("UserReservation with ID {} not found.", id);
                    SaveLogToDatabase(3, $"UserReservation with ID {id} not found.");
                    return NotFound($"UserReservation with ID {id} not found.");
                }

                var userReservationDto = _mapper.Map<UserReservationDto>(userReservation);
                _logger.LogInformation("Retrieved UserReservation with ID {}.", id);
                SaveLogToDatabase(2, $"Retrieved UserReservation with ID {id}.");
                return Ok(userReservationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving UserReservation with ID {}: {}", id, ex.Message);
                SaveLogToDatabase(4, $"An error occurred while retrieving UserReservation with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST api/userreservation
        [HttpPost]
        [Authorize]
        public ActionResult<UserReservationDto> Create([FromBody] UserReservationDto userReservationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid while creating a UserReservation.");
                    SaveLogToDatabase(3, "ModelState is invalid while creating a UserReservation.");
                    return BadRequest(ModelState);
                }

                var userReservation = _mapper.Map<UserReservation>(userReservationDto);

                _context.UserReservations.Add(userReservation);
                _context.SaveChanges();

                var createdUserReservationDto = _mapper.Map<UserReservationDto>(userReservation);
                _logger.LogInformation("Created UserReservation with ID {}.", createdUserReservationDto.Id);
                SaveLogToDatabase(2, $"Created UserReservation with ID {createdUserReservationDto.Id}.");
                return Ok(createdUserReservationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while creating the UserReservation: {}", ex.Message);
                SaveLogToDatabase(4, $"An error occurred while creating the UserReservation: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT api/userreservation/5
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<UserReservationDto> Update(int id, [FromBody] UserReservationDto userReservationDto)
        {
            try
            {
                var userReservation = _context.UserReservations.FirstOrDefault(ur => ur.Id == id);
                if (userReservation == null)
                {
                    _logger.LogWarning("UserReservation with ID {} not found.", id);
                    SaveLogToDatabase(3, $"UserReservation with ID {id} not found.");
                    return NotFound($"UserReservation with ID {id} not found.");
                }

                _mapper.Map(userReservationDto, userReservation);
                _context.SaveChanges();

                var updatedUserReservationDto = _mapper.Map<UserReservationDto>(userReservation);
                _logger.LogInformation("Updated UserReservation with ID {}.", id);
                SaveLogToDatabase(2, $"Updated UserReservation with ID {id}.");
                return Ok(updatedUserReservationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while updating UserReservation with ID {}: {}", id, ex.Message);
                SaveLogToDatabase(4, $"An error occurred while updating UserReservation with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE api/userreservation/5
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult<UserReservationDto> Delete(int id)
        {
            try
            {
                var userReservation = _context.UserReservations.FirstOrDefault(ur => ur.Id == id);
                if (userReservation == null)
                {
                    _logger.LogWarning("UserReservation with ID {} not found.", id);
                    SaveLogToDatabase(3, $"UserReservation with ID {id} not found.");
                    return NotFound($"UserReservation with ID {id} not found.");
                }

                _context.UserReservations.Remove(userReservation);
                _context.SaveChanges();

                var deletedUserReservationDto = _mapper.Map<UserReservationDto>(userReservation);
                _logger.LogInformation("Deleted UserReservation with ID {}.", id);
                SaveLogToDatabase(2, $"Deleted UserReservation with ID {id}.");
                return Ok(deletedUserReservationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while deleting UserReservation with ID {}: {}", id, ex.Message);
                SaveLogToDatabase(4, $"An error occurred while deleting UserReservation with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
