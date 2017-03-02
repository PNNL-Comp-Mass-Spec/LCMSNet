//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/17/2009
//
// Last modified 08/17/2009
//						03/02/2011 (DAC) - Added status bar and associated code
//*********************************************************************************************************

using System.Windows.Forms;

namespace LcmsNetDataClasses.Devices
{
    public partial class controlBaseDeviceControl : UserControl
    {
        #region Members

        /// <summary>
        /// The associated device.
        /// </summary>
        protected IDevice m_device;

        #endregion

        #region Constructors

        public controlBaseDeviceControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        // <summary>
        /// Sets the base device and updates the name text field.
        /// </summary>
        /// <param name="device"></param>
        protected void SetBaseDevice(IDevice device)
        {
            m_device = device;
        }

        #endregion

        protected virtual void UpdateStatusDisplay(string message)
        {
            //TODO: Add back
        }

        #region Delegates

        private delegate void delegateStatusUpdate(string newStatus);

        #endregion

        #region Events

        /// <summary>
        /// An event that indicates the name has changed.
        /// </summary>
        public event DelegateNameChanged NameChanged;

        public virtual void OnNameChanged(string newname)
        {
            if (NameChanged != null)
            {
                NameChanged(this, newname);
            }
        }

        /// <summary>
        /// An event that indicates the control needs to be saved
        /// </summary>
        public event DelegateSaveRequired SaveRequired;

        public virtual void OnSaveRequired()
        {
            if (SaveRequired != null)
            {
                SaveRequired(this);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether the device is currently running or not.
        /// </summary>
        public virtual bool Running { get; set; }

        /// <summary>
        /// Gets or sets the name of the control
        /// </summary>
        public new string Name
        {
            get { return m_device.Name; }
            set { }
        }

        #endregion
    }
}