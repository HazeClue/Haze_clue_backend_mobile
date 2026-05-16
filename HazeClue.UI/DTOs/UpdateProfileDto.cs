using System.ComponentModel.DataAnnotations;

namespace HazeClue.UI.DTOs
{
    public class UpdateProfileDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
        
        public string? Nickname { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
    }
}
