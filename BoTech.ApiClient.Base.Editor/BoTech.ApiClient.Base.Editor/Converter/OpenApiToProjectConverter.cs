using BoTech.ApiClient.Base.Editor.Models;
using BoTech.ApiClient.Base.Editor.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Avalonia.Media.Transformation;
using BoTech.ApiClient.Base.Editor.Models.Api.Request;
using BoTech.ApiClient.Base.Editor.Models.Api.Request.Parameters;
using BoTech.ApiClient.Base.Editor.Models.Api.Request.TypeDescriptors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;


namespace BoTech.ApiClient.Base.Editor.Converter
{
    internal static class OpenApiToProjectConverter
    {
        public static void ConvertOpenApiToModels(Project projectContainingTheModels, OpenApiDocument openApiDocument)
        {
            AddAllEndpointsToProject(projectContainingTheModels, openApiDocument);
        }
        private static void AddAllEndpointsToProject(Project projectContainingTheModels, OpenApiDocument openApiDocument)
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
                    AddRequestParamsToTheEndpoint(currentEndpoint, actualOperation);
                    controllerWhichContainsTheEndpoint.DefinedEndpointsInController.Add(currentEndpoint);
                }
            }
        }

        private static void AddRequestParamsToTheEndpoint(Endpoint endpoint, OpenApiOperation operation)
        {
            if(operation.Parameters is null)
                return;
            if (operation.RequestBody is not null && operation.RequestBody.Content is not null)
            {
                BodyParameter bodyParam = new BodyParameter()
                {
                    Description = operation.RequestBody.Description ?? "",
                    IsDeprecated = false,
                    IsRequired = operation.RequestBody.Required,
                    TypeDescriptor =
                        OpenApiSchemaToTypeDescriptorConverter.GenerateTypeDescriptorFromOpenApiSchema(
                            operation.RequestBody.Content.First().Value.Schema)
                };
                endpoint.RequestDescription.Parameters.Add(bodyParam);
            }
            foreach (IOpenApiParameter parameter in operation.Parameters)
            {
                if(parameter.Schema is null)
                    return;
                //THIS CODE IS SHIT
                switch (parameter.In)
                {
                    case ParameterLocation.Query:
                        endpoint.RequestDescription.Parameters.Add(
                            new QueryParameter()
                            {
                                Name = parameter.Name ?? "",
                                Description = parameter.Description ?? "",
                                IsDeprecated = parameter.Deprecated,
                                IsRequired = parameter.Required,
                                TypeDescriptor = OpenApiSchemaToTypeDescriptorConverter.GenerateTypeDescriptorFromOpenApiSchema(parameter.Schema)
                            });
                        break;
                    case ParameterLocation.QueryString:
                        endpoint.RequestDescription.Parameters.Add(
                            new QueryParameter()
                            {
                                Name = parameter.Name ?? "",
                                Description = parameter.Description ?? "",
                                IsDeprecated = parameter.Deprecated,
                                IsRequired = parameter.Required,
                                TypeDescriptor = OpenApiSchemaToTypeDescriptorConverter.GenerateTypeDescriptorFromOpenApiSchema(parameter.Schema)
                            });
                        break;
                    case ParameterLocation.Header:
                        endpoint.RequestDescription.Parameters.Add(
                            new HeaderParameter()
                            {
                                Name = parameter.Name ?? "",
                                Description = parameter.Description ?? "",
                                IsDeprecated = parameter.Deprecated,
                                IsRequired = parameter.Required,
                                TypeDescriptor = OpenApiSchemaToTypeDescriptorConverter.GenerateTypeDescriptorFromOpenApiSchema(parameter.Schema)
                            });
                        break;
                    case ParameterLocation.Path:
                        endpoint.RequestDescription.RouteParameters.Add(
                            RouteElement.CreateRouteElementFromRoutParameter(
                                new RouteParameter()
                                {
                                    Name = parameter.Name ?? "",
                                    Description = parameter.Description ?? "",
                                    IsDeprecated = parameter.Deprecated,
                                    IsRequired = parameter.Required,
                                    TypeDescriptor = OpenApiSchemaToTypeDescriptorConverter.GenerateTypeDescriptorFromOpenApiSchema(parameter.Schema)
                                }
                             ));
                        break;
                    case ParameterLocation.Cookie:
                        endpoint.RequestDescription.Parameters.Add(
                            new CookieParameter()
                            {
                                Name = parameter.Name ?? "",
                                Description = parameter.Description ?? "",
                                IsDeprecated = parameter.Deprecated,
                                IsRequired = parameter.Required,
                                TypeDescriptor = OpenApiSchemaToTypeDescriptorConverter.GenerateTypeDescriptorFromOpenApiSchema(parameter.Schema)
                            });
                        break;
                    default:
                        throw new ArgumentException(
                            $"The parameter Type is unknown and can't be converted: {parameter}");
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
            // Check is needed because in a default state the status code does not contain a real number.
            if (int.TryParse(statusCode, out int statusCodeAsInt)) 
            {
                if (Enum.IsDefined(typeof(HttpStatusCode), statusCodeAsInt))
                {
                    return (HttpStatusCode)statusCodeAsInt;
                }
            }
            return default(HttpStatusCode);
        }
        private static Endpoint CreateEndpointModelForController(Controller controller, HttpMethod method, string routeToTheEndpoint)
        {
            List<string> routeElements = routeToTheEndpoint.Split('/').ToList();
            // Remove split error item.
            routeElements.Remove("");
            // remove the controller name because the Controller already contains the info.
            routeElements.Remove(controller.ControllerName);
            // filter all rout params like /{userId}/
            List<string> routeParams = routeElements.FindAll(routeItem => routeItem.Contains("{"));

            // delete all filtered items first.
            routeElements.RemoveAll(routeParams.Contains);
            // if the endpoint has a name it will be the last item in the Collection.
            string methodName = "NO_NAME";
            if (routeElements.Count == 1)
                methodName = routeElements[0];
            return new Endpoint()
            {
                MethodName = methodName,
                Route = routeToTheEndpoint,
                HttpMethod = method,
                RouteParameters = routeElements
            };
        }
        private static Controller CreateOrGetControllerModelFromOperationOrProject(OpenApiOperation operation, Project project)
        {
            string? controllerName = operation.Tags.FirstOrDefault()?.Name;
            if (controllerName is null)
                throw new ArgumentException(
                    "Can not evaluate the controller name by using the OpenApi definition of your api.");
            Controller? existingController = project.CustomizedControllers.Find(cont => cont.ControllerName == controllerName);
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
