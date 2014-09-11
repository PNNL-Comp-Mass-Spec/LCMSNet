using System;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;

namespace LcmsNet.Method.Forms
{
    public delegate void DelegateLCMethodEventOptimize(object sender, bool optimize);
    public delegate void DelegateLCMethodEventLocked(object sender, bool enabled, classLCMethodData methodData);

    /// <summary>
    /// LC-Method User Interface for building an LC-Method.
    /// </summary>
    public partial class controlLCMethodEvent : UserControl
    {
        #region Members
        /// <summary>
        /// Device reference to control
        /// </summary>
        private IDevice mobj_device;
        /// <summary>
        /// Method data that has been selected to be displayed.
        /// </summary>
        private classLCMethodData mobj_methodData;
        /// <summary>
        /// List of device methods and parameters to use.
        /// </summary>
        private Dictionary<IDevice, List<classLCMethodData>> mdict_deviceMappings;
        /// <summary>
        /// Flag indicating if this event is a placeholder so that we know it's an unlocking event         
        /// </summary>
        private bool mbool_isLockingEvent = false;
        private bool mbool_stoppedHere;
        #endregion

        /// <summary>
        /// Fired when the user wants to optimize with this event.
        /// </summary>
        public event DelegateLCMethodEventOptimize UseForOptimization;
        /// <summary>
        /// Fired when this event is to be locked.
        /// </summary>
        public event DelegateLCMethodEventLocked Lock;
        /// <summary>
        /// Fired when an event changes.
        /// </summary>
        public event EventHandler EventChanged;

        /// <summary>
        /// Default Constructor         
        /// </summary>
        public controlLCMethodEvent(int eventNum=1)
        {
            InitializeComponent();
            mobj_device = null;
            this.labelEventNumber.Text = eventNum.ToString();
            StoppedHere = false;
            Initialize();
        }

        ~controlLCMethodEvent()
        {
            mobj_methodData.BreakPointEvent -= BreakPointEvent_Handler;
            mobj_methodData.Simulated -= Simulated_Handler;
            mobj_methodData.SimulatingEvent -= Simulating_Handler;
        }


        /// <summary>
        /// Initializes the device mappings structures and registers events to listen for data from the device manager.
        /// </summary>
        private void Initialize()
        {
            
            /// 
            /// Add the devices to the method editor
            /// 
            mdict_deviceMappings = new Dictionary<IDevice,List<classLCMethodData>>();
            RegisterDevices();

            /// 
            /// Handle user interface events to display context of method editors
            /// 
            mcomboBox_method.SelectedIndexChanged  += new EventHandler(mcomboBox_method_SelectedIndexChanged);
            mcomboBox_devices.SelectedIndexChanged += new EventHandler(mcomboBox_devices_SelectedIndexChanged);
            controlBreakpoint1.Changed += controlBreakpoint1_Changed;
            /// 
            /// Register to listen for device additions or deletions.
            /// 
            Devices.classDeviceManager.Manager.DeviceAdded   += new LcmsNetDataClasses.Devices.DelegateDeviceUpdated(Manager_DeviceAdded);
            Devices.classDeviceManager.Manager.DeviceRemoved += new LcmsNetDataClasses.Devices.DelegateDeviceUpdated(Manager_DeviceRemoved);
            Devices.classDeviceManager.Manager.DeviceRenamed += new LcmsNetDataClasses.Devices.DelegateDeviceUpdated(Manager_DeviceRenamed);
        }

        void controlBreakpoint1_Changed(object sender, BreakpointArgs e)
        {
            mobj_methodData.BreakPoint = e.IsSet;
            this.Refresh();
        }

        /// <summary>
        /// Constructor for an unlocking event.
        /// </summary>
        /// <param name="device"></param>
        public controlLCMethodEvent(classLCMethodData methodData, bool locked)
        {
            InitializeComponent();
            Initialize();

            /// 
            /// Every device is a reference held in the device manager...except for the timer
            /// object.  This object is created every time as a non-critical object because
            /// it does not require simultaneous use by various threads.  However, we have to
            /// make sure here the device is not a timer, because if it is, then we must
            /// find the device name string "Timer" from the combo box so we can 
            /// get the right reference.
            /// 
            if (methodData.Device.GetType().Equals(typeof(classTimerDevice)) == false)
            {
                mobj_device = methodData.Device;
                int index = mcomboBox_devices.Items.IndexOf(mobj_device);
                mcomboBox_devices.SelectedIndex = index;
            }
            else
            {
                /// 
                /// BAH! TODO: Clean this up5
                /// 
                mobj_device = methodData.Device;
                int i = 0;
                foreach (object o in mcomboBox_devices.Items)
                {
                    IDevice device = o as IDevice;
                    if (device != null && device.Name == "Timer")
                    {
                        break;
                    }
                    i++;
                }
                mcomboBox_devices.SelectedIndex = i;
            }
            
            mcomboBox_method.Enabled    = (locked == false);            
            mcomboBox_devices.Enabled   = (locked == false);            
            mpanel_extras.Visible       = (locked == false);
            mpanel_parameters.Visible   = (locked == false);

            mcheckBox_optimizeFor.Checked = methodData.OptimizeWith;

            if (locked == true)
            {
                mcomboBox_method.Items.Add("Unlock");
                mcomboBox_method.SelectedIndex = 0;
            }
            else
            {
                LoadMethodInformation(mobj_device);
                /// 
                /// We have to do some reference trickery here, since method data is reconstructed
                /// for every lcevent then we need to set the method data for this object 
                /// with the method we previously selected.  This way we preserve the parameter values
                /// etc. 
                /// 
                int index = FindMethodIndex(methodData);
                mcomboBox_method.Items[index]  = methodData;
                mcomboBox_method.SelectedIndex = index;

                LoadMethodParameters(methodData);
            }

            mbool_isLockingEvent        = locked;
            mobj_methodData             = methodData;
        }

        private int FindMethodIndex(classLCMethodData method)
        {            
            int i = 0;
            foreach(object o in mcomboBox_method.Items)
            {
                classLCMethodData data = o as classLCMethodData;
                if (data != null)
                {
                    if (data.MethodAttribute.Name.Equals(method.MethodAttribute.Name))
                        return i;
                }
                i++;
            }
            return -1;
        }

        #region Device Manager Event Listeners
        /// <summary>
        /// Handles when a device is renamed, it updates the internal device data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceRenamed(object sender, IDevice device)
        {
            if (device.DeviceType == enumDeviceType.Fluidics)
                return;

            if (mcomboBox_devices.Items.Contains(device))
            {
                int index = mcomboBox_devices.Items.IndexOf(device);
                mcomboBox_devices.Items[index] = device;
            }
        }
        /// <summary>
        /// Updates the list of available devices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceRemoved(object sender, LcmsNetDataClasses.Devices.IDevice device)
        {
            if (device.DeviceType == enumDeviceType.Fluidics)
                return;

            if (mcomboBox_devices.Items.Contains(device))           
                mcomboBox_devices.Items.Remove(device);

            if (mdict_deviceMappings.ContainsKey(device) == true)
                mdict_deviceMappings.Remove(device);

            if (mcomboBox_devices.Items.Count < 1)
                mcomboBox_devices.Enabled = false;
        }
        /// <summary>
        /// Updates the list of available devices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceAdded(object sender, LcmsNetDataClasses.Devices.IDevice device)
        {
            /// 
            /// If this was the first device added for some odd reason, then make sure we enable the 
            /// device.
            /// 
            bool isFirstDevice = false;

            if (device.DeviceType == enumDeviceType.Fluidics)
                return;


            if (mcomboBox_devices.Items.Count < 1)
            {
                mcomboBox_devices.Enabled = true;
                isFirstDevice             = true;
            }
            
            if (mcomboBox_devices.Items.Contains(device) == false)
                mcomboBox_devices.Items.Add(device);

            if (mdict_deviceMappings.ContainsKey(device) == false)
            {
                List<classLCMethodData> methodPairs = ReflectDevice(device);
                mdict_deviceMappings.Add(device, methodPairs);
            }

            /// 
            /// Make sure we select a device
            /// 
            if (isFirstDevice == true)
            {
                mcomboBox_devices.SelectedIndex = 0;
                UpdateSelectedDevice();
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether the event has been selected or not.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return mcheckBox_selected.Checked;
            }
            set
            {
                mcheckBox_selected.Checked = value;
            }
        }
        /// <summary>
        /// Gets the method selected to run by the user.
        /// </summary>
        public classLCMethodData SelectedMethod
        {
            get
            {
                if (mobj_methodData != null)
                {
                    /// 
                    /// Link the method to a device
                    /// 
                    mobj_methodData.Device = mobj_device;
                    mobj_methodData.OptimizeWith = mcheckBox_optimizeFor.Checked;
                    mobj_methodData.BreakPoint = controlBreakpoint1.IsSet;
                    /// 
                    /// Make sure that we build the method so that the values are updated
                    /// from the control used to interface them....
                    /// 
                    mobj_methodData.BuildMethod();                    
                }
                mobj_methodData.BreakPointEvent += BreakPointEvent_Handler;
                mobj_methodData.Simulated += Simulated_Handler;
                mobj_methodData.SimulatingEvent += Simulating_Handler;
                return mobj_methodData;
            }
        }

        public void SetBreakPoint(bool value)
        {
            controlBreakpoint1.IsSet = value;
        }

        /// <summary>
        /// Gets or sets whether to optimize the method alignment with this event.
        /// </summary>
        public bool OptimizeWith
        {
            get
            {
                return mcheckBox_optimizeFor.Checked;
            }
            set
            {
                mcheckBox_optimizeFor.Checked = value;
            }
        }
        /// <summary>
        /// Gets flag indicating whether this event editor is a placeholder for a locking event.
        /// </summary>
        public bool IsLockingEvent
        {
            get
            {
                return mbool_isLockingEvent;
            }
        }
        /// <summary>
        /// Returns the device selected
        /// </summary>
        public IDevice Device
        {
            get
            {
                return mobj_device;
            }
        }

        private bool IsCurrent
        {
            get;
            set;
        }

        private bool StoppedHere
        {
            get
            {
                return mbool_stoppedHere;
            }
            set
            {
                mbool_stoppedHere = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Displays the given device method names and selected controls.
        /// </summary>
        /// <param name="device"></param>
        private void LoadMethodInformation(IDevice device)
        {
            /// 
            /// Make sure the device is not null
            /// 
            if (device == null)
                return;

            /// 
            /// Clear out the combo-box
            /// 
            mcomboBox_method.Items.Clear();

            /// 
            /// We know that timer devices are always re-created.  Meaning their references
            /// are not persisted throughout a method or across methods.  Thus we need to test to see
            /// if this is true, if so, then we need to just find the device in the 
            /// device manager and get the device method mappings that way.
            /// 
            classTimerDevice timer = device as classTimerDevice;
            if (timer != null)
            {   
                /// 
                /// Find the timer device.
                /// 
                foreach(IDevice tempDevice in mdict_deviceMappings.Keys)
                {
                    timer = tempDevice as classTimerDevice;
                    if (timer != null)
                    {
                        device = tempDevice;
                        break;
                    }
                }
            }
            /// 
            /// Add the method information into the combo-box as deemed by the device.
            /// 
            List<classLCMethodData> methods = mdict_deviceMappings[device];
            foreach (classLCMethodData method in methods)
            {
                mcomboBox_method.Items.Add(method);
            }

            if (mcomboBox_method.Items.Count > 0)
            {
                mcomboBox_method.SelectedIndex = 0;
                UpdateSelectedMethod();
            }
        }

        /// <summary>
        /// Displays the given device method names and selected controls.
        /// </summary>
        /// <param name="device"></param>
        private void LoadMethodParameters(classLCMethodData method)
        {
            mpanel_parameters.SuspendLayout();

            /// 
            /// Make sure the device is not null
            /// 
            if (method == null)
                return;

            /// 
            /// Clear out the combo-box
            ///             
            try
            {
                foreach (Control c in mpanel_parameters.Controls)
                {
                    ILCEventParameter param = c as ILCEventParameter;
                    if (param != null)
                    {
                        param.EventChanged -= this.param_EventChanged;
                    }
                }
            }catch
            {
            }
            mpanel_parameters.Controls.Clear();
            mpanel_parameters.ColumnStyles.Clear();

            /// 
            /// If the method requires sample input then we just ignore adding any controls.
            ///             
            classLCMethodEventParameter parameters = method.Parameters;

            /// 
            /// This readjusts the number of parameters that are sample specific
            /// 
            int count               = parameters.Controls.Count  * 2;
            int indexOfSampleData   = method.MethodAttribute.SampleParameterIndex;
            
            /// 
            /// Update the style so we have the right spacing
            /// 
            float percent = 100.0F / Convert.ToSingle(count);
            for (int i = 0; i < parameters.Controls.Count; i++)
            {
                if (i != indexOfSampleData)
                    mpanel_parameters.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, percent));
            }

            /// 
            /// Add the controls to the mpanel_parameter controls
            ///                       
            for (int j = 0; j < parameters.Controls.Count; j++)
            {

                if (j != indexOfSampleData)
                {
                    /// 
                    /// Get the name of the parameter
                    /// 
                    string name = parameters.Names[j];

                    /// 
                    /// Get the control for the parameter
                    /// 
                    Control control = parameters.Controls[j];

                    /// 
                    /// Construct the description for the parameter
                    /// 
                    Label descriptionLabel = new Label();
                    descriptionLabel.Text = name;
                    descriptionLabel.AutoSize = true;
                    mpanel_parameters.Controls.Add(descriptionLabel);

                    /// 
                    /// Add the control itself
                    /// 
                    control.Dock = DockStyle.Left;
                    control.BringToFront();
                    mpanel_parameters.Controls.Add(control);

                    ILCEventParameter param = control as ILCEventParameter;
                    param.ParameterValue    = parameters.Values[j];
                    param.EventChanged      += new EventHandler(param_EventChanged);
                }
            }
            mpanel_parameters.ResumeLayout();
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
            /// 
            /// Update the user interface.
            /// 
            IDevice device = mcomboBox_devices.Items[mcomboBox_devices.SelectedIndex] as IDevice;
            if (device != mobj_device)
            {
                mobj_device = device;
                if (mobj_device != null)
                {
                    if (mdict_deviceMappings.ContainsKey(mobj_device) == false)
                    {
                        ReflectDevice(mobj_device);
                    }
                    LoadMethodInformation(mobj_device);
                }
            }
        }
        private void UpdateSelectedMethod()
        {
            /// 
            /// Update the user interface.
            /// 
            classLCMethodData method = mcomboBox_method.Items[mcomboBox_method.SelectedIndex] as classLCMethodData;
            if (method != mobj_methodData)
            {
                if(mobj_methodData != null)
                { 
                    mobj_methodData.BreakPointEvent -= BreakPointEvent_Handler;
                    mobj_methodData.Simulated -= Simulated_Handler;
                }                
                mobj_methodData = method;
                mobj_methodData.BreakPointEvent += BreakPointEvent_Handler;
                mobj_methodData.Simulated += Simulated_Handler;
                if (mobj_device != null)
                {
                    LoadMethodParameters(mobj_methodData);
                }
            }
        }

        /// <summary>
        /// Fires the event changed event.
        /// </summary>
        private void OnEventChanged()
        {
            if (EventChanged != null)
            {
                EventChanged(this, null);
            }
        }

        /// <summary>
        /// Updates the label to display which number this event is in the list of events.
        /// </summary>
        /// <param name="eventNum">integer representing the place in the event list of this event</param>
        public void updateEventNum(int eventNum)
        {
            this.labelEventNumber.Text = eventNum.ToString();
        }
        #endregion

        #region Registration and Reflection
        /// <summary>
        /// Registers the devices with the user interface from the device manager.
        /// </summary>
        private void RegisterDevices()
        {
            foreach (IDevice device in Devices.classDeviceManager.Manager.Devices)
            {

                if (device.DeviceType == enumDeviceType.Fluidics)
                    continue;

                List<classLCMethodData> methodPairs = ReflectDevice(device);
                mdict_deviceMappings.Add(device, methodPairs);
                mcomboBox_devices.Items.Add(device);
            }

            if (mcomboBox_devices.Items.Count > 0)
            {
                mcomboBox_devices.SelectedIndex = 0;
                UpdateSelectedDevice();
            }
            else
            {
                mcomboBox_devices.Enabled = false;
            }
        }
        /// <summary>
        /// Reflects the given device and puts the method and parameter information in the appropiate combo boxes.
        /// </summary>
        public List<classLCMethodData> ReflectDevice(IDevice device)
        {
            if (device == null)
                throw new NullReferenceException("Device cannot be null.");
            
            Type type = device.GetType();

            /// 
            /// List of method editing pairs
            /// 
            List<classLCMethodData> methodPairs = new List<classLCMethodData>();

            /// 
            /// We are trying to enumerate all the methods for this device building their method-parameter pairs.           
            /// 
            foreach (MethodInfo method in type.GetMethods())
            {
                object[] customAttributes = method.GetCustomAttributes(typeof(classLCMethodAttribute), true);
                foreach (object objAttribute in customAttributes)
                {
                    /// 
                    /// If the method has a custom LC Method Attribute, then we want to look at the parameters used
                    /// 
                    classLCMethodAttribute attr = objAttribute as classLCMethodAttribute;
                    if (attr != null)
                    {
                        /// 
                        /// Grab the parameters used for this method
                        /// 
                        ParameterInfo[] info                   = method.GetParameters();
                        classLCMethodEventParameter parameters = new classLCMethodEventParameter();                     

                        /// 
                        /// Here we are looking to see if the method has a parameter 
                        /// that requires a data provider.
                        /// 
                        if (info.Length > 0) 
                        {
                            /// 
                            /// Make sure that we have parameter data, and also make sure that 
                            /// the parameter we are going to use is a sample data object.  
                            /// Then for each parameter, see if we can add it to a control to display.
                            /// 
                            int i = 0;
                            foreach (ParameterInfo paramInfo in info)
                            {
                                Control control = null;
                                object  value   = null;

                                ///  
                                /// If the method editor has to use sample data then
                                /// we skip adding a control...but allow for 
                                /// other data to be loaded.
                                ///  
                                if (attr.RequiresSampleInput == true && i == attr.SampleParameterIndex)
                                {                                    
                                    parameters.AddParameter(null, null, paramInfo.Name, attr.DataProvider);                                    
                                }
                                else if (string.IsNullOrEmpty(attr.DataProvider) == false && i == attr.DataProviderIndex)
                                {
                                        /// 
                                        /// Figure out what index to adjust the data provider for.
                                        /// 
                                        control                         = null;                                        
                                        controlParameterComboBox combo  = new controlParameterComboBox();

                                        /// 
                                        /// Register the event to automatically get new data when the data provider has new
                                        /// stuff.
                                        /// 
                                        device.RegisterDataProvider(attr.DataProvider, new DelegateDeviceHasData(combo.FillData));
                                        control = combo;
                                        /// 
                                        /// Set the data if we have it, otherwise, cross your fingers batman!
                                        /// 
                                        value = null;
                                        if (combo.Items.Count > 0)
                                            value = combo.Items[0];
                                        parameters.AddParameter(value, control, paramInfo.Name, attr.DataProvider);
                                }
                                else
                                {
                                        /// 
                                        /// Get a control to display 
                                        /// 
                                        control = GetControlFromType(paramInfo.ParameterType);

                                        /// 
                                        /// We need to get a default value, so just ask the 
                                        /// type for it.
                                        ///                                 
                                        if (paramInfo.ParameterType.IsEnum)
                                        {
                                            Array aenums = Enum.GetValues(paramInfo.ParameterType);
                                            object[] enums = new object[aenums.Length];
                                            aenums.CopyTo(enums, 0);
                                            value = enums[0];
                                        }

                                        /// 
                                        /// If the control is not null, then we can add it to display.
                                        /// If it is null, then it is of a type we know nothing about.
                                        /// And well you're SOL.
                                        /// 
                                        if (control != null)
                                        {
                                            parameters.AddParameter(value, control, paramInfo.Name, attr.DataProvider);
                                        }
                                }
                                i++;
                            }
                        }   
                     
                        /// 
                        /// Construct the new method from what we found
                        /// during the reflection phase and add it to the list of 
                        /// possible methods to call for this device.
                        /// 
                        classLCMethodData newMethod = new classLCMethodData(device,
                                                                            method, 
                                                                            attr, 
                                                                            parameters);
                        methodPairs.Add(newMethod);
                    }
                }
            }
            return methodPairs;
        }
        /// <summary>
        /// Given a parameter type, figure out what kind of control is associated with it.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Control GetControlFromType(Type t)
        {
            Control control = null;

            if (t.IsEnum == true)
            {
                /// 
                /// Grab the enumeration values for the parameter 
                /// 
                Array aenums = Enum.GetValues(t);
                object[] enums = new object[aenums.Length];
                aenums.CopyTo(enums, 0);

                /// 
                /// Add the parameters to the combo box before we do anything.
                ///                
                controlParameterComboBox box = new controlParameterComboBox();
                box.DropDownStyle = ComboBoxStyle.DropDownList;
                box.Items.AddRange(enums);
                box.SelectedIndex = 0;
                control = box;

            }
            else if (classParameterTypeFactory.IsNumeric(t))
            {
                control = new controlParameterNumericUpDown();
            }
            else if (typeof(string) == t)
            {
                control = new controlParameterTextBox();
            }

            return control;
        }
        #endregion

        #region Event handlers

        private void BreakPointEvent_Handler(object sender, BreakEventArgs e)
        {
            
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<BreakEventArgs>(BreakPointEvent_Handler), new object[2]{sender, e});
            }
            else
            {
                StoppedHere = e.IsStopped;
                this.Refresh();
            }
        }

        private void Simulating_Handler(object sender, EventArgs e)
        {
            IsCurrent = true;
            this.Refresh();
        }

        private void Simulated_Handler(object sender, EventArgs e)
        {
            IsCurrent = false;
            this.Refresh();
        }
    
        /// <summary>
        ///  when the device changes update the user interface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mcomboBox_devices_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedDevice();
            OnEventChanged();
        }
        /// <summary>
        /// Handles when the user selects a new method for the current device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mcomboBox_method_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedMethod();
            OnEventChanged();
        }
        /// <summary>
        /// Handles when the user wants to hold or not hold the device for a set period of time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mcheckBox_hold_CheckedChanged(object sender, EventArgs e)
        {            
            if (Lock != null)
            {
                classLCMethodData methodData = mcomboBox_method.SelectedItem as classLCMethodData;
                //Lock(this, mcheckBox_hold.Checked, methodData);
            }
            OnEventChanged();
        }
        /// <summary>
        /// Tells the other devices that this method should be used in optimization.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mcheckBox_optimizeFor_CheckedChanged(object sender, EventArgs e)
        {
            if (UseForOptimization != null)
                UseForOptimization(this, mcheckBox_optimizeFor.Checked);
            mobj_methodData.OptimizeWith = mcheckBox_optimizeFor.Checked;
            OnEventChanged();
        }
        #endregion

        /// <summary>
        /// Internal class that provides methods to map types to boolean values for testing.
        /// </summary>
        internal static class classParameterTypeFactory
        {
            /// <summary>
            /// Returns true if the type is a double, short, long, int, uint, ushort, ulong, or float.
            /// </summary>
            /// <param name="t">Type to interrogate.</param>
            /// <returns>True if numeric, false if not or null.</returns>
            public static bool IsNumeric(Type t)
            {

                if (t == null)
                    return false;

                bool isNumeric = false;
                isNumeric = isNumeric || (typeof(int) == t);
                isNumeric = isNumeric || (typeof(uint) == t);
                isNumeric = isNumeric || (typeof(ulong) == t);
                isNumeric = isNumeric || (typeof(long) == t);
                isNumeric = isNumeric || (typeof(short) == t);
                isNumeric = isNumeric || (typeof(ushort) == t);
                isNumeric = isNumeric || (typeof(double) == t);
                isNumeric = isNumeric || (typeof(float) == t);
                return isNumeric;
            }
        }

        private void mpanel_parameters_Paint(object sender, PaintEventArgs e)
        {
            Color backColor = Color.White;
            if (IsCurrent && !StoppedHere)
            {
                backColor = Color.Green;
            }
            else if (StoppedHere)
            {
                backColor = Color.Yellow;
            }
            else if (mobj_methodData.BreakPoint)
            {
                backColor = Color.Maroon;
            }
            mpanel_parameters.BackColor = backColor;
            mpanel_extras.BackColor = backColor;
        }

        private void controlLCMethodEvent_Paint(object sender, PaintEventArgs e)
        {
            Color backColor = Color.White;
            if(IsCurrent && !StoppedHere)
            {
                backColor = Color.Green;
            }
            else if(StoppedHere)
            {
                backColor = Color.Yellow;
            }
            else if (mobj_methodData.BreakPoint)
            {
                backColor = Color.Maroon;
            }
            this.BackColor = backColor;
        }
    }
}
