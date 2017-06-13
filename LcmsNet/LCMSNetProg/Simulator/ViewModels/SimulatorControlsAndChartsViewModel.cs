using System;
using System.IO;
using FluidicsSDK;
using LcmsNet.Method;
using LcmsNet.Method.ViewModels;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
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

            checkList = new ModelCheckListViewModel(classFluidicsModerator.Moderator, classFluidicsModerator.Moderator.GetModelCheckers());

            chartsVm = new ChartViewModel();
            methodStageVm = new LCMethodStageViewModel();
        }

        private static SimulatorControlsAndChartsViewModel instance;

        private readonly FluidicsSimulator.FluidicsSimulator simInstance;
        private readonly ModelCheckListViewModel checkList;
        private readonly LCMethodStageViewModel methodStageVm;
        private readonly ChartViewModel chartsVm;
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

        public ModelCheckListViewModel CheckList => checkList;
        public ChartViewModel ChartsVm => chartsVm;
        public LCMethodStageViewModel MethodStageVm => methodStageVm;

        public int SimDelayMs
        {
            get { return simDelayMs; }
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

        /// <summary>
        /// Gets or sets the method animation preview options.
        /// </summary>
        public classMethodPreviewOptions MethodPreviewOptions { get; set; }

        /// <summary>
        /// Saves the given method to file.
        /// </summary>
        /// <param name="method"></param>
        public bool SaveMethod(classLCMethod method)
        {
            // Method is null!!! OH MAN - that isn't my fault so we'll ignore it!
            if (method == null)
                return false;

            // Create a new writer.
            var writer = new classLCMethodWriter();

            // Construct the path
            var path = Path.Combine(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_APPLICATIONPATH), classLCMethodFactory.CONST_LC_METHOD_FOLDER);
            path = Path.Combine(path, method.Name + classLCMethodFactory.CONST_LC_METHOD_EXTENSION);

            // Write the method out!
            return writer.WriteMethod(path, method);
        }
    }
}
