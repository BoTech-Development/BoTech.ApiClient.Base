namespace BoTech.ApiClient.Base.Editor.Models.Api.Request.TypeDescriptors;

public class CollectionPropertyType
{
    /// <summary>
    /// When not null, this property describes an object-based item type of the collection
    /// </summary>
    public ObjectDescriptor? ObjectType { get; set; } = null;
    /// <summary>
    /// When not null, this property describes a primitive item type of the collection
    /// </summary>
    public PrimitiveType? PrimitiveType { get; set; } = null;
}