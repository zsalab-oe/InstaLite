using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InstaLite.Views;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InstaLite.ViewModels;

public partial class AuthViewModel : ObservableObject
{
    [ObservableProperty] private string username = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private string? error;
    [ObservableProperty] private bool isBusy;

    [RelayCommand]
    private async Task LoginAsync()
    {
        IsBusy = true;
        try
        {
            var (ok, err) = await App.Auth.SignInAsync(Username, Password).ConfigureAwait(false);
            if (!ok) { Error = err; return; }
            await Shell.Current.GoToAsync($"//feed");
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private Task GoToSignUpAsync() => Shell.Current.GoToAsync(nameof(Views.SignUpPage));

    [RelayCommand]
    private async Task SignUpAsync()
    {
        IsBusy = true;
        try
        {
            var (ok, err) = await App.Auth.SignUpAsync(Username, Password).ConfigureAwait(false);
            if (!ok) { Error = err; return; }
            await Shell.Current.GoToAsync(nameof(Views.LoginPage)); // .Navigation.PopToRootAsync(true);
        }
        finally { IsBusy = false; }
    }
}
