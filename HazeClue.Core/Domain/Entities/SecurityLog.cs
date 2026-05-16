using System;

namespace HazeClue.Core.Domain.Entities
{
    public class SecurityLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }
        
        public string Event { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string IpAddress { get; set; } = string.Empty;
    }
}
