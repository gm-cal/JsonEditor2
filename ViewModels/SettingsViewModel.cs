using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ViewModels{
    public class SettingsViewModel : INotifyPropertyChanged{
        private string indentStyle = "4 Spaces";
        private string indentShortcut = "Tab";
        private string unindentShortcut = "Shift+Tab";

        public string IndentStyle{
            get => indentStyle;
            set{ indentStyle = value; OnPropertyChanged(); }
        }
        public string IndentShortcut{
            get => indentShortcut;
            set{ indentShortcut = value; OnPropertyChanged(); }
        }
        public string UnindentShortcut{
            get => unindentShortcut;
            set{ unindentShortcut = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null){
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
