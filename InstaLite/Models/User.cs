namespace InstaLite.Models;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public required string Username { get; set; }
    public string AvatarUrl { get; set; } = "https://i.pravatar.cc/150?img=3";
    public string Bio { get; set; } = "Hello from InstaLite!";
    public int Followers { get; set; }
    public int Following { get; set; }
}
