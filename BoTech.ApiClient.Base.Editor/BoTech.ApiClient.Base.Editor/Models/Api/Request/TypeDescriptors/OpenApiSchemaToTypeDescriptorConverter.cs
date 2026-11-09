using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.OpenApi;

namespace BoTech.ApiClient.Base.Editor.Models.Api.Request.TypeDescriptors
{
    internal static class OpenApiSchemaToTypeDescriptorConverter
    {
        public static UnifiedTypeDescriptor GenerateTypeDescriptorFromOpenApiSchema(IOpenApiSchema schema)
        {
            if (schema.Type == JsonSchemaType.Object)
            {
                return UnifiedTypeDescriptor.CreateObjectType(GenerateObjectTypeDescriptor(schema));
            }
            else if (schema.Type == JsonSchemaType.Array)
            {
                return GenerateArrayTypeDescriptor(schema);
            }
            else // The type is primitive
            {
                return GeneratePrimitiveTypeDescriptor(schema);
            }
        }
        private static ObjectDescriptor GenerateObjectTypeDescriptor(IOpenApiSchema schema)
        {
            ObjectDescriptor descriptor = new ObjectDescriptor()
            {
                TypeName = schema.Id ?? "NO_NAME"
            };
            if(schema.Properties is not null)
            {
                foreach (KeyValuePair<string, IOpenApiSchema> property in schema.Properties)
                {
                    descriptor.Properties.Add(property.Key, 
                        GenerateTypeDescriptorFromOpenApiSchema(property.Value));
                }
            }
            return descriptor;
        }
        private static UnifiedTypeDescriptor GenerateArrayTypeDescriptor(IOpenApiSchema schema)
        {
            if (schema.Items is null || schema.Items.Type is null)
                throw new ArgumentException(
                    $"Can not evaluate the type of the collection in the open api definition: {schema}");
            if (schema.Items.Type == JsonSchemaType.Object)
            {
                return UnifiedTypeDescriptor.CreateObjectArrayType(
                    GenerateObjectTypeDescriptor(schema));
            }
            else if (schema.Items.Type == JsonSchemaType.Array)
            {
                return UnifiedTypeDescriptor.CreateSubArrayType(
                    GenerateTypeDescriptorFromOpenApiSchema(schema.Items));
            }
            else // The type is a primitive collection
            {
                return UnifiedTypeDescriptor.CreatePrimitiveArrayType(
                    ParsePrimitiveTypeEnumFromOpenApiRepresentation(schema.Items.Type));
            }
        }
        private static UnifiedTypeDescriptor GeneratePrimitiveTypeDescriptor(IOpenApiSchema schema)
        {
            return UnifiedTypeDescriptor.CreatePrimitiveType(
                ParsePrimitiveTypeEnumFromOpenApiRepresentation(schema.Type));
        }

        private static PrimitiveType ParsePrimitiveTypeEnumFromOpenApiRepresentation(JsonSchemaType? type)
        {
            if (type is null)
                throw new ArgumentException($"Can not evaluate the type of an primitive value.");
            return Enum.Parse<PrimitiveType>(Enum.GetName(typeof(JsonSchemaType), type) ?? throw new ArgumentException("Can not get the string representation of JsonSchemaType enum item."));
        }
    }
}
