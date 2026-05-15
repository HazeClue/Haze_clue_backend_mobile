using System.ComponentModel.DataAnnotations;

namespace HazeClue.UI.DTOs
{
    public class CreateDeviceDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string MacAddress { get; set; } = string.Empty;
    }
}
