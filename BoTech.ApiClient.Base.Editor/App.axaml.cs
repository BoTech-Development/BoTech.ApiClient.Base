using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BoTech.ApiClient.Base.Editor.Services;
using BoTech.ApiClient.Base.Editor.ViewModels;
using BoTech.ApiClient.Base.Editor.Views;

namespace BoTech.ApiClient.Base.Editor;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ServiceGenerator.CreateServices();
        ServiceGenerator.InitializeDialogManager();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = ServiceGenerator.ServiceProvider.GetService(typeof(MainViewModel))
            };
            StorageProviderService.CreateInstance(desktop.MainWindow); // Init file dialog helper
        }

        base.OnFrameworkInitializationCompleted();
    }
}