using Avalonia.Platform.Storage;
using ReactiveUI;
using ReactiveUI.Primitives;
using ShadUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoTech.ApiClient.Base.Editor.Controllers;
using BoTech.ApiClient.Base.Editor.Models;
using BoTech.ApiClient.Base.Editor.Services;

namespace BoTech.ApiClient.Base.Editor.ViewModels.Dialogs
{
    internal class CreateNewProjectDialogViewModel : ViewModelBase
    {
        public string ProjectName
        {
            get => field;
            set => this.RaiseAndSetIfChanged(ref field, value);
        } = "";

        public string ProjectLocation
        {
            get => field;
            set => this.RaiseAndSetIfChanged(ref field, value);
        } = "";

        public string OpenApiDefinitionJsonFilePath
        {
            get => field;
            set => this.RaiseAndSetIfChanged(ref field, value);
        } = "";

        public string SharedModelProjectFile
        {
            get => field;
            set => this.RaiseAndSetIfChanged(ref field, value);
        } = "";

        public ReactiveCommand<RxVoid, RxVoid> OpenProjectLocationFileSelectorCommand { get; set; } 
        public ReactiveCommand<RxVoid, RxVoid> OpenOpenApiFileSelectorCommand { get; set; }
        public ReactiveCommand<RxVoid, RxVoid> OpenSharedModelsProjectFileSelectorCommand { get; set; }
        public ReactiveCommand<RxVoid, RxVoid> CreateProjectCommand { get; set; }

        public CreateNewProjectDialogViewModel(DialogManager dialogManager, ToastManager toastManager) : base(dialogManager, toastManager)
        {
            OpenProjectLocationFileSelectorCommand =
                ReactiveCommand.Create(() =>
                {
                    IStorageFolder? folder = OpenFolderSelector("Select a folder for the result project.");
                    if(folder is not null)
                        ProjectLocation = folder.Path.AbsolutePath;
                });
            OpenOpenApiFileSelectorCommand =
                ReactiveCommand.Create(() =>
                {
                    IStorageFile? file = OpenFileSelector("Select a the swagger.json file.");
                    if(file is not null)
                        OpenApiDefinitionJsonFilePath = file.Path.AbsolutePath;
                });
            OpenSharedModelsProjectFileSelectorCommand =
                ReactiveCommand.Create(() =>
                {
                    IStorageFile? file = OpenFileSelector("Select the .csproj of the shared models Project.");
                    if (file is not null)
                        SharedModelProjectFile = file.Path.AbsolutePath;
                });
            CreateProjectCommand = ReactiveCommand.Create(CreateProject);
        }
        /// <summary>
        /// opens a native folder selector view
        /// </summary>
        /// <param name="title">The title of the view.</param>
        /// <returns>the selected folder or null when no folder selected.</returns>
        private IStorageFolder? OpenFolderSelector(string title)
        {
            return StorageProviderService.GetStorageProvider().OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                Title = title,
                AllowMultiple = false
            }).Result.FirstOrDefault();
        }
        /// <summary>
        /// opens a native file selector view
        /// </summary>
        /// <param name="title">The title of the view.</param>
        /// <returns>the selected file or null when no file selected.</returns>
        private IStorageFile? OpenFileSelector(string title)
        {
            return StorageProviderService.GetStorageProvider().OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = title,
                AllowMultiple = false
            }).Result.FirstOrDefault();
        }

        private void CreateProject()
        {
            DialogManager.Close(this);
            ProjectController.GetInstance().OnProjectLoaded += (sender, eventArgs) =>
            {
                EditorViewController.Instance!.InitializeViewsForCurrentProject(); // Nullable is necessary to check because it is set MainViewModel.cs .
            };
            ProjectController.GetInstance().CreateNewAndOpenProject(new CreateNewProjectData()
            {
                OpenApiDefinitionJsonFilePath = this.OpenApiDefinitionJsonFilePath,
                ProjectName = this.ProjectName,
                ProjectLocation = this.ProjectLocation,
                SharedModelProjectFile = this.SharedModelProjectFile
            });
        }
    }
}
