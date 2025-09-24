namespace InstaLite.Models;

public class Comment
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public required string PostId { get; set; }
    public required string UserId { get; set; }
    public required string Text { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // convenience
    public User? User { get; set; }
}
