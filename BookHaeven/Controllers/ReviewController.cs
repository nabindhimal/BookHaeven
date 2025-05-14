using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BookHaeven.Data;
using BookHaeven.Dtos;
using BookHaeven.Dtos.Review;
using BookHaeven.Interface;
using BookHaeven.Mappers;
using BookHaeven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookHaeven.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IBookRepository _bookRepository;

        private readonly ApplicationDbContext _context;

        public ReviewsController(
            IReviewRepository reviewRepository,
            IOrderRepository orderRepository,
            IBookRepository bookRepository,
            ApplicationDbContext context)
        {
            _reviewRepository = reviewRepository;
            _orderRepository = orderRepository;
            _bookRepository = bookRepository;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] ReviewDto reviewDto)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // 1. Validate the order exists and belongs to the user
                var order = await _orderRepository.GetByIdAsync(reviewDto.OrderId);
                if (order == null || order.UserId != userId)
                {
                    return BadRequest(new { Message = "Order not found or doesn't belong to you" });
                }

                // 2. Verify the order is completed
                if (order.Status != OrderStatus.Completed)
                {
                    return BadRequest(new { Message = "You can only review books from completed orders" });
                }

                // 3. Verify the book exists in the order
                var bookInOrder = order.OrderItems.Any(oi => oi.BookId == reviewDto.BookId);
                if (!bookInOrder)
                {
                    return BadRequest(new { Message = "This book was not part of your order" });
                }

                // 4. Check if user already reviewed this book from this order
                if (await _reviewRepository.HasReviewedBookAsync(userId, reviewDto.BookId, reviewDto.OrderId))
                {
                    return BadRequest(new { Message = "You've already reviewed this book from this order" });
                }

                // 5. Validate rating
                if (reviewDto.Rating < 1 || reviewDto.Rating > 5)
                {
                    return BadRequest(new { Message = "Rating must be between 1 and 5" });
                }

                // 6. Create the review
                var review = new Review
                {
                    UserId = userId,
                    BookId = reviewDto.BookId,
                    OrderId = reviewDto.OrderId,
                    Rating = reviewDto.Rating,
                    Comment = reviewDto.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                var createdReview = await _reviewRepository.CreateAsync(review);

                // 7. Update book's average rating
                var averageRating = await _reviewRepository.GetAverageRatingAsync(reviewDto.BookId);
                await _bookRepository.UpdateAverageRatingAsync(reviewDto.BookId, averageRating);

                return Ok(new { 
                    Message = "Review submitted successfully",
                    Review = createdReview.ToDto() 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while submitting your review", Error = ex.Message });
            }
        }

        [HttpGet("book/{bookId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsForBook(Guid bookId)
        {
            try
            {
                var reviews = await _reviewRepository.GetByBookIdAsync(bookId);
                return Ok(new
                {
                    Reviews = reviews.Select(r => r.ToDto()),
                    AverageRating = await _reviewRepository.GetAverageRatingAsync(bookId)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving reviews", Error = ex.Message });
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserReviews()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var reviews = await _context.Reviews
                    .Include(r => r.Book)
                    .Include(r => r.Order)
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                return Ok(reviews.Select(r => new
                {
                    r.Id,
                    BookTitle = r.Book?.Name ?? "Unknown Book",
                    BookId = r.BookId,
                    OrderId = r.OrderId,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving your reviews", Error = ex.Message });
            }
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetOrderReviews(Guid orderId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                
                // Verify the order belongs to the user
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null || order.UserId != userId)
                {
                    return BadRequest(new { Message = "Order not found or doesn't belong to you" });
                }

                var reviews = await _context.Reviews
                    .Include(r => r.Book)
                    .Where(r => r.OrderId == orderId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                return Ok(reviews.Select(r => new
                {
                    r.Id,
                    BookTitle = r.Book?.Name ?? "Unknown Book",
                    BookId = r.BookId,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving order reviews", Error = ex.Message });
            }
        }
    }
}