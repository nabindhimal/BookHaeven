using System;

namespace BookHaeven.Dtos.Order;
using BookHaeven.Models;


// OrderDto.cs
public class OrderDto
{
    public Guid Id { get; set; }
    public string ClaimCode { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? PickupDate { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public Guid BookId { get; set; }
    public string BookTitle { get; set; }
    public int Quantity { get; set; }
    public string BookImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;
}

// Extension method for mapping
public static class OrderMappers
{
    public static OrderDto ToDto(this Order order) => new()
    {
        Id = order.Id,
        ClaimCode = order.ClaimCode,
        OrderDate = order.OrderDate,
        PickupDate = order.PickupDate,
        Status = order.Status.ToString(),
        TotalAmount = order.TotalAmount,
        DiscountAmount = order.DiscountAmount,
        Items = order.OrderItems.Select(i => new OrderItemDto
        {
            BookId = i.BookId,
            BookTitle = i.Book?.Name ?? "Unknown",
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            BookImageUrl = i.BookImageUrl ?? i.Book?.ImageUrl
            // BookImageUrl = i.BookImageUrl
        }).ToList()
    };
}
