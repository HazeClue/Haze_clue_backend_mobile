namespace HazeClue.UI.DTOs
{
    public class UpdateNotificationSettingsDto
    {
        public bool GeneralNotification { get; set; }
        public bool Sound { get; set; }
        public bool Vibrate { get; set; }
        
        public bool AppUpdates { get; set; }
        public bool ServiceAlerts { get; set; }
        
        public bool NewServiceAvailable { get; set; }
        public bool NewTipsAvailable { get; set; }
    }
}
