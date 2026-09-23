using ReactiveUI;
using ShadUI;

namespace BoTech.ApiClient.Base.Editor.ViewModels;

public abstract class ViewModelBase : ReactiveObject
{
    public readonly DialogManager DialogManager;
    public readonly ToastManager ToastManager;
    public ViewModelBase(DialogManager dialogManager, ToastManager toastManager)
    {
        DialogManager = dialogManager;
        ToastManager = toastManager;
    }
}
