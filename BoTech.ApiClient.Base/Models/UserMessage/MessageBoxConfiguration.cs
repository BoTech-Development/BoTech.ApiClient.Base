namespace BoTech.ApiClient.Base.Models.UserMessage;
/// <summary>
/// This class can be used to create ui for message Box
/// </summary>
public class MessageBoxConfiguration
{
    public string Title { get; init; }
    public string Message { get; init; }
    public string DeveloperInformation { get; init; }
    public bool Success { get; init; }
}
