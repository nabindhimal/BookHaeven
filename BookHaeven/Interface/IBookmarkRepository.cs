using System;
using BookHaeven.Models;

namespace BookHaeven.Interface;

public interface IBookmarkRepository
{
    Task<Bookmark?> GetByIdAsync(Guid id);
    Task<IEnumerable<Bookmark>> GetByUserIdAsync(Guid userId);
    Task<Bookmark> CreateAsync(Bookmark bookmark);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> IsBookmarkedAsync(Guid userId, Guid bookId);

}
