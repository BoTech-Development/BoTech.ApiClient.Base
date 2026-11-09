using System.Globalization;
using BoTech.ApiClient.Base.Models.UserMessage;
using BoTech.ApiClient.Base.Services;
using BoTech.HttpClientHelper;

namespace BoTech.ApiClient.Base.Models;
/// <summary>
/// Result of each Api client endpoint.
/// </summary>
public class ActionResult
{
    /// <summary>
    /// 
    /// </summary>
    public bool IsSuccess { get; init; }
    /// <summary>
    /// The internal http response
    /// </summary>
    public HttpResponseMessage? Message { get; init; } 
    /// <summary>
    /// The errro that may occured by invoking the request
    /// </summary>
    public Exception? Error { get; init; }
    public required Dictionary<CultureInfo, string> UserMessage { get; init; }
    /// <summary>
    /// The data object which was encoded by the HttpClientHelper
    /// </summary>
    public object? ParsedData { get; init; }

    public static ActionResult FromRequestResult(string endpoint, RequestResult requestResult, HttpResultToUserMessageConverter converter)
    {
        if (requestResult.Error != null)
            return new ActionResult()
            {
                IsSuccess = requestResult.IsSuccess(),
                Error = requestResult.Error,
                UserMessage = converter.GetUserMessageFromRequestResult(endpoint, requestResult),
                Message = requestResult.ResponseMessage,
                ParsedData = requestResult.ParsedData
            };
        return new ActionResult()
        {
            IsSuccess = requestResult.IsSuccess(),
            Error = null,
            UserMessage = converter.GetUserMessageFromRequestResult(endpoint, requestResult),
            Message = requestResult.ResponseMessage,
            ParsedData = requestResult.ParsedData
        };
    }

    public MessageBoxConfiguration GetMessageBoxConfiguration(string languageCode)
    {
        string message = UserMessage.First(culturePair => culturePair.Key.TwoLetterISOLanguageName == languageCode).Value;
        string developerInfo = (Message == null ? "Status Code: " + Message.StatusCode + "\n" + "Reason: " + Message.ReasonPhrase + "\n" : "") + "Internal Exceptions:\n" + Error.ToString();
        if (IsSuccess)
            return MessageBoxConfiguration.SuccessMessageBox("Success", message, developerInfo);
        return MessageBoxConfiguration.ErrorMessageBox("Error", message, developerInfo);
    }

    public override string ToString()
    {
        return "Status: " + (IsSuccess ? "Success" : "Error") + "\n"
            + "Message: " + Message + "\n" 
            + "Errors: " + Error + "\n"
            + "UserMessage: " + UserMessage;
    }
}