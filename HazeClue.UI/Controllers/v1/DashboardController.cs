using HazeClue.Infrastructure.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HazeClue.UI.Controllers.v1
{
    [Authorize]
    [ApiVersion("1.0")]
    public class DashboardController : CustomControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sessions = await _context.Sessions
                .Where(s => s.UserId == userId)
                .ToListAsync();

            // Simulate calculating average attention from sessions
            // Here we just return 85% as a dummy if there are no sessions, else 90%
            var avgAttention = sessions.Any() ? 90 : 85; 

            return Ok(new
            {
                avgAttention = avgAttention,
                totalSessions = sessions.Count,
                totalDevices = await _context.Devices.CountAsync(d => d.UserId == userId)
            });
        }
    }
}
