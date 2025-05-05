using BookHaeven.Dtos.Announcement;
using BookHaeven.Interface;
using BookHaeven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookHaeven.Controllers
{
    [ApiController]
    [Route("api/announcements")]
    public class AnnouncementsController : ControllerBase
    {
        private readonly IAnnouncementRepository _repo;

        public AnnouncementsController(IAnnouncementRepository repo)
        {
            _repo = repo;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnnouncementDto>>> GetAll()
        {
            var announcements = await _repo.GetAllAsync();
            return Ok(announcements.Select(a => ToDto(a)));
        }

        [Authorize]
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<AnnouncementDto>>> GetActive()
        {
            var active = await _repo.GetActiveAsync();
            return Ok(active.Select(a => ToDto(a)));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AnnouncementDto>> Create(CreateAnnouncementDto dto)
        {
            if (dto.EndTime <= dto.StartTime)
                return BadRequest("End time must be after start time.");

            var announcement = new Announcement
            {
                Id = Guid.NewGuid(),
                Message = dto.Message,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };

            var created = await _repo.CreateAsync(announcement);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, ToDto(created));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        private static AnnouncementDto ToDto(Announcement a) => new()
        {
            Id = a.Id,
            Message = a.Message,
            StartTime = a.StartTime,
            EndTime = a.EndTime,
            CreatedAt = a.CreatedAt,
            IsActive = a.IsActive
        };
    }

}
