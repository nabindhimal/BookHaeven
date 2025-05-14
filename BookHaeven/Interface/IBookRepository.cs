using System;
using BookHaeven.Dtos.Book;
using BookHaeven.Models;

namespace BookHaeven.Interface;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id);
    Task<Book> CreateAsync(Book book);
    Task<Book?> UpdateAsync(Guid id, UpdateBookDto bookDto);
    Task<Book?> DeleteAsync(Guid id);

    Task<List<ViewBookDto>> GetPaginatedAsync(int pageNumber, int pageSize, Guid? userId);
    Task<Book?> GetByTitleAsync(string title);

    Task<IEnumerable<Book>> SearchAndFilterAsync(BookQueryDto query);

    Task<Book> GetByIsbnAsync(string isbn);
    Task UpdateAverageRatingAsync(Guid bookId, double? averageRating);
}
