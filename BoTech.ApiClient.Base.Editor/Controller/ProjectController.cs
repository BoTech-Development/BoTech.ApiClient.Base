using BoTech.ApiClient.Base.Editor.Models;
using Microsoft.OpenApi.Readers;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using BoTech.ApiClient.Base.Editor.Converter;
using Microsoft.Build.Evaluation;
using Project = BoTech.ApiClient.Base.Editor.Models.Project;
using BoTech.ApiClient.Base.Editor.Models.Api;

namespace BoTech.ApiClient.Base.Editor.Controller
{
    /// <summary>
    /// This singleton manages the project.
    /// </summary>
    internal class ProjectController
    {
        /// <summary>
        /// Points to the .bacproj [B]oTech[A]pi[C]lient[Proj]ect file.
        /// </summary>
        public string PathToTheProjectFile { get; private set; }
        /// <summary>
        /// Deserialized project from <see cref="PathToTheProjectFile"/>
        /// </summary>
        public Project CurrentLoadedProject { get; private set; }
        /// <summary>
        /// Will be raised when after the first loading of the project.
        /// Can be used to update the ui.
        /// </summary>
        public EventHandler<Project> OnProjectLoaded;

        private static ProjectController? _instance;
        private ProjectController() { }

        public static ProjectController GetInstance()
        {
            _instance ??= new ProjectController(); // ??= is the same as if(_instance is null) _instance = new ...
            return _instance;
        }

        public void CreateNewAndOpenProject(CreateNewProjectData newProjectInfo)
        {
            CurrentLoadedProject = new Project()
            {
                CustomizedControllers = new List<Models.Api.Controller>(),
                SharedModelsProjectFilePath = newProjectInfo.SharedModelProjectFile,
                ApiClientProjectFilePath = newProjectInfo.ProjectLocation + newProjectInfo.ProjectName + "/" + newProjectInfo.ProjectName + ".csproj"
            };
            LoadOpenApiDefinitionToCurrentProject(newProjectInfo.OpenApiDefinitionJsonFilePath);
            CreateApiClientProjectForCurrentProject();
            PathToTheProjectFile = newProjectInfo.ProjectLocation + newProjectInfo.ProjectName + ".bacproj";
            StoreProject();
            OnProjectLoaded?.Invoke(this, CurrentLoadedProject);
        }
        public void StoreProject()
        {
            string serializedProject = JsonSerializer.Serialize(CurrentLoadedProject);
            File.WriteAllText(PathToTheProjectFile, serializedProject);
        }
        private void LoadOpenApiDefinitionToCurrentProject(string openApiJsonFilePath)
        {
            Stream stream = File.OpenRead(openApiJsonFilePath);
            CurrentLoadedProject.InfoAboutImplementingApi = new OpenApiStreamReader().Read(stream, out var diagnostic);
            OpenApiToProjectConverter.ConvertOpenApiToModels(CurrentLoadedProject, CurrentLoadedProject.InfoAboutImplementingApi);
        }
        
        /// <summary>
        /// This method should build the basic .csproj .dll library project, which is empty.
        /// </summary>
        private void CreateApiClientProjectForCurrentProject()
        {
            // TODO: implement the create api client project method.
        }


    }
}
