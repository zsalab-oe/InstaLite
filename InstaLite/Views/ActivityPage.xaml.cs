using InstaLite.ViewModels;

namespace InstaLite.Views;

public partial class ActivityPage : ContentPage
{
    private readonly ActivityPageViewModel _vm;

    public ActivityPage()
    {
        InitializeComponent();
        this._vm = BindingContext as ActivityPageViewModel ?? new ActivityPageViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();

    }
}