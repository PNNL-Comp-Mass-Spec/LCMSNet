/*********************************************************************************************************
 * Written by Brian LaMarche, Dave Clark, John Ryan for the US Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2009, Battelle Memorial Institute
 * Created 06/19/2009
 *
 *********************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LcmsNetSDK.Data;
using LcmsNetSDK.Logging;

namespace LcmsNetSDK.Devices
{
    /// <summary>
    /// Device manager class for maintaining a list of all devices used by the application.
    /// </summary>
    public class classDeviceManager : IDeviceManager
    {
        private const string CONST_MOBILE_PHASE_NAME = "mobilephase-";
        private const string CONST_MOBILE_PHASE_COMMENT = "mobilephase-comment-";

        /*#region System Health
        /// <summary>
        /// Writes the system health to the provider.
        /// </summary>
        public void WriteSystemHealth(string path)
        {
            // Retrieve the data from the aggregates
            List<FinchAggregateData> aggregates = new List<FinchAggregateData>();
            FinchAggregateData aggregate = new FinchAggregateData();
            aggregate.Components         = new List<FinchComponentData>();
            aggregate.Error              = null;
            aggregate.LastUpdate         = DateTime.Now;
            aggregate.Status             = "";
            aggregate.Name               = LcmsNetDataClasses.classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTNAME);

            foreach (IDevice device in m_devices)
            {
                FinchComponentData data = device.GetData();
                if (data != null)
                {
                    aggregate.Components.Add(data);
                }
            }

            // Then write the aggregate data to file.
            aggregates.Add(aggregate);
            try
            {
                //writer.WriteAggregates(aggregates, path);
                FinchRestHttpClass http = new FinchRestHttpClass();
                http.URL                = Properties.Settings.Default.FinchServerURL;
                http.WriteAggregates(aggregates, path);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Could not create system health message data.", ex);
            }
        }
        #endregion*/

        #region Static Property

        /// <summary>
        /// Gets or sets the static device manager reference.
        /// </summary>
        public static classDeviceManager Manager => m_deviceManager ?? (m_deviceManager = new classDeviceManager());

        #endregion

        public void OnSettingChanged(object sender, SettingChangedEventArgs e)
        {
            if (e.SettingName.Equals("EmulationEnabled"))
            {
                Emulate = Convert.ToBoolean(e.SettingValue);
            }
        }

        #region Members

        /// <summary>
        /// A current list of devices the application is using.
        /// </summary>
        private readonly List<IDevice> m_devices;

        /// <summary>
        /// Static Device Manager Reference.
        /// </summary>
        private static classDeviceManager m_deviceManager;

        /// <summary>
        /// Fired when a device is successfully added.
        /// </summary>
        public event DelegateDeviceUpdated DeviceAdded;

        /// <summary>
        /// Fired when a device is successfully removed.
        /// </summary>
        public event DelegateDeviceUpdated DeviceRemoved;

        /// <summary>
        /// Fired when a device has been renamed.
        /// </summary>
        public event DelegateDeviceUpdated DeviceRenamed;

        /// <summary>
        /// Defines where pump methods are to be stored.
        /// </summary>
        public const string CONST_PUMP_METHOD_PATH = "PumpMethods";

        /// <summary>
        /// Path to the device plug-ins.
        /// </summary>
        public const string CONST_DEVICE_PLUGIN_PATH = "Plugins";

        /// <summary>
        /// Tag for the configuration file.
        /// </summary>
        private const string CONST_DEVICE_NAME_TAG = "DeviceName";

        /// <summary>
        /// Tag for the configuration file.
        /// </summary>
        private const string CONST_DEVICE_TYPE_TAG = "DeviceType";

        /// <summary>
        /// Tag for the configuration file.
        /// </summary>
        private const string CONST_DEVICE_TYPE_PATH = "PluginPath";

        /// <summary>
        /// Fired when status changes for the device manager.
        /// </summary>
        public event EventHandler<classDeviceManagerStatusArgs> InitialzingDevice;

        /// <summary>
        /// Fired when new plugins are loaded.
        /// </summary>
        public event EventHandler PluginsLoaded;

        /// <summary>
        /// Fired when all devices are initialized.
        /// </summary>
        public event EventHandler DevicesInitialized;

        /// <summary>
        /// A list of loaded plugin assemblies.
        /// </summary>
        private readonly Dictionary<string, List<classDevicePluginInformation>> m_plugins;

        /// <summary>
        /// Flag to indicate whether plug-ins are already being loaded via a directory operation.
        /// </summary>
        private bool m_loadingPlugins;

        /// <summary>
        /// Flag tracking whether the devices are emulated or not.
        /// </summary>
        private bool m_emulateDevices;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.  Sets the static device manager object reference to this.
        /// </summary>
        private classDeviceManager()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            m_plugins = new Dictionary<string, List<classDevicePluginInformation>>();
            AvailablePlugins = new List<classDevicePluginInformation>();
            m_devices = new List<IDevice>();
            //Manager              = this;

            // var bridge = new DeviceManagerBridge(this);

            m_loadingPlugins = false;
            m_emulateDevices = true;
            classLCMSSettings.SettingChanged += OnSettingChanged;
        }

        /// <summary>
        /// Found the assembly that was required.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var data = args.Name.Split(',');
            var basicName = data[0];
            var assemblyName = basicName + ".dll";
            var testPath = Path.Combine(CONST_DEVICE_PLUGIN_PATH, assemblyName);
            testPath = Path.GetFullPath(testPath);

            if (File.Exists(testPath))
            {
                return Assembly.LoadFile(testPath);
            }
            return null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of registered devices.
        /// </summary>
        public List<IDevice> Devices => m_devices;

        /// <summary>
        /// Gets the list of available plug-ins.
        /// </summary>
        public List<classDevicePluginInformation> AvailablePlugins { get; }

        /// <summary>
        /// Gets or sets whether to emulate the devices or not.
        /// </summary>
        public bool Emulate
        {
            get { return m_emulateDevices; }
            set
            {
                m_emulateDevices = value;
                SetEmulationFlags();
            }
        }

        /// <summary>
        /// Gets the device count.
        /// </summary>
        public int DeviceCount => m_devices.Count;

        /// <summary>
        /// Gets the number of initialized devices.
        /// </summary>
        public int InitializedDeviceCount
        {
            get
            {
                // Find out how many are initialized.
                var total = 0;
                foreach (var device in m_devices)
                {
                    if (device.Status == enumDeviceStatus.Initialized || device.Status == enumDeviceStatus.InUseByMethod)
                    {
                        total++;
                    }
                }

                return total;
            }
        }

        #endregion

        #region Configuration Extraction and Loading

        /// <summary>
        /// Loads a device
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public void LoadPersistentConfiguration(classDeviceConfiguration configuration)
        {
            var exceptionsToThrow = new List<Exception>();
            var dllFilePath = Assembly.GetExecutingAssembly().Location;
            var dllFolder = GetExecutingAssemblyFolderPath();

            for (var i = 0; i < configuration.DeviceCount; i++)
            {
                var deviceName = configuration[i];
                var settings = configuration.GetDeviceSettings(deviceName);

                // We may have to load the type from another assembly, so look it up.
                string path = null;
                if (settings.ContainsKey(CONST_DEVICE_TYPE_PATH))
                {
                    path = settings[CONST_DEVICE_TYPE_PATH] as string;
                    if (!Path.IsPathRooted(path))
                    {
                        // Assume this is a relative path, e.g.
                        //  Plugins\Agilent.dll
                        path = Path.Combine(dllFolder, path);
                    }
                }

                Type type = null;
                var typeDetermined = false;
                var deviceTypeName = GetSetting(settings, CONST_DEVICE_TYPE_TAG, "Undefined_DeviceType");

                if (path != null && dllFilePath != path)
                {
                    if (!File.Exists(path))
                    {
                        // Plugin DLL not found
                        // Check in the Plugins folder below the folder with the .exe
                        var pluginFilename = Path.GetFileName(path);
                        var alternatePath = Path.Combine(dllFolder, "Plugins", pluginFilename);
                        if (File.Exists(alternatePath))
                            path = alternatePath;
                    }

                    if (File.Exists(path))
                    {
                        foreach (var t in Assembly.LoadFrom(path).GetTypes())
                        {
                            // Compare t.FullName to typeName, e.g.
                            //  "Agilent.Properties.Resources"

                            // Alternatively use t.AssemblyQualifiedName, e.g.
                            //  "Agilent.Properties.Resources, Agilent, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
                            if (t.FullName == deviceTypeName)
                            {
                                type = t;
                                typeDetermined = true;
                                break;
                            }
                        }
                    }
                }

                if (!typeDetermined)
                {
                    // DLL not loaded; see if the DeviceType refers to a class already in memory
                    type = Type.GetType(deviceTypeName);
                }

                if (type != null && !typeof (IDevice).IsAssignableFrom(type))
                {
                    var name = GetSetting(settings, CONST_DEVICE_NAME_TAG, "Undefined_DeviceName");
                    exceptionsToThrow.Add(new InvalidCastException("The specified DeviceType (" + deviceTypeName + ") is invalid for DeviceName " + name));
                    continue;
                }

                // 1. Create an instance of the device.
                // 2. Name the device
                // 3. Extract settings and bind them to the device.

                IDevice device;

                try
                {
                    device = Activator.CreateInstance(type) as IDevice;
                    device.Name = CreateUniqueDeviceName(deviceName);
                }
                catch (Exception ex)
                {
                    exceptionsToThrow.Add(ex);
                    continue;
                }

                // Emulation needs to be set on devices before any other properties, otherwise it may not be seen properly on a check
                // for emulation mode at startup/initialization.
                device.Emulation = m_emulateDevices;

                // Get all writeable properties.
                var properties = type.GetProperties();

                // Then map them so we dont have to do a N^2 search to set a value.
                var propertyMap = new Dictionary<string, PropertyInfo>();
                foreach (var property in properties)
                {
                    var attributes = property.GetCustomAttributes(typeof (classPersistenceAttribute), true);
                    foreach (var o in attributes)
                    {
                        var setting = o as classPersistenceAttribute;
                        if (setting != null)
                        {
                            propertyMap.Add(setting.SettingName, property);
                        }
                    }
                }

                // Then finally load the values into the property.
                foreach (var key in settings.Keys)
                {
                    if (!propertyMap.ContainsKey(key) || !propertyMap[key].CanWrite)
                        continue;

                    var data = settings[key];

                    // Enumerations are a special breed.
                    var propertyType = propertyMap[key].PropertyType;
                    if (propertyType.IsEnum)
                    {
                        data = Enum.Parse(propertyType, data.ToString());
                    }
                    else
                    {
                        data = Convert.ChangeType(data, propertyMap[key].PropertyType);
                    }

                    try
                    {
                        propertyMap[key].SetValue(device, data, BindingFlags.SetProperty, null, null, null);
                    }
                    catch (Exception ex)
                    {
                        exceptionsToThrow.Add(ex);
                    }
                }

                var pump = device as IPump;
                if (pump == null)
                {
                    // Add the device.
                    AddDevice(device);
                    continue;
                }

                // Reconstruct any mobile phase data
                var phases = new Dictionary<int, MobilePhase>();
                pump.MobilePhases.Clear();

                foreach (var key in settings.Keys)
                {
                    var isComment = key.Contains(CONST_MOBILE_PHASE_COMMENT);
                    var isName = key.Contains(CONST_MOBILE_PHASE_NAME);
                    int phaseId;

                    if (isComment && int.TryParse(key.Replace(CONST_MOBILE_PHASE_COMMENT, ""), out phaseId))
                    {
                        if (!phases.ContainsKey(phaseId))
                        {
                            phases.Add(phaseId, new MobilePhase());
                        }
                        var phase = phases[phaseId];
                        var value = settings[key];
                        phase.Comment = value.ToString();
                    }
                    else if (isName && int.TryParse(key.Replace(CONST_MOBILE_PHASE_NAME, ""), out phaseId))
                    {
                        if (!phases.ContainsKey(phaseId))
                        {
                            phases.Add(phaseId, new MobilePhase());
                        }
                        var phase = phases[phaseId];
                        var value = settings[key];
                        phase.Name = value.ToString();
                    }
                }

                foreach (var phase in phases.Values)
                {
                    pump.MobilePhases.Add(phase);
                }

                // Add the device.
                AddDevice(device);
            }
        }

        /// <summary>
        /// Saves all devices to the configuration
        /// </summary>
        /// <param name="configuration"></param>
        public void ExtractToPersistConfiguration(ref classDeviceConfiguration configuration)
        {
            var dllFolder = GetExecutingAssemblyFolderPath();

            foreach (var device in Devices)
            {
                if ((device.DeviceType != enumDeviceType.Component) && (device.DeviceType != enumDeviceType.Fluidics))
                {
                    continue;
                }

                var deviceType = device.GetType();
                var properties = deviceType.GetProperties();

                // Store the device type name, e.g. Agilent.Properties.Resources
                configuration.AddSetting(device.Name, CONST_DEVICE_TYPE_TAG, deviceType.FullName);

                // Store the path to the plugin DLL
                // Using relative paths if the DLL is in the folder below the path tracked by dllFolder

                var pluginPath = deviceType.Assembly.Location;
                if (pluginPath != null && dllFolder.Length > 0)
                {
                    if (pluginPath.StartsWith(dllFolder, StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Store the relative path to the plugin DLL
                        pluginPath = pluginPath.Substring(dllFolder.Length);

                        // Trim the leading slash
                        pluginPath = pluginPath.TrimStart('\\');
                    }
                }

                configuration.AddSetting(device.Name, CONST_DEVICE_TYPE_PATH, pluginPath);

                configuration.AddSetting(device.Name, CONST_DEVICE_NAME_TAG, device.Name);

                foreach (var property in properties)
                {
                    if (!property.CanWrite)
                        continue;

                    var attributes = property.GetCustomAttributes(
                        typeof (classPersistenceAttribute),
                        true);
                    // Make sure the propety is tagged to be persisted.
                    if (attributes.Length < 1)
                        continue;

                    foreach (var attributeObject in attributes)
                    {
                        var settingAttribute =
                            attributeObject as classPersistenceAttribute;

                        if (settingAttribute != null)
                        {
                            var data = property.GetValue(device, BindingFlags.GetProperty,
                                null,
                                null,
                                null);
                            configuration.AddSetting(device.Name, settingAttribute.SettingName, data);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Return the path to the directory with the DLL running this code
        /// </summary>
        /// <returns>Directory path, or an empty string if the path cannot be determined</returns>
        private string GetExecutingAssemblyFolderPath()
        {

            var dllPath = Assembly.GetExecutingAssembly().Location;
            if (dllPath != null)
            {
                var dllInfo = new FileInfo(dllPath);
                return dllInfo.DirectoryName ?? "";
            }

            return string.Empty;

        }

        private string GetSetting(IDictionary<string, object> settings, string settingName, string valueIfMissing)
        {
            if (settings.ContainsKey(settingName))
            {
                var value = settings[settingName] as string;
                return value;
            }

            return valueIfMissing;

        }

        #endregion

        #region Device Naming, Usage, Add, Delete

        /// <summary>
        /// Searches the device manager for a device with the same name.
        /// </summary>
        /// <param name="deviceName">Name to search the device manager for.</param>
        /// <returns>True if device name is in use.  False if the device name is free</returns>
        public bool DeviceNameExists(string deviceName)
        {
            if (deviceName == null)
                return false;

            foreach (var dev in m_devices)
            {
                if (dev.Name.Equals(deviceName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Finds the devices that are named with the supplied name and type.
        /// </summary>
        /// <param name="deviceName">Name of the device of question.</param>
        /// <param name="deviceType">Type of the device of question.</param>
        /// <returns>IDevice object reference if it exists, null if it does not.</returns>
        public IDevice FindDevice(string deviceName, Type deviceType)
        {
            IDevice device = null;

            //
            // Find the device in the list, might be better if we used a
            // dictionary instead.
            //
            // Then see if the device type matches as well...
            //
            foreach (var dev in m_devices)
            {
                var devType = dev.GetType();
                if (dev.Name.Equals(deviceName, StringComparison.OrdinalIgnoreCase) && devType == deviceType)
                {
                    device = dev;
                    break;
                }
            }
            return device;
        }

        /// <summary>
        /// Finds a device just by name.
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public IDevice FindDevice(string deviceName)
        {
            IDevice device = null;

            //
            // Find the device in the list, might be better if we used a
            // dictionary instead.
            //
            // Then see if the device type matches as well...
            //
            foreach (var dev in m_devices)
            {
                if (dev.Name.Equals(deviceName, StringComparison.OrdinalIgnoreCase))
                {
                    device = dev;
                    break;
                }
            }
            return device;
        }

        /// <summary>
        /// Creates a unique device name from the basename provided.
        /// </summary>
        /// <param name="baseName"></param>
        /// <returns></returns>
        public string CreateUniqueDeviceName(string baseName)
        {
            var newName = baseName;
            var deviceCount = 0;

            // We expect that there are not an infinite number of device names
            while (DeviceNameExists(newName))
            {
                newName = baseName + deviceCount;
                deviceCount++;
            }

            return newName;
        }

        /// <summary>
        /// Renames the device with the given basename after checking to see if that name is reserved.
        /// </summary>
        /// <param name="device">Device to rename</param>
        /// <param name="basename">Name to use for the device.</param>
        public void RenameDevice(IDevice device, string basename)
        {
            var oldName = device.Name;

            //
            // If this happens, then they are trying to name the device
            // the same thing....
            //
            if (basename.Equals(oldName, StringComparison.OrdinalIgnoreCase))
                return;

            var newName = CreateUniqueDeviceName(basename);
            device.Name = newName;

            DeviceRenamed?.Invoke(this, device);
        }

        /// <summary>
        /// Adds the device to the device manager if the name is not a duplicate and the same object reference does not exist.
        /// </summary>
        /// <returns>True if the device was added.  False if device was not added.</returns>
        public bool AddDevice(IDevice device)
        {
            //
            // No null devices allowed.
            //
            if (device == null)
                return false;

            //
            // No duplicate references allowed.
            //
            if (m_devices.Contains(device))
                return false;

            //
            // No duplicate names allowed.
            //
            if (DeviceNameExists(device.Name))
                return false;

            device.Emulation = m_emulateDevices;

            m_devices.Add(device);

            DeviceAdded?.Invoke(this, device);

            return true;
        }

        /// <summary>
        /// Creates a new device based on the plug-in information.
        /// </summary>
        /// <param name="plugin">Device plug-in used to create a new device.</param>
        /// <param name="initialize">Indicates whether to initialize the device if added succesfully </param>
        /// <returns>True if successful, False if it fails.</returns>
        public bool AddDevice(classDevicePluginInformation plugin, bool initialize)
        {
            var device = Activator.CreateInstance(plugin.DeviceType) as IDevice;
            if (device == null)
            {
                return false;
            }

            device.Name = CreateUniqueDeviceName(device.Name);
            var added = AddDevice(device);

            if (added && initialize)
            {
                InitializeDevice(device);
            }
            return added;
        }

        /// <summary>
        /// Removes the device from the device manager.
        /// </summary>
        /// <param name="device">Device to remove.</param>
        /// <returns>True if device was removed successfully.  False if the device could not be removed at that time.</returns>
        public bool RemoveDevice(IDevice device)
        {
            //
            // Make sure we have the reference
            //
            if (m_devices.Contains(device) == false)
                return false;

            device.Shutdown();

            m_devices.Remove(device);

            DeviceRemoved?.Invoke(this, device);

            return true;
        }

        /// <summary>
        /// Updates devices with emulated flags
        /// </summary>
        private void SetEmulationFlags()
        {
            foreach (var device in m_devices)
            {
                device.Emulation = m_emulateDevices;
            }
        }

        #endregion

        #region Shutdown and Initialization

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public bool ShutdownDevices()
        {
            return ShutdownDevices(false);
        }

        /// <summary>
        /// Calls the shutdown method for each device.
        /// </summary>
        /// <returns>True if shutdown successful.  False if shutdown failed.</returns>
        public bool ShutdownDevices(bool clearDevices)
        {
            var worked = true;
            foreach (var device in m_devices)
            {
                worked = (worked && device.Shutdown());
            }

            if (clearDevices)
            {
                var tempDevices = new List<IDevice>();
                tempDevices.AddRange(m_devices);
                foreach (var device in tempDevices)
                {
                    try
                    {
                        RemoveDevice(device);
                    }
                    catch
                    {
                        //TODO: CRAP!  what happens if the device cannot be removed.
                    }
                }
            }

            return worked;
        }

        /// <summary>
        /// Initializes the device.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public bool InitializeDevice(IDevice device)
        {
            InitialzingDevice?.Invoke(this, new classDeviceManagerStatusArgs("Initializing " + device.Name));
            try
            {
                var errorMessage = "";
                var initialized = device.Initialize(ref errorMessage);
                if (initialized == false)
                {
                    device.Status = enumDeviceStatus.Error;
                    // We wrap error details in the event args class so that it can also be used via a delegate
                    // And then shove it into an exception class.  this way we force the
                    // caller to handle any exception, but allow them to propogate to any observers.
                    var args = new classDeviceErrorEventArgs(
                        errorMessage,
                        null,
                        enumDeviceErrorStatus.ErrorAffectsAllColumns,
                        device);
                    throw new classDeviceInitializationException(args);
                }
                device.Status = enumDeviceStatus.Initialized;
            }
            catch (classDeviceInitializationException)
            {
                device.Status = enumDeviceStatus.Error;
                throw;
            }
            catch (Exception ex)
            {
                device.Status = enumDeviceStatus.Error;
                var args = new classDeviceErrorEventArgs("Error intializing device.",
                    ex,
                    enumDeviceErrorStatus.ErrorAffectsAllColumns,
                    device);
                throw new classDeviceInitializationException(args);
            }

            return true;
        }

        /// <summary>
        /// Initializes all the devices if they have not been initialized already.
        /// </summary>
        /// <returns></returns>
        public List<classDeviceErrorEventArgs> InitializeDevices(bool reinitializeAlreadyInitialized)
        {
            var devices = new List<classDeviceErrorEventArgs>();
            foreach (var device in m_devices)
            {
                try
                {
                    var alreadyInitialized = (device.Status == enumDeviceStatus.Initialized ||
                                               device.Status == enumDeviceStatus.Initialized);
                    if (!reinitializeAlreadyInitialized && !alreadyInitialized)
                    {
                        InitializeDevice(device);
                    }
                    else
                    {
                        InitializeDevice(device);
                    }
                }
                catch (classDeviceInitializationException ex)
                {
                    classApplicationLogger.LogError(0,
                        string.Format("{0} could not be initialized.  {1}",
                            device.Name,
                            ex.ErrorDetails.Error), ex.ErrorDetails.Exception);
                    devices.Add(ex.ErrorDetails);
                }
            }

            DevicesInitialized?.Invoke(this, new EventArgs());
            return devices;
        }

        /// <summary>
        /// Initializes all the devices if they have not been initialized already.
        /// </summary>
        /// <returns></returns>
        public List<classDeviceErrorEventArgs> InitializeDevices()
        {
            return InitializeDevices(true);
        }

        #endregion

        #region Plug-in Management

        /// <summary>
        /// Loads the satellite assemblies required for type checking.
        /// </summary>
        /// <param name="path"></param>
        public void LoadSatelliteAssemblies(string path)
        {
            try
            {
                var files = Directory.GetFiles(path, "*.dll");
                foreach (var file in files)
                {
                    try
                    {
                        var fullPath = Path.GetFullPath(file);
                        Assembly.LoadFile(fullPath);
                    }
                    catch (BadImageFormatException)
                    {
                        classApplicationLogger.LogMessage(0,
                            string.Format("The dll {0} is not a .net assembly.  Skipping.", file));
                    }
                    catch (Exception ex)
                    {
                        classApplicationLogger.LogError(0, "Could not load satellite assemblies", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Could not load satellite assemblies", ex);
            }
        }

        /// <summary>
        /// Loads supported device plugin types.
        /// </summary>
        /// <param name="assembly">Assembly to load device types from.</param>
        /// <param name="forceReload">Flag indicating whether to force a re-load the assemblies if they have already been loaded.</param>
        public void LoadPlugins(Assembly assembly, bool forceReload)
        {
            var assemblyPath = assembly.Location;
            if (assemblyPath == null)
                return;

            if (m_plugins.ContainsKey(assemblyPath))
            {
                if (!forceReload)
                {
                    throw new Exception("The plug-in assembly has already been loaded.");
                }
                // Remove the old plug-ins from the list.
                foreach (var plugin in m_plugins[assemblyPath])
                {
                    AvailablePlugins.Remove(plugin);
                }
                // Remove the old plug-in link in the plug-in dictionary.
                m_plugins.Remove(assemblyPath);
            }

            // Map the assembly path to a list of available plug-ins and also update the list of available plug-ins
            var supportedPlugins = RetrieveSupportedDevicePluginTypes(assembly);
            AvailablePlugins.AddRange(supportedPlugins);
            m_plugins.Add(assemblyPath, supportedPlugins);
        }

        /// <summary>
        /// Retrieves plug-ins from the assembly at the path provided.
        /// </summary>
        /// <param name="assemblyPath">File path to assembly.</param>
        /// <param name="forceReload">Flag indicating whether to force a re-load the assemblies if they have already been loaded.</param>
        public void LoadPlugins(string assemblyPath, bool forceReload)
        {
            if (m_plugins.ContainsKey(assemblyPath))

            {
                if (!forceReload)
                {
                    throw new Exception("The plug-in assembly has already been loaded.");
                }
                // Remove the old plug-ins from the list.
                foreach (var plugin in m_plugins[assemblyPath])
                {
                    AvailablePlugins.Remove(plugin);
                }
                // Remove the old plug-in link in the plug-in dictionary.
                m_plugins.Remove(assemblyPath);
            }

            // Map the assembly path to a list of available plug-ins and also update the list of available plug-ins
            var supportedPlugins =
                RetrieveSupportedDevicePluginTypes(Path.GetFullPath(assemblyPath));
            AvailablePlugins.AddRange(supportedPlugins);
            m_plugins.Add(assemblyPath, supportedPlugins);

            if (PluginsLoaded != null && !m_loadingPlugins)
            {
                PluginsLoaded(this, null);
            }
        }

        /// <summary>
        /// Loads supported device plugin types from a directory.
        /// </summary>
        /// <param name="directoryPath">Directory of assemblies to load.</param>
        /// <param name="filter">Assembly file filter.</param>
        /// <param name="forceReload">Flag indicating whether to force a re-load the assemblies if they have already been loaded.</param>
        public void LoadPlugins(string directoryPath, string filter, bool forceReload)
        {
            // Signal others we are the ones doing the loading and alerting.
            m_loadingPlugins = true;

            var files = Directory.GetFiles(directoryPath, filter);
            foreach (var assemblyPath in files)
            {
                LoadPlugins(assemblyPath, forceReload);
            }
            m_loadingPlugins = false;

            PluginsLoaded?.Invoke(this, null);
        }

        /// <summary>
        /// Loads supported device plugin types.
        /// </summary>
        /// <param name="assembly">Assembly to load device types from.</param>
        /// <returns>All types that support IDevice and have been attributed with a glyph and device control attribute.</returns>
        private List<classDevicePluginInformation> RetrieveSupportedDevicePluginTypes(Assembly assembly)
        {
            var supportedTypes = new List<classDevicePluginInformation>();

            var types = assembly.GetExportedTypes();
            foreach (var objectType in types)
            {
                // Map the controls
                if (typeof (IDevice).IsAssignableFrom(objectType))
                {
                    var attributes = objectType.GetCustomAttributes(typeof (classDeviceControlAttribute), true);
                    foreach (var attribute in attributes)
                    {
                        var control = attribute as classDeviceControlAttribute;
                        if (control != null)
                        {
                            //TODO: Brian changed this...kind of chris made him do it...but we are going to revisit all of the bad things that could happen
                            // if we left this thing uncommented...basically trying to transition from this crap anyway...
                            //if (control.GlyphType != null && control.ControlType != null)
                            {
                                var pluginInfo = new classDevicePluginInformation(objectType,
                                    control);
                                supportedTypes.Add(pluginInfo);
                            }
                        }
                    }
                }
            }

            return supportedTypes;
        }

        /// <summary>
        /// Retrieves plug-ins from the assembly at the path provided.
        /// </summary>
        /// <param name="assemblyPath">File path to assembly.</param>
        /// <returns>All types that support IDevice and have been attributed with a glyph and device control attribute.</returns>
        private List<classDevicePluginInformation> RetrieveSupportedDevicePluginTypes(string assemblyPath)
        {
            var supportedTypes = new List<classDevicePluginInformation>();
            try
            {
                var fileAssembly = Assembly.LoadFile(assemblyPath);
                var subTypes = RetrieveSupportedDevicePluginTypes(fileAssembly);
                supportedTypes.AddRange(subTypes);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Could not load the plugin from dll: " + assemblyPath, ex);
                //TODO: throw exception ! let people know this failed.
            }
            return supportedTypes;
        }

        #endregion
    }

    /// <summary>
    ///
    /// </summary>
    public class classDeviceInitializationException : Exception
    {
        public classDeviceInitializationException(classDeviceErrorEventArgs errorArgs)
        {
            ErrorDetails = errorArgs;
        }

        public classDeviceErrorEventArgs ErrorDetails { get; }
    }
}