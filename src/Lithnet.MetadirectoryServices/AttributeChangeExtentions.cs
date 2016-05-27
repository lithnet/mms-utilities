using System;
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
                list.Add((T)valueChange.Value);
            }

            return list;
        }
    }
}
