using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using BookHaeven.Dtos.Book;
using BookHaeven.Interface;
using BookHaeven.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookHaeven.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookController : ControllerBase
    {
        private readonly IBookRepository _repo;
        private readonly IBookmarkRepository _bookmarkRepo;


        public BookController(IBookRepository repo, IBookmarkRepository bookmarkRepo)
        {
            _repo = repo;
            _bookmarkRepo = bookmarkRepo;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookDto createBookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingBook = await _repo.GetByTitleAsync(createBookDto.Name);
            if (existingBook != null)
            {
                return Conflict(new { message = "A book with the same name already exists." });
            }

            var existingByIsbn = await _repo.GetByIsbnAsync(createBookDto.ISBN);
            if (existingByIsbn != null)
            {
                return Conflict(new { message = "A book with the same ISBN already exists." });
            }

            var book = createBookDto.ToBookFromCreateBookDto();
            await _repo.CreateAsync(book);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book.ToViewBookDto());

        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePartial(Guid id, [FromBody] UpdateBookDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if ISBN is being changed to one that already exists
            if (!string.IsNullOrWhiteSpace(updateDto.ISBN))
            {
                var existingByIsbn = await _repo.GetByIsbnAsync(updateDto.ISBN);
                if (existingByIsbn != null && existingByIsbn.Id != id)
                {
                    return Conflict(new { message = "A book with the same ISBN already exists." });
                }
            }

            var updatedBook = await _repo.UpdateAsync(id, updateDto);
            if (updatedBook == null)
            {
                return NotFound();
            }

            return Ok(updatedBook.ToViewBookDto());
        }


        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid Id)
        {
            var Book = await _repo.GetByIdAsync(Id);
            if (Book == null) return NotFound();

            // Add bookmark status if user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                Book.IsBookmarked = await _bookmarkRepo.IsBookmarkedAsync(userId, Id);
            }

            return Ok(Book.ToViewBookDto());
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<List<ViewBookDto>>> GetPaginatedBooks(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
            Guid? userId = null;

            // Check if user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }

            var books = await _repo.GetPaginatedAsync(page, pageSize, userId);
            return Ok(books);
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks([FromQuery] BookQueryDto query)
        {
            // CORS headers
            Response.Headers.Append("Access-Control-Allow-Origin", "*");

            // Logging from backend to solve integration problem with frontend
            Console.WriteLine($"Searching books with parameters: {JsonSerializer.Serialize(query)}");
            Console.WriteLine($"Searching books with parameters: Title = {query.Title}, Genre = {query.Genre}, SortBy = {query.SortBy}");
            var books = await _repo.SearchAndFilterAsync(query);
            Console.WriteLine($"Number of books found: {books.Count()}");

            var bookDtos = books.Select(b => b.ToViewBookDto()).ToList();

            if (bookDtos.Count == 0)
            {
                // Log when no books are found
                Console.WriteLine("No books found that match the criteria.");
                return Ok(new { message = "No books match the given criteria." });
            }

            // Log when books are found and returned
            Console.WriteLine($"Returning {bookDtos.Count} books.");
            return Ok(bookDtos);
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var book = await _repo.DeleteAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return NoContent();
        }

    }



}