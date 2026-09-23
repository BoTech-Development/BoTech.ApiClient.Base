using System;
using System.Collections.Generic;
using System.Text;

namespace BoTech.ApiClient.Base.Editor.Models
{
    internal class CreateNewProjectData
    {
        /// <summary>
        /// Where should the new project be created?
        /// </summary>
        public required string ProjectLocation { get; init; }
        /// <summary>
        /// The .json file which contains the definition of the api.
        /// </summary>
        public required string OpenApiDefinitionJsonFilePath { get; init; }
        /// <summary>
        /// When a shared models project exists, the path to the .csproj file will be stored here.
        /// The shared models project should contain all DTO's.
        /// </summary>
        public string SharedModelProjectFile { get; init; } = "";
        /// <summary>
        /// The name of the new Project.
        /// </summary>
        public required string ProjectName { get; init; }
    }
}
