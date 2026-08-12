using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using BoTech.ApiClient.Base.Attributes;
using BoTech.ApiClient.Base.Models;
using BoTech.ApiClient.Base.Services;
using BoTech.HttpClientHelper;

namespace BoTech.ApiClient.Base.Client;

public class ApiClientBase
{
    private string _baseUrl;
    private string _controllerName;
    protected readonly HttpRequestHelper _httpRequestHelper;
    protected readonly HttpResultToUserMessageConverter _httpResultToUserMessageConverter;
    public ApiClientBase(HttpRequestHelper requestHelper, string controllerName, string baseUrl)
    {
        _httpRequestHelper = requestHelper;
        _httpResultToUserMessageConverter = new HttpResultToUserMessageConverter(baseUrl, controllerName);
    }

    public ActionResult<T> Result<T>(RequestResult<T> requestResult)
    {
        // Frame 0 = this method
        // Frame 1 = the method that called this method
        var frame = new StackFrame(1);
        MethodBase? caller = frame.GetMethod();
        if(caller == null) throw new Exception("caller is null");
        string endpoint = string.Empty;
        foreach (object attributeInstance in caller.GetCustomAttributes(inherit: true))
        {
            if (attributeInstance.GetType() == typeof(ApiClientEndpoint))
            {
                endpoint = ((ApiClientEndpoint)attributeInstance).EndpointName;
            }
        }
        if (string.IsNullOrEmpty(endpoint)) throw new InvalidOperationException("You might not defined the attribute ApiClientEndpoint which is required for all methods that call this method.");
        return ActionResult<T>.FromRequestResult(endpoint, requestResult, _httpResultToUserMessageConverter);
    }
}