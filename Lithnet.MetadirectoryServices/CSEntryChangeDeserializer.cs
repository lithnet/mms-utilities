using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.MetadirectoryServices;
using System.Xml.Linq;
using System.IO;

namespace Lithnet.MetadirectoryServices
{
    /// <summary>
    /// Contains methods for deserializing XML representations of <see cref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChange</see> objects created by the <see cref="Lithnet.MetadirectoryServices.CSEntryChangeSerializer"/> class
    /// </summary>
    public static class CSEntryChangeDeserializer
    {
        /// <summary>
        /// Deserializes a file into a list of <see cref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChange</see> objects
        /// </summary>
        /// <param name="file">The file of <see ref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChanges</see> to deserialize</param>
        /// <returns>A list of <see cref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChanges</see></returns>
        public static IList<CSEntryChange> Deserialize(string file)
        {
            List<CSEntryChange> csentries = new List<CSEntryChange>();

            using (StreamReader r = new StreamReader(file))
            {
                using (XmlReader reader = XmlReader.Create(r))
                {
                    var doc = XDocument.Load(reader);

                    foreach (var node in doc.Root.Elements("object-change"))
                    {
                        CSEntryChange csentry = CSEntryChangeDeserializer.Deserialize(node);
                        csentries.Add(csentry);
                    }

                    reader.Close();
                }
                return csentries;
            }
        }

        /// <summary>
        /// Deserializes an XML representation of a <see cref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChange</see>
        /// </summary>
        /// <param name="element">An <c ref="System.Linq.Xml.XElement">XElement</c> containing a cs-entry element</param>
        /// <returns>a <see cref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChange</see> object</returns>
        public static CSEntryChange Deserialize(XElement element)
        {
            CSEntryChange csentry = CSEntryChange.Create();
            CSEntryChangeDeserializer.Deserialize(element, csentry);
            return csentry;
        }

        /// <summary>
        /// Deserializes an XML representation of a <see cref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChange</see> into an existing object
        /// </summary>
        /// <param name="element">An <c ref="System.Linq.Xml.XElement">XElement</c> containing a cs-entry element</param>
        /// <param name="csentry">The <see cref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChange</see> to populate</param>
        private static void Deserialize(XElement element, CSEntryChange csentry)
        {
            if (element.Name.LocalName != "object-change")
            {
                throw new ArgumentException("The XML node provided was not an <object-change> node", "node");
            }

            foreach (var child in element.Elements())
            {
                if (child.Name.LocalName == "modification-type")
                {
                    ObjectModificationType modificationType;
                    string modificationTypeString = (string)child;

                    if (Enum.TryParse<ObjectModificationType>(modificationTypeString, out modificationType))
                    {
                        csentry.ObjectModificationType = modificationType;
                    }
                    else
                    {
                        throw new InvalidCastException(string.Format("Cannot convert '{0}' to type {1}", modificationTypeString, typeof(ObjectModificationType).Name));
                    }
                }
                else if (child.Name.LocalName == "object-class")
                {
                    csentry.ObjectType = (string)child;
                }
                else if (child.Name.LocalName == "dn")
                {
                    csentry.DN = (string)child;
                }
                else if (child.Name.LocalName == "attribute-changes")
                {
                    XmlReadAttributeChangesNode(child, csentry);
                }
                else if (child.Name.LocalName == "anchor-attributes")
                {
                    XmlReadAnchorAttributesNode(child, csentry);
                }
            }
        }

        /// <summary>
        /// Reads the <![CDATA[<attribute-changes>]]> node of the XML representation of the <see cref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChange</see>
        /// </summary>
        /// <param name="element">An <c ref="System.Linq.Xml.XElement">XElement</c> containing an <![CDATA[<attribute-changes>]]> element</param>
        /// <param name="csentry">The <see cref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChange</see> to populate</param>
        private static void XmlReadAttributeChangesNode(XElement element, CSEntryChange csentry)
        {
            foreach (var child in element.Elements().Where(t => t.Name.LocalName == "attribute-change"))
            {
                CSEntryChangeDeserializer.XmlReadAttributeChangeNode(child, csentry);
            }
        }

        /// <summary>
        /// Reads the <![CDATA[<anchor-attributes>]]> node of the XML representation of the <see cref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChange</see>
        /// </summary>
        /// <param name="element">An <c ref="System.Linq.Xml.XElement">XElement</c> containing an <![CDATA[<ancchor-attributes>]]> element</param>
        /// <param name="csentry">The <see cref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChange</see> to populate</param>
        private static void XmlReadAnchorAttributesNode(XElement element, CSEntryChange csentry)
        {
            foreach (var child in element.Elements().Where(t => t.Name.LocalName == "anchor-attribute"))
            {
                CSEntryChangeDeserializer.XmlReadAnchorAttributeNode(child, csentry);
            }
        }

        /// <summary>
        /// Reads the <![CDATA[<anchor-attribute>]]> node of the XML representation of the <see cref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChange</see>
        /// </summary>
        /// <param name="element">An <c ref="System.Linq.Xml.XElement">XElement</c> containing an <![CDATA[<ancchor-attribute>]]> element</param>
        /// <param name="csentry">The <see cref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChange</see> to populate</param>
        private static void XmlReadAnchorAttributeNode(XElement element, CSEntryChange csentry)
        {
            string name = null;
            string value = null;

            foreach (var child in element.Elements())
            {
                if (child.Name.LocalName == "name")
                {
                    name = (string)child;
                }
                else if (child.Name.LocalName == "value")
                {
                    value = (string)child;
                }
            }

            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(value))
            {
                return;
            }


            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The name and value elements of an <anchor-attribute> must not be null");
            }

            AnchorAttribute anchor = AnchorAttribute.Create(name, value);

            csentry.AnchorAttributes.Add(anchor);
        }

        /// <summary>
        /// Reads the <![CDATA[<attribute-change]]> node of the XML representation of the <see cref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChange</see>
        /// </summary>
        /// <param name="element">An <c ref="System.Linq.Xml.XElement">XElement</c> containing an <![CDATA[<attribute-change>]]> element</param>
        /// <param name="csentry">The <see cref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChange</see> to populate</param>
        private static void XmlReadAttributeChangeNode(XElement element, CSEntryChange csentry)
        {
            string name = null;
            AttributeModificationType modificationType = AttributeModificationType.Unconfigured;
            AttributeType dataType = AttributeType.Undefined;
            List<ValueChange> valueChanges = null;
            AttributeChange attributeChange = null;

            foreach (var child in element.Elements())
            {
                if (child.Name.LocalName == "name")
                {
                    name = (string)child;
                }
                else if (child.Name.LocalName == "modification-type")
                {
                    string modificationTypeString = (string)child;

                    if (!Enum.TryParse<AttributeModificationType>(modificationTypeString, out modificationType))
                    {
                        throw new InvalidCastException(string.Format("Cannot convert '{0}' to type {1}", modificationTypeString, typeof(AttributeModificationType).Name));
                    }
                }
                else if (child.Name.LocalName == "data-type")
                {
                    string dataTypeString = (string)child;

                    if (!Enum.TryParse<AttributeType>(dataTypeString, out dataType))
                    {
                        throw new InvalidCastException(string.Format("Cannot convert '{0}' to type '{1}'", dataTypeString, typeof(AttributeType).Name));
                    }
                }
                else if (child.Name.LocalName == "value-changes")
                {
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        throw new ArgumentException("The attribute name must appear first in the list of <attribute-change> elements");
                    }

                    if (dataType == AttributeType.Undefined)
                    {
                        dataType = AttributeType.String;
                    }

                    valueChanges = CSEntryChangeDeserializer.XmlReadValueChangesNode(child, dataType);
                }
            }

            switch (modificationType)
            {
                case AttributeModificationType.Add:
                    if (valueChanges.Count == 0)
                    {
                        // discard attribute change with no values
                        return;
                    }

                    attributeChange = AttributeChange.CreateAttributeAdd(name, valueChanges.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList());
                    break;

                case AttributeModificationType.Replace:
                    if (valueChanges.Count == 0)
                    {
                        // discard attribute change with no values
                        return;
                        //throw new ArgumentException("The attribute replace in the CSEntry provided no values");
                    }

                    attributeChange = AttributeChange.CreateAttributeReplace(name, valueChanges.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList());
                    break;

                case AttributeModificationType.Delete:
                    attributeChange = AttributeChange.CreateAttributeDelete(name);
                    break;

                case AttributeModificationType.Update:
                    if (valueChanges.Count == 0)
                    {
                        // discard attribute change with no values
                        return;
                        //throw new ArgumentException("The attribute update in the CSEntry provided no values");
                    }

                    attributeChange = AttributeChange.CreateAttributeUpdate(name, valueChanges);

                    break;

                case AttributeModificationType.Unconfigured:
                default:
                    throw new NotSupportedException(string.Format("The modification type is not supported {0}", modificationType));
            }

            csentry.AttributeChanges.Add(attributeChange);

        }

        /// <summary>
        /// Reads the <![CDATA[<value-changes>]]> node of the XML representation of the <see cref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChange</see>
        /// </summary>
        /// <param name="element">An <c ref="System.Linq.Xml.XElement">XElement</c> containing an <![CDATA[<value-changes>]]> element</param>
        /// <param name="attributeType">The type of attribute to read</param>
        /// <returns>A list of ValueChanges</returns>
        private static List<ValueChange> XmlReadValueChangesNode(XElement element, AttributeType attributeType)
        {
            List<ValueChange> valueChanges = new List<ValueChange>();

            foreach (var child in element.Elements().Where(t => t.Name.LocalName == "value-change"))
            {
                ValueChange change = CSEntryChangeDeserializer.XmlReadValueChangeNode(child, attributeType);
                if (change != null)
                {
                    valueChanges.Add(change);
                }
            }

            return valueChanges;
        }

        /// <summary>
        /// Reads the <![CDATA[<value-change>]]> node of the XML representation of the <see cref="Microsoft.MetadirectoryServices.CSEntryChange">CSEntryChange</see>
        /// </summary>
        /// <param name="element">An <c ref="System.Linq.Xml.XElement">XElement</c> containing an <![CDATA[<value-change>]]> element</param>
        /// <param name="attributeType">The type of attribute to read</param>
        /// <returns>A ValueChange object</returns>
        private static ValueChange XmlReadValueChangeNode(XElement element, AttributeType attributeType)
        {
            ValueModificationType modificationType = ValueModificationType.Unconfigured;
            string value = null;

            foreach (var child in element.Elements())
            {
                if (child.Name.LocalName == "modification-type")
                {
                    string modificationTypeString = (string)child;

                    if (!Enum.TryParse<ValueModificationType>(modificationTypeString, out modificationType))
                    {
                        throw new InvalidCastException(string.Format("Cannot convert '{0}' to type {1}", modificationTypeString, typeof(ValueModificationType).Name));
                    }
                }
                else if (child.Name.LocalName == "value")
                {
                    value = (string)child;
                }
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The value-change value was blank");
            }

            switch (modificationType)
            {
                case ValueModificationType.Add:
                    return ValueChange.CreateValueAdd(TypeConverter.ConvertData(value, attributeType));

                case ValueModificationType.Delete:
                    return ValueChange.CreateValueDelete(TypeConverter.ConvertData(value, attributeType));

                case ValueModificationType.Unconfigured:
                default:
                    throw new NotSupportedException(string.Format("The modification type is not supported {0}", modificationType));
            }
        }
    }
}
