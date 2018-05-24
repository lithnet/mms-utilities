using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;

namespace Lithnet.MetadirectoryServices
{
    /// <summary>
    /// Contains extensions to the <see cref="Microsoft.MetadirectoryServices.CSEntryChange"/> object
    /// </summary>
    public static class CSEntryChangeExtensions
    {
        /// <summary>
        /// Gets the value of the specified anchor attribute if it exists, or the default value of the type if it doesn't
        /// </summary>
        /// <typeparam name="T">The data type of the anchor attribute</typeparam>
        /// <param name="csentry">The CSEntryChange to get the anchor value from</param>
        /// <param name="anchorName">The name of the anchor attribute</param>
        /// <returns>The value of the anchor attribute, or default(T) if the anchor was not present</returns>
        public static T GetAnchorValueOrDefault<T>(this CSEntryChange csentry, string anchorName)
        {
            AnchorAttribute anchor = csentry.AnchorAttributes.FirstOrDefault(t => t.Name == anchorName);

            if (anchor != null)
            {
                return (T)anchor.Value;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Gets the specified anchor attribute
        /// </summary>
        /// <param name="csentry">The CSEntryChange to get the anchor from</param>
        /// <param name="anchorName">The name of the anchor attribute</param>
        /// <returns>The anchor attribute, or null if the anchor was not present</returns>
        public static AnchorAttribute GetAnchorAttribute(this CSEntryChange csentry, string anchorName)
        {
            AnchorAttribute anchor = csentry.AnchorAttributes.FirstOrDefault(t => t.Name == anchorName);

            if (anchor != null)
            {
                return anchor;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Searches the CSEntryChange for an AttributeChange with the name 'DN', and if found, returns the new value
        /// </summary>
        /// <typeparam name="T">The type of the DN attribute</typeparam>
        /// <param name="csentry">The CSEntryChange to evaluate</param>
        /// <returns>The new DN, or default(T) if a DN change was not present</returns>
        public static T GetNewDNOrDefault<T>(this CSEntryChange csentry)
        {
            AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == "DN");

            if (change != null)
            {
                return (T)change.ValueChanges.First(t => t.ModificationType == ValueModificationType.Add).Value;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Converts an enumeration of values to a list of ValueChanges
        /// </summary>
        /// <typeparam name="T">The type of the data in the list</typeparam>
        /// <param name="list">The list of values to convert to ValueChanges</param>
        /// <param name="modificationType">The modification type to apply to the values in the ValueChange</param>
        /// <returns>A list of ValueChange objects</returns>
        public static IList<ValueChange> ToValueChange<T>(this IEnumerable<T> list, ValueModificationType modificationType)
        {
            List<ValueChange> changes = new List<ValueChange>();

            foreach (T value in list)
            {
                switch (modificationType)
                {
                    case ValueModificationType.Add:
                        changes.Add(ValueChange.CreateValueAdd(value));
                        break;

                    case ValueModificationType.Delete:
                        changes.Add(ValueChange.CreateValueDelete(value));
                        break;

                    case ValueModificationType.Unconfigured:
                    default:
                        throw new NotSupportedException("The modification type is unknown or unsupported");
                }
            }

            return changes;
        }

        /// <summary>
        /// Converts an enumeration of values to a list of ValueChanges with the modification type set to 'add'
        /// </summary>
        /// <typeparam name="T">The type of the data in the list</typeparam>
        /// <param name="list">The list of values to convert to ValueChanges</param>
        /// <returns>A list of ValueChange objects with their modification types set to 'add'</returns>
        public static IList<ValueChange> ToValueChangeAdd<T>(this IEnumerable<T> list)
        {
            List<ValueChange> changes = new List<ValueChange>();

            foreach (T value in list)
            {
                changes.Add(ValueChange.CreateValueAdd(value));
            }

            return changes;
        }

        /// <summary>
        /// Converts an enumeration of values to a list of ValueChanges with the modification type set to 'delete'
        /// </summary>
        /// <typeparam name="T">The type of the data in the list</typeparam>
        /// <param name="list">The list of values to convert to ValueChanges</param>
        /// <returns>A list of ValueChange objects with their modification types set to 'delete'</returns>
        public static IList<ValueChange> ToValueChangeDelete<T>(this IEnumerable<T> list)
        {
            List<ValueChange> changes = new List<ValueChange>();

            foreach (T value in list)
            {
                changes.Add(ValueChange.CreateValueDelete(value));
            }

            return changes;
        }

        /// <summary>
        /// Gets a value indicating if the specified attribute is present in the list of attribute changes
        /// </summary>
        /// <param name="csentry">The CSEntryChange to evaluate</param>
        /// <param name="attributeName">The name of the attribute to find</param>
        /// <returns>True if the CSEntryChange contains an AttributeChange for the specified attribute</returns>
        public static bool HasAttributeChange(this CSEntryChange csentry, string attributeName)
        {
            return csentry.AttributeChanges.Any(t => t.Name == attributeName);
        }

        /// <summary>
        /// Gets all the value adds for the specified attribute
        /// </summary>
        /// <typeparam name="T">The data type of the atttribute</typeparam>
        /// <param name="csentry">The CSEntryChange</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>A list of values from the attribute change if it was present. If the attribute change was not present, or contained no 'add' modifications, an empty list is returned</returns>
        public static IList<T> GetValueAdds<T>(this CSEntryChange csentry, string attributeName)
        {
            return csentry.GetValueChanges<T>(attributeName, ValueModificationType.Add);
        }

        /// <summary>
        /// Gets all the value deletes for the specified attribute
        /// </summary>
        /// <typeparam name="T">The data type of the atttribute</typeparam>
        /// <param name="csentry">The CSEntryChange</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>A list of values from the attribute change if it was present. If the attribute change was not present, or contained no 'delete' modifications, an empty list is returned</returns>
        public static IList<T> GetValueDeletes<T>(this CSEntryChange csentry, string attributeName)
        {
            return csentry.GetValueChanges<T>(attributeName, ValueModificationType.Delete);
        }

        /// <summary>
        /// Gets the value changes from the specified attribute 
        /// </summary>
        /// <typeparam name="T">The data type of the atttribute</typeparam>
        /// <param name="csentry">The CSEntryChange</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="modificationType">The modification type of the values to get</param>
        /// <returns>A list of values from the attribute change if it was present. If the attribute change was not present, or contained no modifications matching the type specified by the <paramref name="modificationType"/> parameter, an empty list is returned</returns>
        public static IList<T> GetValueChanges<T>(this CSEntryChange csentry, string attributeName, ValueModificationType modificationType)
        {
            AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == attributeName);

            if (change == null)
            {
                return new List<T>();
            }

            return change.GetValueChanges<T>(modificationType);
        }

        /// <summary>
        /// Gets the value add for a single-valued attribute
        /// </summary>
        /// <typeparam name="T">The data type of the atttribute</typeparam>
        /// <param name="csentry">The CSEntryChange</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>The value of attribute if present, or default(T) if it is not. If the AttributeChange contains multiple value adds, an exceptioin is thrown</returns>
        public static T GetValueAdd<T>(this CSEntryChange csentry, string attributeName)
        {
            AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == attributeName);

            if (change == null)
            {
                return default(T);
            }

            return change.GetValueAdd<T>();
        }

        /// <summary>
        /// Creates an AttributeChange with a single value addition
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="value">The value to add</param>
        public static void CreateAttributeAdd(this CSEntryChange csentry, string attributeName, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attributeName, value));
        }

        /// <summary>
        /// Creates an AttributeChange with a mutli-valued addition
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="values">The values to add</param>
        public static void CreateAttributeAdd(this CSEntryChange csentry, string attributeName, IList<object> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attributeName, values));
        }

        /// <summary>
        /// Creates an AttributeChange with a modification type of delete
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="attributeName">The name of the attribute</param>
        public static void CreateAttributeDelete(this CSEntryChange csentry, string attributeName)
        {
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(attributeName));
        }

        /// <summary>
        /// Creates an AttributeChange with a single value replacement
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="value">The new value for the attribute</param>
        public static void CreateAttributeReplace(this CSEntryChange csentry, string attributeName, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeReplace(attributeName, value));
        }

        /// <summary>
        /// Creates an AttributeChange with a mutli-valued replacement
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="values">The new values for the attribute</param>
        public static void CreateAttributeReplace(this CSEntryChange csentry, string attributeName, IList<object> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Count == 0)
            {
                csentry.CreateAttributeDelete(attributeName);
                return;
            }

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeReplace(attributeName, values));
        }

        /// <summary>
        /// Creates an AttributeChange of type Update, with ValueChanges created for the specified value adds and deletes
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="valueAdds">The values to add in the update operation</param>
        /// <param name="valueDeletes">The values to delete in the update operation</param>
        public static void CreateAttributeUpdate(this CSEntryChange csentry, string attributeName, IList<object> valueAdds, IList<object> valueDeletes)
        {
            List<ValueChange> valueChanges = new List<ValueChange>();

            if (valueAdds != null)
            {
                foreach (object value in valueAdds)
                {
                    valueChanges.Add(ValueChange.CreateValueAdd(value));
                }
            }

            if (valueDeletes != null)
            {
                foreach (object value in valueDeletes)
                {
                    valueChanges.Add(ValueChange.CreateValueDelete(value));
                }
            }

            if (valueChanges.Count > 0)
            {
                csentry.CreateAttributeUpdate(attributeName, valueChanges);
            }
        }

        /// <summary>
        /// Creates an AttributeChange of type Update using the specified value changes
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="valueChanges">The values changes to add to the update operation</param>
        public static void CreateAttributeUpdate(this CSEntryChange csentry, string attributeName, IList<ValueChange> valueChanges)
        {
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate(attributeName, valueChanges));
        }

        /// <summary>
        /// Creates an AttributeChange of the specified type
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="modificationType">The type of modification to apply to the attribute</param>
        /// <param name="values">The values to apply to the modification operation</param>
        public static void CreateAttributeChange(this CSEntryChange csentry, string attributeName, AttributeModificationType modificationType, IList<object> values)
        {
            switch (modificationType)
            {
                case AttributeModificationType.Add:
                    if (values.Count > 0)
                    {
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attributeName, values));
                    }

                    break;

                case AttributeModificationType.Delete:
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(attributeName));
                    break;

                case AttributeModificationType.Replace:
                    if (values.Count > 0)
                    {
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeReplace(attributeName, values));
                    }
                    else
                    {
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(attributeName));
                    }

                    break;

                case AttributeModificationType.Update:
                    throw new NotSupportedException("Update operations are not supported by this method");

                case AttributeModificationType.Unconfigured:
                default:
                    throw new InvalidOperationException("Unknown modification type");
            }
        }

        /// <summary>
        /// Creates an AttributeChange of the specified type
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="modificationType">The type of modification to apply to the attribute</param>
        /// <param name="values">The value changes to apply to the modification operation</param>
        public static void CreateAttributeChange(this CSEntryChange csentry, string attributeName, AttributeModificationType modificationType, IList<ValueChange> values)
        {
            switch (modificationType)
            {
                case AttributeModificationType.Delete:
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(attributeName));
                    break;

                case AttributeModificationType.Add:
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attributeName, values));
                    break;

                case AttributeModificationType.Replace:
                    if (values == null || values.Count == 0)
                    {
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(attributeName));
                    }
                    else
                    {
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeReplace(attributeName, values));
                    }
                    break;

                case AttributeModificationType.Update:
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate(attributeName, values));
                    break;

                case AttributeModificationType.Unconfigured:
                default:
                    throw new InvalidOperationException("Unknown modification type");
            }
        }

        /// <summary>
        /// Creates an AttributeChange of the specified type
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="modificationType">The type of modification to apply to the attribute</param>
        /// <param name="value">The value to apply to the modification operation</param>
        public static void CreateAttributeChange(this CSEntryChange csentry, string attributeName, AttributeModificationType modificationType, object value)
        {
            switch (modificationType)
            {
                case AttributeModificationType.Add:
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attributeName, value));
                    break;

                case AttributeModificationType.Delete:
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(attributeName));
                    break;

                case AttributeModificationType.Replace:
                    if (value == null)
                    {
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeReplace(attributeName, value));
                    }
                    else
                    {
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(attributeName));
                    }
                    break;

                case AttributeModificationType.Update:
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate(attributeName, value));
                    break;

                case AttributeModificationType.Unconfigured:
                default:
                    throw new InvalidOperationException("Unknown modification type");
            }
        }

        /// <summary>
        /// Creates an AttributeChange of the specified type
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="modificationType">The type of modification to apply to the attribute</param>
        /// <param name="value">The value to apply to the modification operation</param>
        /// <typeparam name="T">The type of data</typeparam>
        public static void CreateAttributeChange<T>(this CSEntryChange csentry, string attributeName, AttributeModificationType modificationType, Nullable<T> value) where T : struct
        {
            switch (modificationType)
            {
                case AttributeModificationType.Add:
                    if (value != null)
                    {
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attributeName, value.Value));
                    }
                    break;

                case AttributeModificationType.Delete:
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(attributeName));
                    break;

                case AttributeModificationType.Replace:
                    if (value == null)
                    {
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeReplace(attributeName, value));
                    }
                    else
                    {
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(attributeName));
                    }
                    break;

                case AttributeModificationType.Update:
                    if (value != null)
                    {
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate(attributeName, value));
                    }
                    break;

                case AttributeModificationType.Unconfigured:
                default:
                    throw new InvalidOperationException("Unknown modification type");
            }
        }

        /// <summary>
        /// Creates an AttributeChange of the specified type, provided that the attribute is present in the provided schema type
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="type">The schema type of the object class of the CSEntryChange</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="modificationType">The type of modification to apply to the attribute</param>
        /// <param name="values">The values to apply to the modification operation</param>
        public static void CreateAttributeChangeIfInSchema(this CSEntryChange csentry, SchemaType type, string attributeName, AttributeModificationType modificationType, IList<object> values)
        {
            if (type.HasAttribute(attributeName) && values != null && values.Count > 0)
            {
                csentry.CreateAttributeChange(attributeName, modificationType, values);
            }
        }

        /// <summary>
        /// Creates an AttributeChange of the specified type, provided that the attribute is present in the provided schema type
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="type">The schema type of the object class of the CSEntryChange</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="modificationType">The type of modification to apply to the attribute</param>
        /// <param name="valueChanges">The value changes to apply to the modification operation</param>
        public static void CreateAttributeChangeIfInSchema(this CSEntryChange csentry, SchemaType type, string attributeName, AttributeModificationType modificationType, IList<ValueChange> valueChanges)
        {
            if (type.HasAttribute(attributeName) && valueChanges != null && valueChanges.Count > 0)
            {
                csentry.CreateAttributeChange(attributeName,  modificationType, valueChanges);
            }
        }

        /// <summary>
        /// Creates an AttributeChange of the specified type, provided that the attribute is present in the provided schema type
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="type">The schema type of the object class of the CSEntryChange</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="modificationType">The type of modification to apply to the attribute</param>
        /// <param name="value">The value to apply to the modification operation</param>
        public static void CreateAttributeChangeIfInSchema(this CSEntryChange csentry, SchemaType type, string attributeName, AttributeModificationType modificationType, bool value)
        {
            csentry.CreateAttributeChangeIfInSchemaInternal(type, attributeName, modificationType, value);
        }

        /// <summary>
        /// Creates an AttributeChange of the specified type, provided that the attribute is present in the provided schema type
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="type">The schema type of the object class of the CSEntryChange</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="modificationType">The type of modification to apply to the attribute</param>
        /// <param name="value">The value to apply to the modification operation</param>
        public static void CreateAttributeChangeIfInSchema(this CSEntryChange csentry, SchemaType type, string attributeName, AttributeModificationType modificationType, bool? value)
        {
            if (value != null && value.HasValue)
            {
                csentry.CreateAttributeChangeIfInSchemaInternal(type, attributeName, modificationType, value);
            }
        }

        /// <summary>
        /// Creates an AttributeChange of the specified type, provided that the attribute is present in the provided schema type
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="type">The schema type of the object class of the CSEntryChange</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="modificationType">The type of modification to apply to the attribute</param>
        /// <param name="value">The value to apply to the modification operation</param>
        public static void CreateAttributeChangeIfInSchema(this CSEntryChange csentry, SchemaType type, string attributeName, AttributeModificationType modificationType, long value)
        {
            csentry.CreateAttributeChangeIfInSchemaInternal(type, attributeName, modificationType, value);
        }

        /// <summary>
        /// Creates an AttributeChange of the specified type, provided that the attribute is present in the provided schema type
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="type">The schema type of the object class of the CSEntryChange</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="modificationType">The type of modification to apply to the attribute</param>
        /// <param name="value">The value to apply to the modification operation</param>
        public static void CreateAttributeChangeIfInSchema(this CSEntryChange csentry, SchemaType type, string attributeName, AttributeModificationType modificationType, long? value)
        {
            if (value != null && value.HasValue)
            {
                csentry.CreateAttributeChangeIfInSchemaInternal(type, attributeName, modificationType, value);
            }
        }

        /// <summary>
        /// Creates an AttributeChange of the specified type, provided that the attribute is present in the provided schema type
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="type">The schema type of the object class of the CSEntryChange</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="modificationType">The type of modification to apply to the attribute</param>
        /// <param name="value">The value to apply to the modification operation</param>
        public static void CreateAttributeChangeIfInSchema(this CSEntryChange csentry, SchemaType type, string attributeName, AttributeModificationType modificationType, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                csentry.CreateAttributeChangeIfInSchemaInternal(type, attributeName, modificationType, value);
            }
        }

        /// <summary>
        /// Creates an AttributeChange of the specified type, provided that the attribute is present in the provided schema type
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add the AttributeChange to</param>
        /// <param name="type">The schema type of the object class of the CSEntryChange</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="modificationType">The type of modification to apply to the attribute</param>
        /// <param name="value">The value to apply to the modification operation</param>
        public static void CreateAttributeChangeIfInSchema(this CSEntryChange csentry, SchemaType type, string attributeName, AttributeModificationType modificationType, byte[] value)
        {
            csentry.CreateAttributeChangeIfInSchemaInternal(type, attributeName, modificationType, value);
        }

        private static void CreateAttributeChangeIfInSchemaInternal(this CSEntryChange csentry, SchemaType type, string attributeName, AttributeModificationType modificationType, object value)
        {
            if (type.HasAttribute(attributeName))
            {
                csentry.CreateAttributeChange(attributeName, modificationType, value);
            }
        }

        /// <summary>
        /// Creates a text summary of the CSEntryChange and its contents
        /// </summary>
        /// <param name="csentry">The CSEntryChange to generate the summary for</param>
        /// <returns>A string containing the detailed information stored in the CSEntryChange</returns>
        public static string ToDetailString(this CSEntryChange csentry)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("CSEntryChange: " + csentry.Identifier.ToString());
            sb.AppendLine("DN: " + csentry.DN);
            sb.AppendLine("Object class: " + csentry.ObjectType);
            sb.AppendLine("Object modification type: " + csentry.ObjectModificationType.ToString());
            sb.AppendLine("Import error code: " + csentry.ErrorCodeImport.ToString());
            sb.AppendLine("Error name: " + csentry.ErrorName);
            sb.AppendLine("Error detail: " + csentry.ErrorDetail);

            sb.AppendLine("Anchor Attributes");
            foreach (var anchorAttribute in csentry.AnchorAttributes)
            {
                sb.AppendFormat("  Attribute: {0}, Modification type: {1}, Value: {2}\n", anchorAttribute.Name, anchorAttribute.DataType.ToString(), anchorAttribute.Value.ToSmartString());
            }

            sb.AppendLine("Attribute Changes");
            foreach (var attributeChange in csentry.AttributeChanges)
            {
                sb.AppendFormat("  Attribute: {0}, Type: {1}, Multivalued: {2}, Modification Type: {3}\n", attributeChange.Name, attributeChange.DataType.ToString(), attributeChange.IsMultiValued.ToString(), attributeChange.ModificationType.ToString());
                foreach (var valueChange in attributeChange.ValueChanges)
                {
                    sb.AppendFormat("      {0}: {1}\n", valueChange.ModificationType.ToString(), valueChange.Value.ToSmartString());
                }
            }

            return sb.ToString();
        }
    }
}