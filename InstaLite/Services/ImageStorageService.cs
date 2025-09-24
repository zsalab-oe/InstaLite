using Microsoft.Maui.Storage;

namespace InstaLite;

public class ImageStorageService
{
    // Stub: in production, integrate MediaPicker/FilePicker and save to local path.
    public Task<string> PickImageAsync(CancellationToken ct = default)
    {
        // Return a random picsum image to simulate picking
        var r = new Random().Next(1000, 9999);
        return Task.FromResult($"https://picsum.photos/seed/{r}/600/600");
    }

    public async Task<string> SaveAsync(Stream stream, string fileName, CancellationToken ct = default)
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, fileName);
        using var fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
        await stream.CopyToAsync(fs, ct).ConfigureAwait(false);
        return path;
    }
}
