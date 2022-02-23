﻿using System;
using System.ComponentModel;
using System.Windows.Controls;
using LcmsNetSDK;
using LcmsNetSDK.Devices;

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
            get => packingMaterial;
            set => this.RaiseAndSetIfChanged(ref packingMaterial, value);
        }

        public double InnerDiameter
        {
            get => innerDiameter;
            set => this.RaiseAndSetIfChanged(ref innerDiameter, value);
        }

        public double Length
        {
            get => length;
            set => this.RaiseAndSetIfChanged(ref length, value);
        }

        #region IDeviceControl Members

        public event EventHandler<string> NameChanged;

        public event Action SaveRequired;

        public bool Running { get; set; }

        public IDevice Device
        {
            get => column;
            set => RegisterDevice(value);
        }

        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }
        public string DeviceStatus { get; } = string.Empty;

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