using Microsoft.AspNetCore.Identity;

namespace HazeClue.Core.Domain.Entities
{
    public class AppUser : IdentityUser<string>
    {
        public string FullName { get; set; } = string.Empty;
    }
}
