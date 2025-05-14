using System;
using BookHaeven.Models;

namespace BookHaeven.Interface;

public interface IOrderRepository
{
    Task<Order> GetByIdAsync(Guid id);
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId);
    Task<int> GetSuccessfulOrderCount(Guid userId);
    Task<Order> CreateFromCartAsync(Guid userId);
    Task<bool> CancelOrderAsync(Guid orderId, Guid userId);
    Task<bool> CompleteOrderAsync(Guid orderId);

    Task<IEnumerable<Order>> GetPendingOrdersAsync();
}