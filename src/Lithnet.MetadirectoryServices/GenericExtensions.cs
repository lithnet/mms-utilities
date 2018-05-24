using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.MetadirectoryServices
{
    /// <summary>
    /// Contains extensions for objects not directly related to MetadirectoryServices components, such as strings and DateTimes
    /// </summary>
    public static class GenericExtensions
    {
        /// <summary>
        /// A .NET custom format string that represents the ISO8601 date format that is used by the FIM Service
        /// </summary>
        public const string ResourceManagementServiceDateFormat = @"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff";

        /// <summary>
        /// A .NET custom format string that represents the ISO8601 date format that is used by the FIM Service, but has the milliseconds component set to '0'
        /// </summary>
        public const string ResourceManagementServiceDateFormatZeroedMilliseconds = @"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'000";

        /// <summary>
        /// Converts a date time to the ISO 8601 date string required by the Resource Management Service
        /// </summary>
        /// <param name="dateTime">The date and time to convert</param>
        /// <returns>An ISO 8601 date format string</returns>
        public static string ToResourceManagementServiceDateFormat(this DateTime dateTime)
        {
            return GenericExtensions.ToResourceManagementServiceDateFormat(dateTime, false, false);
        }

        /// <summary>
        /// Converts a date time to the ISO 8601 date string required by the Resource Management Service
        /// </summary>
        /// <param name="dateTime">The date and time to convert</param>
        /// <param name="convertToUtc">A value indicating that if the date does not have a Kind of UTC, it will be converted to UTC</param>
        /// <param name="zeroMilliseconds">A value indicating whether the millisecond component of the date should be zeroed to avoid rounding/round-trip issues</param>
        /// <returns>An ISO 8601 date format string</returns>
        public static string ToResourceManagementServiceDateFormat(this DateTime dateTime, bool convertToUtc, bool zeroMilliseconds)
        {
            if (convertToUtc)
            {
                dateTime = dateTime.ToUniversalTime();
            }
            
            if (zeroMilliseconds)
            {
                return dateTime.ToString(GenericExtensions.ResourceManagementServiceDateFormatZeroedMilliseconds);
            }
            else
            {
                return dateTime.ToString(GenericExtensions.ResourceManagementServiceDateFormat);

            }
        }

        /// <summary>
        /// Converts a secure string back to its standard string representation
        /// </summary>
        /// <param name="securePassword">The secure string value</param>
        /// <returns>The plain-text representation of the secure string</returns>
        public static string ConvertToUnsecureString(this SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException(nameof(securePassword));

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        /// <summary>
        /// Converts an enumeration of strings into a comma separated list
        /// </summary>
        /// <param name="strings">The enumeration of string objects</param>
        /// <returns>The comma separated list of strings</returns>
        public static string ToCommaSeparatedString(this IEnumerable<string> strings)
        {
            string newString = string.Empty;

            if (strings != null)
            {
                foreach (string text in strings)
                {
                    newString = newString.AppendWithCommaSeparator(text);
                }
            }

            return newString;
        }

        /// <summary>
        /// Converts an enumeration of strings into a comma separated list
        /// </summary>
        /// <param name="strings">The enumeration of string objects</param>
        /// <param name="separator">The character or string to use to separate the strings</param>
        /// <returns>The comma separated list of strings</returns>
        public static string ToSeparatedString(this IEnumerable<string> strings, string separator)
        {
            string newString = string.Empty;

            foreach (string text in strings)
            {
                newString = newString.AppendWithSeparator(separator, text);
            }

            return newString;
        }

        /// <summary>
        /// Converts an enumeration of strings into a comma separated list
        /// </summary>
        /// <param name="strings">The enumeration of string objects</param>
        /// <returns>The comma separated list of strings</returns>
        public static string ToNewLineSeparatedString(this IEnumerable<string> strings)
        {
            StringBuilder builder = new StringBuilder();

            foreach (string text in strings)
            {
                builder.AppendLine(text);
            }

            return builder.ToString().TrimEnd();
        }

        /// <summary>
        /// Appends two string together with a comma and a space
        /// </summary>
        /// <param name="text">The original string</param>
        /// <param name="textToAppend">The string to append</param>
        /// <returns>The concatenated string</returns>
        public static string AppendWithCommaSeparator(this string text, string textToAppend)
        {
            string newString = string.Empty;

            if (!string.IsNullOrWhiteSpace(text))
            {
                text += ", ";
            }
            else
            {
                text = string.Empty;
            }

            newString = text + textToAppend;
            return newString;
        }

        /// <summary>
        /// Appends two string together with a comma and a space
        /// </summary>
        /// <param name="text">The original string</param>
        /// <param name="separator">The character or string to use to separate the strings</param>
        /// <param name="textToAppend">The string to append</param>
        /// <returns>The concatenated string</returns>
        public static string AppendWithSeparator(this string text, string separator, string textToAppend)
        {
            string newString = string.Empty;

            if (!string.IsNullOrWhiteSpace(text))
            {
                text += separator;
            }
            else
            {
                text = string.Empty;
            }

            newString = text + textToAppend;
            return newString;
        }

        /// <summary>
        /// Gets an informative string representation of an object
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>An informative string representation of an object</returns>
        public static string ToSmartString(this object obj)
        {
            if (obj is byte[])
            {
                byte[] cast = (byte[])obj;
                return Convert.ToBase64String(cast);
            }
            else if (obj is long)
            {
                return ((long)obj).ToString();
            }
            else if (obj is string)
            {
                return ((string)obj).ToString();
            }
            else if (obj is bool)
            {
                return ((bool)obj).ToString();
            }
            else if (obj is Guid)
            {
                return ((Guid)obj).ToString();
            }
            else if (obj is DateTime)
            {
                return ((DateTime)obj).ToString(ResourceManagementServiceDateFormat);
            }
            else if (obj == null)
            {
                return "null";
            }
            else
            {
                return obj.ToString();
            }
        }

        /// <summary>
        /// Gets an informative string representation of an object
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>An informative string representation of an object, or a null value if the object is null</returns>
        public static string ToSmartStringOrNull(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            else
            {
                return obj.ToSmartString();
            }
        }

        /// <summary>
        /// Gets an informative string representation of an object
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>An informative string representation of an object, or a null value if the object is null</returns>
        public static string ToSmartStringOrEmptyString(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            else
            {
                return obj.ToSmartString();
            }
        }

        /// <summary>
        /// Truncates a string to the specified length
        /// </summary>
        /// <param name="obj">The string to truncate</param>
        /// <param name="totalLength">The length to truncate to</param>
        /// <returns>A string shortened to the specified length</returns>
        public static string TruncateString(this string obj, int totalLength)
        {
            if (string.IsNullOrWhiteSpace(obj))
            {
                return obj;
            }

            if (totalLength <= 3)
            {
                throw new ArgumentException("The maxlength value must be greater than 3", nameof(totalLength));
            }

            if (obj.Length > totalLength)
            {
                return obj.Substring(0, totalLength - 3) + "...";
            }
            else
            {
                return obj;
            }
        }

        /// <summary>
        /// Gets a value indicating whether two enumerations contain the same elements, even if they are in different orders
        /// </summary>
        /// <typeparam name="T">The type of items in the enumerations</typeparam>
        /// <param name="enumeration1">The first list to compare</param>
        /// <param name="enumeration2">The second list to compare</param>
        /// <returns>A value indicating if the two enumerations contain the same objects</returns>
        public static bool ContainsSameElements<T>(this IEnumerable<T> enumeration1, IEnumerable<T> enumeration2)
        {
            List<T> list1 = enumeration1.ToList();
            List<T> list2 = enumeration2.ToList();

            if (list1.Count != list2.Count)
            {
                return false;
            }

            return list1.Intersect(list2).Count() == list1.Count;
        }

        /// <summary>
        /// Converts an ExtendedAttributeType value to its equivalent MetadirectoryServices AttributeType
        /// </summary>
        /// <param name="type">The ExtendedAttributeType value</param>
        /// <returns>An equivalent AttributeType value</returns>
        public static AttributeType ToAttributeType(this ExtendedAttributeType type)
        {
            switch (type)
            {
                case ExtendedAttributeType.String:
                    return AttributeType.String;

                case ExtendedAttributeType.Integer:
                    return AttributeType.Integer;

                case ExtendedAttributeType.Reference:
                    return AttributeType.Reference;

                case ExtendedAttributeType.Binary:
                    return AttributeType.Binary;

                case ExtendedAttributeType.Boolean:
                    return AttributeType.Boolean;

                case ExtendedAttributeType.Undefined:
                    return AttributeType.Undefined;

                case ExtendedAttributeType.DateTime:
                    return AttributeType.String;

                default:
                    throw new UnknownOrUnsupportedDataTypeException();
            }
        }

        /// <summary>
        /// Converts an AttributeType value to its equivalent ExtendedAttributeType value
        /// </summary>
        /// <param name="type">The AttributeType value</param>
        /// <returns>An equivalent ExtendedAttributeType value</returns>
        public static ExtendedAttributeType ToExtendedAttributeType(this AttributeType type)
        {
            switch (type)
            {
                case AttributeType.Binary:
                    return ExtendedAttributeType.Binary;

                case AttributeType.Boolean:
                    return ExtendedAttributeType.Boolean;

                case AttributeType.Integer:
                    return ExtendedAttributeType.Integer;

                case AttributeType.Reference:
                    return ExtendedAttributeType.Reference;

                case AttributeType.String:
                    return ExtendedAttributeType.String;

                case AttributeType.Undefined:
                    return ExtendedAttributeType.Undefined;

                default:
                    throw new UnknownOrUnsupportedDataTypeException();
            }
        }

        /// <summary>
        /// <para>Truncates a DateTime to a specified resolution.</para>
        /// <para>A convenient source for resolution is TimeSpan.TicksPerXXXX constants.</para>
        /// </summary>
        /// <param name="date">The DateTime object to truncate</param>
        /// <param name="resolution">e.g. to round to nearest second, TimeSpan.TicksPerSecond</param>
        /// <returns>Truncated DateTime</returns>
        public static DateTime Truncate(this DateTime date, long resolution)
        {
            return new DateTime(date.Ticks - (date.Ticks % resolution), date.Kind);
        }

        /// <summary>
        /// Converts a ValueCollection to a generic list of objects
        /// </summary>
        /// <param name="values">The value collection object</param>
        /// <returns>A generic list of objects</returns>
        public static IList<object> ToList(this ValueCollection values)
        {
            List<object> list = new List<object>();

            foreach (Value value in values.OfType<Value>())
            {
                switch (value.DataType)
                {
                    case AttributeType.Binary:
                        list.Add(value.ToBinary());
                        break;

                    case AttributeType.Boolean:
                        list.Add(value.ToBoolean());
                        break;

                    case AttributeType.Integer:
                        list.Add(value.ToInteger());
                        break;

                    case AttributeType.String:
                    case AttributeType.Reference:
                        list.Add(value.ToString());
                        break;

                    case AttributeType.Undefined:
                    default:
                        throw new UnknownOrUnsupportedDataTypeException();
                }
            }

            return list;
        }
    }
}
