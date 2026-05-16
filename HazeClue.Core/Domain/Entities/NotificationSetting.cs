using System;

namespace HazeClue.Core.Domain.Entities
{
    public class NotificationSetting
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }
        
        public bool GeneralNotification { get; set; } = true;
        public bool Sound { get; set; } = false;
        public bool Vibrate { get; set; } = true;
        
        public bool AppUpdates { get; set; } = false;
        public bool ServiceAlerts { get; set; } = true;
        
        public bool NewServiceAvailable { get; set; } = false;
        public bool NewTipsAvailable { get; set; } = true;
    }
}
