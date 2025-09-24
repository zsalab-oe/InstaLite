using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InstaLite.Models;

namespace InstaLite.ViewModels;

[QueryProperty(nameof(PostId), "postId")]
public partial class PostDetailViewModel : ObservableObject
{
    private readonly InMemoryRepository _repo = App.Repository;
    private readonly string? _me = App.Auth.GetCurrentUserId();

    [ObservableProperty] private string? postId;
    [ObservableProperty] private Post? post;
    public ObservableCollection<Comment> Comments { get; } = new();
    [ObservableProperty] private string newComment = "";
    [ObservableProperty] private bool isBusy;

    partial void OnPostIdChanged(string? value) => Task.Run(LoadAsync);

    private async Task LoadAsync()
    {
        if (string.IsNullOrEmpty(PostId)) return;
        IsBusy = true;
        Comments.Clear();
        try
        {
            Post = await _repo.GetPostAsync(PostId!).ConfigureAwait(false);
            var items = await _repo.GetCommentsAsync(PostId!).ConfigureAwait(false);
            foreach (var c in items) Comments.Add(c);
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task AddCommentAsync()
    {
        if (string.IsNullOrWhiteSpace(NewComment) || _me == null || string.IsNullOrEmpty(PostId)) return;
        var txt = NewComment;
        NewComment = "";
        await _repo.AddCommentAsync(PostId!, _me, txt).ConfigureAwait(false);
        var list = await _repo.GetCommentsAsync(PostId!).ConfigureAwait(false);
        Comments.Clear();
        foreach (var c in list)
            Comments.Add(c);
    }
}
