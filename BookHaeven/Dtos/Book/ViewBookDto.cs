using System;

namespace BookHaeven.Dtos.Book;

public class ViewBookDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string ISBN { get; set; }
    public string Description { get; set; }

    public string Author { get; set; }
    public string Publisher { get; set; }
    public string Language { get; set; }
    public string Genre { get; set; }

    public DateTime PublicationDate { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }

    public int Stock { get; set; }
    public bool IsAvailableInLibrary { get; set; }

    public bool IsOnSale { get; set; }
    public decimal? DiscountPercentage { get; set; }

    public DateTime? SaleStartDate { get; set; }
    public DateTime? SaleEndDate { get; set; }

    public DateTime CreatedAt { get; set; }
    public double? AverageRating { get; set; }

    public string ImageUrl { get; set; }
}
