using System.Windows.Input;
using InstaLite.Models;

namespace InstaLite.Views.Controls;

public partial class PostCard : ContentView
{
    public static readonly BindableProperty LikeCommandProperty =
        BindableProperty.Create(nameof(LikeCommand), typeof(ICommand), typeof(PostCard));

    public static readonly BindableProperty OpenCommandProperty =
        BindableProperty.Create(nameof(OpenCommand), typeof(ICommand), typeof(PostCard));

    public ICommand? LikeCommand
    {
        get => (ICommand?)GetValue(LikeCommandProperty);
        set => SetValue(LikeCommandProperty, value);
    }

    public ICommand? OpenCommand
    {
        get => (ICommand?)GetValue(OpenCommandProperty);
        set => SetValue(OpenCommandProperty, value);
    }

    public PostCard() => InitializeComponent();

    private void LikeClicked(object sender, EventArgs e) => ExecuteLike();

    private void OnDoubleTapped(object sender, TappedEventArgs e) => ExecuteLike();

    private void OnSingleTapped(object sender, TappedEventArgs e)
    {
        if (BindingContext is Post p && OpenCommand?.CanExecute(p) == true)
            OpenCommand.Execute(p);
    }

    private void CommentClicked(object sender, EventArgs e)
    {
        if (BindingContext is Post p && OpenCommand?.CanExecute(p) == true)
            OpenCommand.Execute(p);
    }

    private void ExecuteLike()
    {
        if (BindingContext is Post p && LikeCommand?.CanExecute(p) == true)
            LikeCommand.Execute(p);
    }
}
