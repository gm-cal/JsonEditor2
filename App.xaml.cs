using System.Windows;
using Services;
using ViewModels;

public partial class App : Application{
    private readonly IFileService fileService = new FileService();
    private readonly IJsonService jsonService = new JsonService();
//    private readonly ITextService textService = new TextService();

    protected override void OnStartup(StartupEventArgs e){
        base.OnStartup(e);
        MainWindow window = new MainWindow(fileService, jsonService);
        window.Show();
    }
}
