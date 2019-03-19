using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace IISAppPools
{
    class IISJudgment
    {    
        public bool IsWebServer()
        {
            RegistryKey localKey = LocalKey();
            RegistryKey micrKey = localKey.OpenSubKey("SOFTWARE\\Microsoft\\");
            string[] micrKeyNames = micrKey.GetSubKeyNames();
            
            if (micrKeyNames.Contains("InetStp"))
            {
                return true;
            }

            return false;
        }

        public bool IsPassWebServer()
        {
            RegistryKey localKey = LocalKey();
            RegistryKey compKey = localKey.OpenSubKey("SOFTWARE\\Microsoft\\InetStp\\Components\\");
            string[] compKeyNames = compKey.GetValueNames();

            if (compKeyNames.Contains("ASPNET") & compKeyNames.Contains("StaticContent"))
            {
                return true;
            }

            return false;
        }
        private RegistryKey LocalKey()
        {
            RegistryKey key;
            if (Environment.Is64BitOperatingSystem)
            {
                key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            } else
            {
                key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            }

            return key;
        }
    }
}
