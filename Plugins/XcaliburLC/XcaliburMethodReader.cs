using System;
using System.IO;
using System.Linq;
using LcmsNetSDK.Logging;
using ThermoFisher.CommonCore.Data.Business;

namespace LcmsNetPlugins.XcaliburLC
{
    internal static class XcaliburMethodReader
    {
        public static string GetMethodText(string methodFilePath)
        {
            if (!File.Exists(methodFilePath))
            {
                return "";
            }

            try
            {
                var method = InstrumentMethodReaderFactory.ReadFile(methodFilePath);
                if (method.IsError)
                {
                    ApplicationLogger.LogError(LogLevel.Warning, $"Error reading Xcalibur method file '{methodFilePath}': {method.FileError.ErrorMessage}");
                    return "";
                }

                return string.Join("\n", method.Devices.Select(x => $"Method for '{x.Key}':\n{x.Value.MethodText}"));
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(LogLevel.Warning, $"Error reading Xcalibur method file '{methodFilePath}'", ex);
                return "";
            }
        }
    }
}
