using System;
using System.Collections.Generic;
using System.Text;

namespace BoTech.ApiClient.Base.Editor.Models.Api.Request.TypeDescriptors
{
    public class UnifiedTypeDescriptor
    {
        /// <summary>
        /// The type of the parameter when it is primitive else this value will be none.
        /// </summary>
        public PrimitiveType PrimitiveType { get; init; } = PrimitiveType.None;
        /// <summary>
        /// This class describes any object.
        /// </summary>
        public ObjectDescriptor? ObjectType { get; init; }
        /// <summary>
        /// The generic type of the collection or the Array.
        /// This property is only not null when this type is an array or collection of a primitive type.
        /// </summary>
        public PrimitiveType ArrayOfPrimitivesType { get; init; } = PrimitiveType.None;
        /// <summary>
        /// The generic type of the collection or the Array.
        /// This property is only not null when this type is an array or collection of an object.
        /// </summary>
        public ObjectDescriptor? ArrayOfObjectsType { get; init; }
        /// <summary>
        /// This value will be not null when the type is at least a two-dimensional array.
        /// This object describes the sub array type.
        /// </summary>
        public UnifiedTypeDescriptor? SubArrayTypeDescriptor { get; init; }

        public bool IsPrimitive() => ObjectType is null && ArrayOfObjectsType is null;
        public bool IsArray() => ArrayOfPrimitivesType != PrimitiveType.None ||
                                             ArrayOfObjectsType is not null;
        public bool IsSubArray() => SubArrayTypeDescriptor != null;

        private UnifiedTypeDescriptor() { }

        public static UnifiedTypeDescriptor CreatePrimitiveType(PrimitiveType type)
        {
            return new UnifiedTypeDescriptor()
            {
                PrimitiveType = type
            };
        }
        public static UnifiedTypeDescriptor CreateObjectType(ObjectDescriptor type)
        {
            return new UnifiedTypeDescriptor()
            {
                ObjectType = type
            };
        }
        public static UnifiedTypeDescriptor CreatePrimitiveArrayType(PrimitiveType type)
        {
            return new UnifiedTypeDescriptor()
            {
                ArrayOfPrimitivesType = type
            };
        }
        public static UnifiedTypeDescriptor CreateObjectArrayType(ObjectDescriptor type)
        {
            return new UnifiedTypeDescriptor()
            {
                ArrayOfObjectsType = type
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subArrayType"></param>
        /// <returns></returns>
        public static UnifiedTypeDescriptor CreateSubArrayType(UnifiedTypeDescriptor subArrayType)
        {
            return new UnifiedTypeDescriptor()
            { 
                SubArrayTypeDescriptor = subArrayType
            };
        }
    }
}
