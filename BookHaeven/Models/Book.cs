using System;
using System.ComponentModel.DataAnnotations;

namespace BookHaeven.Models;

public class Book
{
    [Key]
    public Guid Id { get; set; }


    // Basic book info fields

    [Required(ErrorMessage = "Book name is required.")]
    [StringLength(100, ErrorMessage="Book name cannot exceed 100 characters.")]
    public required string Name { get; set; }


    [StringLength(20)]
    [Required(ErrorMessage = "ISBN is required.")]
    public required string ISBN { get; set; }

    public string Description { get; set; } = "";

    [Required(ErrorMessage = "Book author is required.")]
    [StringLength(50, ErrorMessage="Author name cannot exceed 50 characters.")]
    public required string Author { get; set; }

    [Required(ErrorMessage ="Book publisher name is required.")]
    [StringLength(50, ErrorMessage="Publisher name cannot exceed 50 characters.")]
    public required string Publisher { get; set; }

    [Required(ErrorMessage ="Book language is required.")]
    [StringLength(50, ErrorMessage="Language cannot exceed 50 characters.")]
    public required string Language { get; set; }


    [Required(ErrorMessage ="Book genre is required.")]
    [StringLength(50, ErrorMessage="Genre exceed 50 characters.")]
    public required string Genre { get; set; }

    public DateTime PublicationDate { get; set; } 

    [Range(0, 999999)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public bool IsAvailableInLibrary { get; set; }

    // Sale Info
    public bool IsOnSale { get; set; }

    [Range(0, 100)]
    public decimal? DiscountPercentage { get; set; }

    public DateTime? SaleStartDate { get; set; }
    public DateTime? SaleEndDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public double? AverageRating { get; set; }


    public string? ImageUrl { get; set; }





    
    
    public ICollection<Review>? Reviews { get; set; }
    public ICollection<Bookmark>? Bookmarks { get; set; }
    

    

}
