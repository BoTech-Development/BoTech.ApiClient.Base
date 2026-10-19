using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BoTech.ApiClient.Base.Editor.Services;
using BoTech.ApiClient.Base.Editor.Services.AvaloniaTopLevel;
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
            desktop.MainWindow = new MainWindow();
            TopLevelProvider.CreateInstance(desktop.MainWindow); // Init file dialog helper
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
        {
            singleViewFactoryApplicationLifetime.MainViewFactory =
                () =>
                {
                    MainView mainView = new MainView
                    {
                        DataContext = ServiceGenerator.ServiceProvider.GetService(typeof(MainViewModel))
                    };
                    TopLevelProvider.CreateInstance(mainView); // Init file dialog helper
                    return mainView;
                };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = ServiceGenerator.ServiceProvider.GetService(typeof(MainViewModel))
            };
            TopLevelProvider.CreateInstance(singleViewPlatform.MainView); // Init file dialog helper
        }
        base.OnFrameworkInitializationCompleted();
    }
}