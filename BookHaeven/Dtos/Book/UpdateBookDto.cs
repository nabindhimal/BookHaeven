using System;
using System.ComponentModel.DataAnnotations;

namespace BookHaeven.Dtos.Book;

public class UpdateBookDto
{

    [StringLength(100)]
    public string Name { get; set; }


    [StringLength(20)]
    public string ISBN { get; set; }

    public string Description { get; set; } = "";

    [StringLength(50)]
    public string Author { get; set; }

    [StringLength(50)]
    public string Publisher { get; set; }

    [StringLength(50)]
    public string Language { get; set; }

    
    [StringLength(50)]
    public string Genre { get; set; }

    public DateTime PublicationDate { get; set; }

    [Range(0, 999999)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public bool IsAvailableInLibrary { get; set; }

    public bool IsOnSale { get; set; }

    [Range(0, 100)]
    public decimal? DiscountPercentage { get; set; }

    public DateTime? SaleStartDate { get; set; }
    public DateTime? SaleEndDate { get; set; }

    public string? ImageUrl { get; set; }
}





