namespace InstaLite.Models
{
    public class Activity
    {
        public string UserId { get; set; } = Guid.NewGuid().ToString("N");

        public ActivityType Type { get; set; }

        // Optional short title like "Morning Run"
        public string? Title { get; set; }

        // Local time the activity started
        public DateTime StartLocal { get; set; } = DateTime.Now;

        // Store TimeSpan as ticks for SQLite compatibility
        public long DurationTicks { get; set; } = TimeSpan.FromMinutes(30).Ticks;

        public TimeSpan Duration
        {
            get => TimeSpan.FromTicks(DurationTicks);
            set => DurationTicks = value.Ticks;
        }


        // Metrics (nullable when not applicable)
        public double? DistanceKm { get; set; } // e.g., running/cycling/swimming
        public int? Calories { get; set; }
        public int? EffortRpe { get; set; } // 1–10 perceived exertion

        public string? Notes { get; set; }

        public bool IsPublic { get; set; } = false; // if you later want to share to feed

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime EndLocal => StartLocal + Duration;

        public string ToShareText()
        {
            var title = string.IsNullOrWhiteSpace(Title) ? Type.ToString() : Title!;
            var dist = DistanceKm.HasValue ? $", {DistanceKm:0.##} km" : string.Empty;
            var cals = Calories.HasValue ? $", {Calories} kcal" : string.Empty;
            return $"{title}: {Duration:hh\\:mm} on {StartLocal:d} at {StartLocal:t}{dist}{cals}. #InstaLite";
        }
    }
}
