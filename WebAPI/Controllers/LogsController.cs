using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;


namespace WebAPI.Controllers
{
    [Route("api/logs")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly OnlineLibraryContext _context;

        public LogsController(OnlineLibraryContext context)
        {
            _context = context;
        }

        // GET: api/logs/get/{N}
        [HttpGet("get/{N?}")]
        public ActionResult<IEnumerable<Log>> GetLogs(int N = 10)
        {
            var logs = _context.Logs
                               .OrderByDescending(l => l.Timestamp)
                               .Take(N)
                               .ToList();

            return Ok(logs);
        }

        // GET: api/logs/count
        [HttpGet("count")]
        public ActionResult<int> GetLogCount()
        {
            var count = _context.Logs.Count();
            return Ok(count);
        }
    }
}
