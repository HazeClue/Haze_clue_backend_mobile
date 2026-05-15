using System;

namespace HazeClue.Core.Domain.Entities
{
    public class PuzzleResult
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SessionId { get; set; } = string.Empty;
        public FocusSession? Session { get; set; }
        public int Score { get; set; }
        public int CompletionTimeSeconds { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
