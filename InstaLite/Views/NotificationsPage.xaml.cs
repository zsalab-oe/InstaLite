using InstaLite.ViewModels;

namespace InstaLite.Views;

public partial class NotificationsPage : ContentPage
{
    public NotificationsPage()
    {
        InitializeComponent();
        Loaded += async (s, e) =>
        {
            if (BindingContext is NotificationsViewModel vm) await vm.LoadCommand.ExecuteAsync(null);
        };
    }
}
