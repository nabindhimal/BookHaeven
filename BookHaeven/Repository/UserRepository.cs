using System;
using BookHaeven.Data;
using BookHaeven.Interface;
using BookHaeven.Models;
using Microsoft.EntityFrameworkCore;

namespace BookHaeven.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<User> AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> DeleteAsync(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null) return null;

        _context.Remove(user);
        _context.SaveChanges();
        return user;

    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public Task UpdateAsync(User user)
    {
        throw new NotImplementedException();
    }
}
