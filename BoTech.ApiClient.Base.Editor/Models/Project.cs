using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace BoTech.ApiClient.Base.Editor.Models
{
    internal class Project
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ControllerInfo> InfoOfAllCustomizedControllers { get; set; }
        /// <summary>
        /// The path to .csproj file which contains the api client for the api.
        /// </summary>
        public string ApiClientProjectFilePath { get; set; }
        /// <summary>
        /// Info about the api
        /// Serialized version is stored in the <see cref="SerializedImplementingApiInfo"/>
        /// </summary>
        [JsonIgnore]
        public OpenApiDocument InfoAboutImplementingApi { get; set; }

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
