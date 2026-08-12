namespace BoTech.ApiClient.Base.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ApiClientEndpoint : Attribute
{
    public string EndpointName { get; }
    public ApiClientEndpoint(string endpointName)
    {
        EndpointName = endpointName;
    }
}