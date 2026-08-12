using System.Globalization;
using System.Net;

using BoTech.ApiClient.Base.Models.UserMessage;
using BoTech.HttpClientHelper;
using Newtonsoft.Json;

namespace BoTech.ApiClient.Base.Services;

public class HttpResultToUserMessageConverter
{
    private List<UserMessagesForEndpoint> _configuration = new List<UserMessagesForEndpoint>();
    private string _controllerName;
    private string _baseUrl;
    public HttpResultToUserMessageConverter(string baseUrl, string controllerName)
    { 
        _controllerName = controllerName; 
        _baseUrl = baseUrl;
    }

    public void AddConfigurationFromJsonResource(string resourceName)
    {
        string json = ResourceFileLoader.LoadFile(resourceName);
        List<UserMessagesForEndpoint>? configuration = JsonConvert.DeserializeObject<List<UserMessagesForEndpoint>>(json);
        if (configuration == null)
            throw new FormatException(
                "The coinfiguration file was found but is empty or does not implement the correct object.");
        _configuration = configuration;
    }
    public HttpResultToUserMessageConverter AddConfiguration(string message, string languageCode, HttpStatusCode statusCode, string returnedString, string actionName)
    {
        UserMessagesForEndpoint? userMessageForHttpResult = _configuration.Find(umfe => umfe.Endpoint == actionName);
        if (userMessageForHttpResult == null)
        {
            userMessageForHttpResult = new UserMessagesForEndpoint(_controllerName, actionName, _baseUrl);
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