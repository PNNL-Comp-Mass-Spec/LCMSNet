using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows.Controls;
using FluidicsSDK.Devices.Valves;
using LcmsNetData.Logging;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNetPlugins.VICI.Valves
{
    public class ValveVICIMultiPosViewModel : ValveVICIViewModelBase
    {
        #region Constructors

        public ValveVICIMultiPosViewModel()
        {
            //Populate the combobox
            PopulateComboBox();
            SetValvePositionCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => SetValvePosition()));
        }

        protected override void RegisterDevice(IDevice device)
        {
            if (!(device is ValveVICIMultiPos mp))
            {
                return;
            }

            Valve = mp;
            Valve.PosChanged += OnPosChanged;

            RegisterBaseDevice(Valve);

            PopulateComboBox();
        }

        private void PopulateComboBox()
        {
            if (valve != null)
            {
                using (valvePositionComboBoxOptions.SuppressChangeNotifications())
                {
                    valvePositionComboBoxOptions.AddRange(Enum.GetValues(valve.GetStateType()).Cast<object>().Select(x => x.ToString()));
                }
            }
        }

        #endregion

        #region Events

        //Position change
        public virtual void OnPosChanged(object sender, ValvePositionEventArgs<int> newPosition)
        {
            RxApp.MainThreadScheduler.Schedule(() => CurrentValvePosition = newPosition.Position.ToString());
        }
        #endregion

        #region Members
        /// <summary>
        /// Class that interfaces the hardware.
        /// </summary>
        private ValveVICIMultiPos valve;

        private readonly ReactiveList<string> valvePositionComboBoxOptions = new ReactiveList<string>();
        private string selectedValvePosition = "";

        #endregion

        #region Properties

        public IReadOnlyReactiveList<string> ValvePositionComboBoxOptions => valvePositionComboBoxOptions;

        public string SelectedValvePosition
        {
            get => selectedValvePosition;
            set => this.RaiseAndSetIfChanged(ref selectedValvePosition, value);
        }

        /// <summary>
        /// Get or sets the flag determining if the system is in emulation mode.
        /// </summary>
        public bool Emulation
        {
            get => valve.Emulation;
            set => valve.Emulation = value;
        }

        /// <summary>
        /// Gets or sets the device associated with this control.
        /// </summary>
        public override IDevice Device
        {
            get => valve;
            set
            {
                if (!IsInDesignMode)
                {
                    RegisterDevice(value);
                }
            }
        }

        public ValveVICIMultiPos Valve
        {
            get => valve;
            private set => this.RaiseAndSetIfChanged(ref valve, value);
        }

        public ReactiveCommand<Unit, Unit> SetValvePositionCommand { get; }

        #endregion

        #region Methods

        public override UserControl GetDefaultView()
        {
            return new ValveVICIMultiPosView();
        }

        private async void SetValvePosition()
        {
            if (string.IsNullOrWhiteSpace(SelectedValvePosition))
            {
                ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_USER, "A valve position selection should be made.");
                return;
            }

            var enumValue = Enum.Parse(valve.GetStateType(), SelectedValvePosition);
            var pos = (int) enumValue;

            await Task.Run(() => SetPosition(pos));
        }

        private void SetPosition(int pos)
        {
            valve.SetPosition(pos);
        }

        #endregion
    }
}
