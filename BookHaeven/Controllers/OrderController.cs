using System.Security.Claims;
using BookHaeven.Dtos.Order;
using BookHaeven.Interface;
using BookHaeven.Models;
using BookHaeven.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BookHaeven.Controllers
{
    [Authorize]
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IAnnouncementRepository _announcementRepo;

        public OrderController(IOrderRepository orderRepo, IEmailService emailService, IUserRepository userRepository, IAnnouncementRepository announcementRepo)
        {
            _orderRepo = orderRepo;
            _emailService = emailService;
            _userRepository = userRepository;
            _announcementRepo = announcementRepo;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetUserOrders()
        {
            var userId = GetUserId();
            var orders = await _orderRepo.GetByUserIdAsync(userId);
            return Ok(orders.Select(o => o.ToDto()));
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
        {
            var userId = GetUserId();

            var order = await _orderRepo.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order.ToDto());
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<OrderDto>> Checkout()
        {
            var userId = GetUserId();
            var user = await _userRepository.GetByIdAsync(userId);

            try
            {
                var order = await _orderRepo.CreateFromCartAsync(userId);

                // Send email with claim code
                await _emailService.SendOrderConfirmation(user.Email, order);

                return Ok(order.ToDto());
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetPendingOrders()
        {
            var orders = await _orderRepo.GetPendingOrdersAsync();
            return Ok(orders.Select(o => o.ToDto()));
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var success = await _orderRepo.CancelOrderAsync(id, GetUserId());
            return success ? NoContent() : NotFound();
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteOrder(Guid id)
        {
            var success = await _orderRepo.CompleteOrderAsync(id);
            if (!success) return NotFound();

            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return NotFound();

            // Get book titles from order items
            var bookTitles = order.OrderItems
                .Select(oi => oi.Book?.Name ?? "Unknown Book")
                .Distinct()
                .ToList();

            // announcement message
            var announcementMessage = bookTitles.Count switch
            {
                0 => $"Order {order.ClaimCode} completed!",
                1 => $"Order {order.ClaimCode} completed for book: {bookTitles[0]}",
                _ => $"Order {order.ClaimCode} completed for books: {string.Join(", ", bookTitles.Take(bookTitles.Count - 1))} and {bookTitles.Last()}"
            };

            // Create an announcement that will last for 5 minutes
            var announcement = new Announcement
            {
                Id = Guid.NewGuid(),
                Message = announcementMessage,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddMinutes(5),
                CreatedAt = DateTime.UtcNow
            };

            await _announcementRepo.CreateAsync(announcement);

            return NoContent();
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);


    }
}
