namespace LcmsNetSDK.Devices
{
    public interface IHasDataProvider
    {
        /// <summary>
        /// Register controls that are being disposed for a given data provider.
        /// </summary>
        /// <param name="key">Data provider name</param>
        /// <param name="remoteMethod">Delegate method to call from the device to synch events to.</param>
        void RegisterDataProvider(string key, DelegateDeviceHasData remoteMethod);

        /// <summary>
        /// De-register controls that are being disposed for a given data provider.
        /// </summary>
        /// <param name="key">Data provider name</param>
        /// <param name="remoteMethod">Delegate method to call from the device to synch events to.</param>
        void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod);
    }
}
