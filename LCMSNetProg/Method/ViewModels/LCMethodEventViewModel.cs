using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Media;
using DynamicData;
using DynamicData.Binding;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;
using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    public delegate void DelegateLCMethodEventOptimize(object sender, bool optimize);

    public delegate void DelegateLCMethodEventLocked(object sender, bool enabled, LCMethodEventData methodData);

    /// <summary>
    /// LC-Method User Interface for building an LC-Method.
    /// </summary>
    public class LCMethodEventViewModel : ReactiveObject
    {
        #region Static Data

        /// <summary>
        /// List of device methods and parameters to use.
        /// </summary>
        private static readonly Dictionary<IDevice, List<LCMethodEventData>> DeviceMappings = new Dictionary<IDevice, List<LCMethodEventData>>();

        private static readonly SourceList<IDevice> DevicesList = new SourceList<IDevice>();

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
            EventNumber = eventNum;
            StoppedHere = false;
            EventUnlocked = true;

            Breakpoint = new BreakpointViewModel();

            // Handle user interface events to display context of method editors
            this.WhenAnyValue(x => x.SelectedDevice).Subscribe(x => this.SelectedDeviceChanged());
            this.WhenAnyValue(x => x.SelectedLCEvent).Subscribe(x => this.SelectedMethodEventChanged());
            this.WhenAnyValue(x => x.OptimizeWith).Subscribe(x => this.OptimizeForChanged());
            Breakpoint.BreakpointChanged += Breakpoint_Changed;

            if (DevicesList.Count > 0)
            {
                SelectedDevice = DevicesList.Items.First();
                UpdateSelectedDevice();
            }

            DevicesList.Connect().WhereReasonsAre(ListChangeReason.Add, ListChangeReason.AddRange).Subscribe(_ => this.DeviceAdded());
            var resortTrigger = DevicesList.Connect().WhenValueChanged(x => x.Name).Throttle(TimeSpan.FromMilliseconds(250)).Select(_ => Unit.Default);
            DevicesList.Connect().Sort(SortExpressionComparer<IDevice>.Ascending(x => x.Name), resort: resortTrigger).ObserveOn(RxApp.MainThreadScheduler).Bind(out var devicesBound).Subscribe();
            DevicesComboBoxOptions = devicesBound;

            methodsComboBoxOptions.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var methodsBound).Subscribe();
            MethodsComboBoxOptions = methodsBound;
            eventParameterList.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var eventParametersBound).Subscribe();
            EventParameterList = eventParametersBound;

            devicesComboBoxEnabled = this.WhenAnyValue(x => x.DevicesComboBoxOptions.Count).Select(x => x > 0).ToProperty(this, x => x.DevicesComboBoxEnabled);
        }

        /// <summary>
        /// Constructor for a LC event.
        /// </summary>
        /// <param name="methodData"></param>
        /// <param name="locked"></param>
        public LCMethodEventViewModel(LCMethodEventData methodData, bool locked) : this(1)
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

            EventUnlocked = !locked;

            OptimizeWith = methodData.OptimizeWith;

            if (locked)
            {
                // Create a dummy classLCMethodData
                var tempObj = new LCMethodEventData(null, null, new LCMethodEventAttribute("Unlock", 0.0), null);
                methodsComboBoxOptions.Add(tempObj);
                SelectedLCEvent = tempObj;
            }
            else
            {
                LoadMethodInformation(SelectedDevice);
                // We have to do some reference trickery here, since method data is reconstructed
                // for every lcevent then we need to set the method data for this object
                // with the method we previously selected. This way we preserve the parameter values etc.
                methodsComboBoxOptions.Edit(list =>
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        var data = list[i];
                        if (data != null && data.MethodEventAttribute.Name.Equals(methodData.MethodEventAttribute.Name))
                        {
                            list[i] = methodData;
                            break;
                        }
                    }
                });

                SelectedLCEvent = methodData;

                LoadMethodEventParameters(methodData);
            }

            IsLockingEvent = locked;
            this.methodEventData = methodData;
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
            if (methodEventData != null)
            {
                methodEventData.BreakPointEvent -= BreakPointEvent_Handler;
                methodEventData.Simulated -= Simulated_Handler;
                methodEventData.SimulatingEvent -= Simulating_Handler;
            }
        }

        void Breakpoint_Changed(object sender, BreakpointArgs e)
        {
            if (methodEventData != null)
                methodEventData.BreakPoint = e.IsSet;
        }

        #region Members

        /// <summary>
        /// Method data that has been selected to be displayed.
        /// </summary>
        private LCMethodEventData methodEventData;

        private BreakpointViewModel breakpoint;
        private int eventNumber = 1;
        private bool optimizeWith = false;
        private IDevice selectedDevice = null;
        private LCMethodEventData selectedLCEvent = null;
        private readonly SourceList<LCMethodEventData> methodsComboBoxOptions = new SourceList<LCMethodEventData>();
        private readonly SourceList<ILCEventParameter> eventParameterList = new SourceList<ILCEventParameter>();
        private bool isSelected = false;
        private readonly ObservableAsPropertyHelper<bool> devicesComboBoxEnabled;

        #endregion

        #region Properties

        public BreakpointViewModel Breakpoint
        {
            get => breakpoint;
            set => this.RaiseAndSetIfChanged(ref breakpoint, value);
        }

        public int EventNumber
        {
            get => eventNumber;
            set => this.RaiseAndSetIfChanged(ref eventNumber, value);
        }

        /// <summary>
        /// The selected device
        /// </summary>
        public IDevice SelectedDevice
        {
            get => selectedDevice;
            set => this.RaiseAndSetIfChanged(ref selectedDevice, value);
        }

        /// <summary>
        /// The selected method event
        /// </summary>
        public LCMethodEventData SelectedLCEvent
        {
            get => selectedLCEvent;
            set => this.RaiseAndSetIfChanged(ref selectedLCEvent, value);
        }

        public ReadOnlyObservableCollection<IDevice> DevicesComboBoxOptions { get; }
        public ReadOnlyObservableCollection<LCMethodEventData> MethodsComboBoxOptions { get; }
        public ReadOnlyObservableCollection<ILCEventParameter> EventParameterList { get; }
        public bool DevicesComboBoxEnabled => devicesComboBoxEnabled.Value && EventUnlocked;
        public bool EventUnlocked { get; }

        public bool IsSelected
        {
            get => isSelected;
            set => this.RaiseAndSetIfChanged(ref isSelected, value);
        }

        /// <summary>
        /// Gets the method selected to run by the user.
        /// </summary>
        public LCMethodEventData SelectedMethod
        {
            get
            {
                if (methodEventData != null)
                {
                    // Link the method to a device
                    methodEventData.Device = SelectedDevice;
                    methodEventData.OptimizeWith = OptimizeWith;
                    methodEventData.BreakPoint = Breakpoint.IsSet;

                    // Make sure that we build the method so that the values are updated
                    // from the control used to interface them....
                    methodEventData.BuildEvent();

                    methodEventData.BreakPointEvent += BreakPointEvent_Handler;
                    methodEventData.Simulated += Simulated_Handler;
                    methodEventData.SimulatingEvent += Simulating_Handler;
                }

                return methodEventData;
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
                if (methodEventData != null && methodEventData.BreakPoint)
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
            get => optimizeWith;
            set => this.RaiseAndSetIfChanged(ref optimizeWith, value);
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

            if (DevicesList.Items.Contains(device))
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

            if (!DevicesList.Items.Contains(device))
            {
                DevicesList.Add(device);
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
                SelectedDevice = DevicesList.Items.First();
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
            methodsComboBoxOptions.Edit(list =>
            {
                // Clear out the combo-box
                list.Clear();
                list.AddRange(methods);

                if (list.Count > 0)
                {
                    SelectedLCEvent = list[0];
                }
            });

            if (methodsComboBoxOptions.Count > 0)
            {
                UpdateSelectedMethodEvent();
            }
        }

        /// <summary>
        /// Displays the given device method names and selected controls.
        /// </summary>
        /// <param name="methodEvent"></param>
        private void LoadMethodEventParameters(LCMethodEventData methodEvent)
        {
            // Make sure the device is not null
            if (methodEvent == null)
                return;

            // Clear out the combo-box
            try
            {
                foreach (var vm in EventParameterList)
                {
                    vm.EventChanged -= param_EventChanged;
                }
            }
            catch
            {
            }

            eventParameterList.Clear();
            //mpanel_parameters.ColumnStyles.Clear();

            // If the method requires sample input then we just ignore adding any controls.
            var parameters = methodEvent.Parameters;

            // This readjusts the number of parameters that are sample specific
            var count = parameters.Count * 2;
            var indexOfSampleData = methodEvent.MethodEventAttribute.SampleParameterIndex;

            // Update the style so we have the right spacing
            //var percent = 100.0F / Convert.ToSingle(count);
            //for (var i = 0; i < parameters.Controls.Count; i++)
            //{
            //    if (i != indexOfSampleData)
            //        mpanel_parameters.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, percent));
            //}

            // Add the controls to the mpanel_parameter controls
            for (var j = 0; j < parameters.Count; j++)
            {
                if (j != indexOfSampleData)
                {
                    var parameter = parameters[j];
                    // Get the name of the parameter
                    var name = parameter.Name;

                    // Get the ViewModel for the parameter
                    var vm = parameter.ViewModel;

                    // Construct the description for the parameter
                    vm.ParameterLabel = name;

                    // Add the control itself
                    eventParameterList.Add(vm);

                    vm.ParameterValue = parameter.Value;
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

        private void UpdateSelectedMethodEvent()
        {
            // Update the user interface.
            var method = SelectedLCEvent;
            if (method != methodEventData)
            {
                if (methodEventData != null)
                {
                    methodEventData.BreakPointEvent -= BreakPointEvent_Handler;
                    methodEventData.Simulated -= Simulated_Handler;
                }
                // Create a clone of the method event so that we don't accidentally link multiple events together (same data) until the method is reloaded.
                methodEventData = method?.Clone();
                if (method != null)
                {
                    methodEventData.BreakPointEvent += BreakPointEvent_Handler;
                    methodEventData.Simulated += Simulated_Handler;
                    if (SelectedDevice != null)
                    {
                        LoadMethodEventParameters(methodEventData);
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
        /// Reflects the given device and puts the method and parameter information in the appropriate combo boxes.
        /// </summary>
        public static List<LCMethodEventData> ReflectDevice(IDevice device)
        {
            if (device == null)
                throw new NullReferenceException("Device cannot be null.");

            var type = device.GetType();

            // List of method editing pairs
            var methodPairs = new List<LCMethodEventData>();

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
                        var parameters = new List<LCMethodEventParameter>();

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
                                ILCEventParameter vm = null;
                                object value = null;

                                // If the method editor has to use sample data then
                                // we skip adding a control...but allow for
                                // other data to be loaded.
                                if (attr.RequiresSampleInput && i == attr.SampleParameterIndex)
                                {
                                    parameters.Add(new LCMethodEventParameter(paramInfo.Name, null, null, attr.DataProvider));
                                }
                                else if (string.IsNullOrEmpty(attr.DataProvider) == false && i == attr.DataProviderIndex)
                                {
                                    // Figure out what index to adjust the data provider for.
                                    var combo = new EventParameterEnumViewModel();

                                    // Register the event to automatically get new data when the data provider has new stuff.
                                    device.RegisterDataProvider(attr.DataProvider, combo.FillData);
                                    vm = combo;
                                    // Set the data if we have it, otherwise, cross your fingers batman!
                                    if (combo.ComboBoxOptions.Count > 0)
                                        value = combo.ComboBoxOptions[0];
                                    parameters.Add(new LCMethodEventParameter(paramInfo.Name, value, vm, attr.DataProvider));
                                }
                                else
                                {
                                    // Get a control to display
                                    vm = GetEventParametersFromType(paramInfo.ParameterType);

                                    // We need to get a default value, so just ask the
                                    // type for it.
                                    if (paramInfo.ParameterType.IsEnum)
                                    {
                                        value = Enum.GetValues(paramInfo.ParameterType).GetValue(0);
                                    }

                                    // If the control is not null, then we can add it to display.
                                    // If it is null, then it is of a type we know nothing about.
                                    // And well you're SOL.
                                    if (vm != null)
                                    {
                                        parameters.Add(new LCMethodEventParameter(paramInfo.Name, value, vm, attr.DataProvider));
                                    }
                                }
                                i++;
                            }
                        }

                        // Construct the new method from what we found
                        // during the reflection phase and add it to the list of
                        // possible methods to call for this device.
                        var newMethod = new LCMethodEventData(device, method, attr, parameters);
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
        public static ILCEventParameter GetEventParametersFromType(Type t)
        {
            ILCEventParameter control = null;

            if (t.IsEnum)
            {
                // Add the parameters to the combo box before we do anything.
                var enumControl = new EventParameterEnumViewModel();
                control = enumControl;
                // Grab the enumeration values for the parameter
                enumControl.FillData(null, Enum.GetValues(t).Cast<object>().ToList());
                enumControl.SelectedOption = enumControl.ComboBoxOptions.FirstOrDefault();
            }
            else if (NumericTypes.Contains(t))
            {
                control = new EventParameterNumericViewModel(t);
            }
            else if (typeof(string) == t)
            {
                control = new EventParameterTextViewModel();
            }
            else if (typeof(bool) == t)
            {
                control = new EventParameterBoolViewModel();
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
        /// Handles when the user selects a new method event for the current device.
        /// </summary>
        private void SelectedMethodEventChanged()
        {
            UpdateSelectedMethodEvent();
            OnEventChanged();
        }

        /// <summary>
        /// Tells the other devices that this method should be used in optimization.
        /// </summary>
        private void OptimizeForChanged()
        {
            UseForOptimization?.Invoke(this, OptimizeWith);
            if (methodEventData != null)
                methodEventData.OptimizeWith = OptimizeWith;
            OnEventChanged();
        }

        public override string ToString()
        {
            var eventData = SelectedMethod.Device.Name;
            if (SelectedMethod?.MethodEventAttribute != null)
            {
                eventData = $"{SelectedMethod.Device.Name} - {SelectedMethod.MethodEventAttribute.Name}";
            }

            return eventData;
        }

        #endregion
    }
}
