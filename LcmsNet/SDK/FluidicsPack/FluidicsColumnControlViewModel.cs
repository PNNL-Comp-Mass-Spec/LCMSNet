using System.ComponentModel;
using System.Windows.Controls;
using LcmsNetDataClasses.Devices;
using LcmsNetSDK;

namespace FluidicsPack
{
    public class FluidicsColumnControlViewModel : INotifyPropertyChangedExt, IDeviceControl
    {
        private FluidicsColumn column;

        public FluidicsColumnControlViewModel()
        {
            Name = "FluidicsColumnControl";
        }

        private void RegisterDevice(IDevice device)
        {
            if (device == null)
            {
                column = null;
                return;
            }
            column = device as FluidicsColumn;
            if (column != null)
            {
                PackingMaterial = column.PackingMaterial;
                InnerDiameter = column.InnerDiameter;
                Length = column.Length;
            }
        }

        private string packingMaterial = "";
        private double innerDiameter;
        private double length;
        private string name = "";

        public string PackingMaterial
        {
            get { return packingMaterial; }
            set { this.RaiseAndSetIfChanged(ref packingMaterial, value); }
        }

        public double InnerDiameter
        {
            get { return innerDiameter; }
            set { this.RaiseAndSetIfChanged(ref innerDiameter, value); }
        }

        public double Length
        {
            get { return length; }
            set { this.RaiseAndSetIfChanged(ref length, value); }
        }

        #region IDeviceControl Members
        public event DelegateNameChanged NameChanged
        {
            add { }
            remove { }
        }

        public event DelegateSaveRequired SaveRequired
        {
            add { }
            remove { }
        }

        public bool Running
        {
            get;
            set;
        }

        public IDevice Device
        {
            get
            {
                return column;
            }
            set
            {
                RegisterDevice(value);
            }
        }

        public string Name
        {
            get { return name; }
            set { this.RaiseAndSetIfChanged(ref name, value); }
        }

        public UserControl GetDefaultView()
        {
            return new FluidicsColumnControlView();
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
