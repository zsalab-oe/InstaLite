using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InstaLite.Models;

namespace InstaLite.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly InMemoryRepository _repo = App.Repository;
    private readonly string? _me = App.Auth.GetCurrentUserId();

    [ObservableProperty] private User? user;
    public ObservableCollection<Post> Posts { get; } = new();
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool isFollowing;

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (_me == null) return;
        IsBusy = true;
        try
        {
            User = await _repo.GetUserByIdAsync(_me).ConfigureAwait(false);
            var posts = await _repo.GetUserPostsAsync(_me).ConfigureAwait(false);
            Posts.Clear(); foreach (var p in posts) Posts.Add(p);
            IsFollowing = false; // On own profile
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task EditProfileAsync()
    {
        if (User == null) return;
        var bio = await Shell.Current.DisplayPromptAsync("Edit Bio", "Update your bio:", initialValue: User.Bio);
        if (bio != null) { User.Bio = bio; OnPropertyChanged(nameof(User)); }
    }

    [RelayCommand]
    private Task OpenSettingsAsync() => Shell.Current.GoToAsync(nameof(Views.SettingsPage));

    [RelayCommand]
    private Task SignOutAsync()
    {
        App.Auth.SignOut();
        return Shell.Current.GoToAsync(nameof(Views.LoginPage));
    }
}
