using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos;
using WebAPI.Models;


namespace WebAPI.Controllers
{
    [Route("api/image")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly OnlineLibraryContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ImageController> _logger;

        public ImageController(OnlineLibraryContext context, IMapper mapper, ILogger<ImageController> logger)
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

        // GET: api/image
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<ImageDto>> GetAll()
        {
            try
            {
                var images = _context.Images.ToList();
                var imageDtos = _mapper.Map<IEnumerable<ImageDto>>(images);
                _logger.LogInformation("Retrieved all images.");
                SaveLogToDatabase(2, "Retrieved all images.");
                return Ok(imageDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving all images: {}", ex.Message);
                SaveLogToDatabase(4, $"An error occurred while retrieving all images: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/image/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<ImageDto> Get(int id)
        {
            try
            {
                var image = _context.Images.FirstOrDefault(i => i.Id == id);
                if (image == null)
                {
                    _logger.LogWarning("Image with ID {} not found.", id);
                    SaveLogToDatabase(3, $"Image with ID {id} not found.");
                    return NotFound($"Image with ID {id} not found.");
                }

                var imageDto = _mapper.Map<ImageDto>(image);
                _logger.LogInformation("Retrieved image with ID {}.", id);
                SaveLogToDatabase(2, $"Retrieved image with ID {id}.");
                return Ok(imageDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving the image with ID {}: {}", id, ex.Message);
                SaveLogToDatabase(4, $"An error occurred while retrieving the image with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST api/image
        [HttpPost]
        [Authorize]
        public ActionResult<ImageDto> Create([FromBody] ImageDto imageDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid while creating an image.");
                    SaveLogToDatabase(3, "ModelState is invalid while creating an image.");
                    return BadRequest(ModelState);
                }

                var image = _mapper.Map<Image>(imageDto);

                _context.Images.Add(image);
                _context.SaveChanges();

                imageDto.Id = image.Id;
                _logger.LogInformation("Created image with ID {}.", imageDto.Id);
                SaveLogToDatabase(2, $"Created image with ID {imageDto.Id}.");
                return Ok(imageDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while creating the image: {}", ex.Message);
                SaveLogToDatabase(4, $"An error occurred while creating the image: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT api/image/5
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<ImageDto> Update(int id, [FromBody] ImageDto imageDto)
        {
            try
            {
                var image = _context.Images.FirstOrDefault(i => i.Id == id);
                if (image == null)
                {
                    _logger.LogWarning("Image with ID {} not found.", id);
                    SaveLogToDatabase(3, $"Image with ID {id} not found.");
                    return NotFound($"Image with ID {id} not found.");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid while updating the image.");
                    SaveLogToDatabase(3, "ModelState is invalid while updating the image.");
                    return BadRequest(ModelState);
                }

                _mapper.Map(imageDto, image);
                _context.SaveChanges();

                var createdImageDto = _mapper.Map<ImageDto>(image);
                _logger.LogInformation("Updated image with ID {}.", id);
                SaveLogToDatabase(2, $"Updated image with ID {id}.");
                return Ok(createdImageDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while updating the image with ID {}: {}", id, ex.Message);
                SaveLogToDatabase(4, $"An error occurred while updating the image with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE api/image/5
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult<ImageDto> Delete(int id)
        {
            try
            {
                var image = _context.Images.FirstOrDefault(i => i.Id == id);
                if (image == null)
                {
                    _logger.LogWarning("Image with ID {} not found.", id);
                    SaveLogToDatabase(3, $"Image with ID {id} not found.");
                    return NotFound($"Image with ID {id} not found.");
                }

                _context.Images.Remove(image);
                _context.SaveChanges();

                var deletedImageDto = _mapper.Map<ImageDto>(image);
                _logger.LogInformation("Deleted image with ID {}.", id);
                SaveLogToDatabase(2, $"Deleted image with ID {id}.");
                return Ok(deletedImageDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while deleting the image with ID {}: {}", id, ex.Message);
                SaveLogToDatabase(4, $"An error occurred while deleting the image with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
