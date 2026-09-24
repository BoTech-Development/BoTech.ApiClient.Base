using BoTech.ApiClient.Base.Editor.Models;
using BoTech.ApiClient.Base.Editor.Models.Api;
using BoTech.ApiClient.Base.Models.UserMessage;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;


namespace BoTech.ApiClient.Base.Editor.Converter
{
    internal static class OpenApiToProjectConverter
    {
        public static void ConvertOpenApiToModels(Project projectContainingTheModels, OpenApiDocument openApiDocument)
        {
            foreach (KeyValuePair<string, OpenApiPathItem> path in openApiDocument.Paths)
            {
                string route = path.Key;
                OpenApiPathItem endpointsInfo = path.Value; // Contains info like HttpMethods for the specific endpoints. Multiple endpoints because there might be multiple implementations of a Method with the same signature.
                foreach (KeyValuePair<OperationType, OpenApiOperation> operation in endpointsInfo.Operations)
                {
                    OperationType typeOfOperation = operation.Key;
                    OpenApiOperation actualOperation = operation.Value;

                    Models.Api.Controller controllerWhichContainsTheEndpoint =
                        CreateOrGetControllerModelFromOperationOrProject(actualOperation, projectContainingTheModels);
                    Endpoint currentEndpoint =
                        CreateEndpointModelForController(controllerWhichContainsTheEndpoint, typeOfOperation, route);
                    AddPossibleResponsesToEndpoint(currentEndpoint, actualOperation.Responses);
                    controllerWhichContainsTheEndpoint.DefinedEndpointsInController.Add(currentEndpoint);
                }
            }
        }

        private static void AddPossibleResponsesToEndpoint(Endpoint endpoint, OpenApiResponses responses)
        {
            foreach (KeyValuePair<string, OpenApiResponse> apiResponse in responses)
            {
                string statusCode = apiResponse.Key;
                OpenApiResponse response = apiResponse.Value;

                endpoint.PossibleResults.Add(new EndpointResult()
                {
                    ResponseType = response.Content?.Keys.FirstOrDefault() ?? string.Empty,
                    DtoName = GetNameOfDtoIfExistsForResponse(response),
                    ServerResultStatusCode = ConvertStatusCodeStringToEnum(statusCode)
                });
            }
        }

        private static string GetNameOfDtoIfExistsForResponse(OpenApiResponse response)
        {
            if (response.Content is not null && response.Content.Values.Count > 0)
            {
                OpenApiMediaType mediaType = response.Content.Values.First();
                OpenApiSchema schema = mediaType.Schema;
                return schema.Reference.Id;
            }
            return "";
        }
        private static HttpStatusCode ConvertStatusCodeStringToEnum(string statusCode)
        {
            if (Enum.IsDefined(typeof(HttpStatusCode), statusCode))
            {
                return Enum.Parse<HttpStatusCode>(statusCode);
            }
            return default(HttpStatusCode);
        }
        private static Endpoint CreateEndpointModelForController(Models.Api.Controller controller, OperationType type, string routeToTheEndpoint)
        {
            string methodName = "";
            int indexOfControllerName = routeToTheEndpoint.IndexOf(controller.ControllerName);
            if (indexOfControllerName > 0)
                methodName = routeToTheEndpoint.Substring(indexOfControllerName + controller.ControllerName.Length);
            else
                methodName = routeToTheEndpoint;
            return new Endpoint()
            {
                MethodName = methodName.Replace("/",""),
                Route = routeToTheEndpoint,
                HttpMethod = new HttpMethod(type.ToString().ToUpperInvariant())
            };
        }
        private static Models.Api.Controller CreateOrGetControllerModelFromOperationOrProject(OpenApiOperation operation, Project project)
        {
            string? controllerName = operation.Tags.FirstOrDefault()?.Name;
            if (controllerName is null)
                throw new ArgumentException(
                    "Can not evaluate the controller name by using the OpenApi definition of your api.");
            Models.Api.Controller? existingController = project.CustomizedControllers.Find(cont => cont.ControllerName == controllerName);
            if (existingController == null)
            {
                existingController = new Models.Api.Controller()
                {
                    ControllerName = controllerName,
                    DefinedEndpointsInController = new List<Endpoint>()
                };
                project.CustomizedControllers.Add(existingController);
            }
            return existingController;
        }
    }
}
