using BoTech.ApiClient.Base.Editor.Models.Api.Request.TypeDescriptors;

namespace BoTech.ApiClient.Base.Editor.Models.Api.Request.Parameters;

public interface IPrimitiveParameter : IParameterBase
{
    /// <summary>
    /// The name of the parameter. (name of the argument)
    /// </summary>
    public string Name { get; set; }

}