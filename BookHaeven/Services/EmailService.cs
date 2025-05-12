using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BookHaeven.Models;
using Microsoft.Extensions.Configuration;

namespace BookHaeven.Services
{
    public interface IEmailService
    {
        Task SendOrderConfirmation(string email, Order order);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendOrderConfirmation(string email, Order order)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var fromAddress = smtpSettings["FromAddress"];
            var fromName = smtpSettings["FromName"];

            var subject = $"Your BookHaven Order #{order.ClaimCode}";
            var body = $@"
                <h1>Thank you for your order!</h1>
                <p>Order #: {order.ClaimCode}</p>
                <p>Date: {order.OrderDate.ToString("f")}</p>
                <p>Total: ${order.TotalAmount:0.00}</p>
                {(order.DiscountAmount > 0 ? $"<p>Discount: ${order.DiscountAmount:0.00}</p>" : "")}
                
                <h2>Order Items:</h2>
                <ul>
                    {string.Join("", order.OrderItems.Select(i => 
                        $"<li>{i.Book.Name} - {i.Quantity} x ${i.UnitPrice:0.00}</li>"))}
                </ul>

                <p>Please visit the store to claim your order.</p>
                <p>Thank you for shopping with BookHaven!</p>
            ";

            using (var message = new MailMessage())
            {
                message.From = new MailAddress(fromAddress, fromName);
                message.To.Add(new MailAddress(email));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using (var client = new SmtpClient(smtpSettings["Host"]))
                {
                    client.Port = int.Parse(smtpSettings["Port"]);
                    client.Credentials = new NetworkCredential(
                        smtpSettings["FromAddress"],
                        smtpSettings["Password"]);
                    client.EnableSsl = bool.Parse(smtpSettings["EnableSsl"]);

                    await client.SendMailAsync(message);
                }
            }
        }
    }
}