using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookHaeven.Data;
using BookHaeven.Interface;
using BookHaeven.Models;
using Microsoft.EntityFrameworkCore;

namespace BookHaeven.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateFromCartAsync(Guid userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // Get cart items
            var cartItems = await _context.Carts
                .Include(c => c.Book)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                throw new InvalidOperationException("Cart is empty");

            // Calculate discounts
            var discount = await CalculateDiscountAsync(userId, cartItems.Count);

            // Create order
            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Pending
            };

            // Convert cart items to order items
            foreach (var cartItem in cartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    BookId = cartItem.BookId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Book.Price,
                    BookImageUrl = cartItem.Book.ImageUrl
                });
            }

            // Calculate totals
            order.TotalAmount = order.OrderItems.Sum(i => i.UnitPrice * i.Quantity);
            order.DiscountAmount = order.TotalAmount * discount;
            order.TotalAmount -= order.DiscountAmount;

            // Save order
            _context.Orders.Add(order);
            
            // Clear cart
            _context.Carts.RemoveRange(cartItems);
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return order;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Order> GetByIdAsync(Guid id)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<int> GetSuccessfulOrderCount(Guid userId)
    {
        return await _context.Orders
            .CountAsync(o => o.UserId == userId && o.Status == OrderStatus.Completed);
    }

    public async Task<bool> CancelOrderAsync(Guid orderId, Guid userId)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
        
        if (order == null || order.Status != OrderStatus.Pending)
            return false;
        
        order.Status = OrderStatus.Cancelled;
        await _context.SaveChangesAsync();
        return true;
    }

    // public async Task<bool> CompleteOrderAsync(Guid orderId, Guid userId)
    // {
    //     var order = await _context.Orders
    //         .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
        
    //     if (order == null || order.Status != OrderStatus.Pending)
    //         return false;
        
    //     order.Status = OrderStatus.Completed;
    //     order.PickupDate = DateTime.UtcNow;
        
    //     await _context.SaveChangesAsync();
    //     return true;
    // }


    public async Task<bool> CompleteOrderAsync(Guid orderId)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId);
        
        if (order == null || order.Status != OrderStatus.Pending)
            return false;
        
        order.Status = OrderStatus.Completed;
        order.PickupDate = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<decimal> CalculateDiscountAsync(Guid userId, int itemCount)
    {
        decimal discount = 0;
        
        // 5% discount for 5+ items
        if (itemCount >= 5) discount += 0.05m;
        
        // Additional 10% for loyal customers (10+ orders)
        var successfulOrders = await GetSuccessfulOrderCount(userId);
        if (successfulOrders >= 10) discount += 0.10m;
        
        return discount;
    }

    public async Task<bool> ExistsAsync(Guid orderId, Guid userId)
    {
        return await _context.Orders
            .AnyAsync(o => o.Id == orderId && o.UserId == userId);
    }

    public async Task<IEnumerable<Order>> GetPendingOrdersAsync()
    {
        return await _context.Orders
            .Where(o => o.Status == OrderStatus.Pending)
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
            .OrderBy(o => o.OrderDate)
            .ToListAsync();
    }


    
}