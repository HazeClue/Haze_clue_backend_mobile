using System.ComponentModel.DataAnnotations;

namespace HazeClue.UI.DTOs
{
    public class CreateSessionDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public string? DeviceId { get; set; }
    }
}
