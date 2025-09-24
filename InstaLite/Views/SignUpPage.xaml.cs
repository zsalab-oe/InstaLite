using InstaLite.ViewModels;

namespace InstaLite.Views;

public partial class SignUpPage : ContentPage
{
    public SignUpPage()
    {
        InitializeComponent();
        BindingContext = new AuthViewModel();
    }
}
