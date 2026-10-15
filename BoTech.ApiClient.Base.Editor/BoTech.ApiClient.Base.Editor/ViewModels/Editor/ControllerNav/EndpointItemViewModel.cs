using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Media;
using BoTech.ApiClient.Base.Editor.Models.Api;

namespace BoTech.ApiClient.Base.Editor.ViewModels.Editor.ControllerNav
{
    internal class EndpointItemViewModel
    {
        public required string EndpointName { get; set; }
        public required IBrush HttpMethodIndicatorBorderColor { get; set; }
        public required IBrush HttpMethodIndicatorBackgroundColor { get; set; }
        public required string HttpMethodName { get; set; }
        public required string Route { get; set; }

        public bool Equals(Endpoint endpoint)
        {
            return EndpointName.Equals(endpoint.MethodName) && HttpMethodName.Equals(endpoint.HttpMethod.Method) && Route.Equals(endpoint.Route);
        }
    }
}
