namespace HazeClue.Core.Domain.Entities
{
    public class Device
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public string Status { get; set; } = "offline";
        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }
    }
}
