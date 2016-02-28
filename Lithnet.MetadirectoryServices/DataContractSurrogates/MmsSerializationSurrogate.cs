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

            return null;
        }

        public System.CodeDom.CodeTypeDeclaration ProcessImportedType(System.CodeDom.CodeTypeDeclaration typeDeclaration, System.CodeDom.CodeCompileUnit compileUnit)
        {
            return typeDeclaration;
        }
    }
}
