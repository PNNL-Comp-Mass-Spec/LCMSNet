﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using DynamicData;
using LcmsNetData;
using LcmsNetData.Logging;
using LcmsNetSDK.Devices;

namespace LcmsNetPlugins.Eksigent.Pumps
{
    public class EksigentPumpControlViewModel : BaseDeviceControlViewModel, IDeviceControl, INotifyPropertyChangedExt
    {
        public EksigentPumpControlViewModel()
        {
            methodComboBoxOptions.Connect().ObserveOn(ReactiveUI.RxApp.MainThreadScheduler).Bind(out var methodComboBoxOptionsBound).Subscribe();
            MethodComboBoxOptions = methodComboBoxOptionsBound;

            ShowMethodEditorCommand = ReactiveUI.ReactiveCommand.Create(ShowMethodMenu);
            ShowDirectControlCommand = ReactiveUI.ReactiveCommand.Create(ShowDirectControl);
            ShowMobilePhasesCommand = ReactiveUI.ReactiveCommand.Create(ShowMobilePhaseMenu);
            UpdateMethodsCommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () => await Task.Run(UpdateMethods));
            StartPumpCommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () => await Task.Run(StartPump));
            StopPumpCommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () => await Task.Run(StopPump));
            ShowIntrumentConfigCommand = ReactiveUI.ReactiveCommand.Create(ShowInstrumentConfig);
            ShowAdvancedSettingsCommand = ReactiveUI.ReactiveCommand.Create(ShowAdvancedSettings);
            ShowDiagnosticsCommand = ReactiveUI.ReactiveCommand.Create(ShowDiagnosticsMenu);
            ShowMainWindowCommand = ReactiveUI.ReactiveCommand.Create(ShowMainWindow);
            ShowAlertsCommand = ReactiveUI.ReactiveCommand.Create(ShowAlertsMenu);
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
                ApplicationLogger.LogError(0, "Could not register the Eksigent control software OCX. " + ex.Message, ex);
            }
        }

        #region IDeviceControl Members

        public override IDevice Device
        {
            get => m_pump;
            set => RegisterDevice(value);
        }

        public override UserControl GetDefaultView()
        {
            return new EksigentPumpControlView();
        }

        #endregion

        /// <summary>
        /// Notification driver object.
        /// </summary>
        private EksigentPump m_pump;

        private readonly SourceList<string> methodComboBoxOptions = new SourceList<string>();
        private string selectedMethod = "";
        private int minChannelNumber = 0;
        private int maxChannelNumber = 1;
        private int channelNumber = 1;
        private string statusText = "Unknown";

        public ReadOnlyObservableCollection<string> MethodComboBoxOptions {get; }

        public string SelectedMethod
        {
            get => selectedMethod;
            set => this.RaiseAndSetIfChanged(ref selectedMethod, value);
        }

        public int MinChannelNumber
        {
            get => minChannelNumber;
            private set => this.RaiseAndSetIfChanged(ref minChannelNumber, value);
        }

        public int MaxChannelNumber
        {
            get => maxChannelNumber;
            private set => this.RaiseAndSetIfChanged(ref maxChannelNumber, value);
        }

        public int ChannelNumber
        {
            get => channelNumber;
            set => this.RaiseAndSetIfChanged(ref channelNumber, value);
        }

        public string StatusText
        {
            get => statusText;
            private set => this.RaiseAndSetIfChanged(ref statusText, value);
        }

        public ReactiveUI.ReactiveCommand<Unit, Unit> ShowMethodEditorCommand { get; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ShowDirectControlCommand { get; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ShowMobilePhasesCommand { get; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> UpdateMethodsCommand { get; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> StartPumpCommand { get; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> StopPumpCommand { get; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ShowIntrumentConfigCommand { get; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ShowAdvancedSettingsCommand { get; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ShowDiagnosticsCommand { get; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ShowMainWindowCommand { get; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ShowAlertsCommand { get; }

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

        private void UpdateStatusLabelText(object sender, DeviceStatusEventArgs e)
        {
            StatusText = e.Message;
        }

        /// <summary>
        /// Handles pump status changes, not just status of the object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pump_PumpStatus(object sender, DeviceStatusEventArgs e)
        {
            UpdateStatusLabelText(sender, e);
        }

        /// <summary>
        /// Updates the list box with the appropriate method names.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void Pump_MethodNames(object sender, List<object> data)
        {
            methodComboBoxOptions.Edit(list =>
            {
                list.Clear();
                list.AddRange(data.Select(x => x.ToString()));
            });
        }

        private void m_pump_StatusUpdate(object sender, DeviceStatusEventArgs e)
        {
            UpdateStatusDisplay(e.Message);
        }

        private void m_pump_Error(object sender, DeviceErrorEventArgs e)
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
                ApplicationLogger.LogError(0, ex.Message);
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
                ApplicationLogger.LogError(0, ex.Message);
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
                ApplicationLogger.LogError(0, ex.Message);
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
                ApplicationLogger.LogError(0, ex.Message);
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
                ApplicationLogger.LogError(0, ex.Message);
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
                ApplicationLogger.LogError(0, ex.Message);
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
                ApplicationLogger.LogError(0, ex.Message);
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
                ApplicationLogger.LogError(0, ex.Message);
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
                ApplicationLogger.LogError(0, ex.Message);
            }
        }

        #endregion
    }
}
