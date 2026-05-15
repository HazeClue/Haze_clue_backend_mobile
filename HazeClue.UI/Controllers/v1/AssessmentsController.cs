using HazeClue.Core.Domain.Entities;
using HazeClue.Infrastructure.DbContext;
using HazeClue.UI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace HazeClue.UI.Controllers.v1
{
    [Authorize]
    [ApiVersion("1.0")]
    public class AssessmentsController : CustomControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AssessmentsController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("health")]
        public async Task<IActionResult> SubmitHealthAssessment([FromBody] HealthAssessmentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var assessment = new HealthAssessment
            {
                UserId = userId,
                AssessmentDataJson = JsonSerializer.Serialize(new { dto.MultiSelections, dto.SingleSelections, dto.Age }),
                EligibilityStatus = "eligible", // Defaulting to eligible for now
                CreatedAt = DateTime.UtcNow
            };

            _context.HealthAssessments.Add(assessment);
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Health assessment saved successfully.", eligibilityStatus = assessment.EligibilityStatus });
        }

        [HttpPost("tdcs-consent")]
        public async Task<IActionResult> SubmitTdcsConsent([FromBody] ConsentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var consent = new ConsentRecord
            {
                UserId = userId,
                ConsentType = "tdcs",
                ConsentedAt = DateTime.UtcNow,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"
            };

            _context.ConsentRecords.Add(consent);

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.OnboardingCompleted = true;
                await _userManager.UpdateAsync(user);
            }

            await _context.SaveChangesAsync();
            
            return Ok(new { message = "TDCS Consent recorded successfully." });
        }
    }
}
