using Avalonia.Media.Imaging;
using BoTech.ApiClient.Base.Editor.Services;

namespace BoTech.ApiClient.Base.Editor.ViewModels.Dialogs
{
    public class AboutDialogViewModel : ViewModelBase
    {
        public string VersionStringIncludingDate { get; set; }

        public AboutDialogViewModel()
        {
            ApplicationUpdateService.UpdateInfo info = ApplicationUpdateService.GetInstance().CurrentVersion;
            VersionStringIncludingDate = info.VersionString + " (" + info.ReleaseDateTime + ")";
        }
    }
}
