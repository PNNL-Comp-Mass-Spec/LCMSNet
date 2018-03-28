using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace LcmsNetSDK
{
    public class SystemInformationReporter
    {
        public static string BuildSystemInformation()
        {
            var name = "[SystemInfo]\r\n";
            name += string.Format("Machine Name = {0}\r\n", Environment.MachineName);
            name += string.Format("CartName = {0}\r\n", classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTNAME));
            name += string.Format("CartConfigName = {0}\r\n", classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTCONFIGNAME));
            try
            {
                var hostName = Dns.GetHostName();
                var entry = Dns.GetHostEntry(hostName);
                var addresses = entry.AddressList;
                var i = 0;
                foreach (var address in addresses)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
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
            var assem = Assembly.GetEntryAssembly();
            var assemName = assem.GetName();
            var ver = assemName.Version;
            var name = "[ApplicationInfo]\r\n";
            name += string.Format("Application {0}\r\nVersion {1}\r\n", assemName.Name, ver);

            var os = Environment.OSVersion;
            ver = os.Version;
            name += string.Format("Operating System: {0} ({1})\r\n", os.VersionString, ver);

            name += string.Format("Computer Name: {0}\r\n", Environment.MachineName);
            ver = Environment.Version;
            name += string.Format("CLR Version {0}\r\n", ver);

            return name;
        }
    }
}