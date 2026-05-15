using HazeClue.Core.Domain.Entities;
using HazeClue.Infrastructure.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HazeClue.UI.Controllers.v1
{
    [Authorize]
    [ApiVersion("1.0")]
    public class AssessmentsController : CustomControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AssessmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("health")]
        public IActionResult SubmitHealthAssessment([FromBody] object assessmentData)
        {
            // Placeholder: Parse assessmentData and save to a HealthAssessment entity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            return Ok(new { message = "Health assessment saved successfully." });
        }

        [HttpPost("tdcs-consent")]
        public IActionResult SubmitTdcsConsent([FromBody] object consentData)
        {
            // Placeholder: Parse consentData and save to a ConsentRecord entity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            return Ok(new { message = "TDCS Consent recorded successfully." });
        }
    }
}
