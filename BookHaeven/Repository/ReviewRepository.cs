using System;
using BookHaeven.Data;
using BookHaeven.Interface;
using BookHaeven.Models;
using Microsoft.EntityFrameworkCore;

namespace BookHaeven.Repository;


public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IOrderRepository _orderRepository;

    public ReviewRepository(ApplicationDbContext context, IOrderRepository orderRepository)
    {
        _context = context;
        _orderRepository = orderRepository;
    }

    public async Task<Review> CreateAsync(Review review)
    {
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        
        // Update book's average rating
        var averageRating = await GetAverageRatingAsync(review.BookId);
        var book = await _context.Books.FindAsync(review.BookId);
        if (book != null)
        {
            book.AverageRating = averageRating;
            await _context.SaveChangesAsync();
        }
        
        return review;
    }

    public async Task<bool> HasPurchasedBookAsync(Guid userId, Guid bookId)
    {
        // Check if there's any completed order for this user that contains this book
        return await _context.Orders
            .Include(o => o.OrderItems)
            .AnyAsync(o => o.UserId == userId && 
                         o.Status == OrderStatus.Completed &&
                         o.OrderItems.Any(oi => oi.BookId == bookId));
    }

    public async Task<IEnumerable<Review>> GetByBookIdAsync(Guid bookId)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Order)
            .Where(r => r.BookId == bookId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<double?> GetAverageRatingAsync(Guid bookId)
    {
        return await _context.Reviews
            .Where(r => r.BookId == bookId)
            .AverageAsync(r => (double?)r.Rating);
    }

    public async Task<bool> HasReviewedBookAsync(Guid userId, Guid bookId, Guid orderId)
    {
        return await _context.Reviews
            .AnyAsync(r => r.UserId == userId && 
                          r.BookId == bookId && 
                          r.OrderId == orderId);
    }

    public async Task<bool> IsBookInOrder(Guid orderId, Guid bookId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        return order?.OrderItems.Any(oi => oi.BookId == bookId) ?? false;
    }
}