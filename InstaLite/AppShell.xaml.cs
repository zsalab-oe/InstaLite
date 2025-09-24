using InstaLite.Views;
using System.Threading.Tasks;

namespace InstaLite;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register non-tab routes
        Routing.RegisterRoute(nameof(Views.LoginPage), typeof(Views.LoginPage));
        Routing.RegisterRoute(nameof(Views.SignUpPage), typeof(Views.SignUpPage));
        Routing.RegisterRoute(nameof(Views.PostDetailPage), typeof(Views.PostDetailPage));
        Routing.RegisterRoute(nameof(Views.SettingsPage), typeof(Views.SettingsPage));
        Routing.RegisterRoute(nameof(Views.RewardPage), typeof(Views.RewardPage));


        Routing.RegisterRoute(nameof(Views.SearchPage), typeof(Views.SearchPage));
        Routing.RegisterRoute(nameof(Views.NotificationsPage), typeof(Views.NotificationsPage));
    }

    private async void Notification_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NotificationsPage));
    }

    private async void Search_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SearchPage));
    }
}
