using Microsoft.AspNetCore.Identity;

namespace HazeClue.Core.Domain.Entities
{
    public class AppUser : IdentityUser<string>
    {
        public string FullName { get; set; } = string.Empty;
        public string? OtpCode { get; set; }
        public DateTime? OtpExpiry { get; set; }
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
        public bool OnboardingCompleted { get; set; } = false;
        public string? EligibilityStatus { get; set; }
    }
}
