using System.Security.Claims;
using BookHaeven.Dtos.Cart;
using BookHaeven.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookHaeven.Controllers
{
    [Authorize]
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepo;
        private readonly IBookRepository _bookRepo;

        public CartController(ICartRepository cartRepo, IBookRepository bookRepo)
        {
            _cartRepo = cartRepo;
            _bookRepo = bookRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCart()
        {
            var userId = GetUserId();
            var cartItems = await _cartRepo.GetByUserIdAsync(userId);

            return Ok(cartItems.Select(item => new CartItemDto
            {
                Id = item.Id,
                BookId = item.BookId,
                BookTitle = item.Book.Name,
                BookImageUrl = item.Book.ImageUrl,
                OriginalPrice = item.Book.Price,
                DiscountPercentage = item.Book.DiscountPercentage,
                Price = item.Book.DiscountPercentage.HasValue
                    ? item.Book.Price * (1 - item.Book.DiscountPercentage.Value / 100)
                    : item.Book.Price,
                Quantity = item.Quantity,
                AvailableStock = item.Book.Stock
            }));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItem(Guid id, [FromBody] UpdateCartItemDto dto)
        {
            var userId = GetUserId();
            var cartItem = await _cartRepo.GetByIdAsync(id);

            if (cartItem == null || cartItem.UserId != userId)
                return NotFound();

            try
            {
                // Get the book to check stock
                var book = await _bookRepo.GetByIdAsync(cartItem.BookId);
                if (book == null) return NotFound("Book not found");

                if (dto.Quantity <= 0)
                    return BadRequest("Quantity must be positive");

                if (book.Stock < dto.Quantity)
                    return BadRequest(new
                    {
                        error = "STOCK_ERROR",
                        message = $"Not enough stock. Only {book.Stock} available"
                    });

                cartItem.Quantity = dto.Quantity;
                cartItem.UpdatedAt = DateTime.UtcNow;
                await _cartRepo.UpdateAsync(cartItem);

                return Ok(new CartItemDto
                {
                    Id = cartItem.Id,
                    BookId = cartItem.BookId,
                    BookTitle = cartItem.Book.Name,
                    BookImageUrl = cartItem.Book.ImageUrl,
                    OriginalPrice = cartItem.Book.Price,
                    DiscountPercentage = cartItem.Book.DiscountPercentage,
                    Price = cartItem.Book.DiscountPercentage.HasValue
                        ? cartItem.Book.Price * (1 - cartItem.Book.DiscountPercentage.Value / 100)
                        : cartItem.Book.Price,
                    Quantity = cartItem.Quantity,
                    AvailableStock = cartItem.Book.Stock
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "SERVER_ERROR",
                    message = ex.Message
                });
            }
        }


        [HttpPost("add")]
        public async Task<ActionResult<CartItemDto>> AddToCart(AddToCartDto dto)
        {
            var userId = GetUserId();

            var book = await _bookRepo.GetByIdAsync(dto.BookId);
            if (book == null) return NotFound("Book not found");

            try
            {
                var cartItem = await _cartRepo.AddOrUpdateAsync(userId, dto.BookId, dto.Quantity);

                // Calculate price after discount
                var price = book.DiscountPercentage.HasValue
                    ? book.Price * (1 - book.DiscountPercentage.Value / 100)
                    : book.Price;

                return Ok(new CartItemDto
                {
                    Id = cartItem.Id,
                    BookId = cartItem.BookId,
                    BookTitle = book.Name,
                    BookImageUrl = book.ImageUrl,
                    OriginalPrice = book.Price,
                    DiscountPercentage = book.DiscountPercentage,
                    Price = price,
                    Quantity = cartItem.Quantity,
                    AvailableStock = book.Stock
                });
            }
            catch (InvalidOperationException ex)
            {
                // Return proper error object
                return BadRequest(new
                {
                    error = "STOCK_ERROR",
                    message = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "SERVER_ERROR",
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(Guid id)
        {
            var userId = GetUserId();
            var cartItem = await _cartRepo.GetByIdAsync(id);

            if (cartItem == null || cartItem.UserId != userId)
                return NotFound();

            await _cartRepo.RemoveAsync(id);
            return NoContent();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            await _cartRepo.ClearCartAsync(userId);
            return NoContent();
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetCartItemCount()
        {
            var userId = GetUserId();
            var count = await _cartRepo.GetItemCountAsync(userId);
            return Ok(count);
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}
