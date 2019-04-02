using System;
using System.ComponentModel;
using LcmsNetData;

namespace LcmsNetPlugins.Agilent.Pumps
{
    public class AgilentPumpInfo : INotifyPropertyChangedExt
    {
        private string manufacturer;
        private string model;
        private string serialNumber;
        private string moduleName;
        private string bootFirmware;
        private string mainFirmware;
        private string residentFirmware;
        private string firmwareBuildNumber;
        private DateTime manufactureDateUtc;
        private string options;
        private DateTime moduleDate;

        public string Manufacturer
        {
            get => manufacturer;
            set => this.RaiseAndSetIfChanged(ref manufacturer, value);
        }

        public string Model
        {
            get => model;
            set => this.RaiseAndSetIfChanged(ref model, value);
        }

        public string SerialNumber
        {
            get => serialNumber;
            set => this.RaiseAndSetIfChanged(ref serialNumber, value);
        }

        public string ModuleName
        {
            get => moduleName;
            set => this.RaiseAndSetIfChanged(ref moduleName, value);
        }

        public string BootFirmware
        {
            get => bootFirmware;
            set => this.RaiseAndSetIfChanged(ref bootFirmware, value);
        }

        public string MainFirmware
        {
            get => mainFirmware;
            set => this.RaiseAndSetIfChanged(ref mainFirmware, value);
        }

        public string ResidentFirmware
        {
            get => residentFirmware;
            set => this.RaiseAndSetIfChanged(ref residentFirmware, value);
        }

        public string FirmwareBuildNumber
        {
            get => firmwareBuildNumber;
            set => this.RaiseAndSetIfChanged(ref firmwareBuildNumber, value);
        }

        public DateTime ManufactureDateUtc
        {
            get => manufactureDateUtc;
            set => this.RaiseAndSetIfChanged(ref manufactureDateUtc, value);
        }

        public string Options
        {
            get => options;
            set => this.RaiseAndSetIfChanged(ref options, value);
        }

        public DateTime ModuleDate
        {
            get => moduleDate;
            set => this.RaiseAndSetIfChanged(ref moduleDate, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
