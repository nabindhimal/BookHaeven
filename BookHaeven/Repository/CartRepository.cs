using System;
using BookHaeven.Data;
using BookHaeven.Interface;
using BookHaeven.Models;
using Microsoft.EntityFrameworkCore;

namespace BookHaeven.Repository;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _context;

    public CartRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetByIdAsync(Guid id)
    {
        return await _context.Carts
            .Include(c => c.Book)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Cart>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Carts
            .Where(c => c.UserId == userId)
            .Include(c => c.Book)
            .ToListAsync();
    }

    public async Task<Cart> AddOrUpdateAsync(Guid userId, Guid bookId, int quantity)
    {
        // Get the book first to check stock
        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
            throw new ArgumentException("Book not found");

        if (book.Stock < quantity)
            throw new InvalidOperationException($"Not enough stock. Only {book.Stock} available");

        var existingItem = await _context.Carts
            .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);

        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + quantity;
            if (book.Stock < newQuantity)
                throw new InvalidOperationException($"Not enough stock. Only {book.Stock} available");

            existingItem.Quantity = newQuantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            existingItem = new Cart
            {
                UserId = userId,
                BookId = bookId,
                Quantity = quantity
            };
            _context.Carts.Add(existingItem);
        }

        await _context.SaveChangesAsync();
        return existingItem;
    }

    public async Task<bool> RemoveAsync(Guid cartItemId)
    {
        var cartItem = await _context.Carts.FindAsync(cartItemId);
        if (cartItem == null) return false;

        _context.Carts.Remove(cartItem);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task ClearCartAsync(Guid userId)
    {
        var cartItems = await _context.Carts
            .Where(c => c.UserId == userId)
            .ToListAsync();

        _context.Carts.RemoveRange(cartItems);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetItemCountAsync(Guid userId)
    {
        return await _context.Carts
            .Where(c => c.UserId == userId)
            .SumAsync(c => c.Quantity);
    }
}
