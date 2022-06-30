using System;
using System.Reflection;
using LcmsNetSDK.Devices;

namespace LcmsNetSDK.Method
{
    public interface ILCEvent
    {
        string Name { get; }
        IDevice Device { get; }
        DateTime Start { get; }
        TimeSpan Duration { get; }
        string[] ParameterNames { get; }
        object[] Parameters { get; }
        ILCMethodEvent MethodData { get; }
        MethodInfo Method { get; }
        bool HadError { get; }
    }
}
