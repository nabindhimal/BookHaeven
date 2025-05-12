using System;

namespace BookHaeven.Models;

public class Order 
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? PickupDate { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string ClaimCode { get; set; } = GenerateClaimCode();
    
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual User User { get; set; }

    private static string GenerateClaimCode()
    {
        return Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
    }
}

public enum OrderStatus
{
    Pending,
    Cancelled,
    Completed
}

// OrderItem.cs
public class OrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Guid BookId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public virtual Order Order { get; set; }
    public virtual Book Book { get; set; }
}
