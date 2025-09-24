using InstaLite.Models;

namespace InstaLite;

public class InMemoryRepository
{
    private readonly List<User> _users = new();
    private readonly List<Post> _posts = new();
    private readonly List<Comment> _comments = new();
    private readonly List<(string followerId, string targetId)> _follows = new();
    private readonly List<NotificationItem> _notifications = new();
    private readonly object _gate = new();

    public InMemoryRepository()
    {
        // Seed users
        var u1 = new User { Username = "alice", AvatarUrl = "https://i.pravatar.cc/150?img=1", Bio = "Coffee & code" };
        var u2 = new User { Username = "bob", AvatarUrl = "https://i.pravatar.cc/150?img=2", Bio = "Travel lover" };
        var u3 = new User { Username = "carol", AvatarUrl = "https://i.pravatar.cc/150?img=5", Bio = "Photographer" };
        _users.AddRange(new[] { u1, u2, u3 });

        // Seed posts
        var rnd = new Random();
        foreach (var p in Enumerable.Range(1, 12))
        {
            var owner = _users[rnd.Next(_users.Count)];
            _posts.Add(new Post
            {
                UserId = owner.Id,
                User = owner,
                ImageUrl = $"https://picsum.photos/seed/{p}/600/600",
                Caption = $"Sample post #{p}",
                Likes = rnd.Next(0, 200),
                CreatedAt = DateTime.UtcNow.AddHours(-rnd.Next(1, 120))
            });
        }
    }

    public Task<User?> GetUserByUsernameAsync(string username, CancellationToken ct = default)
        => Task.FromResult(_users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)));

    public Task<User?> GetUserByIdAsync(string id, CancellationToken ct = default)
        => Task.FromResult(_users.FirstOrDefault(u => u.Id == id));

    public Task<List<User>> SearchUsersAsync(string query, CancellationToken ct = default)
    {
        var q = (query ?? "").Trim();
        var res = string.IsNullOrEmpty(q) ? new List<User>() :
            _users.Where(u => u.Username.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
        return Task.FromResult(res);
    }

    public Task<(List<Post> items, bool hasMore)> GetFeedAsync(int skip, int take, CancellationToken ct)
    {
        var ordered = _posts.OrderByDescending(p => p.CreatedAt).ToList();
        var slice = ordered.Skip(skip).Take(take).ToList();
        // attach user
        foreach (var p in slice)
            p.User ??= _users.First(u => u.Id == p.UserId);
        var hasMore = skip + take < ordered.Count;
        return Task.FromResult((slice, hasMore));
    }

    public Task<Post?> GetPostAsync(string postId, CancellationToken ct = default)
    {
        var p = _posts.FirstOrDefault(p => p.Id == postId);
        if (p != null) p.User ??= _users.First(u => u.Id == p.UserId);
        return Task.FromResult(p);
    }

    public Task<List<Post>> GetUserPostsAsync(string userId, CancellationToken ct = default)
    {
        var res = _posts.Where(p => p.UserId == userId).OrderByDescending(p => p.CreatedAt).ToList();
        foreach (var p in res) p.User ??= _users.First(u => u.Id == p.UserId);
        return Task.FromResult(res);
    }

    public Task<List<Post>> SearchPostsAsync(string query, CancellationToken ct = default)
    {
        var q = (query ?? "").Trim();
        var res = string.IsNullOrEmpty(q) ? new List<Post>() :
            _posts.Where(p => p.Caption.Contains(q, StringComparison.OrdinalIgnoreCase)).OrderByDescending(p => p.CreatedAt).ToList();
        foreach (var p in res) p.User ??= _users.First(u => u.Id == p.UserId);
        return Task.FromResult(res);
    }

    public Task LikePostAsync(string postId, string userId, bool like, CancellationToken ct = default)
    {
        lock (_gate)
        {
            var p = _posts.First(p => p.Id == postId);
            if (like && !p.LikedByMe) { p.LikedByMe = true; p.Likes++; }
            else if (!like && p.LikedByMe) { p.LikedByMe = false; p.Likes = Math.Max(0, p.Likes - 1); }
            if (like)
            {
                _notifications.Add(new NotificationItem
                {
                    UserId = p.UserId,
                    FromUserId = userId,
                    Type = NotificationType.Like,
                    Text = $"{_users.First(u => u.Id == userId).Username} liked your post"
                });
            }
        }
        return Task.CompletedTask;
    }

    public Task<List<Comment>> GetCommentsAsync(string postId, CancellationToken ct = default)
    {
        var res = _comments.Where(c => c.PostId == postId).OrderBy(c => c.CreatedAt).ToList();
        foreach (var c in res) c.User ??= _users.First(u => u.Id == c.UserId);
        return Task.FromResult(res);
    }

    public Task AddCommentAsync(string postId, string userId, string text, CancellationToken ct = default)
    {
        var c = new Comment { PostId = postId, UserId = userId, Text = text, CreatedAt = DateTime.UtcNow };
        _comments.Add(c);
        _notifications.Add(new NotificationItem
        {
            UserId = _posts.First(p => p.Id == postId).UserId,
            FromUserId = userId,
            Type = NotificationType.Comment,
            Text = $"{_users.First(u => u.Id == userId).Username} commented: {text}"
        });
        return Task.CompletedTask;
    }

    public Task<Post> AddPostAsync(string userId, string imageUrl, string caption, CancellationToken ct = default)
    {
        var p = new Post { UserId = userId, ImageUrl = imageUrl, Caption = caption, CreatedAt = DateTime.UtcNow, User = _users.First(u => u.Id == userId) };
        _posts.Insert(0, p);
        return Task.FromResult(p);
    }

    public Task FollowAsync(string followerId, string targetUserId, bool follow, CancellationToken ct = default)
    {
        var key = (followerId, targetUserId);
        lock (_gate)
        {
            var exists = _follows.Contains(key);
            if (follow && !exists)
            {
                _follows.Add(key);
                var t = _users.First(u => u.Id == targetUserId);
                t.Followers++;
                var f = _users.First(u => u.Id == followerId);
                f.Following++;
                _notifications.Add(new NotificationItem
                {
                    UserId = targetUserId,
                    FromUserId = followerId,
                    Type = NotificationType.Follow,
                    Text = $"{f.Username} started following you"
                });
            }
            else if (!follow && exists)
            {
                _follows.Remove(key);
                var t = _users.First(u => u.Id == targetUserId);
                t.Followers = Math.Max(0, t.Followers - 1);
                var f = _users.First(u => u.Id == followerId);
                f.Following = Math.Max(0, f.Following - 1);
            }
        }
        return Task.CompletedTask;
    }

    public Task<bool> IsFollowingAsync(string followerId, string targetUserId, CancellationToken ct = default)
        => Task.FromResult(_follows.Contains((followerId, targetUserId)));

    public Task<List<NotificationItem>> GetNotificationsAsync(string userId, CancellationToken ct = default)
    {
        var res = _notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt).ToList();
        foreach (var n in res) n.FromUser ??= _users.First(u => u.Id == n.FromUserId);
        return Task.FromResult(res);
    }

    // For SignUp
    public Task<User> AddUserAsync(string username, string avatarUrl = "")
    {
        var user = new User { Username = username, AvatarUrl = string.IsNullOrEmpty(avatarUrl) ? $"https://i.pravatar.cc/150?u={username}" : avatarUrl };
        _users.Add(user);
        return Task.FromResult(user);
    }
}
