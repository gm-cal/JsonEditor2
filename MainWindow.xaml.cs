using System.Windows;
using JsonEditor2.Services;
using JsonEditor2.ViewModels;

namespace JsonEditor2{
    public partial class MainWindow : Window{
        public MainWindow(IFileService fileService, IJsonService jsonService){
            InitializeComponent();
            DataContext = new MainViewModel(fileService, jsonService);
        }
    }
}
