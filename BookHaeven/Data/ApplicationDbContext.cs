using System;
using BookHaeven.Models;
using Microsoft.EntityFrameworkCore;

namespace BookHaeven.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {

    }


    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique constraint for ISBN
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.ISBN)
            .IsUnique();

        // User-Review relationship
        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Book-Review relationship
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Book)
            .WithMany(b => b.Reviews)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        // User-Bookmark relationship
        modelBuilder.Entity<Bookmark>()
            .HasOne(bm => bm.User)
            .WithMany(u => u.Bookmarks)
            .HasForeignKey(bm => bm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Book-Bookmark relationship
        modelBuilder.Entity<Bookmark>()
            .HasOne(bm => bm.Book)
            .WithMany(b => b.Bookmarks)
            .HasForeignKey(bm => bm.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        // Prevent duplicate bookmarks (one per user per book)
        modelBuilder.Entity<Bookmark>()
            .HasIndex(bm => new { bm.UserId, bm.BookId })
            .IsUnique();
    }
}
