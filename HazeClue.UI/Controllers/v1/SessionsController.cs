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
        public async Task<IActionResult> CompleteSession(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (session == null) return NotFound();

            session.Status = "completed";
            session.CompletedAt = DateTime.UtcNow;
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
    }
}
