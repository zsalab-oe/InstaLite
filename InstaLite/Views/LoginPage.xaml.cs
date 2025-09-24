using InstaLite.ViewModels;

namespace InstaLite.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new AuthViewModel();
    }
}
