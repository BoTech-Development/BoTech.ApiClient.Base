
using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Media;

namespace BoTech.ApiClient.Base.Editor.ViewModels.Editor.UserMessages
{
    internal class EndpointResultItemViewModel
    {
        public required IBrush ServerResultIndicatorBackgroundBrush { get; set; }
        public required IBrush ServerResultIndicatorBorderBrush { get; set; }
        /// <summary>
        /// Should contain the name of the http response type and the number (e.g. 200 OK).
        /// </summary>
        public required string ServerResultStatusCode { get; set; }
        /// <summary>
        /// Should contain the name of the dto class or the return type name (e.g. application/json).
        /// </summary>
        public required string DtoNameOrReturnTypeName { get; set; }
    }
}
