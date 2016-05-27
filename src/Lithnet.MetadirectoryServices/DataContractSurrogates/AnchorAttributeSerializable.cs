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
    public class AnchorAttributeSerializable
    {
        [DataMember]
        public AttributeType DataType { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public object Value { get; set; }

        internal AnchorAttributeSerializable(AnchorAttribute anchor)
        {
            this.SetObject(anchor);
        }

        internal void SetObject(AnchorAttribute anchor)
        {
            this.DataType = anchor.DataType;
            this.Name = anchor.Name;
            this.Value = anchor.Value;
        }

        internal AnchorAttribute GetObject()
        {
            return new AnchorAttributeDetached(this.Name, this.DataType, this.Value);
        }
    }
}