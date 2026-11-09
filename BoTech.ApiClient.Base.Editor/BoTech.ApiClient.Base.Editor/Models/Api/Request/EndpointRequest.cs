using System.Collections.Generic;
using BoTech.ApiClient.Base.Editor.Models.Api.Request.Parameters;

namespace BoTech.ApiClient.Base.Editor.Models.Api.Request;

/// <summary>
/// This type represents a request for an API endpoint.
/// This type is not an actual request, but rather a descriptor of the request.
/// It describes the parameters of the request.
/// </summary>
public class EndpointRequest
{
    /// <summary>
    /// This property describes the route parameters when selected.
    /// </summary>
    public RouteElement RouteParameters { get; init; } = new();
    /// <summary>
    /// The property is used to represent the parameters of the API request,
    /// ensuring proper validation and separation of route-specific and regular parameters.
    /// <br/><br/>
    /// This collection contains instances implementing <see cref="IParameterBase"/>,
    /// excluding any <see cref="RouteParameter"/> instances.
    /// </summary>
    public ParameterCollection Parameters { get; init; } = new();
}