using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using LcmsNetCommonControls.Devices.Pumps;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNet.Devices.Pumps.ViewModels
{
    public class PumpDisplaysViewModel : ReactiveObject
    {
        private readonly List<IPump> pumps;

        readonly Dictionary<IPump, PumpDisplayViewModel> pumpMonitorDictionary;

        public PumpDisplaysViewModel()
        {
            mobilePhases.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var mobilePhasesBound).Subscribe();
            pumpMonitorDisplays.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var pumpMonitorsBound).Subscribe();
            MobilePhases = mobilePhasesBound;
            PumpMonitorDisplays = pumpMonitorsBound;

#if DEBUG
            // Avoid exceptions caused from not being able to access program settings, when being run to provide design-time data context for the designer
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                mobilePhases.Add(new MobilePhase("test1", "comment"));
                mobilePhases.Add(new MobilePhase("test2", "comment"));
                mobilePhases.Add(new MobilePhase("test3", "comment"));
                mobilePhases.Add(new MobilePhase("test4", "comment"));
                pumpMonitorDisplays.Add(new PumpDisplayViewModel("test1"));
                pumpMonitorDisplays.Add(new PumpDisplayViewModel("test2"));
                pumpName = "TestPump";
            }
#endif

            pumps = new List<IPump>();
            pumpMonitorDictionary = new Dictionary<IPump, PumpDisplayViewModel>();
            DeviceManager.Manager.DeviceAdded += Manager_DeviceAdded;
            DeviceManager.Manager.DeviceRemoved += Manager_DeviceRemoved;
            CurrentPump = 0;

            MoveLeftCommand = ReactiveCommand.Create(() => this.MoveLeft(), this.WhenAnyValue(x => x.CurrentPump).Select(x => 0 <= x - 1 && x - 1 < pumps.Count));
            MoveRightCommand = ReactiveCommand.Create(() => this.MoveRight(), this.WhenAnyValue(x => x.CurrentPump).Select(x => 0 <= x + 1 && x + 1 < pumps.Count));
        }

        private int currentPump;
        private string pumpName = "";

        private readonly SourceList<MobilePhase> mobilePhases = new SourceList<MobilePhase>();
        private readonly SourceList<PumpDisplayViewModel> pumpMonitorDisplays = new SourceList<PumpDisplayViewModel>();

        private int CurrentPump
        {
            get => currentPump;
            set => this.RaiseAndSetIfChanged(ref currentPump, value);
        }

        public string PumpName
        {
            get => pumpName;
            set => this.RaiseAndSetIfChanged(ref pumpName, value);
        }

        public ReadOnlyObservableCollection<MobilePhase> MobilePhases { get; }

        public ReadOnlyObservableCollection<PumpDisplayViewModel> PumpMonitorDisplays { get; }

        public ReactiveCommand<Unit, Unit> MoveLeftCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveRightCommand { get; }

        private void MoveLeft()
        {
            CurrentPump = Math.Max(0, --CurrentPump);

            if (pumps.Count < 1)
            {
                PumpName = "";
                return;
            }

            UpdateLabel();
        }

        private void MoveRight()
        {
            CurrentPump = Math.Min(pumps.Count - 1, ++CurrentPump);
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            var pump = pumps[CurrentPump];

            mobilePhases.Edit(list =>
            {
                list.Clear();
                if (pump.MobilePhases != null)
                {
                    list.AddRange(pump.MobilePhases);
                }
            });

            PumpName = pump.Name;
        }

        #region Device Manager Event Handlers

        /// <summary>
        /// Handles when the device manager removes a device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        private void Manager_DeviceRemoved(object sender, IDevice device)
        {
            // Is this a pump?
            var pump = device as IPump;
            if (pump == null)
                return;

            // Make sure we have the pump
            if (pumpMonitorDictionary.ContainsKey(pump) == false)
                return;

            // If it's a pump and we have it we are safe to remove it from the list of controls
            // and the mapping dictionary.
            pumpMonitorDisplays.Remove(pumpMonitorDictionary[pump]);
            pumpMonitorDictionary.Remove(pump);

            if (pumps.Contains(pump))
                pumps.Remove(pump);

            MoveLeft();
        }

        /// <summary>
        /// Checks to update the name of the device.
        /// </summary>
        /// <param name="sender"></param>
        private void pump_DeviceSaveRequired(object sender, EventArgs e)
        {
            var pump = sender as IPump;
            if (pump == null)
                return;

            if (pumpMonitorDictionary.ContainsKey(pump) == false)
                return;

            pumpMonitorDictionary[pump].SetPumpName(pump.Name);
        }

        /// <summary>
        /// Handles when the device manager adds a device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        private void Manager_DeviceAdded(object sender, IDevice device)
        {
            // Make sure the device is a pump.
            var pump = device as IPump;
            if (pump == null)
                return;

            // Make sure we have a reference to the pump
            if (pumpMonitorDictionary.ContainsKey(pump))
                return;

            // Hook into the pumps display calls
            var display = new PumpDisplayViewModel(pump.Name);

            pump.MonitoringDataReceived += display.DisplayMonitoringData;
            pump.DeviceSaveRequired += pump_DeviceSaveRequired;

            // Make sure we reference this pump
            pumpMonitorDictionary.Add(pump, display);
            pumpMonitorDisplays.Add(display);

            if (pump.MobilePhases != null)
            {
                pumps.Add(pump);

                CurrentPump = pumps.Count - 1;
                UpdateLabel();

                mobilePhases.Edit(list =>
                {
                    list.Clear();
                    list.AddRange(pump.MobilePhases);
                });
            }
        }

        #endregion
    }
}
