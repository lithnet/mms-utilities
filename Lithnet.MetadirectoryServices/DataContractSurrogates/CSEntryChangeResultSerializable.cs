using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using Microsoft.MetadirectoryServices;

namespace Lithnet.MetadirectoryServices
{
    [DataContract]
    public class CSEntryChangeResultSerializable
    {
        [DataMember]
        public IList<AttributeChange> AnchorChanges { get; set; }

        [DataMember]
        public MAExportError ErrorCode { get; set; }

        [DataMember]
        public string ErrorDetail { get; set; }

        [DataMember]
        public string ErrorName { get; set; }

        [DataMember]
        public Guid Identifier { get; set; }

        internal CSEntryChangeResultSerializable(CSEntryChangeResult change)
        {
            this.SetObject(change);
        }

        internal void SetObject(CSEntryChangeResult change)
        {
            this.AnchorChanges = change.AnchorAttributes;
            this.ErrorCode = change.ErrorCode;
            this.ErrorDetail = change.ErrorDetail;
            this.ErrorName = change.ErrorName;
            this.Identifier = change.Identifier;
        }

        internal CSEntryChangeResult GetObject()
        {
            return CSEntryChangeResult.Create(this.Identifier, this.AnchorChanges, this.ErrorCode, this.ErrorName, this.ErrorDetail);
        }
    }
}