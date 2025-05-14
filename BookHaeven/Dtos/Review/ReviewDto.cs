using System;

namespace BookHaeven.Dtos.Review;


public class ReviewDto
{
    public Guid BookId { get; set; }
    public Guid OrderId { get; set; }  // To verify the purchase
    public int Rating { get; set; }
    public string Comment { get; set; }
}

public class ReviewResponseDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
