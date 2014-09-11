using System;
using System.Net;
using System.Reflection;

namespace LcmsNetDataClasses
{
    public class SystemInformationReporter
    {
        public static string BuildSystemInformation()
        {
                string name     =   "[SystemInfo]\r\n";
                name +=         string.Format("Machine Name = {0}\r\n", Environment.MachineName);
                name +=         string.Format("CartName = {0}\r\n", LcmsNetDataClasses.classLCMSSettings.GetParameter("CartName"));
                try
                {
                    string hostName       = Dns.GetHostName();
                    IPHostEntry entry     = Dns.GetHostEntry(hostName);
                    IPAddress[] addresses = entry.AddressList;
                    int i = 0;
                    foreach(IPAddress address in addresses)
                    {
                        if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            name += string.Format("IPAddress{0} = {1}\r\n", i++, address);                            
                        }
                    }  
                    name = name.TrimEnd('\r', '\n');
                }
                catch
                {
                }
                return name;
        }
        public static string BuildApplicationInformation()
        {
            Assembly assem = Assembly.GetEntryAssembly();
            AssemblyName assemName = assem.GetName();
            Version ver     = assemName.Version;
            string name     = "[ApplicationInfo]\r\n";
            name    += string.Format("Application {0}\r\nVersion {1}\r\n", assemName.Name, ver.ToString());

            OperatingSystem os = Environment.OSVersion;
            ver     = os.Version;
            name += string.Format("Operating System: {0} ({1})\r\n", os.VersionString, ver.ToString());

            name += string.Format("Computer Name: {0}\r\n", Environment.MachineName);
            ver     = Environment.Version;
            name += string.Format("CLR Version {0}\r\n", ver.ToString());

            return name;
        }
    }
}
