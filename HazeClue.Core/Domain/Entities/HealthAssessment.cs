using System;

namespace HazeClue.Core.Domain.Entities
{
    public class HealthAssessment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }
        public string AssessmentDataJson { get; set; } = string.Empty;
        public string EligibilityStatus { get; set; } = "pending";
        public string FlagsJson { get; set; } = "[]";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
