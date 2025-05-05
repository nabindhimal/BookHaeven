using System;
using System.Collections.Generic;
using System.Linq;
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


        public BookController(IBookRepository repo)
        {
            _repo = repo;
        }

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

        
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid Id)
        {
            var Book = await _repo.GetByIdAsync(Id);
            if (Book == null) return NotFound();
            return Ok(Book.ToViewBookDto());
        }



        [HttpGet("paginated")]
        public async Task<IActionResult> GetPaginatedBooks([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var books = await _repo.GetPaginatedAsync(page, pageSize);
            return Ok(books);
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks([FromQuery] BookQueryDto query)
        {

            var books = await _repo.SearchAndFilterAsync(query);
            var bookDtos = books.Select(b => b.ToViewBookDto()).ToList();
            if (bookDtos.Count == 0) return Ok("No books match the given criteria.");

            return Ok(bookDtos);
        }
    }



}