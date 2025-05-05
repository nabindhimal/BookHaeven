using System;

namespace BookHaeven.Dtos.Announcement;

public class AnnouncementDto
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

