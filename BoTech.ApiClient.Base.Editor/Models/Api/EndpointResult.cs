 using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BoTech.ApiClient.Base.Editor.Models.Api
{
    internal class EndpointResult
    {
        /// <summary>
        /// The status code that the server returns. For instance 404 (NotFound)
        /// </summary>
        public HttpStatusCode ServerResultStatusCode { get; init; }
        /// <summary>
        /// Describes the data type that is returned from the server.
        /// Fo instance this property could contain the following string: application/json
        /// </summary>
        public string ResponseType { get; init; }

        /// <summary>
        /// Can be empty when the endpoint dos not respond with an object.
        /// Otherwise, the property will contain the name of the dto class.
        /// </summary>
        public string DtoName { get; init; } = "";
        /// <summary>
        /// Can be null when the endpoint dos not respond with an object or the system could not find the type.
        /// Otherwise, the property will contain the type of the dto class.
        /// The Dto class should be included in the shared models Project.
        /// </summary>
        public Type? DtoType { get; init; }
        /// <summary>
        /// Contains a natural by the user readable text, describing the server response.
        /// </summary>
        public NaturalLanguageMessage? Message { get; init; }
    }
}
