using BoTech.ApiClient.Base.Editor.Models.Api.Request.TypeDescriptors;
using System.Text.Json.Serialization;

namespace BoTech.ApiClient.Base.Editor.Models.Api.Request.Parameters;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(BodyParameter), nameof(BodyParameter))]
[JsonDerivedType(typeof(CookieParameter), nameof(CookieParameter))]
[JsonDerivedType(typeof(HeaderParameter), nameof(HeaderParameter))]
[JsonDerivedType(typeof(QueryParameter), nameof(QueryParameter))]
[JsonDerivedType(typeof(RouteParameter), nameof(RouteParameter))]
public interface IParameterBase
{
    /// <summary>
    /// Specifies whether the client must provide a value for this parameter.
    /// </summary>
    public bool IsRequired { get; set; }
    /// <summary>
    /// Specifies that a parameter is deprecated and SHOULD be transitioned out of usage. 
    /// </summary>
    public bool IsDeprecated { get; set; }
    /// <summary>
    /// Description of the parameter
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// This object describes the type of the property.
    /// </summary>
    public UnifiedTypeDescriptor TypeDescriptor { get; set; }
}