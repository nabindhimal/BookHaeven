using System;
using System.ComponentModel.DataAnnotations;
using BookHaeven.Dtos.Book;

namespace BookHaeven.Dtos.Bookmark;


public class BookmarkDto
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public DateTime CreatedAt { get; set; }
    public ViewBookDto? Book { get; set; }
}

public class CreateBookmarkDto
{
    [Required]
    public Guid BookId { get; set; }
}