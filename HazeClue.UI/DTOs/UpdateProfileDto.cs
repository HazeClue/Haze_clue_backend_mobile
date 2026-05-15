using System.ComponentModel.DataAnnotations;

namespace HazeClue.UI.DTOs
{
    public class UpdateProfileDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
    }
}
