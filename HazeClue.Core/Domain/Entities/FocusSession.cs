using System;

namespace HazeClue.Core.Domain.Entities
{
    public class FocusSession
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = "active"; // active, completed, paused
        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
