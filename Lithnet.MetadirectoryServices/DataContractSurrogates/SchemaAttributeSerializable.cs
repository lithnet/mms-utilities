using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using Microsoft.MetadirectoryServices;
using Microsoft.MetadirectoryServices.DetachedObjectModel;

namespace Lithnet.MetadirectoryServices
{
    [DataContract]
    public class SchemaAttributeSerializable
    {
        [DataMember]
        public AttributeType DataType { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public AttributeOperation AllowedAttributeOperation { get; set; }

        [DataMember]
        public bool IsAnchor { get; set; }

        [DataMember]
        public bool IsMultiValued { get; set; }

        internal SchemaAttributeSerializable(SchemaAttribute attribute)
        {
            this.SetObject(attribute);
        }

        internal void SetObject(SchemaAttribute attribute)
        {
            this.DataType = attribute.DataType;
            this.Name = attribute.Name;
            this.AllowedAttributeOperation = attribute.AllowedAttributeOperation;
            this.IsAnchor = attribute.IsAnchor;
            this.IsMultiValued = attribute.IsMultiValued;
        }

        internal SchemaAttribute GetObject()
        {
            if (this.IsAnchor)
            {
                return SchemaAttribute.CreateAnchorAttribute(this.Name, this.DataType, this.AllowedAttributeOperation);
            }
            else if (this.IsMultiValued)
            {
                return SchemaAttribute.CreateMultiValuedAttribute(this.Name, this.DataType, this.AllowedAttributeOperation);
            }
            else
            {
                return SchemaAttribute.CreateSingleValuedAttribute(this.Name, this.DataType, this.AllowedAttributeOperation);
            }
        }
    }
}