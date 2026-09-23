using Avalonia.Controls;
using BoTech.ApiClient.Base.Editor.Views;
using ReactiveUI;
using ShadUI;
using Splat;
using System.Runtime.CompilerServices;

namespace BoTech.ApiClient.Base.Editor.ViewModels;

public class MainViewModel : ViewModelBase
{
    /// <summary>
    /// Make it accessible from the view.
    /// </summary>
    public DialogManager DialogManagerProperty => this.DialogManager;

    public MenuView MenuViewContent { get; set; }

    public MainViewModel(DialogManager dialogManager, ToastManager toastManager) : base(dialogManager, toastManager)
    {
        MenuViewContent = new MenuView()
        {
            DataContext = new MenuViewModel(dialogManager, toastManager)
        };
    }
}
