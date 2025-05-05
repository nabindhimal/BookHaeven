using System;

namespace BookHaeven.Dtos.Book;

public class BookQueryDto
{
    public string? Title { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Genre { get; set; }
    public string? Publisher { get; set; }
    public string? Author { get; set; }
    public string? SortBy { get; set; }

}
