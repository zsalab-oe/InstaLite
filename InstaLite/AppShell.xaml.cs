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
    }
}
