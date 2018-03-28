using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows.Controls;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;

namespace Eksigent.Devices.Pumps
{
    public class EksigentPumpControlViewModel : BaseDeviceControlViewModel, IDeviceControl
    {
        public EksigentPumpControlViewModel()
        {
            SetupCommands();
        }

        public void RegisterDevice(IDevice device)
        {
            m_pump = device as EksigentPump;
            if (m_pump != null)
            {
                m_pump.Error += m_pump_Error;
                m_pump.StatusUpdate += m_pump_StatusUpdate;
                m_pump.RequiresOCXInitialization += m_pump_RequiresOCXInitialization;
                m_pump.PumpStatus += Pump_PumpStatus;
                m_pump.MethodNames += Pump_MethodNames;
                m_pump.ChannelNumbers += Pump_ChannelNumbers;
            }
            SetBaseDevice(device);
        }

        private void m_pump_RequiresOCXInitialization(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Could not register the Eksigent control software OCX. " + ex.Message, ex);
            }
        }

        #region IDeviceControl Members

        public IDevice Device
        {
            get { return m_pump; }
            set { RegisterDevice(value); }
        }

        public UserControl GetDefaultView()
        {
            return new EksigentPumpControlView();
        }

        #endregion

        /// <summary>
        /// Notification driver object.
        /// </summary>
        private EksigentPump m_pump;

        private readonly ReactiveUI.ReactiveList<string> methodComboBoxOptions = new ReactiveUI.ReactiveList<string>();
        private string selectedMethod = "";
        private int minChannelNumber = 0;
        private int maxChannelNumber = 1;
        private int channelNumber = 1;
        private string statusText = "Unknown";

        public ReactiveUI.IReadOnlyReactiveList<string> MethodComboBoxOptions => methodComboBoxOptions;

        public string SelectedMethod
        {
            get { return selectedMethod; }
            set { this.RaiseAndSetIfChanged(ref selectedMethod, value); }
        }

        public int MinChannelNumber
        {
            get { return minChannelNumber; }
            private set { this.RaiseAndSetIfChanged(ref minChannelNumber, value); }
        }

        public int MaxChannelNumber
        {
            get { return maxChannelNumber; }
            private set { this.RaiseAndSetIfChanged(ref maxChannelNumber, value); }
        }

        public int ChannelNumber
        {
            get { return channelNumber; }
            set { this.RaiseAndSetIfChanged(ref channelNumber, value); }
        }

        public string StatusText
        {
            get { return statusText; }
            private set { this.RaiseAndSetIfChanged(ref statusText, value); }
        }

        public ReactiveUI.ReactiveCommand<Unit, Unit> ShowMethodEditorCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ShowDirectControlCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ShowMobilePhasesCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> UpdateMethodsCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> StartPumpCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> StopPumpCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ShowIntrumentConfigCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ShowAdvancedSettingsCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ShowDiagnosticsCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ShowMainWindowCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ShowAlertsCommand { get; private set; }

        private void SetupCommands()
        {
            ShowMethodEditorCommand = ReactiveUI.ReactiveCommand.Create(() => ShowMethodMenu());
            ShowDirectControlCommand = ReactiveUI.ReactiveCommand.Create(() => ShowDirectControl());
            ShowMobilePhasesCommand = ReactiveUI.ReactiveCommand.Create(() => ShowMobilePhaseMenu());
            UpdateMethodsCommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () => await Task.Run(() => UpdateMethods()));
            StartPumpCommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () => await Task.Run(() => StartPump()));
            StopPumpCommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () => await Task.Run(() => StopPump()));
            ShowIntrumentConfigCommand = ReactiveUI.ReactiveCommand.Create(() => ShowInstrumentConfig());
            ShowAdvancedSettingsCommand = ReactiveUI.ReactiveCommand.Create(() => ShowAdvancedSettings());
            ShowDiagnosticsCommand = ReactiveUI.ReactiveCommand.Create(() => ShowDiagnosticsMenu());
            ShowMainWindowCommand = ReactiveUI.ReactiveCommand.Create(() => ShowMainWindow());
            ShowAlertsCommand = ReactiveUI.ReactiveCommand.Create(() => ShowAlertsMenu());
        }

        #region Pump Event Handlers

        /// <summary>
        /// Handles the channel numbers.
        /// </summary>
        /// <param name="totalChannels"></param>
        private void Pump_ChannelNumbers(int totalChannels)
        {
            if (ChannelNumber > totalChannels)
            {
                ChannelNumber = totalChannels;
            }

            MinChannelNumber = Math.Min(1, totalChannels);
            MaxChannelNumber = totalChannels;
        }

        private void UpdateStatusLabelText(object sender, classDeviceStatusEventArgs e)
        {
            StatusText = e.Message;
        }

        /// <summary>
        /// Handles pump status changes, not just status of the object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pump_PumpStatus(object sender, classDeviceStatusEventArgs e)
        {
            UpdateStatusLabelText(sender, e);
        }

        /// <summary>
        /// Updates the list box with the appropiate method names.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void Pump_MethodNames(object sender, List<object> data)
        {
            ReactiveUI.RxApp.MainThreadScheduler.Schedule(() =>
            {
                using (methodComboBoxOptions.SuppressChangeNotifications())
                {
                    methodComboBoxOptions.Clear();
                    methodComboBoxOptions.AddRange(data.Select(x => x.ToString()));
                }
            });
        }

        private void m_pump_StatusUpdate(object sender, classDeviceStatusEventArgs e)
        {
            UpdateStatusDisplay(e.Message);
        }

        private void m_pump_Error(object sender, classDeviceErrorEventArgs e)
        {
            UpdateStatusDisplay(e.Error);
        }

        #endregion

        #region Button Event Handlers

        private void UpdateMethods()
        {
            UpdateStatusDisplay("");
            m_pump.GetMethods();
        }

        private void ShowMethodMenu()
        {
            UpdateStatusDisplay("");
            if (ChannelNumber < 1)
            {
                UpdateStatusDisplay("Set the channel first other than zero.");
                return;
            }
            if (string.IsNullOrWhiteSpace(SelectedMethod))
            {
                UpdateStatusDisplay("Select a method first.");
                return;
            }
            m_pump.ShowMethodMenu(ChannelNumber, SelectedMethod);
        }

        private void ShowDirectControl()
        {
            UpdateStatusDisplay("");
            if (ChannelNumber < 1)
            {
                UpdateStatusDisplay("Set the channel first other than zero.");
                return;
            }
            try
            {
                m_pump.ShowDirectControl(ChannelNumber);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, ex.Message);
            }
        }

        private void ShowMobilePhaseMenu()
        {
            UpdateStatusDisplay("");
            if (ChannelNumber < 1)
            {
                UpdateStatusDisplay("Set the channel first other than zero.");
                return;
            }

            try
            {
                m_pump.ShowMobilePhaseMenu(ChannelNumber);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, ex.Message);
            }
        }

        private void ShowInstrumentConfig()
        {
            UpdateStatusDisplay("");

            try
            {
                m_pump.ShowInstrumentConfigMenu();
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, ex.Message);
            }
        }

        private void ShowAdvancedSettings()
        {
            UpdateStatusDisplay("");

            try
            {
                m_pump.ShowAdvancedSettings();
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, ex.Message);
            }
        }

        private void ShowDiagnosticsMenu()
        {
            UpdateStatusDisplay("");
            if (ChannelNumber < 1)
            {
                UpdateStatusDisplay("Set the channel first other than zero.");
                return;
            }

            try
            {
                m_pump.ShowDiagnosticsMenu(ChannelNumber);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, ex.Message);
            }
        }

        private void ShowMainWindow()
        {
            UpdateStatusDisplay("");

            try
            {
                m_pump.ShowMainWindow();
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, ex.Message);
            }
        }

        private void ShowAlertsMenu()
        {
            UpdateStatusDisplay("");

            try
            {
                m_pump.ShowAlertsMenu();
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, ex.Message);
            }
        }

        private void StartPump()
        {
            UpdateStatusDisplay("");

            if (ChannelNumber < 1)
            {
                UpdateStatusDisplay("Set the channel first other than zero.");
                return;
            }
            try
            {
                if (string.IsNullOrWhiteSpace(SelectedMethod))
                {
                    UpdateStatusDisplay("Select a method first.");
                    return;
                }
                m_pump.StartMethod(0, ChannelNumber, SelectedMethod);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, ex.Message);
            }
        }

        private void StopPump()
        {
            UpdateStatusDisplay("");
            if (ChannelNumber < 1)
            {
                UpdateStatusDisplay("Set the channel first other than zero.");
                return;
            }

            try
            {
                m_pump.StopMethod(0, ChannelNumber);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, ex.Message);
            }
        }

        #endregion
    }
}
