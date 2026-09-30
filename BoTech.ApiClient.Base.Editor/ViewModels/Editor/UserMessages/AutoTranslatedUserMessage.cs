using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BoTech.ApiClient.Base.Editor.ViewModels.Editor.UserMessages
{
    internal class AutoTranslatedUserMessage
    {
        /// <summary>
        /// The language of the message.
        /// </summary>
        public required CultureInfo Language { get; set; }
        /// <summary>
        /// The message that is translated to the language specified in the "Language" property.
        /// </summary>
        public required string TranslatedMessage { get; set; }

        public static AutoTranslatedUserMessage Empty { get; set; } = new AutoTranslatedUserMessage()
        {
            Language = CultureInfo.GetCultureInfo("en-US"),
            TranslatedMessage = "Please enter a message to translate."
        };
    }
}
