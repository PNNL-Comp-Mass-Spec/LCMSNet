using System;
using System.Reflection;
using LcmsNetSDK.Devices;

namespace LcmsNetSDK.Method
{
    public class LCEventDummy : ILCEvent
    {
        public string Name { get; set; }
        public IDevice Device { get; set; }
        public DateTime Start { get; set; }
        public TimeSpan Duration { get; set; }
        public string[] ParameterNames { get; set; }
        public object[] Parameters { get; set; }
        internal LCMethodEventEmpty MethodData { get; set; } = new LCMethodEventEmpty();
        ILCMethodEvent ILCEvent.MethodData => MethodData;
        public MethodInfo Method { get; set; }
        public bool HadError { get; set; }
    }
}
