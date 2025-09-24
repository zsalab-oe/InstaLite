using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace InstaLite.ViewModels;

public partial class CreatePostViewModel : ObservableObject
{
    private readonly InMemoryRepository _repo = App.Repository;
    private readonly ImageStorageService _storage = App.ImageStorage;
    private readonly string? _me = App.Auth.GetCurrentUserId();

    [ObservableProperty] private string? imageUrl;
    [ObservableProperty] private string caption = "";
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string? error;

    [RelayCommand]
    private async Task PickImageAsync()
    {
        ImageUrl = await _storage.PickImageAsync().ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (_me == null) { Error = "Please login."; return; }
        if (string.IsNullOrEmpty(ImageUrl)) { Error = "Choose an image."; return; }
        IsBusy = true;
        try
        {
            await _repo.AddPostAsync(_me, ImageUrl!, Caption).ConfigureAwait(false);
            Caption = ""; ImageUrl = null;
            await Shell.Current.DisplayAlert("Posted", "Your post has been created.", "OK");
            // Navigate to Home
            await Shell.Current.GoToAsync($"//feed");
        }
        catch (Exception ex) { Error = ex.Message; }
        finally { IsBusy = false; }
    }
}
