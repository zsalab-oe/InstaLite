using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InstaLite.Models;
using Microsoft.Maui.ApplicationModel;

namespace InstaLite.ViewModels;

public partial class FeedViewModel : ObservableObject
{
    private readonly InMemoryRepository _repo;
    private readonly string? _me;
    private CancellationTokenSource? _cts;
    private bool _hasMore = true;

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool isRefreshing;
    [ObservableProperty] private string? error;
    public ObservableCollection<Post> Posts { get; } = new();

    public FeedViewModel()
    {
        _repo = App.Repository;
        _me = App.Auth.GetCurrentUserId();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Posts.Count == 0)
            await RefreshAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        Error = null;
        IsRefreshing = true;
        _hasMore = true;
        Posts.Clear();
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        try
        {
            await LoadMoreAsync();
        }
        catch (Exception ex) { Error = ex.Message; }
        finally { IsRefreshing = false; }
    }

    [RelayCommand]
    private async Task LoadMoreAsync()
    {
        if (IsBusy || !_hasMore) return;
        IsBusy = true;
        try
        {
            var (items, hasMore) = await _repo.GetFeedAsync(Posts.Count, 6, _cts?.Token ?? CancellationToken.None);
            foreach (var p in items) Posts.Add(p);
            _hasMore = hasMore;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) { Error = ex.Message; }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task ToggleLikeAsync(Post? post)
    {
        if (post == null || _me == null) return;
        try
        {
            var like = !post.LikedByMe;
            await _repo.LikePostAsync(post.Id, _me, like);
            // simple haptic (best-effort)
            try { HapticFeedback.Perform(HapticFeedbackType.Click); } catch { }
            OnPropertyChanged(nameof(Posts));
        }
        catch (Exception ex) { Error = ex.Message; }
    }

    [RelayCommand]
    private async Task OpenPostAsync(Post? post)
    {
        if (post == null) return;
        await Shell.Current.GoToAsync($"{nameof(Views.PostDetailPage)}?postId={post.Id}");
    }
}
