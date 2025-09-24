namespace InstaLite.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private void Switch_Toggled(object sender, ToggledEventArgs e)
    {
        Application.Current!.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
    }

    private async void SignOut_Clicked(object sender, EventArgs e)
    {
        App.Auth.SignOut();
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }
}
