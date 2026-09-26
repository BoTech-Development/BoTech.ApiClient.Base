using Avalonia.Platform.Storage;
using BoTech.ApiClient.Base.Editor.Services;
using BoTech.ApiClient.Base.Editor.ViewModels.Dialogs;
using ReactiveUI;
using ReactiveUI.Primitives;
using ShadUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BoTech.ApiClient.Base.Editor.Controllers;

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
        /// <summary>
        /// 
        /// </summary>
        public ReactiveCommand<RxVoid, RxVoid> OpenCreateNewProjectDialogCommand { get; set; }
        /// <summary>
        /// let the user select the project file and opens the project.
        /// </summary>
        public ReactiveCommand<RxVoid, RxVoid> OpenProjectDialogCommand { get; set; }

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
            OpenCreateNewProjectDialogCommand = ReactiveCommand.Create(OpenCreateNewProjectDialog);
            OpenProjectDialogCommand = ReactiveCommand.Create(OpenProjectDialog);
            new Thread(() => ApplicationUpdateService.GetInstance().CheckForUpdates()).Start();
        }
        /// <summary>
        /// let the user select the project file and opens the project.
        /// </summary>
        private void OpenProjectDialog()
        {
            IStorageFile? file = OpenFileSelector("Select a .bacproj");
            if(file is not null)
            {
                ProjectController.GetInstance().OnProjectLoaded += (sender, eventArgs) =>
                {
                    EditorViewController.Instance!.InitializeViewsForCurrentProject(); // Nullable is necessary to check because it is set MainViewModel.cs .
                };
                ProjectController.GetInstance().LoadProjectFromFile(file.Path.AbsolutePath);
            }
        }
        private IStorageFile? OpenFileSelector(string title)
        {
            return StorageProviderService.GetStorageProvider().OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = title,
                AllowMultiple = false
            }).Result.FirstOrDefault();
        }
        /// <summary>
        /// Shows the create new dialog 
        /// </summary>
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
