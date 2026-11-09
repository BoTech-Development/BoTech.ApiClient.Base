namespace BoTech.ApiClient.Base.Editor.Models.Api.Request.TypeDescriptors;

/// <summary>
/// Represents the data type of property within a DTO (Data Transfer Object).
/// </summary>
public enum PrimitiveType
{
    /// <summary>
    /// Represents a text-based property type within a Data Transfer Object (DTO).
    /// </summary>
    String,
    /// <summary>
    /// Represents a numeric property type within a Data Transfer Object (DTO).
    /// This number can be either an integer or a floating-point value.
    /// </summary>
    Number,
    /// <summary>
    /// Represents an integer property type within a Data Transfer Object (DTO).
    /// </summary>
    Integer,
    /// <summary>
    /// Represents a boolean property type within a Data Transfer Object (DTO).
    /// </summary>
    Boolean,
    /// <summary>
    /// The parameter type could not be loaded
    /// </summary>
    None
}