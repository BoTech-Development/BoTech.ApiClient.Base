using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

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
        /// Defines the type of the request. For instance GET, POST, etc.
        /// </summary>
        public HttpMethod HttpMethod { get; init; }
        /// <summary>
        /// All messages that the server normally returns.
        /// </summary>
        public List<EndpointResult> PossibleResults { get; init; } = new List<EndpointResult>();
    }
}
