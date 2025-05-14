using System.Security.Claims;
using BookHaeven.Dtos.Bookmark;
using BookHaeven.Interface;
using BookHaeven.Mappers;
using BookHaeven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookHaeven.Controllers
{
    [Authorize]
    [Route("api/bookmarks")]
    [ApiController]
    public class BookmarkController : ControllerBase
    {
        private readonly IBookmarkRepository _bookmarkRepo;
        private readonly IBookRepository _bookRepo;
        private readonly IUserRepository _userRepo;

        public BookmarkController(
        IBookmarkRepository bookmarkRepo,
        IBookRepository bookRepo,
        IUserRepository userRepo)
        {
            _bookmarkRepo = bookmarkRepo;
            _bookRepo = bookRepo;
            _userRepo = userRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookmarkDto>>> GetUserBookmarks()
        {
            var userId = GetUserId();
            var bookmarks = await _bookmarkRepo.GetByUserIdAsync(userId);
            return Ok(bookmarks.Select(b => ToDto(b)));
        }

        [HttpPost]
        public async Task<ActionResult<BookmarkDto>> CreateBookmark(CreateBookmarkDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("User ID not found in token.");

            // Check if book exists
            var book = await _bookRepo.GetByIdAsync(dto.BookId);
            if (book == null) return NotFound("Book not found");

            // Check if already bookmarked
            if (await _bookmarkRepo.IsBookmarkedAsync(userId, dto.BookId))
                return BadRequest("Book already bookmarked");

            var bookmark = new Bookmark
            {
                UserId = userId,
                BookId = dto.BookId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _bookmarkRepo.CreateAsync(bookmark);
            return CreatedAtAction(nameof(GetUserBookmarks), new { id = created.Id }, ToDto(created));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookmark([FromRoute] Guid id)
        {
            var userId = GetUserId();
            var bookmark = await _bookmarkRepo.GetByIdAsync(id);

            if (bookmark == null || bookmark.UserId != userId)
                return NotFound();

            await _bookmarkRepo.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("check/{bookId}")]
        public async Task<ActionResult<bool>> IsBookmarked(Guid bookId)
        {
            var userId = GetUserId();
            return await _bookmarkRepo.IsBookmarkedAsync(userId, bookId);
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        private static BookmarkDto ToDto(Bookmark bookmark) => new()
        {
            Id = bookmark.Id,
            BookId = bookmark.BookId,
            CreatedAt = bookmark.CreatedAt,
            Book = bookmark.Book?.ToViewBookDto()
        };
    }
}
