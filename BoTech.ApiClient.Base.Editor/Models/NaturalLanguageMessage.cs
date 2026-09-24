using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BoTech.ApiClient.Base.Editor.Models
{
    internal class NaturalLanguageMessage
    {        
        /// <summary>
        /// The natural language result, which is readable by the user.
        /// </summary>
        public Dictionary<CultureInfo, string> UserMessage { get; init; }
        /// <summary>
        /// The string that should be returned by the endpoint.
        /// Is empty when not necessary to check. <see cref="ShouldCheckReturnedString"/>
        /// </summary>
        public string ExpectedReturnedString { get; init; } = "";
        /// <summary>
        /// Is true when <see cref="ExpectedReturnedString"/> is not empty and the server result should at least contain the string.
        /// </summary>
        public bool ShouldCheckReturnedString => !string.IsNullOrEmpty(ExpectedReturnedString);
    }
}
