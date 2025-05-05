using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookHaeven.Models;

public class Announcement
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Announcement message is required.")]
    public required string Message { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    [NotMapped]
    public bool IsActive => DateTime.UtcNow >= StartTime && DateTime.UtcNow <= EndTime;


}
