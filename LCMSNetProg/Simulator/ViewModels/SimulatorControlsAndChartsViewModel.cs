using System;
using FluidicsSDK;
using LcmsNet.Method.ViewModels;
using ReactiveUI;

namespace LcmsNet.Simulator.ViewModels
{
    public class SimulatorControlsAndChartsViewModel : ReactiveObject
    {
        /// <summary>
        /// Calling this constructor is only for the windows WPF designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public SimulatorControlsAndChartsViewModel() : this(null)
        {
            instance = this;
        }

        /// <summary>
        /// The real constructor
        /// </summary>
        /// <param name="anything">really, anything; only there to differentiate from the public no-parameter constructor</param>
        private SimulatorControlsAndChartsViewModel(object anything)
        {
            simInstance = FluidicsSimulator.FluidicsSimulator.GetInstance;

            CheckList = new ModelCheckListViewModel(FluidicsModerator.Moderator, FluidicsModerator.Moderator.GetModelCheckers());

            ChartsVm = new ChartViewModel();
            MethodStageVm = new LCMethodStageViewModel();
        }

        private static SimulatorControlsAndChartsViewModel instance;

        private readonly FluidicsSimulator.FluidicsSimulator simInstance;
        private int simDelayMs = 500;

        public static SimulatorControlsAndChartsViewModel GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SimulatorControlsAndChartsViewModel(null);
                }
                return instance;
            }
        }

        public ModelCheckListViewModel CheckList { get; }
        public ChartViewModel ChartsVm { get; }
        public LCMethodStageViewModel MethodStageVm { get; }

        public int SimDelayMs
        {
            get => simDelayMs;
            set
            {
                var oldValue = simDelayMs;
                this.RaiseAndSetIfChanged(ref simDelayMs, value);
                if (oldValue != simDelayMs)
                {
                    simInstance.SimulationSpeed = simDelayMs;
                }
            }
        }
    }
}
