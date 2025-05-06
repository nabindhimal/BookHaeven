using System;
using BookHaeven.Data;
using BookHaeven.Interface;
using BookHaeven.Mappers;
using BookHaeven.Models;
using Microsoft.EntityFrameworkCore;

namespace BookHaeven.Repository;

public class BookmarkRepository : IBookmarkRepository
{
    private readonly ApplicationDbContext _context;

    public BookmarkRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Bookmark?> GetByIdAsync(Guid id)
    {
        return await _context.Bookmarks.FindAsync(id);
    }

    public async Task<IEnumerable<Bookmark>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Bookmarks
            .Where(b => b.UserId == userId)
            .Include(b => b.Book)
            .ToListAsync();
    }

    public async Task<Bookmark> CreateAsync(Bookmark bookmark)
    {
        _context.Bookmarks.Add(bookmark);
        await _context.SaveChangesAsync();
        return bookmark;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var bookmark = await _context.Bookmarks.FindAsync(id);
        if (bookmark == null) return false;

        _context.Bookmarks.Remove(bookmark);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsBookmarkedAsync(Guid userId, Guid bookId)
    {
        return await _context.Bookmarks
            .AnyAsync(b => b.UserId == userId && b.BookId == bookId);
    }
}
