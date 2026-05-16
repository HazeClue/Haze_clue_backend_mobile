using HazeClue.Core.Domain.Entities;
using HazeClue.Infrastructure.DbContext;
using HazeClue.UI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HazeClue.UI.Controllers.v1
{
    [Authorize]
    [ApiVersion("1.0")]
    public class SessionsController : CustomControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetSessions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sessions = await _context.Sessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            return Ok(sessions);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSession([FromBody] CreateSessionDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = new FocusSession
            {
                UserId = userId!,
                Title = dto.Title,
                DurationMinutes = dto.DurationMinutes,
                DeviceId = dto.DeviceId,
                CreatedAt = DateTime.UtcNow,
                Status = "active"
            };
            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();
            return Ok(session);
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteSession(string id, [FromBody] CompleteSessionDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (session == null) return NotFound();

            session.Status = "completed";
            session.CompletedAt = DateTime.UtcNow;
            session.AverageConcentration = dto.AverageConcentration;
            session.ActualDurationSeconds = dto.ActualDurationSeconds;
            
            await _context.SaveChangesAsync();
            return Ok(session);
        }

        [HttpPost("{id}/pause")]
        public async Task<IActionResult> PauseSession(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (session == null) return NotFound();

            session.Status = "paused";
            await _context.SaveChangesAsync();
            return Ok(session);
        }

        [HttpPost("{id}/resume")]
        public async Task<IActionResult> ResumeSession(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (session == null) return NotFound();

            session.Status = "active";
            await _context.SaveChangesAsync();
            return Ok(session);
        }

        [HttpPost("{id}/score")]
        public async Task<IActionResult> SubmitScore(string id, [FromBody] PuzzleScoreDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (session == null) return NotFound();

            var puzzleResult = new PuzzleResult
            {
                SessionId = id,
                Score = dto.Score,
                CompletionTimeSeconds = dto.CompletionTimeSeconds
            };

            _context.PuzzleResults.Add(puzzleResult);
            await _context.SaveChangesAsync();
            return Ok(puzzleResult);
        }
        [HttpGet("insights")]
        public async Task<IActionResult> GetInsights()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sessions = await _context.Sessions
                .Where(s => s.UserId == userId && s.Status == "completed")
                .ToListAsync();

            var totalFocusMinutes = sessions.Sum(s => s.DurationMinutes);
            var activeDaysCount = sessions.Select(s => s.CreatedAt.Date).Distinct().Count();
            var averageMinutesPerDay = activeDaysCount > 0 ? (int)Math.Round((double)totalFocusMinutes / activeDaysCount) : 0;

            // Weekly Data (last 7 days)
            var weeklyData = new List<int>();
            for (int i = 6; i >= 0; i--)
            {
                var targetDate = DateTime.UtcNow.Date.AddDays(-i);
                var dailySum = sessions.Where(s => s.CreatedAt.Date == targetDate).Sum(s => s.DurationMinutes);
                weeklyData.Add(dailySum);
            }

            // Monthly Data (last 6 months)
            var monthlyData = new List<int>();
            for (int i = 5; i >= 0; i--)
            {
                var targetMonth = DateTime.UtcNow.Date.AddMonths(-i);
                var monthlySum = sessions
                    .Where(s => s.CreatedAt.Year == targetMonth.Year && s.CreatedAt.Month == targetMonth.Month)
                    .Sum(s => s.DurationMinutes);
                monthlyData.Add(monthlySum);
            }

            return Ok(new
            {
                totalFocusMinutes,
                averageMinutesPerDay,
                weeklyData,
                monthlyData
            });
        }
    }
}
