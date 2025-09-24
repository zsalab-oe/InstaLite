using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace InstaLite.Models
{
    public class Reward : INotifyPropertyChanged
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Store { get; set; } = "FITME SPORTS";
        public string Code { get; set; } = "";
        public string Accent { get; set; } = "";      // e.g., "10%" or "$5"
        public string TailText { get; set; } = " OFF."; // e.g., " OFF.", " OFF ORDERS."
        public bool IsCollected
        {
            get => _isCollected; set { if (_isCollected != value) { _isCollected = value; OnPropertyChanged(); } }
        }
        bool _isCollected;

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
