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
    public class AttributeChangeSerializable
    {
        [DataMember]
        public AttributeType DataType { get; set; }

        [DataMember]
        public bool IsMultiValued { get; set; }

        [DataMember]
        public AttributeModificationType ModificationType { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public IList<ValueChange> ValueChanges { get; private set; }

        internal AttributeChangeSerializable(AttributeChange change)
        {
            this.SetObject(change);
        }

        internal void SetObject(AttributeChange change)
        {
            this.DataType = change.DataType;
            this.IsMultiValued = change.IsMultiValued;
            this.ModificationType = change.ModificationType;
            this.Name = change.Name;
            this.ValueChanges = change.ValueChanges;
        }

        internal AttributeChange GetObject()
        {
            return new AttributeChangeDetached(this.Name, this.ModificationType, this.ValueChanges, this.DataType, this.IsMultiValued, false);
        }
    }
}