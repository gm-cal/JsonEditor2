using System.Windows;
using Services;
using ViewModels;

public partial class MainWindow : Window{
    public MainWindow(IFileService fileService, IJsonService jsonService){
        InitializeComponent();
        DataContext = new MainViewModel(fileService, jsonService);
    }
}
