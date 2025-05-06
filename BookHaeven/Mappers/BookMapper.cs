using System;
using BookHaeven.Dtos.Book;
using BookHaeven.Models;

namespace BookHaeven.Mappers;

public static class BookMapper
{
    public static Book ToBookFromCreateBookDto(this CreateBookDto createBookDto)
    {
        return new Book
        {
            Id = Guid.NewGuid(),
            Name = createBookDto.Name,
            ISBN = createBookDto.ISBN,
            Description = createBookDto.Description,
            Author = createBookDto.Author,
            Publisher = createBookDto.Publisher,
            Language = createBookDto.Language,
            Genre = createBookDto.Genre,
            PublicationDate = createBookDto.PublicationDate,
            Price = createBookDto.Price,
            Stock = createBookDto.Stock,
            IsAvailableInLibrary = createBookDto.IsAvailableInLibrary,
            IsOnSale = createBookDto.IsOnSale,
            DiscountPercentage = createBookDto.DiscountPercentage,
            SaleStartDate = createBookDto.SaleStartDate,
            SaleEndDate = createBookDto.SaleEndDate,
            CreatedAt = DateTime.UtcNow,
            AverageRating = null,
            ImageUrl = createBookDto.ImageUrl,
        };
    }


    public static ViewBookDto ToViewBookDto(this Book book)
    {
        
        return new ViewBookDto
        {
            Id = book.Id,
            Name = book.Name,
            ISBN = book.ISBN,
            Description = book.Description,
            Author = book.Author,
            Publisher = book.Publisher,
            Language = book.Language,
            Genre = book.Genre,
            PublicationDate = book.PublicationDate,
            Price = book.Price,
            DiscountPercentage = book.DiscountPercentage,
            DiscountedPrice = (book.IsOnSale && book.DiscountPercentage.HasValue)
                ? book.Price * (1 - book.DiscountPercentage.Value / 100)
                : null,
            Stock = book.Stock,
            IsAvailableInLibrary = book.IsAvailableInLibrary,
            IsOnSale = book.IsOnSale,
            SaleStartDate = book.SaleStartDate,
            SaleEndDate = book.SaleEndDate,
            CreatedAt = book.CreatedAt,
            AverageRating = book.AverageRating,
            ImageUrl = book.ImageUrl,
            IsBookmarked = book.IsBookmarked
        };
    }


}
