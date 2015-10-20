using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;

namespace Lithnet.MetadirectoryServices
{
    public static class CSEntryChangeExtensions
    {
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

        public static T GetAnchorValueOrDefault<T>(this CSEntryChange csentry)
        {
            AnchorAttribute anchor = csentry.AnchorAttributes.SingleOrDefault();

            if (anchor != null)
            {
                return (T)anchor.Value;
            }
            else
            {
                return default(T);
            }
        }

        public static AnchorAttribute GetAnchorAttribute(this CSEntryChange csentry)
        {
            AnchorAttribute anchor = csentry.AnchorAttributes.SingleOrDefault();

            if (anchor != null)
            {
                return anchor;
            }
            else
            {
                return null;
            }
        }

        public static T GetNewDN<T>(this CSEntryChange csentry)
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

        public static IList<ValueChange> ToValueChange(this IEnumerable<string> list, ValueModificationType modificationType)
        {
            List<ValueChange> changes = new List<ValueChange>();

            foreach (string value in list)
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

        public static IList<ValueChange> ToValueChangeAdd(this IEnumerable<string> list)
        {
            List<ValueChange> changes = new List<ValueChange>();

            foreach (string value in list)
            {
                changes.Add(ValueChange.CreateValueAdd(value));
            }

            return changes;
        }

        public static IList<ValueChange> ToValueChangeDelete(this IEnumerable<string> list)
        {
            List<ValueChange> changes = new List<ValueChange>();

            foreach (string value in list)
            {
                changes.Add(ValueChange.CreateValueDelete(value));
            }

            return changes;
        }

        public static void CreateAttributeChange(this CSEntryChange csentry, SchemaType type, string attributeName, object value, AttributeModificationType updateType)
        {
            if (type.HasAttribute(attributeName))
            {
                if (value != null)
                {
                    csentry.CreateAttributeChange(updateType, attributeName, value);
                }
            }
        }

        public static void CreateAttributeChange(this CSEntryChange csentry, SchemaType type, string attributeName, string value, AttributeModificationType updateType)
        {
            if (type.HasAttribute(attributeName))
            {
                if (!string.IsNullOrEmpty(value))
                {
                    csentry.CreateAttributeChange(updateType, attributeName, value);
                }
            }
        }

        public static void CreateAttributeAdd(this CSEntryChange csentry, string attributeName, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attributeName, value));
        }

        public static void CreateAttributeAdd(this CSEntryChange csentry, string attributeName, IList<object> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attributeName, values));
        }

        public static void CreateAttributeDelete(this CSEntryChange csentry, string attributeName)
        {
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(attributeName));
        }

        public static void CreateAttributeReplace(this CSEntryChange csentry, string attributeName, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeReplace(attributeName, value));
        }

        public static void CreateAttributeReplace(this CSEntryChange csentry, string attributeName, IList<object> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            if (values.Count == 0)
            {
                csentry.CreateAttributeDelete(attributeName);
                return;
            }

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeReplace(attributeName, values));
        }

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

        public static void CreateAttributeUpdate(this CSEntryChange csentry, string attributeName, IList<ValueChange> valueChanges)
        {
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate(attributeName, valueChanges));
        }

        public static void CreateAttributeChange(this CSEntryChange csentry, AttributeModificationType updateType, string attributeName, IList<object> values)
        {
            switch (updateType)
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

                    break;

                case AttributeModificationType.Update:
                    throw new NotSupportedException("Update operations are not supported by this method");

                case AttributeModificationType.Unconfigured:
                default:
                    throw new InvalidOperationException("Unknown modification type");
            }
        }

        public static void CreateAttributeChange(this CSEntryChange csentry, AttributeModificationType updateType, string attributeName, IList<ValueChange> values)
        {
            if (values.Count == 0)
            {
                return;
            }

            if (csentry.AttributeChanges.Contains(attributeName))
            {
                foreach (var valuechange in values)
                {
                    csentry.AttributeChanges[attributeName].ValueChanges.Add(valuechange);
                }

                return;
            }

            switch (updateType)
            {
                case AttributeModificationType.Delete:
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(attributeName));
                    break;

                case AttributeModificationType.Add:
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attributeName, values));
                    break;

                case AttributeModificationType.Replace:
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeReplace(attributeName, values));
                    break;

                case AttributeModificationType.Update:
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate(attributeName, values));
                    break;

                case AttributeModificationType.Unconfigured:
                default:
                    throw new InvalidOperationException("Unknown modification type");
            }
        }

        public static void CreateAttributeChange(this CSEntryChange csentry, AttributeModificationType updateType, string attributeName, object value)
        {
            switch (updateType)
            {
                case AttributeModificationType.Add:
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attributeName, value));
                    break;

                case AttributeModificationType.Delete:
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(attributeName));
                    break;

                case AttributeModificationType.Replace:
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeReplace(attributeName, value));
                    break;

                case AttributeModificationType.Update:
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate(attributeName, value));
                    break;

                case AttributeModificationType.Unconfigured:
                default:
                    throw new InvalidOperationException("Unknown modification type");
            }
        }

        public static void CreateAttributeChange(this CSEntryChange csentry, SchemaType type, string attributeName, IList<string> value, AttributeModificationType updateType)
        {
            if (type.HasAttribute(attributeName))
            {
                if (value != null && value.Count > 0)
                {
                    csentry.CreateAttributeChange(updateType, attributeName, value.Cast<object>().ToList());
                }
            }
        }

        public static void CreateAttributeChange(this CSEntryChange csentry, SchemaType type, string attributeName, int? value, AttributeModificationType updateType)
        {
            if (type.HasAttribute(attributeName))
            {
                if (value != null && value.HasValue)
                {
                    csentry.CreateAttributeChange(updateType, attributeName, value.Value);
                }
            }
        }

        public static void CreateAttributeChange(this CSEntryChange csentry, SchemaType type, string attributeName, bool? value, AttributeModificationType updateType)
        {
            if (type.HasAttribute(attributeName))
            {
                if (value != null && value.HasValue)
                {
                    csentry.CreateAttributeChange(updateType, attributeName, value.Value);
                }
            }
        }

        public static void CreateAttributeChange(this CSEntryChange csentry, SchemaType type, string attributeName, DateTime? value, AttributeModificationType updateType)
        {
            if (type.HasAttribute(attributeName))
            {
                if (value != null && value.HasValue)
                {
                    csentry.CreateAttributeChange(updateType, attributeName, value.Value.ToResourceManagementServiceDateFormat(true));
                }
            }
        }

        public static bool HasAttributeChange(this CSEntryChange csentry, string attributeName)
        {
            return csentry.AttributeChanges.Any(t => t.Name == attributeName);
        }

        public static IList<T> GetValueAdds<T>(this CSEntryChange csentry, string attributeName)
        {
            return csentry.GetValueChanges<T>(attributeName, ValueModificationType.Add);
        }

        public static IList<T> GetValueDeletes<T>(this CSEntryChange csentry, string attributeName)
        {
            return csentry.GetValueChanges<T>(attributeName, ValueModificationType.Delete);
        }

        public static IList<T> GetValueChanges<T>(this CSEntryChange csentry, string attributeName, ValueModificationType modificationType)
        {
            AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == attributeName);

            if (change == null)
            {
                return new List<T>();
            }

            return change.GetValueChanges<T>(modificationType);
        }


        public static T GetValueAdd<T>(this CSEntryChange csentry, string attributeName)
        {
            AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == attributeName);

            if (change == null)
            {
                return default(T);
            }

            return change.GetValueAdd<T>();
        }

        public static T GetValueAdd<T>(this AttributeChange change)
        {
            return change.GetValueAdds<T>().SingleOrDefault();
        }


        public static IList<T> GetValueAdds<T>(this AttributeChange change)
        {
            return change.GetValueChanges<T>(ValueModificationType.Add);
        }

        public static IList<T> GetValueDeletes<T>(this AttributeChange change)
        {
            return change.GetValueChanges<T>(ValueModificationType.Delete);
        }

        public static IList<T> GetValueChanges<T>(this AttributeChange change, ValueModificationType modificationType)
        {
            List<T> list = new List<T>();

            if (change.ValueChanges == null)
            {
                return list;
            }

            IEnumerable<ValueChange> valueChanges = change.ValueChanges.Where(t => t.ModificationType == modificationType);

            foreach (ValueChange valueChange in valueChanges)
            {
                list.Add((T)valueChange.Value);
            }

            return list;
        }
    }
}