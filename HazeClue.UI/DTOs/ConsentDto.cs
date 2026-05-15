using System.ComponentModel.DataAnnotations;

namespace HazeClue.UI.DTOs
{
    public class ConsentDto
    {
        [Required]
        public bool AgreedToTdcs { get; set; }
        [Required]
        public bool AgreedToDataCollection { get; set; }
    }
}
