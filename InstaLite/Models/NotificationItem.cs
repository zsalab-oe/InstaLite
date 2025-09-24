namespace InstaLite.Models;

public enum NotificationType { Like, Comment, Follow }

public class NotificationItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public required string UserId { get; set; } // receiver
    public required string FromUserId { get; set; }
    public required string Text { get; set; }
    public NotificationType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? FromUser { get; set; }
}
