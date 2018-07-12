using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Media;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;
using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    public delegate void DelegateLCMethodEventOptimize(object sender, bool optimize);

    public delegate void DelegateLCMethodEventLocked(object sender, bool enabled, LCMethodData methodData);

    /// <summary>
    /// LC-Method User Interface for building an LC-Method.
    /// </summary>
    public class LCMethodEventViewModel : ReactiveObject
    {
        #region Static Data

        /// <summary>
        /// List of device methods and parameters to use.
        /// </summary>
        private static readonly Dictionary<IDevice, List<LCMethodData>> DeviceMappings = new Dictionary<IDevice, List<LCMethodData>>();

        private static readonly ReactiveList<IDevice> DevicesList = new ReactiveList<IDevice>();

        static LCMethodEventViewModel()
        {
            // Add the devices to the method editor
            RegisterDevices();

            // Register to listen for device additions or deletions.
            DeviceManager.Manager.DeviceAdded += Manager_DeviceAdded;
            DeviceManager.Manager.DeviceRemoved += Manager_DeviceRemoved;
        }

        #endregion

        /// <summary>
        /// Default constructor for the event view model that takes no arguments
        /// Calling this constructor is only for the IDE designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public LCMethodEventViewModel() : this(1)
        { }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public LCMethodEventViewModel(int eventNum)
        {
            SelectedDevice = null;
            EventNumber = eventNum.ToString();
            StoppedHere = false;

            Breakpoint = new BreakpointViewModel();

            // Handle user interface events to display context of method editors
            this.WhenAnyValue(x => x.SelectedDevice).Subscribe(x => this.SelectedDeviceChanged());
            this.WhenAnyValue(x => x.SelectedLCMethod).Subscribe(x => this.SelectedMethodChanged());
            this.WhenAnyValue(x => x.OptimizeWith).Subscribe(x => this.OptimizeForChanged());
            Breakpoint.BreakpointChanged += Breakpoint_Changed;

            if (DevicesList.Count > 0)
            {
                SelectedDevice = DevicesList[0];
                UpdateSelectedDevice();
            }

            this.WhenAnyValue(x => x.DevicesComboBoxOptions.Count).Select(x => x > 0).ToProperty(this, x => x.DevicesComboBoxEnabled, out devicesComboBoxEnabled);
            DevicesList.ItemsAdded.Subscribe(_ => this.DeviceAdded());
        }

        /// <summary>
        /// Constructor for a LC event.
        /// </summary>
        /// <param name="methodData"></param>
        /// <param name="locked"></param>
        public LCMethodEventViewModel(LCMethodData methodData, bool locked) : this(1)
        {
            SelectedDevice = methodData.Device;
            // Every device is a reference held in the device manager...except for the timer
            // object.  This object is created every time as a non-critical object because
            // it does not require simultaneous use by various threads.  However, we have to
            // make sure here the device is not a timer, because if it is, then we must
            // find the device name string "Timer" from the combo box so we can
            // get the right reference.
            if (!(methodData.Device is TimerDevice))
            {
                SelectedDevice = methodData.Device;
            }
            else
            {
                foreach (var d in DevicesComboBoxOptions)
                {
                    if (d.Name.Equals("Timer"))
                    {
                        SelectedDevice = d;
                        break;
                    }
                }
            }

            EventUnlocked = (locked == false);

            OptimizeWith = methodData.OptimizeWith;

            if (locked)
            {
                // Create a dummy classLCMethodData
                var tempObj = new LCMethodData(null, null, new LCMethodEventAttribute("Unlock", 0.0, "", 0, false), null);
                methodsComboBoxOptions.Add(tempObj);
                SelectedLCMethod = tempObj;
            }
            else
            {
                LoadMethodInformation(SelectedDevice);
                // We have to do some reference trickery here, since method data is reconstructed
                // for every lcevent then we need to set the method data for this object
                // with the method we previously selected. This way we preserve the parameter values etc.
                var index = FindMethodIndex(methodData);
                methodsComboBoxOptions[index] = methodData;
                SelectedLCMethod = methodData;

                LoadMethodParameters(methodData);
            }

            IsLockingEvent = locked;
            this.methodData = methodData;
        }

        /// <summary>
        /// Fired when the user wants to optimize with this event.
        /// </summary>
        public event DelegateLCMethodEventOptimize UseForOptimization;

        /// <summary>
        /// Fired when this event is to be locked.
        /// </summary>
#pragma warning disable CS0067 // this is actually used in LCMethodStageViewModel...
        public event DelegateLCMethodEventLocked Lock;
#pragma warning restore CS0067

        /// <summary>
        /// Fired when an event changes.
        /// </summary>
        public event EventHandler EventChanged;

        ~LCMethodEventViewModel()
        {
            if (methodData != null)
            {
                methodData.BreakPointEvent -= BreakPointEvent_Handler;
                methodData.Simulated -= Simulated_Handler;
                methodData.SimulatingEvent -= Simulating_Handler;
            }
        }

        void Breakpoint_Changed(object sender, BreakpointArgs e)
        {
            if (methodData != null)
                methodData.BreakPoint = e.IsSet;
        }

        private int FindMethodIndex(LCMethodData method)
        {
            var i = 0;
            foreach (var data in MethodsComboBoxOptions)
            {
                if (data != null)
                {
                    if (data.MethodAttribute.Name.Equals(method.MethodAttribute.Name))
                        return i;
                }
                i++;
            }
            return -1;
        }

        #region Members

        /// <summary>
        /// Method data that has been selected to be displayed.
        /// </summary>
        private LCMethodData methodData;

        private BreakpointViewModel breakpoint;
        private string eventNumber = "1";
        private bool optimizeWith = false;
        private IDevice selectedDevice = null;
        private LCMethodData selectedLCMethod = null;
        private readonly ReactiveList<LCMethodData> methodsComboBoxOptions = new ReactiveList<LCMethodData>();
        private readonly ReactiveList<EventParameterViewModel> eventParameterList = new ReactiveList<EventParameterViewModel>();
        private bool isSelected = false;
        private readonly ObservableAsPropertyHelper<bool> devicesComboBoxEnabled;

        #endregion

        #region Properties

        public BreakpointViewModel Breakpoint
        {
            get { return breakpoint; }
            set { this.RaiseAndSetIfChanged(ref breakpoint, value); }
        }

        public string EventNumber
        {
            get { return eventNumber; }
            set { this.RaiseAndSetIfChanged(ref eventNumber, value); }
        }

        /// <summary>
        /// The selected device
        /// </summary>
        public IDevice SelectedDevice
        {
            get { return selectedDevice; }
            set { this.RaiseAndSetIfChanged(ref selectedDevice, value); }
        }

        /// <summary>
        /// The selected method
        /// </summary>
        public LCMethodData SelectedLCMethod
        {
            get { return selectedLCMethod; }
            set { this.RaiseAndSetIfChanged(ref selectedLCMethod, value); }
        }

        public IReadOnlyReactiveList<IDevice> DevicesComboBoxOptions => DevicesList;
        public IReadOnlyReactiveList<LCMethodData> MethodsComboBoxOptions => methodsComboBoxOptions;
        public IReadOnlyReactiveList<EventParameterViewModel> EventParameterList => eventParameterList;
        public bool DevicesComboBoxEnabled => devicesComboBoxEnabled.Value && EventUnlocked;
        public bool EventUnlocked { get; }

        public bool IsSelected
        {
            get { return isSelected; }
            set { this.RaiseAndSetIfChanged(ref isSelected, value); }
        }

        /// <summary>
        /// Gets the method selected to run by the user.
        /// </summary>
        public LCMethodData SelectedMethod
        {
            get
            {
                if (methodData != null)
                {
                    // Link the method to a device
                    methodData.Device = SelectedDevice;
                    methodData.OptimizeWith = OptimizeWith;
                    methodData.BreakPoint = Breakpoint.IsSet;

                    // Make sure that we build the method so that the values are updated
                    // from the control used to interface them....
                    methodData.BuildMethod();

                    methodData.BreakPointEvent += BreakPointEvent_Handler;
                    methodData.Simulated += Simulated_Handler;
                    methodData.SimulatingEvent += Simulating_Handler;
                }

                return methodData;
            }
        }

        public void SetBreakPoint(bool value)
        {
            Breakpoint.IsSet = value;
        }

        public SolidColorBrush EventBackColor
        {
            get
            {
                if (IsCurrent && !StoppedHere)
                {
                    return Brushes.Green;
                }
                if (StoppedHere)
                {
                    return Brushes.Yellow;
                }
                if (methodData != null && methodData.BreakPoint)
                {
                    return Brushes.Maroon;
                }
                return Brushes.White;
            }
        }

        /// <summary>
        /// Gets or sets whether to optimize the method alignment with this event.
        /// </summary>
        public bool OptimizeWith
        {
            get { return optimizeWith; }
            set { this.RaiseAndSetIfChanged(ref optimizeWith, value); }
        }

        /// <summary>
        /// Flag indicating if this event is a placeholder so that we know it's a locking event
        /// </summary>
        public bool IsLockingEvent { get; }

        private bool IsCurrent { get; set; }

        private bool StoppedHere { get; set; }

        #endregion

        #region Device Manager Event Listeners (static)

        /// <summary>
        /// Updates the list of available devices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        private static void Manager_DeviceRemoved(object sender, IDevice device)
        {
            if (device.DeviceType == DeviceType.Fluidics)
                return;

            if (DevicesList.Contains(device))
                DevicesList.Remove(device);

            if (DeviceMappings.ContainsKey(device))
                DeviceMappings.Remove(device);
        }

        /// <summary>
        /// Updates the list of available devices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        private static void Manager_DeviceAdded(object sender, IDevice device)
        {
            if (device.DeviceType == DeviceType.Fluidics)
                return;

            if (DevicesList.Contains(device) == false)
            {
                using (DevicesList.SuppressChangeNotifications())
                {
                    DevicesList.Add(device);
                    DevicesList.Sort((x, y) => x.Name.CompareTo(y.Name));
                }
            }

            if (DeviceMappings.ContainsKey(device) == false)
            {
                var methodPairs = ReflectDevice(device);
                DeviceMappings.Add(device, methodPairs);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles automatically selecting a device when there were no available devices before
        /// </summary>
        private void DeviceAdded()
        {
            // Make sure we select a device
            if (DevicesList.Count == 1)
            {
                SelectedDevice = DevicesList[0];
                UpdateSelectedDevice();
            }
        }

        /// <summary>
        /// Displays the given device method names and selected controls.
        /// </summary>
        /// <param name="device"></param>
        private void LoadMethodInformation(IDevice device)
        {
            // Make sure the device is not null
            if (device == null)
                return;

            // We know that timer devices are always re-created.  Meaning their references
            // are not persisted throughout a method or across methods.  Thus we need to test to see
            // if this is true, if so, then we need to just find the device in the
            // device manager and get the device method mappings that way.
            if (device is TimerDevice)
            {
                // Find the timer device.
                foreach (var tempDevice in DeviceMappings.Keys)
                {
                    if (tempDevice is TimerDevice)
                    {
                        device = tempDevice;
                        break;
                    }
                }
            }
            // Add the method information into the combo-box as deemed by the device.
            var methods = DeviceMappings[device];
            using (methodsComboBoxOptions.SuppressChangeNotifications())
            {
                // Clear out the combo-box
                methodsComboBoxOptions.Clear();
                methodsComboBoxOptions.AddRange(methods);
            }

            if (methodsComboBoxOptions.Count > 0)
            {
                SelectedLCMethod = methodsComboBoxOptions[0];
                UpdateSelectedMethod();
            }
        }

        /// <summary>
        /// Displays the given device method names and selected controls.
        /// </summary>
        /// <param name="method"></param>
        private void LoadMethodParameters(LCMethodData method)
        {
            // Make sure the device is not null
            if (method == null)
                return;

            // Clear out the combo-box
            try
            {
                foreach (var vm in eventParameterList)
                {
                    vm.EventChanged -= param_EventChanged;
                }
            }
            catch
            {
            }
            using (eventParameterList.SuppressChangeNotifications())
            {
                eventParameterList.Clear();
            }
            //mpanel_parameters.ColumnStyles.Clear();

            // If the method requires sample input then we just ignore adding any controls.
            var parameters = method.Parameters;

            // This readjusts the number of parameters that are sample specific
            var count = parameters.Controls.Count * 2;
            var indexOfSampleData = method.MethodAttribute.SampleParameterIndex;

            // Update the style so we have the right spacing
            //var percent = 100.0F / Convert.ToSingle(count);
            //for (var i = 0; i < parameters.Controls.Count; i++)
            //{
            //    if (i != indexOfSampleData)
            //        mpanel_parameters.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, percent));
            //}

            // Add the controls to the mpanel_parameter controls
            for (var j = 0; j < parameters.Controls.Count; j++)
            {
                if (j != indexOfSampleData)
                {
                    // Get the name of the parameter
                    var name = parameters.Names[j];

                    // Get the control for the parameter
                    var control = parameters.Controls[j];

                    // Construct the description for the parameter
                    var vm = control as EventParameterViewModel;
                    vm.ParameterLabel = name;

                    // Add the control itself
                    eventParameterList.Add(vm);

                    vm.ParameterValue = parameters.Values[j];
                    vm.EventChanged += param_EventChanged;
                }
            }
        }

        void param_EventChanged(object sender, EventArgs e)
        {
            OnEventChanged();
        }

        /// <summary>
        /// Updates the method and device user interface items.
        /// </summary>
        private void UpdateSelectedDevice()
        {
            // Update the user interface.
            if (SelectedDevice != null)
            {
                if (DeviceMappings.ContainsKey(SelectedDevice) == false)
                {
                    ReflectDevice(SelectedDevice);
                }
                LoadMethodInformation(SelectedDevice);
            }
        }

        private void UpdateSelectedMethod()
        {
            // Update the user interface.
            var method = SelectedLCMethod;
            if (method != methodData)
            {
                if (methodData != null)
                {
                    methodData.BreakPointEvent -= BreakPointEvent_Handler;
                    methodData.Simulated -= Simulated_Handler;
                }
                methodData = method;
                if (method != null)
                {
                    methodData.BreakPointEvent += BreakPointEvent_Handler;
                    methodData.Simulated += Simulated_Handler;
                    if (SelectedDevice != null)
                    {
                        LoadMethodParameters(methodData);
                    }
                }
            }
        }

        /// <summary>
        /// Fires the event changed event.
        /// </summary>
        private void OnEventChanged()
        {
            EventChanged?.Invoke(this, null);
        }

        /// <summary>
        /// Updates the label to display which number this event is in the list of events.
        /// </summary>
        /// <param name="eventNum">integer representing the place in the event list of this event</param>
        public void UpdateEventNum(int eventNum)
        {
            EventNumber = eventNum.ToString();
        }

        #endregion

        #region Registration and Reflection (static)

        /// <summary>
        /// Registers the devices with the user interface from the device manager.
        /// </summary>
        private static void RegisterDevices()
        {
            foreach (var device in DeviceManager.Manager.Devices.Where(x => x.DeviceType != DeviceType.Fluidics).OrderBy(x => x.Name))
            {
                var methodPairs = ReflectDevice(device);
                DeviceMappings.Add(device, methodPairs);
                DevicesList.Add(device);
            }
        }

        /// <summary>
        /// Reflects the given device and puts the method and parameter information in the appropiate combo boxes.
        /// </summary>
        public static List<LCMethodData> ReflectDevice(IDevice device)
        {
            if (device == null)
                throw new NullReferenceException("Device cannot be null.");

            var type = device.GetType();

            // List of method editing pairs
            var methodPairs = new List<LCMethodData>();

            // We are trying to enumerate all the methods for this device building their method-parameter pairs.
            foreach (var method in type.GetMethods())
            {
                var customAttributes = method.GetCustomAttributes(typeof(LCMethodEventAttribute), true);
                foreach (var objAttribute in customAttributes)
                {
                    // If the method has a custom LC Method Attribute, then we want to look at the parameters used
                    var attr = objAttribute as LCMethodEventAttribute;
                    if (attr != null)
                    {
                        // Grab the parameters used for this method
                        var info = method.GetParameters();
                        var parameters = new LCMethodEventParameter();

                        // Here we are looking to see if the method has a parameter
                        // that requires a data provider.
                        if (info.Length > 0)
                        {
                            // Make sure that we have parameter data, and also make sure that
                            // the parameter we are going to use is a sample data object.
                            // Then for each parameter, see if we can add it to a control to display.
                            var i = 0;
                            foreach (var paramInfo in info)
                            {
                                ILCEventParameter control = null;
                                object value = null;

                                // If the method editor has to use sample data then
                                // we skip adding a control...but allow for
                                // other data to be loaded.
                                if (attr.RequiresSampleInput && i == attr.SampleParameterIndex)
                                {
                                    parameters.AddParameter(null, null, paramInfo.Name, attr.DataProvider);
                                }
                                else if (string.IsNullOrEmpty(attr.DataProvider) == false && i == attr.DataProviderIndex)
                                {
                                    // Figure out what index to adjust the data provider for.
                                    var combo = new EventParameterViewModel(EventParameterViewModel.ParameterTypeEnum.Enum);

                                    // Register the event to automatically get new data when the data provider has new stuff.
                                    device.RegisterDataProvider(attr.DataProvider, combo.FillData);
                                    control = combo;
                                    // Set the data if we have it, otherwise, cross your fingers batman!
                                    if (combo.ComboBoxOptions.Count > 0)
                                        value = combo.ComboBoxOptions[0];
                                    parameters.AddParameter(value, control, paramInfo.Name, attr.DataProvider);
                                }
                                else
                                {
                                    // Get a control to display
                                    control = GetEventParametersFromType(paramInfo.ParameterType);

                                    // We need to get a default value, so just ask the
                                    // type for it.
                                    if (paramInfo.ParameterType.IsEnum)
                                    {
                                        var aenums = Enum.GetValues(paramInfo.ParameterType);
                                        var enums = new object[aenums.Length];
                                        aenums.CopyTo(enums, 0);
                                        value = enums[0];
                                    }

                                    // If the control is not null, then we can add it to display.
                                    // If it is null, then it is of a type we know nothing about.
                                    // And well you're SOL.
                                    if (control != null)
                                    {
                                        parameters.AddParameter(value, control, paramInfo.Name, attr.DataProvider);
                                    }
                                }
                                i++;
                            }
                        }

                        // Construct the new method from what we found
                        // during the reflection phase and add it to the list of
                        // possible methods to call for this device.
                        var newMethod = new LCMethodData(device, method, attr, parameters);
                        methodPairs.Add(newMethod);
                    }
                }
            }
            return methodPairs;
        }

        /// <summary>
        /// Hashset to facilitate checking to see if a type is numeric. Use like 'NumericTypes.Contains(testType)'
        /// </summary>
        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(int),
            typeof(uint),
            typeof(ulong),
            typeof(long),
            typeof(short),
            typeof(ushort),
            typeof(double),
            typeof(float),
        };

        /// <summary>
        /// Given a parameter type, figure out what kind of control is associated with it.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static EventParameterViewModel GetEventParametersFromType(Type t)
        {
            EventParameterViewModel control = null;

            if (t.IsEnum)
            {
                // Add the parameters to the combo box before we do anything.
                control = new EventParameterViewModel(EventParameterViewModel.ParameterTypeEnum.Enum);
                using (control.ComboBoxOptions.SuppressChangeNotifications())
                {
                    // Grab the enumeration values for the parameter
                    control.FillData(null, Enum.GetValues(t).Cast<object>().ToList());
                }
                control.SelectedOption = control.ComboBoxOptions.FirstOrDefault();
            }
            else if (NumericTypes.Contains(t))
            {
                control = new EventParameterViewModel(EventParameterViewModel.ParameterTypeEnum.Numeric);
            }
            else if (typeof(string) == t)
            {
                control = new EventParameterViewModel(EventParameterViewModel.ParameterTypeEnum.Text);
            }
            else if (typeof(bool) == t)
            {
                control = new EventParameterViewModel(EventParameterViewModel.ParameterTypeEnum.Boolean);
            }

            return control;
        }

        #endregion

        #region Event handlers

        private void BreakPointEvent_Handler(object sender, BreakEventArgs e)
        {
            StoppedHere = e.IsStopped;
            //TODO: What?: Refresh();
        }

        private void Simulating_Handler(object sender, EventArgs e)
        {
            IsCurrent = true;
            //TODO: What?: Refresh();
        }

        private void Simulated_Handler(object sender, EventArgs e)
        {
            IsCurrent = false;
            //TODO: What?: Refresh();
        }

        /// <summary>
        ///  when the device changes update the user interface.
        /// </summary>
        private void SelectedDeviceChanged()
        {
            UpdateSelectedDevice();
            OnEventChanged();
        }

        /// <summary>
        /// Handles when the user selects a new method for the current device.
        /// </summary>
        private void SelectedMethodChanged()
        {
            UpdateSelectedMethod();
            OnEventChanged();
        }

        /// <summary>
        /// Tells the other devices that this method should be used in optimization.
        /// </summary>
        private void OptimizeForChanged()
        {
            UseForOptimization?.Invoke(this, OptimizeWith);
            if (methodData != null)
                methodData.OptimizeWith = OptimizeWith;
            OnEventChanged();
        }

        #endregion
    }
}
