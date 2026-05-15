using System.ComponentModel.DataAnnotations;

namespace HazeClue.UI.DTOs
{
    public class SupportTicketDto
    {
        [Required]
        public string Subject { get; set; } = string.Empty;
        [Required]
        public string Message { get; set; } = string.Empty;
    }
}
