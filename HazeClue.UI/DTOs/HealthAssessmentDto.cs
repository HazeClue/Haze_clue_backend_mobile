using System.ComponentModel.DataAnnotations;

namespace HazeClue.UI.DTOs
{
    public class HealthAssessmentDto
    {
        public object? MultiSelections { get; set; }
        public object? SingleSelections { get; set; }
        public int? Age { get; set; }
    }
}
