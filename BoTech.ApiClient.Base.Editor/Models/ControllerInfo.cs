using System;
using System.Collections.Generic;
using System.Text;
using BoTech.ApiClient.Base.Models.UserMessage;

namespace BoTech.ApiClient.Base.Editor.Models
{
    internal class ControllerInfo
    {
        /// <summary>
        /// The name of the controller (this name may be not equals to the actual class name)
        /// </summary>
        public string ControllerName { get; init; }
        /// <summary>
        /// This so-called user messages define a natural language response for each server result.
        /// </summary>
        public List<UserMessagesForEndpoint> UserResultMessages { get; init; }
    }
}
