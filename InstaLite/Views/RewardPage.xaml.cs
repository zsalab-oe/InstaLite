using InstaLite.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace InstaLite.Views;

public partial class RewardPage : ContentPage
{
    public ObservableCollection<Reward> Rewards { get; } = new();

    public ICommand CollectCommand { get; }
    public ICommand CopyCodeCommand { get; }

    public RewardPage()
    {
        InitializeComponent();

        CollectCommand = new Command<Reward>(async r =>
        {
            if (r is null) return;
            await Clipboard.SetTextAsync(r.Code);
            r.IsCollected = true;
            Preferences.Set(GetPrefKey(r), true);
            await DisplayAlert("Copied", $"Code {r.Code} copied.", "OK");
        });

        CopyCodeCommand = new Command<Reward>(async r =>
        {
            if (r is null) return;
            await Clipboard.SetTextAsync(r.Code);
            await DisplayAlert("Copied", $"Code {r.Code} copied.", "OK");
        });

        LoadData();
        BindingContext = this;
    }

    void LoadData()
    {
        var seed = new[]
        {
            new Reward { Store="FITME SPORTS", Code="SPORT10", Accent="10%", TailText=" OFF." },
            new Reward { Store="FITME SPORTS", Code="RUN20",   Accent="$5",  TailText=" OFF ORDERS." },
            new Reward { Store="FITME SPORTS", Code="FITME15", Accent="15%", TailText=" OFF." }
        };

        Rewards.Clear();
        foreach (var r in seed)
        {
            r.IsCollected = Preferences.Get(GetPrefKey(r), false);
            Rewards.Add(r);
        }
    }

    static string GetPrefKey(Reward r) => $"reward_collected_{r.Code}";
}
