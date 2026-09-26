using Avalonia.Controls;
using BoTech.ApiClient.Base.Editor.Views;
using ReactiveUI;
using ShadUI;
using Splat;
using System.Runtime.CompilerServices;
using BoTech.ApiClient.Base.Editor.Controllers;
using BoTech.ApiClient.Base.Editor.Views.Editor;

namespace BoTech.ApiClient.Base.Editor.ViewModels;

public class MainViewModel : ViewModelBase
{
    /// <summary>
    /// Make it accessible from the view.
    /// </summary>
    public DialogManager DialogManagerProperty => this.DialogManager;

    public MenuView MenuViewContent { get; set; }
    /// <summary>
    /// nav pane for the controllers defined in the api
    /// </summary>
    public Control ControllerNavPanelView
    {
        get => field; 
        set => this.RaiseAndSetIfChanged(ref field, value);
    }  
    /// <summary>
    /// The main tabbed page.
    /// </summary>
    public Control MainTabPanelView
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
    public MainViewModel(DialogManager dialogManager, ToastManager toastManager) : base(dialogManager, toastManager)
    {
        EditorViewController.CreateInstance(this); // make this instance globally available.
        MenuViewContent = new MenuView()
        {
            DataContext = new MenuViewModel(dialogManager, toastManager)
        };
        ControllerNavPanelView = new TextBlock()
        {
            Text = "Please open or create project!"
        };
        MainTabPanelView = new TextBlock()
        {
            Text = "Please open or create project!"
        };
    }
}
