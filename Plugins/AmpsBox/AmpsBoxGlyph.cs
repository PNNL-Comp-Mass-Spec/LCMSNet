using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using FluidicsSDK.Base;
using LcmsNetSDK;
using LcmsNetSDK.Devices;

namespace AmpsBox
{
    public partial class AmpsBoxGlyph : FluidicsDevice, INotifyPropertyChangedExt //: IDeviceGlyph
    {
        public AmpsBoxGlyph()
        {
        }

        private AmpsBoxDevicePlugin m_box;

        private string name = "Amps Box";

        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        #region IDeviceGlyph Members

        public void RegisterDevice(IDevice device)
        {
            m_box = device as AmpsBoxDevicePlugin;
            Name = device.Name;

            device.DeviceSaveRequired += new EventHandler(device_DeviceSaveRequired);
        }

        void device_DeviceSaveRequired(object sender, EventArgs e)
        {
            if (m_box != null)
            {
                Name = m_box.Name;
            }
        }

        public void DeRegisterDevice()
        {
            if (m_box != null)
            {
                m_box.DeviceSaveRequired -= device_DeviceSaveRequired;
                m_box = null;
            }
        }

        public UserControl UserControl
        {
            get { return new AmpsBoxGlyphView(); }
        }

        public int ZOrder
        {
            get;
            set;
        }

        //public controlDeviceStatusDisplay StatusBar
        //{
        //    get;
        //    set;
        //}
        #endregion

        private void AmpsBoxGlyph_Load(object sender, EventArgs e)
        {

        }

        protected override void SetDevice(IDevice device)
        {
            throw new NotImplementedException();
        }

        protected override void ClearDevice(IDevice device)
        {
            throw new NotImplementedException();
        }

        public override void ActivateState(int state)
        {
            throw new NotImplementedException();
        }

        public override string StateString()
        {
            throw new NotImplementedException();
        }

        public override int CurrentState { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}