using InstaLite.ViewModels;

namespace InstaLite.Views;

public partial class FeedPage : ContentPage
{
    public FeedPage()
    {
        InitializeComponent();
        Loaded += async (s, e) =>
        {
            if (BindingContext is FeedViewModel vm) await vm.InitializeCommand.ExecuteAsync(null);
        };
    }

    private async void CollectionView_RemainingItemsThresholdReached(object sender, EventArgs e)
    {
        if (BindingContext is FeedViewModel vm) await vm.LoadMoreCommand.ExecuteAsync(null);
    }
}
