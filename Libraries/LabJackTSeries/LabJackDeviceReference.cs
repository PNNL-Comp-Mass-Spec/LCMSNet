using System;
using System.Collections.Generic;
using LabJack;

namespace LabJackTSeries
{
    internal class LabJackDeviceReference
    {
        private static readonly Dictionary<string, LabJackDeviceReference> deviceRefMap = new Dictionary<string, LabJackDeviceReference>(1);
        private static readonly object dictionaryLock = new object();

        private LabJackDeviceReference(int deviceType, string identifier, int handle)
        {
            DeviceType = deviceType;
            Identifier = identifier;
            Handle = handle;
            ReferenceCount = 0;
        }

        private void ReleaseUnmanagedResources(bool isFinalize)
        {
            CloseHandle(this, isFinalize);

            if (closed)
            {
                GC.SuppressFinalize(this);
            }
        }

        ~LabJackDeviceReference()
        {
            ReferenceCount = 0;
            ReleaseUnmanagedResources(true);
        }

        private readonly object counterLock = new object();
        private bool closed = false;

        public int DeviceType { get; }
        public string Identifier { get; }
        public int Handle { get; }
        public int ReferenceCount { get; private set; }

        private void AddReference()
        {
            lock (counterLock)
            {
                ReferenceCount++;
            }
        }

        public void DisposeReference()
        {
            lock (counterLock)
            {
                ReferenceCount--;
            }

            if (ReferenceCount < 1)
            {
                ReleaseUnmanagedResources(false);
            }
        }

        private static string GetDictionaryKey(int deviceType, string identifier)
        {
            return $"{deviceType}:{identifier}";
        }

        public static LabJackDeviceReference GetHandle(int deviceType, string identifier)
        {
            if (!string.IsNullOrWhiteSpace(identifier))
            {
                identifier = "ANY";
            }

            var dictionaryKey = GetDictionaryKey(deviceType, identifier);

            // If an ID other than 0/"ANY"/LJM.CONSTANTS.idANY is wanted for the T-series device being initialized
            // it must be changed before calling initialize, or initialize must be called again afterward.
            // Use "-2" or LJM.CONSTANTS.DEMO_MODE to open a demo device.
            // Otherwise, can open by serial number, IP address (networked), or 'Device name' (which can be up to 49 characters, with at least one letter)

            lock (dictionaryLock)
            {
                if (deviceRefMap.TryGetValue(dictionaryKey, out var deviceRef))
                {
                    deviceRef.AddReference();
                    return deviceRef;
                }

                var handle = 0;
                var err = LJM.Open(deviceType, LJM.CONSTANTS.ctUSB, identifier, ref handle);

                if (err == LJM.LJMERROR.NOERROR || (LJM.LJMERROR.WARNINGS_BEGIN < err && err < LJM.LJMERROR.WARNINGS_END))
                {
                    deviceRef = new LabJackDeviceReference(deviceType, identifier, handle);
                    deviceRef.AddReference();
                    deviceRefMap.Add(dictionaryKey, deviceRef);
                    return deviceRef;
                }
            }

            return null;
        }

        private static bool CloseHandle(LabJackDeviceReference deviceRef, bool isFinalize)
        {
            lock (dictionaryLock)
            {
                if (!isFinalize && deviceRef.ReferenceCount > 0)
                {
                    return false;
                }

                var dictionaryKey = GetDictionaryKey(deviceRef.DeviceType, deviceRef.Identifier);
                if (deviceRefMap.ContainsKey(dictionaryKey))
                {
                    deviceRefMap.Remove(dictionaryKey);
                }

                LJM.Close(deviceRef.Handle);
                deviceRef.closed = true;
            }

            return true;
        }
    }
}
