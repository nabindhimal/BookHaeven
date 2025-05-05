using System;
using BookHaeven.Models;

namespace BookHaeven.Interface;

public interface IAnnouncementRepository
{
    Task<IEnumerable<Announcement>> GetAllAsync();
    Task<IEnumerable<Announcement>> GetActiveAsync();
    Task<Announcement?> GetByIdAsync(Guid id);
    Task<Announcement> CreateAsync(Announcement announcement);
    Task<bool> DeleteAsync(Guid id);
}

