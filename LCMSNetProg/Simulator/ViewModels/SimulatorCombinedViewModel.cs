using System;
using LcmsNetCommonControls.ViewModels;
using ReactiveUI;

namespace LcmsNet.Simulator.ViewModels
{
    public class SimulatorCombinedViewModel : ReactiveObject
    {
        public SimulatorCombinedViewModel()
        {
            ControlsVm = SimulatorControlsAndChartsViewModel.GetInstance;
            ConfigVm = SimConfigurationViewModel.GetInstance;
            ConfigPopoutVm = new PopoutViewModel(ConfigVm);
            ControlsPopoutVm = new PopoutViewModel(ControlsVm);

            this.WhenAnyValue(x => x.ConfigPopoutVm.Child, x => x.ConfigPopoutVm.Tacked).Subscribe(x => TackChangeRules(x.Item1, x.Item2));
            this.WhenAnyValue(x => x.ControlsPopoutVm.Child, x => x.ControlsPopoutVm.Tacked).Subscribe(x => TackChangeRules(x.Item1, x.Item2));
        }

        private bool bothTacked = true;

        public SimConfigurationViewModel ConfigVm { get; }
        public SimulatorControlsAndChartsViewModel ControlsVm { get; }
        public PopoutViewModel ConfigPopoutVm { get; }
        public PopoutViewModel ControlsPopoutVm { get; }

        public bool BothTacked
        {
            get => bothTacked;
            set => this.RaiseAndSetIfChanged(ref bothTacked, value);
        }

        private void TackChangeRules(object sender, bool newTackState)
        {
            // Don't allow both to be untacked at the same time.
            if (sender is SimConfigurationViewModel && !newTackState && !ControlsPopoutVm.Tacked)
            {
                ControlsPopoutVm.Tacked = true;
            }
            if (sender is SimulatorControlsAndChartsViewModel && !newTackState && !ConfigPopoutVm.Tacked)
            {
                ConfigPopoutVm.Tacked = true;
            }
            BothTacked = ConfigPopoutVm.Tacked && ControlsPopoutVm.Tacked;
        }
    }
}
