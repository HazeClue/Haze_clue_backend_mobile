using System.ComponentModel.DataAnnotations;

namespace HazeClue.UI.DTOs
{
    public class ForgotPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
