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
        private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache _cache;

        public DashboardController(ApplicationDbContext context, Microsoft.Extensions.Caching.Memory.IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var cacheKey = $"DashboardStats_{userId}";
            
            if (_cache.TryGetValue(cacheKey, out var cachedStats))
            {
                return Ok(cachedStats);
            }

            var sessions = await _context.Sessions
                .Where(s => s.UserId == userId)
                .ToListAsync();

            // Simulate calculating average attention from sessions
            // Here we just return 85% as a dummy if there are no sessions, else 90%
            var avgAttention = sessions.Any() ? 90 : 85; 

            var stats = new
            {
                avgAttention = avgAttention,
                totalSessions = sessions.Count,
                totalDevices = await _context.Devices.CountAsync(d => d.UserId == userId)
            };

            var cacheEntryOptions = new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            _cache.Set(cacheKey, stats, cacheEntryOptions);

            return Ok(stats);
        }
    }
}
