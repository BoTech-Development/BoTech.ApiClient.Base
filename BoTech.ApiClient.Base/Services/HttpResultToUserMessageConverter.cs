using System.Globalization;
using System.Net;

using BoTech.ApiClient.Base.Models.UserMessage;
using BoTech.HttpClientHelper;
using Newtonsoft.Json;

namespace BoTech.ApiClient.Base.Services;
/// <summary>
/// This class can be used to convert the server response into natural language. This class have support for different languages.
/// This class also stores all the config for the Messages.
/// </summary>
public class HttpResultToUserMessageConverter(string baseUrl, string controllerName)
{
    private List<UserMessagesForEndpoint> _configuration = new List<UserMessagesForEndpoint>();

    /// <summary>
    /// This method loads the config from a json files stored in the assembly which calls this method.
    /// </summary>
    /// <param name="resourceName">The name of the file including the namespace.</param>
    /// <exception cref="FormatException">Error by deserializing.</exception>
    public void AddConfigurationFromJsonResource(string resourceName)
    {
        string json = ResourceFileLoader.LoadFile(resourceName);
        List<UserMessagesForEndpoint>? configuration = JsonConvert.DeserializeObject<List<UserMessagesForEndpoint>>(json);
        if (configuration == null)
            throw new FormatException(
                "The configuration file was found but is empty or does not implement the correct object.");
        _configuration = configuration;
    }
    /// <summary>
    /// This method adds the configuration manually.
    /// </summary>
    /// <param name="message">The natural language message</param>
    /// <param name="languageCode">The language code of the language in which the message is written.</param>
    /// <param name="statusCode">Status of the response.</param>
    /// <param name="returnedString"></param>
    /// <param name="actionName">The endpoint fot this request.</param>
    /// <returns>The instance of this class to call this method again.</returns>
    /// <exception cref="InvalidOperationException">Occurs when: A Configuration for this language code already exists.</exception>
    public HttpResultToUserMessageConverter AddConfiguration(string message, string languageCode, HttpStatusCode statusCode, string returnedString, string actionName)
    {
        UserMessagesForEndpoint? userMessageForHttpResult = _configuration.Find(umfe => umfe.Endpoint == actionName);
        if (userMessageForHttpResult == null)
        {
            userMessageForHttpResult = new UserMessagesForEndpoint(controllerName, actionName, baseUrl);
            _configuration.Add(userMessageForHttpResult);
        }
        // Check if there is already a message defined fo the specific case (HttpStatusCode and returnedString).
        UserMessageForHttpResult? messageForSpecificStatusCode = userMessageForHttpResult.UserMessages
            .Find(userMessage => userMessage.ExpectedReturnedString == returnedString &&
                                 userMessage.ExpectedStatusCode == statusCode);
        if (messageForSpecificStatusCode != null)
        {
            // Chaekc if there is already a configuration for the specific language code in the specific case (HttpStatusCode and returnedString).
            IEnumerable<KeyValuePair<CultureInfo, string>>? result = messageForSpecificStatusCode.UserMessage.Where(culturePair =>
                culturePair.Key.TwoLetterISOLanguageName == languageCode);
            // Check if result contains any values
            if (result != null && result.Any())
                throw new InvalidOperationException("A Configuration for this language code already exists.");
        }else
        {
            messageForSpecificStatusCode = new UserMessageForHttpResult(new Dictionary<CultureInfo, string>(), statusCode, returnedString);
        }
        messageForSpecificStatusCode.UserMessage.Add(CultureInfo.GetCultureInfo(languageCode), message);
        return this;
    }
    /// <summary>
    /// Fetches the correct natural language message for the specific endpoint and the server result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="endpoint">The name of the endpoint that was called.</param>
    /// <param name="requestResult">The server result.</param>
    /// <returns>"An unknown error has occured." in different languages when there was an error fetching the correct natural language message, or the message in different languages.</returns>
    /// <exception cref="ArgumentException">The config for the specific endpoint is missing.</exception>
    public Dictionary<CultureInfo, string> GetUserMessageFromRequestResult<T>(string endpoint, RequestResult<T> requestResult)
    {
        UserMessagesForEndpoint? userMessagesForEndpoint = _configuration.Find(umfe => umfe.Endpoint == endpoint);
        if (userMessagesForEndpoint == null) throw new ArgumentException("No configuration found for the endpoint: " + endpoint);
        foreach (UserMessageForHttpResult userMessageForHttpResult in userMessagesForEndpoint.UserMessages)
        {
            if (userMessageForHttpResult.Match(requestResult)) 
                return userMessageForHttpResult.UserMessage;
        }

        return new Dictionary<CultureInfo, string>()
        {
            { CultureInfo.GetCultureInfo("en"), "An unknown error has occured." }
        };
    }
}