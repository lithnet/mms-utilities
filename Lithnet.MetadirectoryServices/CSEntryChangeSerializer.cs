using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using System.Xml;

namespace Lithnet.MetadirectoryServices
{
    /// <summary>
    /// Contains methods for serializing <see cref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChange</see> objects into XML. Objects serialized by this class can be deserialized with the <see cref="Lithnet.MetadirectoryServices.CSEntryChangeDeserializer"/> class
    /// </summary>
    public static class CSEntryChangeSerializer
    {
        /// <summary>
        /// Serializes the collection of CSEntryChange objects into the specified file
        /// </summary>
        /// <param name="csentries">The collection of CSEntryChanges to serialize</param>
        /// <param name="file">The name of the file to serialize the objects to. If this file exists, it will be overwritten</param>
        /// <param name="schema">The current metadirectory services schema. This is required to ensure that attributes are serialized as the correct data type</param>
        public static void Serialize(IEnumerable<CSEntryChange> csentries, string file, Schema schema)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.NewLineChars = Environment.NewLine;
            using (XmlWriter writer = XmlWriter.Create(file, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("cs-entry-changes");

                foreach (CSEntryChange obj in csentries)
                {
                    CSEntryChangeSerializer.Serialize(obj, writer, schema);
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }
        }

        /// <summary>
        /// Serializes the specified CSEntryChange using an existing XmlWriter
        /// </summary>
        /// <param name="csentry">The CSEntryChange to serialize</param>
        /// <param name="writer">An open XmlWriter to write the serialized data to</param>
        /// <param name="schema">The current metadirectory services schema. This is required to ensure that attributes are serialized as the correct data type</param>
        public static void Serialize(CSEntryChange csentry, XmlWriter writer, Schema schema)
        {
            writer.WriteStartElement("object-change");
            writer.WriteElementString("modification-type", csentry.ObjectModificationType.ToString());
            writer.WriteElementString("dn", csentry.DN);
            writer.WriteElementString("object-class", csentry.ObjectType);

            if (csentry.AnchorAttributes.Count > 0)
            {
                writer.WriteStartElement("anchor-attributes");
                foreach (AnchorAttribute anchor in csentry.AnchorAttributes)
                {
                    writer.WriteStartElement("anchor-attribute");
                    writer.WriteElementString("name", anchor.Name);
                    writer.WriteElementString("value", anchor.Value.ToSmartStringOrNull());
                    writer.WriteEndElement(); // </anchor-attribute>
                }
                writer.WriteEndElement(); // </anchor-attributes>
            }

            switch (csentry.ObjectModificationType)
            {
                case ObjectModificationType.Add:
                case ObjectModificationType.Replace:
                case ObjectModificationType.Update:
                    XmlWriteAttributeChangesNode(csentry, writer, schema);
                    break;

                case ObjectModificationType.Delete:
                    break;

                case ObjectModificationType.None:
                case ObjectModificationType.Unconfigured:
                default:
                    throw new NotSupportedException(string.Format("Unsupported modification type {0}", csentry.ObjectModificationType));
            }

            writer.WriteEndElement(); // </object-change>
        }

        /// <summary>
        /// Writes the <![CDATA[<attribute-changes>]]> node 
        /// </summary>
        /// <param name="csentry">The CSEntryChange to serialize</param>
        /// <param name="writer">An open XmlWriter to write the serialized data to</param>
        /// <param name="schema">The current metadirectory services schema</param>
        private static void XmlWriteAttributeChangesNode(CSEntryChange csentry, XmlWriter writer, Schema schema)
        {
            writer.WriteStartElement("attribute-changes");

            foreach (AttributeChange attributeChange in csentry.AttributeChanges)
            {
                writer.WriteStartElement("attribute-change");
                writer.WriteElementString("name", attributeChange.Name);
                writer.WriteElementString("modification-type", attributeChange.ModificationType.ToString());

                if (attributeChange.DataType == AttributeType.Undefined)
                {
                    SchemaType type = schema.Types[csentry.ObjectType];
                    SchemaAttribute attribute = type.Attributes.FirstOrDefault(t => t.Name == attributeChange.Name);

                    if (attribute == null)
                    {
                        writer.WriteElementString("data-type", AttributeType.Undefined.ToString());
                    }
                    else
                    {
                        writer.WriteElementString("data-type", attribute.DataType.ToString());
                    }
                }
                else
                {
                    writer.WriteElementString("data-type", attributeChange.DataType.ToString());
                }

                writer.WriteStartElement("value-changes");

                foreach (ValueChange valueChange in attributeChange.ValueChanges)
                {
                    writer.WriteStartElement("value-change");
                    writer.WriteElementString("modification-type", valueChange.ModificationType.ToString());
                    writer.WriteElementString("value", valueChange.Value.ToSmartString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement(); // </value-changes>
                writer.WriteEndElement(); // </attribute-change>
            }

            writer.WriteEndElement(); // </attribute-changes>
        }
    }
}
