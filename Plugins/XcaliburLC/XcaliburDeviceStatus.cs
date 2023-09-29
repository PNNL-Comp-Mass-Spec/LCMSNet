﻿namespace LcmsNetPlugins.XcaliburLC
{
    internal readonly struct XcaliburDeviceStatus
    {
        public string Name { get; }
        public string StatusString { get; }
        public int StatusCode { get; }
        public string StatusCodeString { get; }

        public XcaliburDeviceStatus(string name, string statusString, int statusCode, string statusCodeString)
        {
            Name = name;
            StatusString = statusString;
            StatusCode = statusCode;
            StatusCodeString = statusCodeString;
        }

        public XcaliburDeviceStatus(string name, int statusCode, string statusCodeString)
        {
            Name = name;
            StatusString = "";
            StatusCode = statusCode;
            StatusCodeString = statusCodeString;
        }

        public override string ToString()
        {
            var statusString = string.IsNullOrWhiteSpace(StatusString) ? "" : $" ({StatusString})";
            return $"{Name}{statusString}: status '{StatusCode}' ({StatusCodeString})";
        }
    }
}