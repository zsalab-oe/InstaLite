using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InstaLite.Models;

namespace InstaLite.ViewModels;

public partial class NotificationsViewModel : ObservableObject
{
    private readonly InMemoryRepository _repo = App.Repository;
    private readonly string? _me = App.Auth.GetCurrentUserId();

    public ObservableCollection<NotificationItem> Items { get; } = new();
    [ObservableProperty] private bool isBusy;

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (_me == null) return;
        IsBusy = true;
        try
        {
            var list = await _repo.GetNotificationsAsync(_me).ConfigureAwait(false);
            Items.Clear(); foreach (var n in list) Items.Add(n);
        }
        finally { IsBusy = false; }
    }
}
