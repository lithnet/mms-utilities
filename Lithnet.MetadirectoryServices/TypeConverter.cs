namespace Lithnet.MetadirectoryServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.MetadirectoryServices;

    /// <summary>
    /// Converts data between types
    /// </summary>
    public static class TypeConverter
    {
        public static void ThrowOnAnyInvalidDataType(IEnumerable<object> obj)
        {
            if (obj == null)
            {
                return;
            }

            foreach (object item in obj)
            {
                TypeConverter.ThrowOnInvalidDataType(item);
            }
        }

        public static void ThrowOnInvalidDataType(object obj)
        {
            if (obj == null)
            {
                return;
            }

            if (!TypeConverter.IsValidDataType(obj))
            {
                throw new InvalidCastException("The specified object type was not valid: " + obj.GetType().Name);
            }
        }

        public static bool IsValidDataType(object obj)
        {
            return obj is bool || obj is long || obj is int || obj is Guid || obj is byte[] || obj is string || obj is DateTime;
        }

        /// <summary>
        /// Tries to convert the object to the specified type
        /// </summary>
        /// <typeparam name="T">The data type to convert the object to</typeparam>
        /// <param name="obj">The object to convert</param>
        /// <param name="value">The converted value</param>
        /// <returns>A value indicating if the conversion was successful</returns>
        public static bool TryConvertData<T>(object obj, out T value)
        {
            try
            {
                value = TypeConverter.ConvertData<T>(obj);
                return true;
            }
            catch
            {
                value = default(T);
                return false;
            }
        }

        /// <summary>
        /// Tries to convert the object to the specified attribute type
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <param name="type">The data type to convert the object to</param>
        /// <param name="value">The converted value</param>
        /// <returns>A value indicating if the conversion was successful</returns>
        public static bool TryConvertData(object obj, AttributeType type, out object value)
        {
            try
            {
                value = TypeConverter.ConvertData(obj, type);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        public static AttributeType GetDataType(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            Type objectType = obj.GetType();

            if (objectType == typeof(string))
            {
                return AttributeType.String;
            }
            else if (objectType == typeof(int) || objectType == typeof(long))
            {
                return AttributeType.Integer;
            }
            else if (objectType == typeof(bool))
            {
                return AttributeType.Boolean;
            }
            else if (objectType == typeof(byte[]))
            {
                return AttributeType.Binary;
            }
            else if (objectType == typeof(Guid))
            {
                return AttributeType.Reference;
            }
            else
            {
                throw new NotSupportedException("The data type is unknown or not supported");
            }
        }

        /// <summary>
        /// Converts the object to the specified attribute type
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <param name="type">The data type to convert the object to</param>
        /// <returns>The converted value</returns>
        public static object ConvertData(object obj, AttributeType type)
        {
            switch (type)
            {
                case AttributeType.Binary:
                    return TypeConverter.ConvertToBinary(obj);

                case AttributeType.Boolean:
                    return TypeConverter.ConvertToBoolean(obj);

                case AttributeType.Integer:
                    return TypeConverter.ConvertToLong(obj);

                case AttributeType.Reference:
                    return TypeConverter.ConvertToString(obj);

                case AttributeType.String:
                    return TypeConverter.ConvertToString(obj);

                case AttributeType.Undefined:
                default:
                    throw new NotSupportedException("Unknown or unsupported data type");
            }
        }

        /// <summary>
        /// Converts the object to the type specified by the T parameter
        /// </summary>
        /// <typeparam name="T">The data type to convert the object to</typeparam>
        /// <param name="obj">The object to convert</param>
        /// <returns>The converted value</returns>
        public static T ConvertData<T>(object obj)
        {
            if (typeof(T) == typeof(long))
            {
                return (T)TypeConverter.ConvertToLong(obj);
            }
            else if (typeof(T) == typeof(bool))
            {
                return (T)TypeConverter.ConvertToBoolean(obj);
            }
            else if (typeof(T) == typeof(byte[]))
            {
                return (T)TypeConverter.ConvertToBinary(obj);
            }
            else if (typeof(T) == typeof(Guid))
            {
                return (T)TypeConverter.ConvertToGuid(obj);
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)TypeConverter.ConvertToString(obj);
            }
            else if (typeof(T) == typeof(DateTime))
            {
                return (T)TypeConverter.ConvertToDateTime(obj);
            }
            else
            {
                throw new NotSupportedException("Unknown or unsupported data type");
            }
        }

        /// <summary>
        /// Converts the specified object into a byte array
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>The converted value</returns>
        /// <exception cref="FormatException">Thrown when the specified object cannot be converted to a byte array</exception>
        private static object ConvertToBinary(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            else if (obj is byte[])
            {
                return obj;
            }
            else if (obj is string)
            {
                return Convert.FromBase64String((string)obj);
            }
            else
            {
                throw new FormatException(string.Format("The object of type '{0}' cannot be converted to 'byte[]'", obj.GetType().Name));
            }
        }

        /// <summary>
        /// Converts the specified object into a boolean value
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>The converted value</returns>
        /// <exception cref="FormatException">Thrown when the specified object cannot be converted to a boolean value</exception>
        private static object ConvertToBoolean(object obj)
        {
            if (obj == null)
            {
                throw new FormatException(string.Format("A null object cannot be converted to 'bool'"));
            }
            else if (obj is bool)
            {
                return obj;
            }
            else if (obj is string)
            {
                string str = (string)obj;

                if (str == "0")
                {
                    return false;
                }
                else if (str == "1")
                {
                    return true;
                }
                else
                {
                    return Convert.ToBoolean(str);
                }
            }
            else if (obj is int)
            {
                return Convert.ToBoolean((int)obj);
            }
            else if (obj is long)
            {
                return Convert.ToBoolean((long)obj);
            }
            else
            {
                throw new FormatException(string.Format("The object of type '{0}' cannot be converted to 'bool'", obj.GetType().Name));
            }
        }

        /// <summary>
        /// Converts the specified object into a 64-bit integer
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>The converted value</returns>
        /// <exception cref="FormatException">Thrown when the specified object cannot be converted to a 64-bit integer</exception>
        private static object ConvertToLong(object obj)
        {
            if (obj == null)
            {
                throw new FormatException(string.Format("A null object cannot be converted to 'long'"));
            }
            else if (obj is long)
            {
                return obj;
            }
            else if (obj is int)
            {
                return Convert.ToInt64((int)obj);
            }
            else if (obj is string)
            {
                return Convert.ToInt64((string)obj);
            }
            else if (obj is DateTime)
            {
                return ((DateTime)obj).Ticks;
            }
            else
            {
                throw new FormatException(string.Format("The object of type '{0}' cannot be converted to 'long'", obj.GetType().Name));
            }
        }

        /// <summary>
        /// Converts the specified object into a DateTime
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>The converted value</returns>
        /// <exception cref="FormatException">Thrown when the specified object cannot be converted to a 64-bit integer</exception>
        private static object ConvertToDateTime(object obj)
        {
            if (obj == null)
            {
                throw new FormatException(string.Format("A null object cannot be converted to 'DateTime'"));
            }
            else if (obj is DateTime)
            {
                return obj;
            }
            else if (obj is long)
            {
                return new DateTime((long)obj);
            }
            else if (obj is int)
            {
                return new DateTime(Convert.ToInt64((int)obj));
            }
            else if (obj is string)
            {
                return DateTime.Parse((string)obj);
            }
            else
            {
                throw new FormatException(string.Format("The object of type '{0}' cannot be converted to 'DateTime'", obj.GetType().Name));
            }
        }

        /// <summary>
        /// Converts the specified object into a Guid
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>The converted value</returns>
        /// <exception cref="FormatException">Thrown when the specified object cannot be converted to a Guid</exception>
        private static object ConvertToGuid(object obj)
        {
            if (obj == null)
            {
                throw new FormatException(string.Format("A null object cannot be converted to 'Guid'"));
            }
            else if (obj is Guid)
            {
                return obj;
            }
            else if (obj is string)
            {
                return Guid.Parse((string)obj);
            }
            else
            {
                throw new FormatException(string.Format("The object of type '{0}' cannot be converted to 'Guid'", obj.GetType().Name));
            }
        }

        /// <summary>
        /// Converts the specified object into a string
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>The converted value</returns>
        private static object ConvertToString(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            else if (obj is string)
            {
                return obj;
            }
            else if (obj is bool || obj is long || obj is int || obj is Guid || obj is byte[] || obj is DateTime)
            {
                return obj.ToSmartString();
            }
            else
            {
                throw new FormatException(string.Format("The object of type '{0}' cannot be converted to 'string'", obj.GetType().Name));
            }
        }
    }
}
