using HazeClue.UI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HazeClue.UI.Controllers.v1
{
    [Authorize]
    [ApiVersion("1.0")]
    public class SupportController : CustomControllerBase
    {
        [HttpPost("ticket")]
        public IActionResult SubmitTicket([FromBody] SupportTicketDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Log support ticket to console
            Console.WriteLine($"New Support Ticket: {dto.Subject}");
            Console.WriteLine($"Message: {dto.Message}");

            return Ok(new { message = "Support ticket submitted successfully." });
        }
    }
}
