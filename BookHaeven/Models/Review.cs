using System;
using System.ComponentModel.DataAnnotations;

namespace BookHaeven.Models;

public class Review
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid BookId { get; set; }
    public Book? Book { get; set; }

    public Guid OrderId { get; set; }  // field to track which order the review is for
    public Order? Order { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }

    [MaxLength(1000)]
    public required string Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
