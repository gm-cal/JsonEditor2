using System.Windows;
using Services;
using ViewModels;
using Views;

public partial class MainWindow : Window{
    public MainWindow(IFileService fileService, IJsonService jsonService){
        InitializeComponent();
        DataContext = new MainViewModel(fileService, jsonService);
    }

    private void OnIndent(object sender, RoutedEventArgs e){
        TabsControl.SelectedTextEdit?.IndentSelection();
    }

    private void OnUnindent(object sender, RoutedEventArgs e){
        TabsControl.SelectedTextEdit?.UnindentSelection();
    }

    private void OnSettings(object sender, RoutedEventArgs e){
        SettingsWindow win = new SettingsWindow(){ Owner = this };
        win.ShowDialog();
    }
}
