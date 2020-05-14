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
    public static class CSEntryChangeResultExtensions
    {
        /// <summary>
        /// Gets the value of the specified anchor attribute if it exists, or the default value of the type if it doesn't
        /// </summary>
        /// <typeparam name="T">The data type of the anchor attribute</typeparam>
        /// <param name="csentry">The CSEntryChange to get the anchor value from</param>
        /// <param name="anchorName">The name of the anchor attribute</param>
        /// <returns>The value of the anchor attribute, or default(T) if the anchor was not present</returns>
        public static T GetAnchorValueOrDefault<T>(this CSEntryChangeResult csentry, string anchorName)
        {
            var anchor = csentry.AnchorAttributes.FirstOrDefault(t => t.Name == anchorName);

            if (anchor != null)
            {
                return (T) anchor.GetValueAdd<T>();
            }
            else
            {
                return default(T);
            }
        }
    }
}