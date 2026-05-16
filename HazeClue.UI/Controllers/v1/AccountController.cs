using HazeClue.Core.Domain.Entities;
using HazeClue.UI.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace HazeClue.UI.Controllers.v1
{
    [ApiVersion("1.0")]
    public class AccountController : CustomControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly HazeClue.Infrastructure.DbContext.ApplicationDbContext _context;

        public AccountController(UserManager<AppUser> userManager, IConfiguration configuration, HazeClue.Infrastructure.DbContext.ApplicationDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return BadRequest(new { message = "Email is already in use" });

            var user = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { message = errors });
            }

            var jti = Guid.NewGuid().ToString();
            var token = GenerateJwtToken(user, jti);

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            
            var session = new UserSession
            {
                UserId = user.Id,
                DeviceName = "Registered Device",
                Location = "Unknown Location",
                IpAddress = ipAddress,
                TokenJti = jti
            };
            _context.UserSessions.Add(session);

            var log = new SecurityLog
            {
                UserId = user.Id,
                Event = "Account created and logged in",
                IpAddress = ipAddress
            };
            _context.SecurityLogs.Add(log);

            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                access_token = token,
                user = new { 
                    id = user.Id, 
                    email = user.Email, 
                    fullName = user.FullName,
                    onboardingCompleted = user.OnboardingCompleted
                }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            var isValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isValid)
                return Unauthorized(new { message = "Invalid email or password" });

            var jti = Guid.NewGuid().ToString();
            var token = GenerateJwtToken(user, jti);

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = Request.Headers["User-Agent"].ToString();
            var deviceName = "Unknown Device";
            
            if (!string.IsNullOrEmpty(userAgent))
            {
                if (userAgent.Contains("Android")) deviceName = "Android Device";
                else if (userAgent.Contains("iPhone") || userAgent.Contains("iPad")) deviceName = "iOS Device";
                else if (userAgent.Contains("Windows")) deviceName = "Windows PC";
                else if (userAgent.Contains("Mac OS")) deviceName = "Mac";
                else if (userAgent.Contains("Linux")) deviceName = "Linux PC";
                else deviceName = "Web Browser";
            }

            var session = new UserSession
            {
                UserId = user.Id,
                DeviceName = deviceName,
                Location = "Unknown Location", // Could be mapped via IP
                IpAddress = ipAddress,
                TokenJti = jti
            };
            _context.UserSessions.Add(session);

            var log = new SecurityLog
            {
                UserId = user.Id,
                Event = $"New login from {deviceName}",
                IpAddress = ipAddress
            };
            _context.SecurityLogs.Add(log);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                access_token = token,
                user = new { 
                    id = user.Id, 
                    email = user.Email, 
                    fullName = user.FullName,
                    onboardingCompleted = user.OnboardingCompleted 
                }
            });
        }

        private string GenerateJwtToken(AppUser user, string jti)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"] ?? "7"));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Ok(new { message = "If the email exists, an OTP has been sent." }); // Security best practice

            var otp = new Random().Next(100000, 999999).ToString();
            user.OtpCode = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(15);
            await _userManager.UpdateAsync(user);

            // In a real app, send email here. For demo, we just print or assume it worked.
            Console.WriteLine($"OTP for {user.Email} is {otp}");

            return Ok(new { message = "OTP generated successfully." });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || user.OtpCode != dto.Otp || user.OtpExpiry < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Invalid or expired OTP." });
            }

            // OTP valid, generate a reset token
            var resetToken = Guid.NewGuid().ToString();
            user.ResetToken = resetToken;
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(30);
            user.OtpCode = null; // Clear OTP
            user.OtpExpiry = null;
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "OTP verified.", resetToken });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            // In the mobile flow, the user submits OTP and new password together. We verify OTP again.
            if (user == null || user.OtpCode != dto.Otp || user.OtpExpiry < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Invalid or expired OTP." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });
            }

            user.OtpCode = null;
            user.OtpExpiry = null;
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "Password reset successfully." });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });
            }

            return Ok(new { message = "Password changed successfully." });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Since we use stateless JWT, just returning OK. 
            // Mobile app will delete the token locally.
            return Ok(new { message = "Logged out successfully." });
        }
        [HttpGet("sessions")]
        public async Task<IActionResult> GetActiveSessions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var currentJti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);

            var sessions = await _context.UserSessions
                .Where(s => s.UserId == userId && !s.IsRevoked)
                .OrderByDescending(s => s.LastActive)
                .Select(s => new
                {
                    s.Id,
                    s.DeviceName,
                    s.Location,
                    s.IpAddress,
                    s.LoginTime,
                    s.LastActive,
                    IsCurrent = s.TokenJti == currentJti
                })
                .ToListAsync();

            return Ok(sessions);
        }

        [HttpDelete("sessions/{id}")]
        public async Task<IActionResult> RevokeSession(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var session = await _context.UserSessions.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            
            if (session == null) return NotFound();

            session.IsRevoked = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Session revoked successfully." });
        }

        [HttpDelete("sessions/other")]
        public async Task<IActionResult> RevokeOtherSessions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentJti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);

            var otherSessions = await _context.UserSessions
                .Where(s => s.UserId == userId && s.TokenJti != currentJti && !s.IsRevoked)
                .ToListAsync();

            foreach (var session in otherSessions)
            {
                session.IsRevoked = true;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "All other sessions revoked successfully." });
        }

        [HttpGet("security-logs")]
        public async Task<IActionResult> GetSecurityLogs()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var logs = await _context.SecurityLogs
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .Take(10)
                .Select(l => new
                {
                    l.Id,
                    l.Event,
                    l.CreatedAt,
                    l.IpAddress
                })
                .ToListAsync();

            return Ok(logs);
        }
    }
}
