using System.Windows;
using JsonEditor2.Services;
using JsonEditor2.ViewModels;

namespace JsonEditor2{
    public partial class App : Application{
        private readonly IFileService fileService = new FileService();
        private readonly IJsonService jsonService = new JsonService();

        protected override void OnStartup(StartupEventArgs e){
            base.OnStartup(e);
            MainWindow window = new MainWindow(fileService, jsonService);
            window.Show();
        }
    }
}
