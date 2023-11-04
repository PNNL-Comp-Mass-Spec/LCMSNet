using System;
using System.IO.Ports;
using FluidicsSDK.Devices.Valves;
using LcmsNetSDK;
using LcmsNetSDK.Devices;

namespace LcmsNetPlugins.VICI.Valves.TwoPosition
{
    [Serializable]
    ////[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    [DeviceControl(typeof(ValveVICI2PosViewModel), "Six-port Injection", "Valves Two-Position")]
    public class ValveVICISixPortInjection: ValveVICI2Pos, ISixPortInjectionValve
    {
        private double injectionVolume;

         public ValveVICISixPortInjection() : base()
        {
            injectionVolume = 0;
        }

        public ValveVICISixPortInjection(SerialPort port) : base(port)
        {
            injectionVolume = 0;
        }

        public double GetVolume()
        {
            return injectionVolume;
        }

        public void SetVolume(double volume)
        {
            InjectionVolume = volume;
        }

        [DeviceSavedSetting("InjectionVolume")]
        public double InjectionVolume
        {
            get => injectionVolume;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref injectionVolume, value))
                {
                    InjectionVolumeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler InjectionVolumeChanged;
    }
}
