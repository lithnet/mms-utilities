using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;

namespace Lithnet.MetadirectoryServices
{
    /// <summary>
    /// This class contains extensions to the MetadirectoryServices schema and related objects
    /// </summary>
    public static class SchemaExtensions
    {      
        /// <summary>
        /// Gets a value indicating if the schema type has the specified attribute in its definition
        /// </summary>
        /// <param name="type">The type to evaluate</param>
        /// <param name="attribute">The name of the attribute</param>
        /// <returns>A value indicating if the attribute is present in the schema type</returns>
        public static bool HasAttribute(this SchemaType type, string attribute)
        {
            return type.Attributes.Any(t => t.Name == attribute);
        }

        /// <summary>
        /// Gets a value indicating if the schema collection has the specified object type and attribute in its definition
        /// </summary>
        /// <param name="types">The schema collection to evaluate</param>
        /// <param name="objectClass">The name of the object class</param>
        /// <param name="attribute">The name of the attribute</param>
        /// <returns>A value indicating if the object class and attribute are both present in the schema</returns>
        public static bool HasAttribute(this SchemaTypeKeyedCollection types, string objectClass, string attribute)
        {
            if (types.Contains(objectClass))
            {
                return types[objectClass].Attributes.Any(t => t.Name == attribute);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the schema collection has the specified object type in its definition
        /// </summary>
        /// <param name="types">The schema collection to evaluate</param>
        /// <param name="objectClass">The name of the object class</param>
        /// <returns>A value indicating if the object class is present in the schema</returns>
        public static bool HasObjectClass(this SchemaTypeKeyedCollection types, string objectClass)
        {
            return types.Contains(objectClass);
        }
    }
}