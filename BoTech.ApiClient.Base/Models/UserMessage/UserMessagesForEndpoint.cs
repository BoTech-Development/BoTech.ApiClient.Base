namespace BoTech.ApiClient.Base.Models.UserMessage;

public class UserMessagesForEndpoint
{
    /// <summary>
    /// The name of the controller, which this UserMessage is related to
    /// </summary>
    public string ControllerName { get; init; }
    /// <summary>
    /// The name of the Endpoint, which this UserMessage is related to
    /// </summary>
    public string Endpoint { get; init; }
    /// <summary>
    /// The http path to the controller and action => will be built in the constructor
    /// </summary>
    public string HttpPath { get; init; }
    /// <summary>
    /// All user messages for the specific Endpoint => Needed because endpoint can return multiple status codes.
    /// </summary>
    public List<UserMessageForHttpResult> UserMessages { get; init; }

    public UserMessagesForEndpoint(string controllerName, string endpoint, string baseUrl)
    {
        ControllerName = controllerName;
        Endpoint = endpoint;
        HttpPath = baseUrl +  "/" + controllerName + "/" + endpoint;
    }
}