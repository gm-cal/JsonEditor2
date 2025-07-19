using System.Windows;
using System.Windows.Input;
using ViewModels;

namespace Views{
    public partial class SettingsWindow : Window{
        public SettingsViewModel ViewModel { get; } = new SettingsViewModel();
        public SettingsWindow(){
            InitializeComponent();
            DataContext = ViewModel;
            KeyGestureConverter conv = new KeyGestureConverter();
            ViewModel.IndentStyle = EditorSettings.IndentString == "\t" ? "Tab" :
                (EditorSettings.IndentString.Length == 2 ? "2 Spaces" : "4 Spaces");
            ViewModel.IndentShortcut = conv.ConvertToString(EditorSettings.IndentGesture)!;
            ViewModel.UnindentShortcut = conv.ConvertToString(EditorSettings.UnindentGesture)!;
            ViewModel.ShowLineNumbers = EditorSettings.ShowLineNumbers;
        }
        private void OnOk(object sender, RoutedEventArgs e){
            EditorSettings.Apply(ViewModel);
            DialogResult = true;
        }
    }
}
