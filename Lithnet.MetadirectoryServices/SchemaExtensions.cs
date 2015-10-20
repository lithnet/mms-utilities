using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;

namespace Lithnet.MetadirectoryServices
{
    public static class SchemaExtensions
    {      
        public static bool HasAttribute(this SchemaType type, string attribute)
        {
            return type.Attributes.Any(t => t.Name == attribute);
        }

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

        public static bool HasObjectClass(this SchemaTypeKeyedCollection types, string objectClass)
        {
            return types.Contains(objectClass);
        }
    }
}