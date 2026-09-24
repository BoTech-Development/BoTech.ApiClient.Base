using System.Globalization;
using System.Net;
using BoTech.HttpClientHelper;

namespace BoTech.ApiClient.Base.Models.UserMessage;
/// <summary>
/// This class contains information about how to convert a http response into a user understandable message.
/// </summary>
public class UserMessageForHttpResult
{
    /// <summary>
    /// All Usermessage in connection with the language code (Set 1) (https://en.wikipedia.org/wiki/List_of_ISO_639_language_codes)
    /// The language code is the key and the text is the value.
    /// </summary>
    public Dictionary<CultureInfo, string> UserMessage { get; init; }
    /// <summary>
    /// The expected status code which will be returned
    /// </summary>
    public HttpStatusCode ExpectedStatusCode { get; init; }

    /// <summary>
    /// The string that should be returned by the endpoint.
    /// Is empty when not necessary to check. <see cref="ShouldCheckReturnedString"/>
    /// </summary>
    public string ExpectedReturnedString { get; init; } = "";

    /// <summary>
    /// Is true when <see cref="ExpectedReturnedString"/> is not empty and the server result should at least contain the string.
    /// </summary>
    public bool ShouldCheckReturnedString => !string.IsNullOrEmpty(ExpectedReturnedString);

    public UserMessageForHttpResult(Dictionary<CultureInfo, string> message, HttpStatusCode statusCode, string returnedString)
    {
        UserMessage = message;
        ExpectedStatusCode = statusCode;
        ExpectedReturnedString = returnedString;
        
    }
    /// <summary>
    /// This method can be used to find the correct user message for a specific result coming from the server.
    /// </summary>
    /// <typeparam name="T">Generic type of returned model. Not necessary for this method.</typeparam>
    /// <param name="requestResult"> The server result.</param>
    /// <returns>true when this (natural language) message can be assigned to the server result else false</returns>
    public bool Match<T>(RequestResult<T> requestResult)
    {
        if (requestResult.ResponseMessage != null)
        {
            if(requestResult.ResponseMessage.StatusCode != ExpectedStatusCode) return false;
            string responseMessage = requestResult.ResponseMessage.Content.ReadAsStringAsync().Result;
            if(!string.IsNullOrEmpty(responseMessage) && !responseMessage.Contains(ExpectedReturnedString)) return false;
            return true;
        }
        else
            return false; // Could not check
    }
}