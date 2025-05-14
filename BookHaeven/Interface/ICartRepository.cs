using System;
using BookHaeven.Models;

namespace BookHaeven.Interface;

public interface ICartRepository
{
    Task<Cart?> GetByIdAsync(Guid id);
    Task<IEnumerable<Cart>> GetByUserIdAsync(Guid userId);
    Task<Cart> AddOrUpdateAsync(Guid userId, Guid bookId, int quantity);
    Task<bool> RemoveAsync(Guid cartItemId);
    Task ClearCartAsync(Guid userId);
    Task<int> GetItemCountAsync(Guid userId);
    Task UpdateAsync(Cart cartItem);
}
