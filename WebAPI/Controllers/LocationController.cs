using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos;
using WebAPI.Models;


namespace WebAPI.Controllers
{
    [Route("api/location")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly OnlineLibraryContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<LocationController> _logger;

        public LocationController(OnlineLibraryContext context, IMapper mapper, ILogger<LocationController> logger)
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

        // GET: api/location
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<LocationDto>> GetAll()
        {
            try
            {
                var locations = _context.Locations.ToList();
                var locationDtos = _mapper.Map<IEnumerable<LocationDto>>(locations);
                _logger.LogInformation("Retrieved all locations.");
                SaveLogToDatabase(2, "Retrieved all locations.");
                return Ok(locationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving all locations: {}", ex.Message);
                SaveLogToDatabase(4, $"An error occurred while retrieving all locations: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/location/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<LocationDto> Get(int id)
        {
            try
            {
                var location = _context.Locations.FirstOrDefault(l => l.Id == id);
                if (location == null)
                {
                    _logger.LogWarning("Location with ID {} not found.", id);
                    SaveLogToDatabase(3, $"Location with ID {id} not found.");
                    return NotFound($"Location with ID {id} not found.");
                }

                var locationDto = _mapper.Map<LocationDto>(location);
                _logger.LogInformation("Retrieved location with ID {}.", id);
                SaveLogToDatabase(2, $"Retrieved location with ID {id}.");
                return Ok(locationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving the location with ID {}: {}", id, ex.Message);
                SaveLogToDatabase(4, $"An error occurred while retrieving the location with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST api/location
        [HttpPost]
        [Authorize]
        public ActionResult<LocationDto> Create([FromBody] LocationDto locationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid while creating a location.");
                    SaveLogToDatabase(3, "ModelState is invalid while creating a location.");
                    return BadRequest(ModelState);
                }

                var location = _mapper.Map<Location>(locationDto);

                _context.Locations.Add(location);
                _context.SaveChanges();

                var createdLocationDto = _mapper.Map<LocationDto>(location);
                _logger.LogInformation("Created location with ID {}.", createdLocationDto.Id);
                SaveLogToDatabase(2, $"Created location with ID {createdLocationDto.Id}.");
                return Ok(createdLocationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while creating the location: {}", ex.Message);
                SaveLogToDatabase(4, $"An error occurred while creating the location: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT api/location/5
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<LocationDto> Update(int id, [FromBody] LocationDto locationDto)
        {
            try
            {
                var location = _context.Locations.FirstOrDefault(l => l.Id == id);
                if (location == null)
                {
                    _logger.LogWarning("Location with ID {} not found.", id);
                    SaveLogToDatabase(3, $"Location with ID {id} not found.");
                    return NotFound($"Location with ID {id} not found.");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid while updating the location.");
                    SaveLogToDatabase(3, "ModelState is invalid while updating the location.");
                    return BadRequest(ModelState);
                }

                _mapper.Map(locationDto, location);
                _context.SaveChanges();

                var updatedLocationDto = _mapper.Map<LocationDto>(location);
                _logger.LogInformation("Updated location with ID {}.", id);
                SaveLogToDatabase(2, $"Updated location with ID {id}.");
                return Ok(updatedLocationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while updating the location with ID {}: {}", id, ex.Message);
                SaveLogToDatabase(4, $"An error occurred while updating the location with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // DELETE api/location/5
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult<LocationDto> Delete(int id)
        {
            try
            {
                var location = _context.Locations.FirstOrDefault(l => l.Id == id);
                if (location == null)
                {
                    _logger.LogWarning("Location with ID {} not found.", id);
                    SaveLogToDatabase(3, $"Location with ID {id} not found.");
                    return NotFound($"Location with ID {id} not found.");
                }

                _context.Locations.Remove(location);
                _context.SaveChanges();

                var deletedLocation = _mapper.Map<LocationDto>(location);
                _logger.LogInformation("Deleted location with ID {}.", id);
                SaveLogToDatabase(2, $"Deleted location with ID {id}.");
                return Ok(deletedLocation);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while deleting the location with ID {}: {}", id, ex.Message);
                SaveLogToDatabase(4, $"An error occurred while deleting the location with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
