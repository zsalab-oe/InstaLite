using Microsoft.Maui.Storage;

namespace InstaLite;

public class AuthService
{
    private readonly InMemoryRepository _repo;
    private const string SessionKey = "session_userid";

    public AuthService(InMemoryRepository repo) => _repo = repo;

    public string? GetCurrentUserId() => Preferences.Get(SessionKey, null);

    public void SignOut() => Preferences.Remove(SessionKey);

    public async Task<(bool ok, string? error)> SignInAsync(string username, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return (false, "Username and password required.");
        var user = await _repo.GetUserByUsernameAsync(username, ct).ConfigureAwait(false);
        if (user == null) return (false, "User not found.");
        Preferences.Set(SessionKey, user.Id);
        return (true, null);
    }

    public async Task<(bool ok, string? error)> SignUpAsync(string username, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return (false, "Username and password required.");
        var existing = await _repo.GetUserByUsernameAsync(username, ct).ConfigureAwait(false);
        if (existing != null) return (false, "Username already exists.");
        var user = await _repo.AddUserAsync(username).ConfigureAwait(false);
        Preferences.Set(SessionKey, user.Id);
        return (true, null);
    }
}
