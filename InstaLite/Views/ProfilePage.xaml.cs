using InstaLite.ViewModels;

namespace InstaLite.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
        Loaded += async (s, e) =>
        {
            if (BindingContext is ProfileViewModel vm) await vm.LoadCommand.ExecuteAsync(null);
        };
    }
}
