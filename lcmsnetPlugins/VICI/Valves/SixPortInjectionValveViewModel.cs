﻿using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Controls;
using FluidicsSDK.Devices.Valves;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNetPlugins.VICI.Valves
{
    public class SixPortInjectionValveViewModel : ValveVICI2PosViewModel
    {
        public SixPortInjectionValveViewModel()
        {
            SetInjectionVolumeCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => SetInjectionVolume()));
        }

        private void SetInjectionVolume()
        {
            if (Device is ISixPortInjectionValve injector)
                injector.InjectionVolume = InjectionVolume;
        }

        protected override void RegisterDevice(IDevice device)
        {
            base.RegisterDevice(device);

            if (Device is ISixPortInjectionValve injector)
            {
                InjectionVolume = injector.InjectionVolume;
                injector.InjectionVolumeChanged += Injector_InjectionVolumeChanged;
            }
        }

        private void Injector_InjectionVolumeChanged(object sender, System.EventArgs e)
        {
            if (Device is ISixPortInjectionValve injector)
            {
                InjectionVolume = injector.InjectionVolume;
            }
        }

        private double injectionVolume = 0;

        public double InjectionVolume
        {
            get => injectionVolume;
            set => this.RaiseAndSetIfChanged(ref injectionVolume, value);
        }

        public ReactiveCommand<Unit, Unit> SetInjectionVolumeCommand { get; private set; }

        public override UserControl GetDefaultView()
        {
            return new SixPortInjectionValveView();
        }
    }
}
