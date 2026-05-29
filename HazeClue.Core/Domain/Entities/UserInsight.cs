using System;

namespace HazeClue.Core.Domain.Entities
{
    public enum InsightType
    {
        DailyTip,
        WeeklySummary,
        Alert
    }

    public class UserInsight
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }
        
        public InsightType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        
        // Optional JSON payload for complex weekly summaries (e.g. chart data points)
        public string AdditionalDataJson { get; set; } = "{}";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
