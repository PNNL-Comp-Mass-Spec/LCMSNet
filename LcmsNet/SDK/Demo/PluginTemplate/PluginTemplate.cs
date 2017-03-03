// insert using statement for the devices driver library here

/*
[Serializable]
// for full understanding of the following attribute please see the LcmsNetSDK.
[classDeviceControlAttribute(typeof(YOURUSERCONTROLHERE), typeof(YOURCUSTOMFLUIDICSGLYPHHERE), "RenameThisTemplate", "DeviceCategoryGoesHere")]
public class RenameThisTemplate:IDevice // If using a pre-existing FluidicsSDK glyph, you should inherit the interface for that glyph, instead of using its type in the attribute above.
{
    #region PluginMethods
    // constructor
    public RenameThisTemplate()
    {
        //setup code
    }


    //This is a template for a method exposed to LcmsNet to operate the device, copy+paste as many as you need.
    // for full understanding of the following attribute please see the LcmsNetSDK.
    [classLCMethodAttribute("YourMethodHere", 1.0, false, "", -1, false)]
    public void YourMethodHere()
    {
        // operation code goes here
    }
    #endregion

    #region IDeviceMethods

    /// <summary>
    /// Calls an initialization sequence for the device to perform after construction.
    /// </summary>
    /// <returns>True if initialization successful.  False if initialization failed.</returns>
    bool Initialize(ref string errorMessage)
    {
        // intialization code goes here
        return true;
    }
    /// <summary>
    /// Calls a shutdown sequence for the device to stop all acquiring/control.
    /// </summary>
    /// <returns>True if shutdown successful.  False if failure occured.</returns>
    bool Shutdown()
    {
        //shutdown code goes here
        return true;
    }
    /// <summary>
    /// Register controls that are being disposed for a given data provider.
    /// </summary>
    /// <param name="key">Data provider name</param>
    /// <param name="remoteMethod">Delegate method to call from the device to synch events to.</param>
    void RegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
    {
        // register here
    }
    /// <summary>
    /// De-register controls that are being disposed for a given data provider.
    /// </summary>
    /// <param name="key">Data provider name</param>
    /// <param name="remoteMethod">Delegate method to call from the device to synch events to.</param>
    void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
    {
        // unregister here
    }
    /// <summary>        
    /// Write the performance data and other required information associated with this device after a run.
    /// </summary>
    /// <param name="filepath">Path to write data to.</param>
    /// <param name="name">Name of method to gather performance data about.</param>
    /// <param name="parameters">Parameter data to use when writing output.</param>
    void WritePerformanceData(string directoryPath, string methodName, object[] parameters)
    {
        // write data here
    }

    #endregion

    #region Events
    /// <summary>
    /// Fired when a property changes in the device.
    /// </summary>
    event EventHandler DeviceSaveRequired;
    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the version name of the hardware if applicable.
    /// </summary>
    string Version { get; set; }
    /// <summary>
    /// Gets or sets the status of the device.
    /// </summary>
    enumDeviceStatus Status { get; set; }
    /// <summary>
    /// Gets or sets the abort event for scheduling.
    /// </summary>
    ManualResetEvent AbortEvent { get; set; }
    /// <summary>
    /// Gets or sets the error type for a given device.
    /// </summary>
    enumDeviceErrorStatus ErrorType
    {
        get;
        set;
    }
    /// <summary>
    /// Gets or sets the device type.
    /// </summary>
    enumDeviceType DeviceType
    {
        get;
    }
    /// <summary>
    /// Gets or sets whether the device is emulation mode or not.
    /// </summary>
    bool Emulation
    { get; set; }
    #endregion
}
*/