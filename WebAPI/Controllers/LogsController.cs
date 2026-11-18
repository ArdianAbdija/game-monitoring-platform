using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameMonitoring.Data.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    
    /// API controller that exposes stored application log records and log counts.
   
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LogsController : ControllerBase
    {
        private readonly GameMonitoringContext _context;

        
        /// Initializes the LogsController and receives required services through dependency injection.
       
        public LogsController(GameMonitoringContext context)
        {
            _context = context;
        }

        // GET: api/logs/get/10
        
        /// Returns the most recent log entries, limited by the number requested by the client.
       
        [HttpGet("get/{n}")]
        public async Task<IActionResult> GetLastLogs(int n)
        {
            if (n <= 0)
                return BadRequest("Number of logs must be greater than 0.");

            var logs = await _context.Logs
                .OrderByDescending(l => l.Timestamp)
                .Take(n)
                .ToListAsync();

            return Ok(logs);
        }

        // GET: api/logs/count
        
        /// Returns the total number of log records stored in the database.
       
        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _context.Logs.CountAsync();
            return Ok(count);
        }
    }
}