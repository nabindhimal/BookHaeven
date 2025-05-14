using System;
using BookHaeven.Models;

namespace BookHaeven.Interface;

public interface IReviewRepository
{
    Task<Review> CreateAsync(Review review);
    Task<bool> HasPurchasedBookAsync(Guid userId, Guid bookId);
    Task<IEnumerable<Review>> GetByBookIdAsync(Guid bookId);
    Task<double?> GetAverageRatingAsync(Guid bookId);
    Task<bool> HasReviewedBookAsync(Guid userId, Guid bookId, Guid orderId);
}