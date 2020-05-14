using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.MetadirectoryServices
{
    /// <summary>
    /// Contains extensions for configuration parameter collections
    /// </summary>
    public static class ConfigParameterExtensions
    {
        public static string GetStringValueOrDefault(this KeyedCollection<string, ConfigParameter> parameters, string parameterName, string defaultValue)
        {
            if (!parameters.Contains(parameterName))
            {
                return defaultValue;
            }

            ConfigParameter p = parameters[parameterName];

            if (p.IsEncrypted)
            {
                return p.SecureValue.ConvertToUnsecureString();
            }
            else
            {
                return p.Value;
            }
        }

        public static int GetIntValueOrDefault(this KeyedCollection<string, ConfigParameter> parameters, string parameterName, int defaultValue)
        {
            if (!parameters.Contains(parameterName))
            {
                return defaultValue;
            }

            ConfigParameter p = parameters[parameterName];

            if (int.TryParse(p.Value, out int result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        public static int GetIntValueOrDefault(this KeyedCollection<string, ConfigParameter> parameters, string parameterName)
        {
            return GetIntValueOrDefault(parameters, parameterName, 0);
        }

        public static string GetStringValueOrDefault(this KeyedCollection<string, ConfigParameter> parameters, string parameterName)
        {
            return GetStringValueOrDefault(parameters, parameterName, null);
        }

        public static bool GetBoolValueOrDefault(this KeyedCollection<string, ConfigParameter> parameters, string parameterName, bool defaultValue)
        {
            if (!parameters.Contains(parameterName))
            {
                return defaultValue;
            }

            ConfigParameter p = parameters[parameterName];

            return p.Value == "1";
        }

        public static bool GetBoolValueOrDefault(this KeyedCollection<string, ConfigParameter> parameters, string parameterName)
        {
            return GetBoolValueOrDefault(parameters, parameterName, false);
        }
    }
}