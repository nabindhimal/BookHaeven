using System;

namespace BookHaeven.Dtos.Cart;


public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = null!;
    public string BookImageUrl { get; set; } = null!;
    public decimal OriginalPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    
    public decimal Price { get; set; } 
    
    public int Quantity { get; set; }
    public decimal TotalPrice => Price * Quantity;
    public int AvailableStock { get; set; }
}