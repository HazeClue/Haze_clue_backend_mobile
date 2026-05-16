using System;

namespace HazeClue.Core.Domain.Entities
{
    public class UserSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }
        
        public string DeviceName { get; set; } = "Unknown Device";
        public string Location { get; set; } = "Unknown Location";
        public string IpAddress { get; set; } = string.Empty;
        
        public DateTime LoginTime { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        
        public bool IsRevoked { get; set; } = false;
        
        public string TokenJti { get; set; } = string.Empty;
    }
}
