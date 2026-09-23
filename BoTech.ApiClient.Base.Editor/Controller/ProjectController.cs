using BoTech.ApiClient.Base.Editor.Models;
using Microsoft.OpenApi.Readers;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

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
        public string PathToTheProjectFile { get; private set;  }
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
            Project projectToCreate = new Project();
            LoadOpenApiDefinitionTo(projectToCreate, newProjectInfo.OpenApiDefinitionJsonFilePath);
            PathToTheProjectFile = newProjectInfo.ProjectLocation + "\\" + newProjectInfo.ProjectName + ".bacproj";
            CurrentLoadedProject = projectToCreate;
            StoreProject();
            OnProjectLoaded?.Invoke(this, projectToCreate);
        }
        public void StoreProject()
        {
            string serializedProject = JsonSerializer.Serialize(CurrentLoadedProject);
            File.WriteAllText(PathToTheProjectFile, serializedProject);
        }
        private void LoadOpenApiDefinitionTo(Project projectToLoad, string openApiJsonFilePath)
        {
            Stream stream = File.OpenRead(openApiJsonFilePath);
            projectToLoad.InfoAboutImplementingApi = new OpenApiStreamReader().Read(stream, out var diagnostic);
        }


        private void Test()
        {

            using var stream = File.OpenRead("swagger.json");

            var document = new OpenApiStreamReader()
                .Read(stream, out var diagnostic);

            foreach (var path in document.Paths)
            {
                Console.WriteLine(path.Key);

                foreach (var operation in path.Value.Operations)
                {
                    Console.WriteLine($"  {operation.Key}");
                    Console.WriteLine($"  {operation.Value.Summary}");
                }
            }
        }
    }
}
