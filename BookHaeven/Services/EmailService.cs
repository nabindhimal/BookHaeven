using System;
using BookHaeven.Models;

namespace BookHaeven.Services;

public interface IEmailService
{
    Task SendOrderConfirmation(string email, Order order);
}

public class EmailService : IEmailService
{
    public async Task SendOrderConfirmation(string email, Order order)
    {
        var subject = $"Your BookHaven Order #{order.ClaimCode}";
        var body = $@"
            <h1>Thank you for your order!</h1>
            <p>Order #: {order.ClaimCode}</p>
            <p>Date: {order.OrderDate.ToString("f")}</p>
            <p>Total: ${order.TotalAmount}</p>
            <p>Discount: ${order.DiscountAmount}</p>
            
            <h2>Order Items:</h2>
            <ul>
                {string.Join("", order.OrderItems.Select(i => 
                    $"<li>{i.Book.Name} - {i.Quantity} x ${i.UnitPrice}</li>"))}
            </ul>
        ";
        
    
    }
}
