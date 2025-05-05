using System;

namespace BookHaeven.Models;

public class Bookmark
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid BookId { get; set; }
    public Book? Book { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
