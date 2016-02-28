using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;

namespace Lithnet.MetadirectoryServices
{
    public class MmsSerializationSurrogate : IDataContractSurrogate
    {
        public object GetCustomDataToExport(Type clrType, Type dataContractType)
        {
            return null;
        }

        public object GetCustomDataToExport(System.Reflection.MemberInfo memberInfo, Type dataContractType)
        {
            return null;
        }

        public Type GetDataContractType(Type type)
        {
            if (typeof(CSEntryChange).IsAssignableFrom(type))
            {
                return typeof(CSEntryChangeSerializable);
            }
            else if (typeof(AttributeChange).IsAssignableFrom(type))
            {
                return typeof(AttributeChangeSerializable);
            }
            else if (typeof(ValueChange).IsAssignableFrom(type))
            {
                return typeof(ValueChangeSerializable);
            }
            else if (typeof(AnchorAttribute).IsAssignableFrom(type))
            {
                return typeof(AnchorAttributeSerializable);
            }
            else if (typeof(SchemaAttribute).IsAssignableFrom(type))
            {
                return typeof(SchemaAttributeSerializable);
            }
            else if (typeof(SchemaType).IsAssignableFrom(type))
            {
                return typeof(SchemaTypeSerializable);
            }
            else if (typeof(Schema).IsAssignableFrom(type))
            {
                return typeof(SchemaSerializable);
            }

            return type;
        }

        public object GetDeserializedObject(object obj, Type targetType)
        {
            ValueChangeSerializable valueChange = obj as ValueChangeSerializable;

            if (valueChange != null)
            {
                return valueChange.GetObject();
            }

            AttributeChangeSerializable attributeChange = obj as AttributeChangeSerializable;

            if (attributeChange != null)
            {
                return attributeChange.GetObject();
            }

            CSEntryChangeSerializable csentry = obj as CSEntryChangeSerializable;

            if (csentry != null)
            {
                return csentry.GetObject();
            }

            AnchorAttributeSerializable anchor = obj as AnchorAttributeSerializable;

            if (anchor != null)
            {
                return anchor.GetObject();
            }

            SchemaAttributeSerializable schemaAttribute = obj as SchemaAttributeSerializable;

            if (schemaAttribute != null)
            {
                return schemaAttribute.GetObject();
            }

            SchemaTypeSerializable schemaType = obj as SchemaTypeSerializable;

            if (schemaType != null)
            {
                return schemaType.GetObject();
            }

            SchemaSerializable schema = obj as SchemaSerializable;

            if (schema != null)
            {
                return schema.GetObject();
            }

            return obj;
        }

        public void GetKnownCustomDataTypes(System.Collections.ObjectModel.Collection<Type> customDataTypes)
        {
        }

        public object GetObjectToSerialize(object obj, Type targetType)
        {
            ValueChange valueChange = obj as ValueChange;

            if (valueChange != null)
            {
                return new ValueChangeSerializable(valueChange);
            }

            AttributeChange attributeChange = obj as AttributeChange;

            if (attributeChange != null)
            {
                return new AttributeChangeSerializable(attributeChange);
            }
                
            CSEntryChange csentry = obj as CSEntryChange;

            if (csentry != null)
            {
                return new CSEntryChangeSerializable(csentry);
            }

            AnchorAttribute anchor = obj as AnchorAttribute;

            if (anchor != null)
            {
                return new AnchorAttributeSerializable(anchor);
            }

            SchemaAttribute schemaAttribute = obj as SchemaAttribute;

            if (schemaAttribute != null)
            {
                return new SchemaAttributeSerializable(schemaAttribute);
            }

            SchemaType schemaType = obj as SchemaType;

            if (schemaType != null)
            {
                return new SchemaTypeSerializable(schemaType);
            }

            Schema schema = obj as Schema;

            if (schema != null)
            {
                return new SchemaSerializable(schema);
            }

            return obj;
        }

        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            if (typeName.Equals("CSEntryChangeSerializable"))
            {
                return typeof(CSEntryChange);
            }
            else if (typeName.Equals("AttributeChangeSerializable"))
            {
                return typeof(AttributeChange);
            }
            else if (typeName.Equals("ValueChangeSerializable"))
            {
                return typeof(ValueChange);
            }
            else if (typeName.Equals("AnchorAttributeSerializable"))
            {
                return typeof(AnchorAttribute);
            }
            else if (typeName.Equals("SchemaAttributeSerializable"))
            {
                return typeof(SchemaAttribute);
            }
            else if (typeName.Equals("SchemaTypeSerializable"))
            {
                return typeof(SchemaType);
            }
            else if (typeName.Equals("SchemaSerializable"))
            {
                return typeof(Schema);
            }

            return null;
        }

        public System.CodeDom.CodeTypeDeclaration ProcessImportedType(System.CodeDom.CodeTypeDeclaration typeDeclaration, System.CodeDom.CodeCompileUnit compileUnit)
        {
            return typeDeclaration;
        }
    }
}
