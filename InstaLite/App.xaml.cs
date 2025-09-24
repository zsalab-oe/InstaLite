using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;

namespace InstaLite;

public partial class App : Application
{
    public static InMemoryRepository Repository { get; } = new();
    public static ImageStorageService ImageStorage { get; } = new();
    public static AuthService Auth { get; } = new(Repository);

    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();

        // Navigate to Login if no session
        var uid = Auth.GetCurrentUserId();
        if (string.IsNullOrEmpty(uid))
        {
            Dispatcher.Dispatch(async () =>
            {
                await Shell.Current.GoToAsync(nameof(Views.LoginPage));
            });
        }
    }
}
