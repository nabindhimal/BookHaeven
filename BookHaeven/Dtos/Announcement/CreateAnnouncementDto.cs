using System;
using System.ComponentModel.DataAnnotations;

namespace BookHaeven.Dtos.Announcement;

public class CreateAnnouncementDto
{
    [Required]
    public string Message { get; set; } = string.Empty;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }
}

