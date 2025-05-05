using System;
using BookHaeven.Data;
using BookHaeven.Interface;
using BookHaeven.Models;
using Microsoft.EntityFrameworkCore;

namespace BookHaeven.Repository;

public class AnnouncementRepository : IAnnouncementRepository
{
    private readonly ApplicationDbContext _context;

    public AnnouncementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Announcement>> GetAllAsync()
    {
        return await _context.Announcements.ToListAsync();
    }

    public async Task<IEnumerable<Announcement>> GetActiveAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Announcements
            .Where(a => now >= a.StartTime && now <= a.EndTime)
            .ToListAsync();
    }

    public async Task<Announcement?> GetByIdAsync(Guid id)
    {
        return await _context.Announcements.FindAsync(id);
    }

    public async Task<Announcement> CreateAsync(Announcement announcement)
    {
        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();
        return announcement;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.Announcements.FindAsync(id);
        if (existing == null) return false;

        _context.Announcements.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}

