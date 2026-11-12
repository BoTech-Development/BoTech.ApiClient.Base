using BoTech.ApiClient.Base.Editor.Services;
using BoTech.ApiClient.Base.Editor.ViewModels;
using Ursa.Controls;

namespace BoTech.ApiClient.Base.Editor.Views;

public partial class MainWindow : UrsaWindow
{
    public MainWindow()
    {
        InitializeComponent();
        this.mainContent.Content = new MainView()
        {
            DataContext = new MainViewModel()
        };
    }
}