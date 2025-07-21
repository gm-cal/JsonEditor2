using System.Windows;
using Controls;
using Services;
using ViewModels;
using Views;

public partial class MainWindow : Window{
    public MainWindow(IFileService fileService, IJsonService jsonService, ITextService textService){
        InitializeComponent();
        DataContext = new MainViewModel(fileService, jsonService, textService);
    }

    private TabControls tabsControl => TabsControl;

    private void OnIndent(object sender, RoutedEventArgs e){
        tabsControl.SelectedTextEdit?.IndentSelection();
    }

    private void OnUnindent(object sender, RoutedEventArgs e){
        tabsControl.SelectedTextEdit?.UnindentSelection();
    }

    private void OnSettings(object sender, RoutedEventArgs e){
        SettingsWindow win = new SettingsWindow(){ Owner = this };
        win.ShowDialog();
    }
}
