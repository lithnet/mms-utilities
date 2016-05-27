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
    public class SchemaSerializable
    {
        [DataMember]
        public IList<SchemaType> Types{ get; set; }

        internal SchemaSerializable(Schema s)
        {
            this.SetObject(s);
        }

        internal void SetObject(Schema s)
        {
            this.Types = s.Types;
        }

        internal Schema GetObject()
        {
            Schema s = Schema.Create();
            
            if (this.Types != null)
            {
                foreach(SchemaType t in this.Types)
                {
                    s.Types.Add(t);
                }
            }

            return s;
        }
    }
}