using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Win32;
using System.Management;

namespace SharedLibrary
{
    public static class Node
    {
        public static string GetMachineGUID()
        {
            ManagementObject os = new ManagementObject("Win32_OperatingSystem=@");
            string serial = (string)os["SerialNumber"];
            //object val = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Cryptography", "MachineGuid", null);
            //if (val == null) return "null";
            return serial;
        }

    }
}
