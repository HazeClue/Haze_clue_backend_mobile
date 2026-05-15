using HazeClue.Core.Domain.Entities;
using HazeClue.Infrastructure.DbContext;
using HazeClue.UI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HazeClue.UI.Controllers.v1
{
    [Authorize]
    [ApiVersion("1.0")]
    public class DevicesController : CustomControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DevicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDevices()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var devices = await _context.Devices.Where(d => d.UserId == userId).ToListAsync();
            return Ok(devices);
        }

        [HttpPost]
        public async Task<IActionResult> AddDevice([FromBody] CreateDeviceDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var existing = await _context.Devices.FirstOrDefaultAsync(d => d.UserId == userId && d.MacAddress == dto.MacAddress);
            if (existing != null) return BadRequest(new { message = "Device already registered." });

            var device = new Device
            {
                UserId = userId!,
                Name = dto.Name,
                MacAddress = dto.MacAddress,
                Status = "offline"
            };

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();
            return Ok(device);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var device = await _context.Devices.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
            if (device == null) return NotFound();
            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
