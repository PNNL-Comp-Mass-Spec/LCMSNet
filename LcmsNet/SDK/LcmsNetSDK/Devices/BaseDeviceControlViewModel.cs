using System.ComponentModel;
using LcmsNetSDK;

namespace LcmsNetDataClasses.Devices
{
    public class BaseDeviceControlViewModel : INotifyPropertyChangedExt
    {
        #region Members

        /// <summary>
        /// The associated device.
        /// </summary>
        protected IDevice device
        {
            get { return deviceBacker; }
            set { this.RaiseAndSetIfChanged(ref deviceBacker, value); }
        }

        private IDevice deviceBacker;

        #endregion

        #region Methods

        public BaseDeviceControlViewModel()
        {
            this.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(nameof(device)))
                {
                    Name = device.Name;
                }
            };
        }

        /// <summary>
        /// Sets the base device and updates the name text field.
        /// </summary>
        /// <param name="device"></param>
        protected void SetBaseDevice(IDevice device)
        {
            this.device = device;
            Name = device.Name;
        }

        #endregion

        protected virtual void UpdateStatusDisplay(string message)
        {
            //TODO: Add back
        }

        #region Events

        /// <summary>
        /// An event that indicates the name has changed.
        /// </summary>
        public event DelegateNameChanged NameChanged;

        public virtual void OnNameChanged(string newname)
        {
            NameChanged?.Invoke(this, newname);
        }

        /// <summary>
        /// An event that indicates the control needs to be saved
        /// </summary>
        public event DelegateSaveRequired SaveRequired;

        public virtual void OnSaveRequired()
        {
            SaveRequired?.Invoke(this);
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
        public string Name
        {
            get { return device?.Name ?? name; }
            set { this.RaiseAndSetIfChanged(ref name, value); }
        }

        private string name = "";

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
