using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InstaLite.Models;

namespace InstaLite.ViewModels;

public partial class SearchViewModel : ObservableObject
{
    private readonly InMemoryRepository _repo = App.Repository;

    [ObservableProperty] private string query = "";
    public ObservableCollection<User> Users { get; } = new();
    public ObservableCollection<Post> Posts { get; } = new();

    [RelayCommand]
    private async Task RunAsync()
    {
        Users.Clear(); Posts.Clear();
        var u = await _repo.SearchUsersAsync(Query).ConfigureAwait(false);
        var p = await _repo.SearchPostsAsync(Query).ConfigureAwait(false);
        foreach (var x in u) Users.Add(x);
        foreach (var x in p) Posts.Add(x);
    }

    [RelayCommand]
    private Task OpenPostAsync(Post? post) =>
        post == null ? Task.CompletedTask :
        Shell.Current.GoToAsync($"{nameof(Views.PostDetailPage)}?postId={post.Id}");
}
