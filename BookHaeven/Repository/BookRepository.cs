using System;
using BookHaeven.Data;
using BookHaeven.Dtos.Book;
using BookHaeven.Interface;
using BookHaeven.Mappers;
using BookHaeven.Models;
using Microsoft.EntityFrameworkCore;

namespace BookHaeven.Repository;

public class BookRepository : IBookRepository
{

    private readonly ApplicationDbContext _context;

    public BookRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Book> CreateAsync(Book book)
    {
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<Book?> DeleteAsync(Guid id)
    {
        var book = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);
        if (book == null) return null;

        _context.Books.Remove(book);
        _context.SaveChanges();
        return book;
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        return await _context.Books
            .Include(b => b.Reviews)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Book?> GetByTitleAsync(string title)
    {
        return await _context.Books.FirstOrDefaultAsync(b => b.Name.ToLower() == title.ToLower());
    }

    public async Task<List<ViewBookDto>> GetPaginatedAsync(int pageNumber, int pageSize, Guid? userId = null)
    {
        var query = _context.Books.AsQueryable();

        // If userId is provided, include bookmark information
        if (userId.HasValue)
        {
            query = query.Select(b => new Book
            {
                Id = b.Id,
                Name = b.Name,
                ISBN = b.ISBN,
                Author = b.Author,
                Description = b.Description,
                Price = b.Price,
                Stock = b.Stock,
                Language = b.Language,
                Publisher = b.Publisher,
                PublicationDate = b.PublicationDate,
                Genre = b.Genre,
                DiscountPercentage = b.DiscountPercentage,
                IsOnSale = b.IsOnSale,
                ImageUrl = b.ImageUrl,
                AverageRating = b.AverageRating,
                SaleStartDate = b.SaleStartDate,
                SaleEndDate = b.SaleEndDate,
                CreatedAt = DateTime.UtcNow,
                IsAvailableInLibrary = b.IsAvailableInLibrary,
                IsBookmarked = b.Bookmarks.Any(bm => bm.UserId == userId.Value)
            });
        }

        return await query
            .OrderBy(b => b.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(b => b.ToViewBookDto())
            .ToListAsync();
    }


    public async Task<IEnumerable<Book>> SearchAndFilterAsync(BookQueryDto query)
    {
        var books = _context.Books
            .Include(b => b.Reviews)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Title))
            books = books.Where(b => b.Name.ToLower().Contains(query.Title.ToLower()));

        if (query.MinPrice.HasValue)
            books = books.Where(b => b.Price >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            books = books.Where(b => b.Price <= query.MaxPrice.Value);

        if (!string.IsNullOrWhiteSpace(query.Genre))
            books = books.Where(b => b.Genre.ToLower() == query.Genre.ToLower());

        if (!string.IsNullOrWhiteSpace(query.Publisher))
            books = books.Where(b => b.Publisher.ToLower() == query.Publisher.ToLower());

        if (!string.IsNullOrWhiteSpace(query.Author))
            books = books.Where(b => b.Author.ToLower() == query.Author.ToLower());

        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            books = query.SortBy.ToLower() switch
            {
                "price_asc" => books.OrderBy(b => b.Price),
                "price_desc" => books.OrderByDescending(b => b.Price),
                "title_asc" => books.OrderBy(b => b.Name),
                "title_desc" => books.OrderByDescending(b => b.Name),
                "date_desc" => books.OrderByDescending(b => b.PublicationDate),
                "date_asc" => books.OrderBy(b => b.PublicationDate),
                "popularity" => books.OrderByDescending(b => b.Reviews.Count),
                _ => books
            };
        }

        return await books.ToListAsync();
    }


    public async Task<Book> GetByIsbnAsync(string isbn)
    {
        return await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);
    }

    public async Task<Book?> UpdateAsync(Guid id, UpdateBookDto bookDto)
    {
        var existingBook = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);
        if (existingBook == null) return null;

        // Update only the properties that were provided in the DTO
        if (!string.IsNullOrWhiteSpace(bookDto.Name))
            existingBook.Name = bookDto.Name;

        if (!string.IsNullOrWhiteSpace(bookDto.ISBN))
            existingBook.ISBN = bookDto.ISBN;

        if (!string.IsNullOrWhiteSpace(bookDto.Description))
            existingBook.Description = bookDto.Description;

        if (!string.IsNullOrWhiteSpace(bookDto.Author))
            existingBook.Author = bookDto.Author;

        if (!string.IsNullOrWhiteSpace(bookDto.Publisher))
            existingBook.Publisher = bookDto.Publisher;

        if (!string.IsNullOrWhiteSpace(bookDto.Language))
            existingBook.Language = bookDto.Language;

        if (!string.IsNullOrWhiteSpace(bookDto.Genre))
            existingBook.Genre = bookDto.Genre;

        if (bookDto.PublicationDate != default)
            existingBook.PublicationDate = bookDto.PublicationDate;

        existingBook.Price = bookDto.Price;
        existingBook.Stock = bookDto.Stock;
        existingBook.IsAvailableInLibrary = bookDto.IsAvailableInLibrary;
        existingBook.IsOnSale = bookDto.IsOnSale;
        existingBook.DiscountPercentage = bookDto.DiscountPercentage;
        existingBook.SaleStartDate = bookDto.SaleStartDate;
        existingBook.SaleEndDate = bookDto.SaleEndDate;

        if (!string.IsNullOrWhiteSpace(bookDto.ImageUrl))
            existingBook.ImageUrl = bookDto.ImageUrl;

        await _context.SaveChangesAsync();
        return existingBook;
    }

    public async Task UpdateAverageRatingAsync(Guid bookId, double? averageRating)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book != null)
        {
            book.AverageRating = averageRating;
            await _context.SaveChangesAsync();
        }
    }

}
