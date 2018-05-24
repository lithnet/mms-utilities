using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;

namespace Lithnet.MetadirectoryServices
{
    /// <summary>
    /// Contains extensions to the <see cref="Microsoft.MetadirectoryServices.AttributeChange"/> object
    /// </summary>
    public static class AttributeChangeExtentions
    {
        /// <summary>
        /// Gets the value with a modification type of 'add' in the list of value changes. If multiple value adds are present, an exception is thrown
        /// </summary>
        /// <typeparam name="T">The type of data in the attribute</typeparam>
        /// <param name="change">The attribute change</param>
        /// <returns>The value with a modification type of 'add', or default(T) if no value adds are present</returns>
        public static T GetValueAdd<T>(this AttributeChange change)
        {
            return change.GetValueAdds<T>().SingleOrDefault();
        }

        /// <summary>
        /// Gets the value adds present in the attribute change
        /// </summary>
        /// <typeparam name="T">The type of data in the attribute</typeparam>
        /// <param name="change">The attribute change</param>
        /// <returns>A list values present in the attribute change, or an empty list if there were no value adds present</returns>
        public static IList<T> GetValueAdds<T>(this AttributeChange change)
        {
            return change.GetValueChanges<T>(ValueModificationType.Add);
        }

        /// <summary>
        /// Gets the value deletes present in the attribute change
        /// </summary>
        /// <typeparam name="T">The type of data in the attribute</typeparam>
        /// <param name="change">The attribute change</param>
        /// <returns>A list values present in the attribute change, or an empty list if there were no value deletes present</returns>
        public static IList<T> GetValueDeletes<T>(this AttributeChange change)
        {
            return change.GetValueChanges<T>(ValueModificationType.Delete);
        }

        /// <summary>
        /// Gets the values present in the attribute change that match the specified ValueModificationType
        /// </summary>
        /// <typeparam name="T">The type of data in the attribute</typeparam>
        /// <param name="change">The attribute change</param>
        /// <param name="modificationType">The type of value modification to apply</param>
        /// <returns>A list values present in the attribute change, or an empty list if there were no value changes matching the specified type were present</returns>
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
                list.Add(TypeConverter.ConvertData<T>(valueChange.Value));
            }

            return list;
        }

        public static T ApplyAttributeChange<T>(this AttributeChange change)
        {
            if (change.IsMultiValued)
            {
                throw new InvalidOperationException("Cannot use this method on a multivalued attribute");
            }

            return change.GetValueAdd<T>();
        }

        public static IList<object> ApplyAttributeChanges(this AttributeChange change, IList existingItems)
        {
            switch (change.DataType)
            {
                case AttributeType.String:
                case AttributeType.Reference:
                    return ApplyAttributeChanges<string>(change, existingItems, StringComparer.CurrentCulture);

                case AttributeType.Integer:
                    return ApplyAttributeChanges<long>(change, existingItems, EqualityComparer<long>.Default);

                case AttributeType.Binary:
                    return ApplyAttributeChanges<byte[]>(change, existingItems, BinaryEqualityComparer.Default);

                case AttributeType.Boolean:
                    return ApplyAttributeChanges<bool>(change, existingItems, EqualityComparer<bool>.Default);

                default:
                    throw new UnknownOrUnsupportedDataTypeException();
            }
        }

        public static IList<object> ApplyAttributeChanges<T>(this AttributeChange change, IList existingItems, IEqualityComparer<T> comparer)
        {
            if (change.ModificationType == AttributeModificationType.Delete)
            {
                return null;
            }

            if (existingItems == null)
            {
                existingItems = new List<object>();
            }

            HashSet<T> newItems = new HashSet<T>(existingItems.Cast<object>().Select(TypeConverter.ConvertData<T>), comparer);
            ICollection<T> valueAdds = change.GetValueAdds<T>();
            ICollection<T> valueDeletes = change.GetValueDeletes<T>();

            if (change.ModificationType == AttributeModificationType.Replace)
            {
                foreach (var item in existingItems)
                {
                    valueDeletes.Add((T)item);
                }
            }
            
            foreach (T value in valueAdds)
            {
                newItems.Add(value);
            }

            foreach (T value in valueDeletes)
            {
                newItems.Remove(value);
            }

            return newItems.Cast<object>().ToList();
        }
    }
}
