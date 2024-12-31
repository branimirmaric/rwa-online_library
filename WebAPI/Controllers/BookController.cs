using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Dtos;
using WebAPI.Models;


namespace WebAPI.Controllers
{
    [Route("api/book")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly OnlineLibraryContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BookController> _logger;

        public BookController(OnlineLibraryContext context, IMapper mapper, ILogger<BookController> logger)
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

        // GET: api/book
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<BookDto>> GetAll()
        {
            try
            {
                var books = _context.Books.Include(b => b.Genre).Include(b => b.Image).ToList();
                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books);
                _logger.LogInformation("Retrieved all books.");
                SaveLogToDatabase(2, "Retrieved all books.");
                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving all books: {}", ex.Message);
                SaveLogToDatabase(4, $"An error occurred while retrieving all books: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/book/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<BookDto> Get(int id)
        {
            try
            {
                var book = _context.Books.Include(b => b.Genre).Include(b => b.Image).FirstOrDefault(b => b.Id == id);
                if (book == null)
                {
                    _logger.LogWarning("Book with ID {} not found.", id);
                    SaveLogToDatabase(3, $"Book with ID {id} not found.");
                    return NotFound($"Book with ID {id} not found.");
                }

                var bookDto = _mapper.Map<BookDto>(book);
                _logger.LogInformation("Retrieved book with ID {}.", id);
                SaveLogToDatabase(2, $"Retrieved book with ID {id}.");
                return Ok(bookDto);
            }
            catch (Exception ex)
            {

                _logger.LogError("An error occurred while retrieving the book with ID {}: {}", id, ex.Message);
                SaveLogToDatabase(4, $"An error occurred while retrieving the book with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST api/book
        [HttpPost]
        [Authorize]
        public ActionResult<BookDto> Create([FromBody] BookDto bookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid while creating a book.");
                    SaveLogToDatabase(3, "ModelState is invalid while creating a book.");
                    return BadRequest(ModelState);
                }

                var book = _mapper.Map<Book>(bookDto);

                _context.Books.Add(book);
                _context.SaveChanges();

                bookDto.Id = book.Id;
                _logger.LogInformation("Created book with ID {}.", bookDto.Id);
                SaveLogToDatabase(2, $"Created book with ID {bookDto.Id}.");
                return Ok(bookDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while creating the book: {}", ex.Message);
                SaveLogToDatabase(4, $"An error occurred while creating the book: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT api/book/5
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<BookDto> Update(int id, [FromBody] BookDto bookDto)
        {
            try
            {
                var book = _context.Books.FirstOrDefault(b => b.Id == id);
                if (book == null)
                {
                    _logger.LogWarning("Book with ID {} not found.", id);
                    SaveLogToDatabase(3, $"Book with ID {id} not found.");
                    return NotFound($"Book with ID {id} not found.");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid while updating the book.");
                    SaveLogToDatabase(3, "ModelState is invalid while updating the book.");
                    return BadRequest(ModelState);
                }

                _mapper.Map(bookDto, book);
                _context.SaveChanges();

                var updatedBookDto = _mapper.Map<BookDto>(book);
                _logger.LogInformation("Updated book with ID {}.", id);
                SaveLogToDatabase(2, $"Updated book with ID {id}.");
                return Ok(updatedBookDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while updating the book with ID {}: {}", id, ex.Message);
                SaveLogToDatabase(4, $"An error occurred while updating the book with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE api/book/5
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult<BookDto> Delete(int id)
        {
            try
            {
                var book = _context.Books.FirstOrDefault(b => b.Id == id);
                if (book == null)
                {
                    _logger.LogWarning("Book with ID {} not found.", id);
                    SaveLogToDatabase(3, $"Book with ID {id} not found.");
                    return NotFound($"Book with ID {id} not found.");
                }

                _context.Books.Remove(book);
                _context.SaveChanges();

                var deletedBookDto = _mapper.Map<BookDto>(book);
                _logger.LogInformation("Deleted book with ID {}.", id);
                SaveLogToDatabase(2, $"Deleted book with ID {id}.");
                return Ok(deletedBookDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while deleting the book with ID {}: {}", id, ex.Message);
                SaveLogToDatabase(4, $"An error occurred while deleting the book with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/book/search
        [HttpGet("search")]
        public ActionResult<IEnumerable<BookDto>> Search(string name, int page = 1, int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    _logger.LogWarning("Search parameter 'name' is required.");
                    SaveLogToDatabase(3, "Search parameter 'name' is required.");
                    return BadRequest("Search parameter 'name' is required.");
                }

                var query = _context.Books
                                    .Where(b => b.Title.Contains(name))
                                    .Include(b => b.Genre)
                                    .Include(b => b.Image)
                                    .AsQueryable();

                // Straničenje
                var totalItems = query.Count();
                var books = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                if (!books.Any())
                {
                    _logger.LogWarning("No books found matching the search criteria.");
                    SaveLogToDatabase(3, "No books found matching the search criteria.");
                    return NotFound("No books found with the given search criteria.");
                }

                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books);

                // Vraćanje rezultata uz broj stranica i trenutnu stranicu
                var response = new
                {
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                    Books = bookDtos
                };

                _logger.LogInformation("Retrieved books for search '{}'.", name);
                SaveLogToDatabase(2, $"Retrieved books for search '{name}'.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while searching for books: {}", ex.Message);
                SaveLogToDatabase(4, $"An error occurred while searching for books: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
