using System;
using BookHaeven.Dtos.Review;
using BookHaeven.Models;

namespace BookHaeven.Mappers;

public static class ReviewMappers
{
    public static ReviewResponseDto ToDto(this Review review) => new()
    {
        Id = review.Id,
        Username = review.User?.Username ?? "Anonymous",
        Rating = review.Rating,
        Comment = review.Comment,
        CreatedAt = review.CreatedAt
    };
}
