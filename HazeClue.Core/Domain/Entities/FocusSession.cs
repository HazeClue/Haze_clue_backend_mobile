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
        public int DurationMinutes { get; set; } = 0;
        public int Intensity { get; set; } = 0;
        public string? DeviceId { get; set; }
        public Device? Device { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int AverageConcentration { get; set; } = 0;
        public int ActualDurationSeconds { get; set; } = 0;
    }
}
