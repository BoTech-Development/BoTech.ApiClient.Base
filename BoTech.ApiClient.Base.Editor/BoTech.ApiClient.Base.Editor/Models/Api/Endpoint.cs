using System.Collections.Generic;
using System.Net.Http;
using BoTech.ApiClient.Base.Editor.Models.Api.Request;

namespace BoTech.ApiClient.Base.Editor.Models.Api
{
    internal class Endpoint
    {
        /// <summary>
        /// Name of the method for instance "Login"
        /// </summary>
        public string MethodName { get; init; }

        /// <summary>
        /// contains the path to the endpoint (e.g. api/Users/Login), without the base address (e.g. https://api.botech.dev)
        /// </summary>
        public string Route { get; init; }
        /// <summary>
        /// All parameters defined in the route. For instance "{userId}"
        /// </summary>
        public List<string> RouteParameters { get; init; } = new List<string>();
        /// <summary>
        /// Defines the type of the request. For instance GET, POST, etc.
        /// </summary>
        public HttpMethod HttpMethod { get; init; }

        /// <summary>
        /// All messages that the server normally returns.
        /// </summary>
        public List<EndpointResult> PossibleResults { get; init; } = new List<EndpointResult>();

        /// <summary>
        /// This property basically describes the signature of the endpoint through each <see cref="EndpointRequest"/>.
        /// In addition, the object <see cref="EndpointRequest"/> stores, on which way the data is sent to the endpoint.
        /// Empty when there is no parameter needed to send else not empty.
        /// </summary>
        public EndpointRequest RequestDescription { get; init; } = new();
    }
}
