using System;

namespace HazeClue.Core.Domain.Entities
{
    public class ConsentRecord
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }
        public string ConsentType { get; set; } = "tdcs";
        public DateTime ConsentedAt { get; set; } = DateTime.UtcNow;
        public string IpAddress { get; set; } = string.Empty;
    }
}
