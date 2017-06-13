using System;
using LcmsNet.ViewModels;
using ReactiveUI;

namespace LcmsNet.Simulator.ViewModels
{
    public class SimulatorCombinedViewModel : ReactiveObject
    {
        public SimulatorCombinedViewModel()
        {
            controls = SimulatorControlsAndChartsViewModel.GetInstance;
            config = SimConfigurationViewModel.GetInstance;
            configPopoutVm = new PopoutViewModel(config, "LcmsNet Fluidics Configuration", 772, 450);
            controlsPopoutVm = new PopoutViewModel(controls, "LcmsNet Simulator Controls and Charts", 772, 450);

            this.WhenAnyValue(x => x.ConfigPopoutVm.Child, x => x.ConfigPopoutVm.Tacked).Subscribe(x => TackChangeRules(x.Item1, x.Item2));
            this.WhenAnyValue(x => x.ControlsPopoutVm.Child, x => x.ControlsPopoutVm.Tacked).Subscribe(x => TackChangeRules(x.Item1, x.Item2));
        }

        private readonly SimConfigurationViewModel config;
        private readonly SimulatorControlsAndChartsViewModel controls;
        private readonly PopoutViewModel configPopoutVm;
        private readonly PopoutViewModel controlsPopoutVm;

        public SimConfigurationViewModel ConfigVm => config;
        public SimulatorControlsAndChartsViewModel ControlsVm => controls;
        public PopoutViewModel ConfigPopoutVm => configPopoutVm;
        public PopoutViewModel ControlsPopoutVm => controlsPopoutVm;

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
        }
    }
}
