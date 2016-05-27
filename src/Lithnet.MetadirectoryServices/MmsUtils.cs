using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Lithnet.MetadirectoryServices
{
    public static class MmsUtils
    {
        private const string baseKeyName = @"SYSTEM\CurrentControlSet\services\FIMSynchronizationService\Parameters";

        private static RegistryKey baseKey;

        private static RegistryKey BaseKey
        {
            get
            {
                if (MmsUtils.baseKey == null)
                {
                    MmsUtils.baseKey = Registry.LocalMachine.OpenSubKey(MmsUtils.baseKeyName);

                    if (MmsUtils.baseKey == null)
                    {
                        throw new NotSupportedException("The FIM Synchronization Service is not installed on this machine");
                    }
                }

                return MmsUtils.baseKey;
            }
        }

        public static string Path
        {
            get { return MmsUtils.BaseKey.GetValue("Path", null) as string; }
        }

        public static string DBServerName
        {
            get
            {
                string serverName = MmsUtils.BaseKey.GetValue("Server", "localhost") as string;
                if (string.IsNullOrWhiteSpace(serverName))
                {
                    return "localhost";
                }
                else
                {
                    return serverName;
                }
            }
        }

        public static string DBInstanceName
        {
            get { return MmsUtils.BaseKey.GetValue("SQLInstance", null) as string; }
        }

        public static string DBName
        {
            get { return MmsUtils.BaseKey.GetValue("DBName", "FIMSynchronizationService") as string; }
        }
    }
}