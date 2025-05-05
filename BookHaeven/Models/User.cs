using System;
using System.ComponentModel.DataAnnotations;

namespace BookHaeven.Models;

public class User
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [MaxLength(100)]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public required string PasswordHash { get; set; }

    // Available roles are "Admin" and "User"
    [MaxLength(20)]
    public string Role { get; set; } = Roles.User;

    public int SuccessfulOrdersCount { get; set; }

    public ICollection<Bookmark>? Bookmarks { get; set; }

    public ICollection<Review>? Reviews { get; set; }
}

public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
}
