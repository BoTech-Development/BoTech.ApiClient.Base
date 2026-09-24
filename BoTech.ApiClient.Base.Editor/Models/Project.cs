using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using BoTech.ApiClient.Base.Editor.Models.Api;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace BoTech.ApiClient.Base.Editor.Models
{
    internal class Project
    {
        /// <summary>
        /// Each controller which is covered by the api client.
        /// </summary>
        public List<Api.Controller> CustomizedControllers { get; set; } = new List<Api.Controller>();
        /// <summary>
        /// The path to .csproj file which contains the api client for the api.
        /// </summary>
        public string ApiClientProjectFilePath { get; set; }
        /// <summary>
        /// The path to the .csproj, which contains all dto's.
        /// </summary>
        public string SharedModelsProjectFilePath { get; set; } = "";
        /// <summary>
        /// Info about the api
        /// Serialized version is stored in the <see cref="SerializedImplementingApiInfo"/>
        /// </summary>
        [JsonIgnore]
        public OpenApiDocument InfoAboutImplementingApi { get; set; }
        /// <summary>
        /// Serialized Version of <see cref="InfoAboutImplementingApi"/>
        /// </summary>
        public string SerializedImplementingApiInfo => ConvertImplementingApiToJson(this);
        public static string ConvertImplementingApiToJson(Project projectToConvertToJson)
        {
            TextWriter textStream = new StringWriter();
            OpenApiJsonWriter openApiWriter = new OpenApiJsonWriter(textStream);
            projectToConvertToJson.InfoAboutImplementingApi.SerializeAsV3(openApiWriter);
            return textStream.ToString();
        }
    }
}
