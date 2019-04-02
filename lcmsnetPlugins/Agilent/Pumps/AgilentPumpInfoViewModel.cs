using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace LcmsNetPlugins.Agilent.Pumps
{
    public class AgilentPumpInfoViewModel : ReactiveObject
    {
        private AgilentPumpInfo pumpInfo;
        private readonly ObservableAsPropertyHelper<string> manufacturer;
        private readonly ObservableAsPropertyHelper<string> model;
        private readonly ObservableAsPropertyHelper<string> serialNumber;
        private readonly ObservableAsPropertyHelper<string> moduleName;
        private readonly ObservableAsPropertyHelper<string> bootFirmware;
        private readonly ObservableAsPropertyHelper<string> mainFirmware;
        private readonly ObservableAsPropertyHelper<string> residentFirmware;
        private readonly ObservableAsPropertyHelper<string> firmwareBuildNumber;
        private readonly ObservableAsPropertyHelper<DateTime> manufactureDateUtc;
        private readonly ObservableAsPropertyHelper<string> options;
        private readonly ObservableAsPropertyHelper<DateTime> moduleDate;

        public AgilentPumpInfoViewModel()
        {
            manufacturer = this.WhenAnyValue(x => x.PumpInfo, x => x.PumpInfo.Manufacturer).Select(x => x.Item2).ToProperty(this, nameof(Manufacturer), "", true, RxApp.MainThreadScheduler);
            model = this.WhenAnyValue(x => x.PumpInfo, x => x.PumpInfo.Model).Select(x => x.Item2).ToProperty(this, nameof(Model), "", true, RxApp.MainThreadScheduler);
            serialNumber = this.WhenAnyValue(x => x.PumpInfo, x => x.PumpInfo.SerialNumber).Select(x => x.Item2).ToProperty(this, nameof(SerialNumber), "", true, RxApp.MainThreadScheduler);
            moduleName = this.WhenAnyValue(x => x.PumpInfo, x => x.PumpInfo.ModuleName).Select(x => x.Item2).ToProperty(this, nameof(ModuleName), "", true, RxApp.MainThreadScheduler);
            bootFirmware = this.WhenAnyValue(x => x.PumpInfo, x => x.PumpInfo.BootFirmware).Select(x => x.Item2).ToProperty(this, nameof(BootFirmware), "", true, RxApp.MainThreadScheduler);
            mainFirmware = this.WhenAnyValue(x => x.PumpInfo, x => x.PumpInfo.MainFirmware).Select(x => x.Item2).ToProperty(this, nameof(MainFirmware), "", true, RxApp.MainThreadScheduler);
            residentFirmware = this.WhenAnyValue(x => x.PumpInfo, x => x.PumpInfo.ResidentFirmware).Select(x => x.Item2).ToProperty(this, nameof(ResidentFirmware), "", true, RxApp.MainThreadScheduler);
            firmwareBuildNumber = this.WhenAnyValue(x => x.PumpInfo, x => x.PumpInfo.FirmwareBuildNumber).Select(x => x.Item2).ToProperty(this, nameof(FirmwareBuildNumber), "", true, RxApp.MainThreadScheduler);
            manufactureDateUtc = this.WhenAnyValue(x => x.PumpInfo, x => x.PumpInfo.ManufactureDateUtc).Select(x => x.Item2).ToProperty(this, nameof(ManufactureDateUtc), DateTime.MinValue, true, RxApp.MainThreadScheduler);
            options = this.WhenAnyValue(x => x.PumpInfo, x => x.PumpInfo.Options).Select(x => x.Item2).ToProperty(this, nameof(Options), "", true, RxApp.MainThreadScheduler);
            moduleDate = this.WhenAnyValue(x => x.PumpInfo, x => x.PumpInfo.ModuleDate).Select(x => x.Item2).ToProperty(this, nameof(ModuleDate), DateTime.MinValue, true, RxApp.MainThreadScheduler);
        }

        public AgilentPumpInfo PumpInfo
        {
            get => pumpInfo;
            set => this.RaiseAndSetIfChanged(ref pumpInfo, value);
        }

        public string Manufacturer => manufacturer?.Value ?? "";
        public string Model => model?.Value ?? "";
        public string SerialNumber => serialNumber?.Value ?? "";
        public string ModuleName => moduleName?.Value ?? "";
        public string BootFirmware => bootFirmware?.Value ?? "";
        public string MainFirmware => mainFirmware?.Value ?? "";
        public string ResidentFirmware => residentFirmware?.Value ?? "";
        public string FirmwareBuildNumber => firmwareBuildNumber?.Value ?? "";
        public DateTime ManufactureDateUtc => manufactureDateUtc?.Value ?? DateTime.MinValue;
        public string Options => options?.Value ?? "";
        public DateTime ModuleDate => moduleDate?.Value ?? DateTime.MinValue;
    }
}
