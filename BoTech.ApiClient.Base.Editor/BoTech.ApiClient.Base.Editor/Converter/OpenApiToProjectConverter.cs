using BoTech.ApiClient.Base.Editor.Models;
using BoTech.ApiClient.Base.Editor.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Avalonia.Media.Transformation;
using Microsoft.OpenApi;


namespace BoTech.ApiClient.Base.Editor.Converter
{
    internal static class OpenApiToProjectConverter
    {
        public static void ConvertOpenApiToModels(Project projectContainingTheModels, OpenApiDocument openApiDocument)
        {
            foreach (KeyValuePair<string, IOpenApiPathItem> path in openApiDocument.Paths)
            {
                string route = path.Key;
                IOpenApiPathItem endpointsInfo = path.Value; // Contains info like HttpMethods for the specific endpoints. Multiple endpoints because there might be multiple implementations of a Method with the same signature.
                foreach (KeyValuePair<HttpMethod, OpenApiOperation> operation in endpointsInfo.Operations)
                {
                    HttpMethod method = operation.Key;
                    OpenApiOperation actualOperation = operation.Value;

                    Controller controllerWhichContainsTheEndpoint = CreateOrGetControllerModelFromOperationOrProject(actualOperation, projectContainingTheModels);
                    Endpoint currentEndpoint = CreateEndpointModelForController(controllerWhichContainsTheEndpoint, method, route);
                    AddPossibleResponsesToEndpoint(currentEndpoint, actualOperation.Responses);
                    controllerWhichContainsTheEndpoint.DefinedEndpointsInController.Add(currentEndpoint);
                }
            }
        }

        private static void AddPossibleResponsesToEndpoint(Endpoint endpoint, OpenApiResponses? responses)
        {
            if (responses == null)
                return;
            foreach (KeyValuePair<string, IOpenApiResponse> apiResponse in responses)
            {
                string statusCode = apiResponse.Key;
                IOpenApiResponse response = apiResponse.Value;

                endpoint.PossibleResults.Add(new EndpointResult()
                {
                    ResponseType = response.Content?.Keys.FirstOrDefault() ?? string.Empty,
                    DtoName = GetNameOfDtoIfExistsForResponse(response),
                    ServerResultStatusCode = ConvertStatusCodeStringToEnum(statusCode)
                });
            }
        }

        private static string GetNameOfDtoIfExistsForResponse(IOpenApiResponse response)
        {
            IOpenApiSchema? schema = response.Content?
                .Values
                .FirstOrDefault()?
                .Schema;

            return schema?.Id ?? string.Empty;
        }
        private static HttpStatusCode ConvertStatusCodeStringToEnum(string statusCode)
        {
            int statusCodeAsInt = int.Parse(statusCode);
            if (Enum.IsDefined(typeof(HttpStatusCode), statusCodeAsInt))
            {
                return (HttpStatusCode)statusCodeAsInt;
            }
            return default(HttpStatusCode);
        }
        private static Endpoint CreateEndpointModelForController(Models.Api.Controller controller, HttpMethod method, string routeToTheEndpoint)
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
                HttpMethod = method
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
                existingController = new Controller()
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
