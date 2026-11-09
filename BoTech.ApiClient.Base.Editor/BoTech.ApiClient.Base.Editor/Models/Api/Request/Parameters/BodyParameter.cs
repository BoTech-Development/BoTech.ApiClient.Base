using BoTech.ApiClient.Base.Editor.Models.Api.Request.TypeDescriptors;

namespace BoTech.ApiClient.Base.Editor.Models.Api.Request.Parameters;

public class BodyParameter : IObjectParameter
{
    public bool IsRequired { get; set; }
    public bool IsDeprecated { get; set; }
    public string Description { get; set; } = "";
    public required UnifiedTypeDescriptor TypeDescriptor { get; set; }
}