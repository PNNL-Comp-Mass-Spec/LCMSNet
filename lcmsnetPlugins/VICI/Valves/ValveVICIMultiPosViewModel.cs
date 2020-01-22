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
            valve = device as ValveVICIMultiPos;
            if (valve != null)
            {
                valve.PosChanged += OnPosChanged;
            }
            SetBaseDevice(valve);

            ComPort = valve.Port;

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

                    /*
                    string errorMessage = "";
                    valve = (classValveVICIMultiPos)value;
                    try
                    {
                        valve.Initialize(ref errorMessage);
                        mpropertyGrid_Serial.SelectedObject = valve.Port;
                    }
                    catch (ValveExceptionReadTimeout ex)
                    {
                        showError("Timeout (read) when attempting to initialize valve", ex);
                    }
                    catch (ValveExceptionWriteTimeout ex)
                    {
                        showError("Timeout (write) when attempting to initialize valve", ex);
                    }
                    catch (ValveExceptionUnauthorizedAccess ex)
                    {
                        showError("Unauthorized access when attempting to initialize valve", ex);
                    }*/
                }
            }
        }

        public ReactiveCommand<Unit, Unit> SetValvePositionCommand { get; }

        #endregion

        #region Methods

        public override UserControl GetDefaultView()
        {
            return new ValveVICIMultiPosView();
        }

        protected override void ValveControlSelected()
        {
            CurrentValvePosition = valve.LastMeasuredPosition.ToString();
        }

        protected override void RefreshValvePosition()
        {
            try
            {
                CurrentValvePosition = valve.GetPosition().ToString();
            }
            catch (ValveExceptionReadTimeout ex)
            {
                ShowError("Timeout (read) when attempting to get valve position", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                ShowError("Timeout (write) when attempting to get valve position", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access when attempting to get valve position", ex);
            }
        }

        protected override void RefreshValveVersion()
        {
            try
            {
                ValveVersionInfo = valve.GetVersion();
            }
            catch (ValveExceptionReadTimeout ex)
            {
                ShowError("Timeout (read) when attempting to get valve version", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                ShowError("Timeout (write) when attempting to get valve version", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access when attempting to get valve version", ex);
            }
        }

        protected override void OpenPort()
        {
            try
            {
                if (!valve.Port.IsOpen)
                {
                    valve.Port.Open();
                }
            }
            catch (NullReferenceException ex)
            {
                ShowError("Null reference when attempting to open port", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access exception when attempting to open port", ex);
            }
        }

        protected override void ClosePort()
        {
            try
            {
                valve.Port.Close();
            }
            catch (NullReferenceException ex)
            {
                ShowError("Null reference when attempting to close port", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access exception when attempting to close port", ex);
            }
        }

        protected override void SelectedValveIdUpdated()
        {
            var newID = SelectedValveId;
            try
            {
                valve.SetHardwareID(newID);
                CurrentValveId = valve.GetHardwareID();
                OnSaveRequired();
            }
            catch (ValveExceptionReadTimeout ex)
            {
                ShowError("Timeout (read) when attempting to set valve ID", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                ShowError("Timeout (write) when attempting to set valve ID", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access when attempting to set valve ID", ex);
            }
        }

        protected override void RefreshValveId()
        {
            try
            {
                CurrentValveId = valve.GetHardwareID();
            }
            catch (ValveExceptionReadTimeout ex)
            {
                ShowError("Timeout (read) when attempting to get valve ID", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                ShowError("Timeout (write) when attempting to get valve ID", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access when attempting to get valve ID", ex);
            }
        }

        protected override void ClearValveId()
        {
            try
            {
                valve.ClearHardwareID();
                CurrentValveId = valve.GetHardwareID();
                SelectedValveId = ' ';
                OnSaveRequired();
            }
            catch (ValveExceptionReadTimeout ex)
            {
                ShowError("Timeout (read) when attempting to clear valve ID", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                ShowError("Timeout (write) when attempting to clear valve ID", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access when attempting to clear valve ID", ex);
            }
        }

        /// <summary>
        /// Handles initializing the device.
        /// </summary>
        protected override void InitializeDevice()
        {
            if (IsInDesignMode)
                return;

            ComPort = valve.Port;

            var errorMessage = "";
            try
            {
                valve.Initialize(ref errorMessage);
            }
            catch (ValveExceptionReadTimeout ex)
            {
                ShowError("Timeout (read) when attempting to initialize valve", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                ShowError("Timeout (write) when attempting to initialize valve", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access when attempting to initialize valve", ex);
            }

            ValveVersionInfo = valve.Version;
            CurrentValvePosition = valve.LastMeasuredPosition.ToString();
            CurrentValveId= valve.SoftwareID;
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
