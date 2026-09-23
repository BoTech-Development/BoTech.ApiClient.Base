using ReactiveUI;
using ReactiveUI.Primitives;
using ShadUI;
using System;
using System.Diagnostics;
using System.Threading;
using BoTech.ApiClient.Base.Editor.Services;
using BoTech.ApiClient.Base.Editor.ViewModels.Dialogs;

namespace BoTech.ApiClient.Base.Editor.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        /// <summary>
        /// should the update button be visible?
        /// </summary>
        public bool IsUpdateAvailable { get => field; set => this.RaiseAndSetIfChanged(ref field, value); }
        /// <summary>
        /// The latest version to update to.
        /// </summary>
        public string LatestVersion { get => field; set => this.RaiseAndSetIfChanged(ref field, value); }
        /// <summary>
        /// Updates this application to a new version.
        /// </summary>
        public ReactiveCommand<RxVoid, RxVoid> ExecuteUpdateCommand { get; set; }
        /// <summary>
        /// Shows the About dialog 
        /// </summary>
        public ReactiveCommand<RxVoid, RxVoid> ShowAboutDialogCommand { get; set; }
        /// <summary>
        /// Starts the <see cref="ApplicationUpdateService.GetInstance().CheckForUpdates()"/> method
        /// </summary>
        public ReactiveCommand<RxVoid, RxVoid> CheckForUpdatesCommand { get; set; }
        /// <summary>
        /// opens the GitHub page in the web browser
        /// </summary>
        public ReactiveCommand<RxVoid, RxVoid> ShowGithubPageCommand { get; set; }

        public ReactiveCommand<RxVoid, RxVoid> OpenCreateNewProjectCommand { get; set; }

        public MenuViewModel(DialogManager dialogManager, ToastManager toastManager) : base(dialogManager, toastManager)
        {
            ApplicationUpdateService.GetInstance().OnNewUpdateFound +=
                (object? sender, ApplicationUpdateService.UpdateInfo updateInfo) =>
                {
                    IsUpdateAvailable = true;
                    LatestVersion = updateInfo.VersionString;
                };
            ApplicationUpdateService.GetInstance().OnUpdateError += (object? sender, Exception error) =>
            {
                DialogManager.CreateDialog("Error by installing or fetching updates!", "Here is the error: " + error.Message)
                    .WithCancelButton("Close")
                    .Dismissible()
                    .Show();
            };
            CheckForUpdatesCommand = ReactiveCommand.Create(() =>
            { 
                    ApplicationUpdateService.GetInstance().CheckForUpdates();
            });
            ShowGithubPageCommand = ReactiveCommand.Create(OpenGitHubPage);
            ExecuteUpdateCommand = ReactiveCommand.Create(ExecuteUpdate);
            ShowAboutDialogCommand = ReactiveCommand.Create(ShowAboutDialog);
            OpenCreateNewProjectCommand = ReactiveCommand.Create(OpenCreateNewProjectDialog);
            new Thread(() => ApplicationUpdateService.GetInstance().CheckForUpdates()).Start();
        }

        private void OpenCreateNewProjectDialog()
        {
            CreateNewProjectDialogViewModel vm = new CreateNewProjectDialogViewModel(DialogManager, ToastManager);
            DialogManager.CreateDialog(vm)
                .Dismissible()
                .Show();
        }
        /// <summary>
        /// This method shows the info page
        /// </summary>
        public void OpenGitHubPage()
        {
            string url = $"https://aka.botech.dev/bot.acb.editor?from=application&version={ApplicationUpdateService.GetInstance().CurrentVersion.VersionString}";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        /// <summary>
        /// Executes the update but first asks the user if he wants to update.
        /// </summary>
        private void ExecuteUpdate()
        {
            ApplicationUpdateService.UpdateInfo currentVersion = ApplicationUpdateService.GetInstance().CurrentVersion;
            ApplicationUpdateService.UpdateInfo latestVersion = ApplicationUpdateService.GetInstance().GetNewVersionInfo();

            DialogManager.CreateDialog("Do you want to update now?", 
                    "Your Version: " + currentVersion.VersionString + " (" + currentVersion.ReleaseDateTime + ") \n Latest Version: " + latestVersion.VersionString + " (" + latestVersion.ReleaseDateTime + ") \n Info about the latest Version: \n" + latestVersion.InformationString)
                .WithPrimaryButton("Update Now!", () => ApplicationUpdateService.GetInstance().ExecuteUpdateCommand())
                .WithCancelButton("Remind me later")
                .Show();
        }
        private void ShowAboutDialog()
        {
            DialogManager.CreateDialog(new AboutDialogViewModel(DialogManager, ToastManager))
                .Dismissible()
                .Show();
        }
    }
}
