using System;
using System.Collections.Generic;
using System.Text;

namespace BoTech.ApiClient.Base.Editor.Models.Api.Request.TypeDescriptors;

/// <summary>
/// this data model describes a dto for
/// </summary>
public class ObjectDescriptor 
{
    /// <summary>
    /// Name of the dto class.
    /// </summary>
    public required string TypeName { get; set; }
    /// <summary>
    /// This Namespace where the dto class is defined.
    /// This namespace corresponds to the project which this tool generates.
    /// </summary>
    public string Namespace { get; set; } = "";
    /// <summary>
    /// The key is the property name, and the type <see cref="UnifiedTypeDescriptor"/> type describes any other object, primitive, or collection based type.
    /// </summary>
    public Dictionary<string, UnifiedTypeDescriptor> Properties { get; set; } = new();
}