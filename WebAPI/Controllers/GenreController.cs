using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos;
using WebAPI.Models;


namespace WebAPI.Controllers
{
    [Route("api/genre")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly OnlineLibraryContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<Genre> _logger;

        public GenreController(OnlineLibraryContext context, IMapper mapper, ILogger<Genre> logger)
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

        // GET: api/genre
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<GenreDto>> GetAll()
        {
            try
            {
                var genres = _context.Genres.ToList();
                var genreDtos = _mapper.Map<IEnumerable<GenreDto>>(genres);
                _logger.LogInformation("Retrieved all genres.");
                SaveLogToDatabase(2, "Retrieved all genres.");
                return Ok(genreDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving all genres: {}", ex.Message);
                SaveLogToDatabase(4, $"An error occurred while retrieving all genres: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/genre/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<GenreDto> Get(int id)
        {
            try
            {
                var genre = _context.Genres.FirstOrDefault(g => g.Id == id);
                if (genre == null)
                {
                    _logger.LogWarning("Genre with ID {} not found.", id);
                    SaveLogToDatabase(3, $"Genre with ID {id} not found.");
                    return NotFound($"Genre with ID {id} not found.");
                }

                var genreDto = _mapper.Map<GenreDto>(genre);
                _logger.LogInformation("Retrieved genre with ID {}.", id);
                SaveLogToDatabase(2, $"Retrieved genre with ID {id}.");
                return Ok(genreDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving the genre with ID {}: {}", id, ex.Message);
                SaveLogToDatabase(4, $"An error occurred while retrieving the genre with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST api/genre
        [HttpPost]
        [Authorize]
        public ActionResult<GenreDto> Create([FromBody] GenreDto genreDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid while creating a genre.");
                    SaveLogToDatabase(3, "ModelState is invalid while creating a genre.");
                    return BadRequest(ModelState);
                }

                var genre = _mapper.Map<Genre>(genreDto);

                _context.Genres.Add(genre);
                _context.SaveChanges();

                var createdGenreDto = _mapper.Map<GenreDto>(genre);
                _logger.LogInformation("Created genre with ID {}.", genreDto.Id);
                SaveLogToDatabase(2, $"Created genre with ID {genreDto.Id}.");
                return Ok(createdGenreDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while creating the genre: {}", ex.Message);
                SaveLogToDatabase(4, $"An error occurred while creating the genre: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT api/genre/5
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<GenreDto> Update(int id, [FromBody] GenreDto genreDto)
        {
            try
            {
                var genre = _context.Genres.FirstOrDefault(g => g.Id == id);
                if (genre == null)
                {
                    _logger.LogWarning("Genre with ID {} not found.", id);
                    SaveLogToDatabase(3, $"Genre with ID {id} not found.");
                    return NotFound($"Genre with ID {id} not found.");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid while updating the genre.");
                    SaveLogToDatabase(3, "ModelState is invalid while updating the genre.");
                    return BadRequest(ModelState);
                }

                _mapper.Map(genreDto, genre);
                _context.SaveChanges();

                var updatedGenreDto = _mapper.Map<GenreDto>(genre);
                _logger.LogInformation("Updated genre with ID {}.", id);
                SaveLogToDatabase(2, $"Updated genre with ID {id}.");
                return Ok(updatedGenreDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while updating the genre with ID {}: {}", id, ex.Message);
                SaveLogToDatabase(4, $"An error occurred while updating the genre with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // DELETE api/genre/5
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult<GenreDto> Delete(int id)
        {
            try
            {
                var genre = _context.Genres.FirstOrDefault(g => g.Id == id);
                if (genre == null)
                {
                    _logger.LogWarning("Genre with ID {} not found.", id);
                    SaveLogToDatabase(3, $"Genre with ID {id} not found.");
                    return NotFound($"Genre with ID {id} not found.");
                }

                _context.Genres.Remove(genre);
                _context.SaveChanges();

                var deletedGenreDto = _mapper.Map<GenreDto>(genre);
                _logger.LogInformation("Deleted genre with ID {}.", id);
                SaveLogToDatabase(2, $"Deleted genre with ID {id}.");
                return Ok(deletedGenreDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while deleting the genre with ID {}: {}", id, ex.Message);
                SaveLogToDatabase(4, $"An error occurred while deleting the genre with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
