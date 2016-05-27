using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using Microsoft.MetadirectoryServices;
using System.Collections.ObjectModel;
using Microsoft.MetadirectoryServices.DetachedObjectModel;

namespace Lithnet.MetadirectoryServices
{
    [DataContract]
    public class CSEntryChangeSerializable
    {
        internal CSEntryChangeSerializable(CSEntryChange csentry)
        {
            this.SetObject(csentry);
        }

        [DataMember]
        public string DN { get; set; }

        [DataMember]
        public MAImportError ErrorCodeImport { get; set; }

        [DataMember]
        public string ErrorDetail { get; set; }

        [DataMember]
        public string ErrorName { get; set; }

        [DataMember]
        public Guid Identifier { get; private set; }

        [DataMember]
        public ObjectModificationType ObjectModificationType { get; set; }

        [DataMember]
        public string ObjectType { get; set; }

        [DataMember]
        public string RDN { get; private set; }

        [DataMember]
        public IList<AnchorAttribute> AnchorAttributes { get; private set; }

        [DataMember]
        public IList<AttributeChange> AttributeChanges { get; private set; }

        internal void SetObject(CSEntryChange csentry)
        {
            this.AnchorAttributes = csentry.AnchorAttributes;
            this.AttributeChanges = csentry.AttributeChanges;
            this.DN = csentry.DN;
            this.ErrorCodeImport = csentry.ErrorCodeImport;
            this.ErrorDetail = csentry.ErrorDetail;
            this.ErrorName = csentry.ErrorName;
            this.Identifier = csentry.Identifier;
            this.ObjectModificationType = csentry.ObjectModificationType;
            this.ObjectType = csentry.ObjectType;
        }

        internal CSEntryChange GetObject()
        {
            CSEntryChangeDetached csentry = new CSEntryChangeDetached(this.Identifier, this.ObjectModificationType, this.ErrorCodeImport, this.AttributeChanges.Select(t => t.Name).ToList());
            csentry.DN = this.DN;
            csentry.ErrorDetail = this.ErrorDetail;
            csentry.ErrorName = this.ErrorName;
            csentry.ObjectType = this.ObjectType;

            if (this.AnchorAttributes != null)
            {
                foreach (AnchorAttribute attribute in this.AnchorAttributes)
                {
                    csentry.AnchorAttributes.Add(attribute);
                }
            }

            if (this.AttributeChanges != null)
            {
                foreach (AttributeChange attribute in this.AttributeChanges)
                {
                    csentry.AttributeChanges.Add(attribute);
                }
            }

            return csentry;
        }
    }
}