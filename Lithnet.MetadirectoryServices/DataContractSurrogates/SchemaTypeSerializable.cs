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
    public class SchemaTypeSerializable
    {
        [DataMember]
        public IList<SchemaAttribute> AnchorAttributes { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public IList<SchemaAttribute> Attributes { get; set; }

        [DataMember]
        public bool Locked { get; set; }

        internal SchemaTypeSerializable(SchemaType type)
        {
            this.SetObject(type);
        }

        internal void SetObject(SchemaType type)
        {
            this.AnchorAttributes = type.AnchorAttributes;
            this.Attributes = type.Attributes;
            this.Locked = type.Locked;
            this.Name = type.Name;
        }

        internal SchemaType GetObject()
        {
            SchemaType t = SchemaType.Create(this.Name, this.Locked);

            if (this.AnchorAttributes != null)
            {
                foreach (SchemaAttribute a in this.AnchorAttributes)
                {
                    t.AnchorAttributes.Add(a);
                }
            }

            if (this.Attributes != null)
            {
                foreach (SchemaAttribute a in this.Attributes)
                {
                    t.Attributes.Add(a);
                }
            }

            return t;
        }
    }
}