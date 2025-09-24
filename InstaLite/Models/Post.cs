using System;
//using Windows.System;

namespace InstaLite.Models;

public class Post
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public required string UserId { get; set; }
    public required string ImageUrl { get; set; }
    public string Caption { get; set; } = "";
    public int Likes { get; set; }
    public bool LikedByMe { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Convenience (not serialized)
    public User? User { get; set; }

    public string TimeAgo =>
        CreatedAt == default ? "" :
        (DateTime.UtcNow - CreatedAt) switch
        {
            var ts when ts.TotalMinutes < 1 => "now",
            var ts when ts.TotalMinutes < 60 => $"{(int)ts.TotalMinutes}m",
            var ts when ts.TotalHours < 24 => $"{(int)ts.TotalHours}h",
            var ts when ts.TotalDays < 7 => $"{(int)ts.TotalDays}d",
            _ => CreatedAt.ToLocalTime().ToString("MMM d")
        };
}
