using System.Globalization;
using BoTech.ApiClient.Base.Models.UserMessage;
using BoTech.ApiClient.Base.Services;
using BoTech.HttpClientHelper;

namespace BoTech.ApiClient.Base.Models;
/// <summary>
/// Result of each Api client endpoint.
/// </summary>
/// <typeparam name="T">Type of parsed data</typeparam>
public class ActionResult<T>
{
    public bool IsSuccess { get; init; }
    public HttpResponseMessage? Message { get; init; } 
    public required List<Exception> Errors { get; init; }
    public required Dictionary<CultureInfo, string> UserMessage { get; init; }
    /// <summary>
    /// The data object which was encoded by the HttpClientHelper
    /// </summary>
    public T? ParsedData { get; init; }
    
    private ActionResult()
    {
        
    }

    public static ActionResult<T> FromRequestResult(string endpoint, RequestResult<T> requestResult, HttpResultToUserMessageConverter converter)
    {
        if (requestResult.Error != null)
            return new ActionResult<T>()
            {
                IsSuccess = requestResult.Success,
                Errors = new List<Exception>() { requestResult.Error },
                UserMessage = converter.GetUserMessageFromRequestResult(endpoint, requestResult),
                Message = requestResult.ResponseMessage,
                ParsedData = requestResult.ParsedData
            };
        return new ActionResult<T>()
        {
            IsSuccess = requestResult.Success,
            Errors = new List<Exception>() { },
            UserMessage = converter.GetUserMessageFromRequestResult(endpoint, requestResult),
            Message = requestResult.ResponseMessage,
            ParsedData = requestResult.ParsedData
        };
    }

    public MessageBoxConfiguration GetMessageBoxConfiguration(string languageCode)
    {
        string exceptionString = "";
        foreach (var error in Errors) exceptionString += error.Message + "\n";
        return new MessageBoxConfiguration()
        {
            Title = IsSuccess ? "Success" : "Error",
            Success = IsSuccess,
            Message = UserMessage.First(culturePair => culturePair.Key.TwoLetterISOLanguageName == languageCode).Value,
            DeveloperInformation = "Status Code: " + Message.StatusCode + "\n"
                                   + "Reason: " + Message.ReasonPhrase
                                   + "Internal Exceptions:\n" + exceptionString
        };
    }

    public override string ToString()
    {
        return "Status: " + (IsSuccess ? "Success" : "Error") + "\n"
            + "Message: " + Message + "\n" 
            + "Errors: " + Errors + "\n"
            + "UserMessage: " + UserMessage;
    }
}