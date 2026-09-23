using Avalonia.Media.Imaging;
using BoTech.ApiClient.Base.Editor.Services;
using ShadUI;
using Splat;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace BoTech.ApiClient.Base.Editor.ViewModels.Dialogs
{
    public class AboutDialogViewModel : ViewModelBase
    {
        public string VersionStringIncludingDate { get; set; }

        public AboutDialogViewModel(DialogManager dialogManager, ToastManager toastManager) : base(dialogManager, toastManager)
        {
            ApplicationUpdateService.UpdateInfo info = ApplicationUpdateService.GetInstance().CurrentVersion;
            VersionStringIncludingDate = info.VersionString + " (" + info.ReleaseDateTime + ")";
        }
    }
}
