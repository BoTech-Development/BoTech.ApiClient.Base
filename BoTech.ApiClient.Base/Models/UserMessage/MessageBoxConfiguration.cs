namespace BoTech.ApiClient.Base.Models.UserMessage;
/// <summary>
/// This class can be used to create ui for message Box
/// </summary>
public class MessageBoxConfiguration
{
    /// <summary>
    /// The title of the message box
    /// </summary>
    public string Title { get; init; }
    /// <summary>
    /// The natural message box in the specific language.
    /// </summary>
    public string Message { get; init; }
    /// <summary>
    /// The Internal Exception or more info only necessary for developers.
    /// </summary>
    public string DeveloperInformation { get; init; }
    /// <summary>
    /// True when no error occur
    /// </summary>
    public bool Success { get; init; }

    private MessageBoxConfiguration(string title, string message, string developerInformation, bool success)
    {
        Title = title;
        Message = message;
        DeveloperInformation = developerInformation;
        Success = success;
    }

    public static MessageBoxConfiguration
        SuccessMessageBox(string title, string message, string developerInformation) =>
        new MessageBoxConfiguration(title, message, developerInformation, true);
    public static MessageBoxConfiguration
        ErrorMessageBox(string title, string message, string developerInformation) =>
        new MessageBoxConfiguration(title, message, developerInformation, false);
}
