using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using LcmsNetSDK.System;

namespace LcmsNetSDK
{
    public class SystemInformationReporter
    {
        public static string BuildSystemInformation()
        {
            var name = "[SystemInfo]\r\n";
            name += $"Machine Name = {Environment.MachineName}\r\n";
            name += $"CartName = {LCMSSettings.GetParameter(LCMSSettings.PARAM_CARTNAME)}\r\n";
            name += $"CartConfigName = {LCMSSettings.GetParameter(LCMSSettings.PARAM_CARTCONFIGNAME)}\r\n";
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
                        name += $"IPAddress{i++} = {address}\r\n";
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
            var assembly = Assembly.GetEntryAssembly();
            var assemblyName = assembly.GetName();
            var ver = assemblyName.Version;
            var name = "[ApplicationInfo]\r\n";
            name += $"Application {assemblyName.Name}\r\nVersion {ver}\r\n";

            var os = Environment.OSVersion;
            ver = os.Version;
            name += $"Operating System: {os.VersionString} ({ver})\r\n";

            name += $"Computer Name: {Environment.MachineName}\r\n";
            ver = Environment.Version;
            name += $"CLR Version {ver}\r\n";

            return name;
        }
    }
}