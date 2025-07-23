using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Services;
using ViewModels;

public partial class App : Application{
    public static ServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e){
        base.OnStartup(e);
        ServiceCollection sc = new();
        ConfigureServices(sc);
        Services = sc.BuildServiceProvider();

        MainWindow window = Services.GetRequiredService<MainWindow>();
        window.Show();
    }

    private static void ConfigureServices(IServiceCollection services){
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IJsonService, JsonService>();
        services.AddSingleton<ITextLineInputService, TextLineInputService>();
        services.AddSingleton<ITextService, TextService>();

        services.AddTransient<TextEditorViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();
    }

    protected override void OnExit(ExitEventArgs e){
        Services.Dispose();
        base.OnExit(e);
    }
}
