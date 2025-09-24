using CommunityToolkit.Mvvm.ComponentModel;
using InstaLite.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InstaLite.ViewModels
{
    public class ActivityPageViewModel : ObservableObject, INotifyPropertyChanged
    {
        public const string PrefUserIdKey = "current_user_id";

        private readonly InMemoryRepository _repo;
        private bool _isBusy;

        public ObservableCollection<Activity> Activities { get; } = new();
        public List<ActivityType> ActivityTypes { get; } = Enum.GetValues<ActivityType>().ToList();

        // Form fields
        private ActivityType _selectedType = ActivityType.Running;
        public ActivityType SelectedType { get => _selectedType; set { _selectedType = value; OnPropertyChanged(); } }

        private string? _title;
        public string? Title { get => _title; set { _title = value; OnPropertyChanged(); } }

        // Start date/time split for simple binding
        private DateTime _startDate = DateTime.Now.Date;
        public DateTime StartDate { get => _startDate; set { _startDate = value; OnPropertyChanged(); } }

        private TimeSpan _startTime = DateTime.Now.TimeOfDay;
        public TimeSpan StartTime { get => _startTime; set { _startTime = value; OnPropertyChanged(); } }

        // Use string fields for numeric entries to avoid binding converter hassles
        private string _durationMinutes = "30";
        public string DurationMinutes { get => _durationMinutes; set { _durationMinutes = value; OnPropertyChanged(); } }

        private string _distanceKm = string.Empty;
        public string DistanceKm { get => _distanceKm; set { _distanceKm = value; OnPropertyChanged(); } }

        private string _calories = string.Empty;
        public string Calories { get => _calories; set { _calories = value; OnPropertyChanged(); } }

        private int _effortRpe = 5;
        public int EffortRpe { get => _effortRpe; set { _effortRpe = value; OnPropertyChanged(); } }

        private string? _notes;
        public string? Notes { get => _notes; set { _notes = value; OnPropertyChanged(); } }

        private bool _isPublic = false;
        public bool IsPublic { get => _isPublic; set { _isPublic = value; OnPropertyChanged(); } }

        private string _currentUserId = string.Empty;
        public string CurrentUserId { get => _currentUserId; private set { _currentUserId = value; OnPropertyChanged(); } }

        public bool IsBusy { get => _isBusy; set { _isBusy = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsNotBusy)); } }
        public bool IsNotBusy => !IsBusy;

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ActivityPageViewModel()
        {
            _repo = App.Repository;
            LoadCommand = new Command(async () => await LoadAsync());
            AddCommand = new Command(async () => await AddAsync(), () => IsNotBusy);
        }

        private async Task EnsureUserAsync()
        {
            var uid = Preferences.Get(PrefUserIdKey, string.Empty);
            if (string.IsNullOrWhiteSpace(uid))
            {
                // If your app already sets this elsewhere, remove this block.
                var newUser = await _repo.AddUserAsync($"user_{Guid.NewGuid().ToString("N")[..6]}");
                uid = newUser.Id;
                Preferences.Set(PrefUserIdKey, uid);
            }
            CurrentUserId = uid;
        }

        public async Task LoadAsync(CancellationToken ct = default)
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                //await EnsureUserAsync();
                var list = await _repo.GetUserActivitiesAsync(CurrentUserId, ct);
                Activities.Clear();
                foreach (var a in list)
                    Activities.Add(a);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task AddAsync(CancellationToken ct = default)
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                //await EnsureUserAsync();

                var startLocal = StartDate.Date + StartTime;

                // Parse numeric inputs
                double minutes = 30;
                _ = double.TryParse(DurationMinutes, out minutes);
                TimeSpan duration = TimeSpan.FromMinutes(Math.Max(0, minutes));

                double? distance = null; if (double.TryParse(DistanceKm, out var d)) distance = d;
                int? calories = null; if (int.TryParse(Calories, out var c)) calories = c;

                var created = await _repo.AddActivityAsync(
                    userId: CurrentUserId,
                    type: SelectedType,
                    startLocal: startLocal,
                    duration: duration,
                    title: Title,
                    distanceKm: distance,
                    calories: calories,
                    effortRpe: EffortRpe,
                    notes: Notes,
                    isPublic: IsPublic,
                    ct: ct);

                // Add to top of the list
                Activities.Insert(0, created);

                // Reset a few fields for convenience
                Title = string.Empty;
                DurationMinutes = "30";
                DistanceKm = string.Empty;
                Calories = string.Empty;
                EffortRpe = 5;
                Notes = string.Empty;
                IsPublic = false;
            }
            finally
            {
                IsBusy = false;
                (AddCommand as Command)?.ChangeCanExecute();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
