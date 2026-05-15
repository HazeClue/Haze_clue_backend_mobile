using System.ComponentModel.DataAnnotations;

namespace HazeClue.UI.DTOs
{
    public class PuzzleScoreDto
    {
        [Required]
        public int Score { get; set; }
        public int CompletionTimeSeconds { get; set; }
    }
}
