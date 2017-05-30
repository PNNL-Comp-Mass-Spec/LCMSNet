using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Devices.Pumps;
using LcmsNetSDK.Data;
using ReactiveUI;

namespace LcmsNet.Devices.Pumps.ViewModels
{
    public class PumpDisplaysViewModel : ReactiveObject
    {
        private readonly List<IPump> pumps;

        //TODO: WPF form of "Tack"/"UnTack": private PumpDisplaysWindow pumpDisplaysWindow = null;

        readonly Dictionary<IPump, PumpDisplayViewModel> pumpMonitorDictionary;

        public PumpDisplaysViewModel()
        {
#if DEBUG
            // Avoid exceptions caused from not being able to access program settings, when being run to provide design-time data context for the designer
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                MobilePhases.Add(new MobilePhase("test1", "comment"));
                MobilePhases.Add(new MobilePhase("test2", "comment"));
                MobilePhases.Add(new MobilePhase("test3", "comment"));
                MobilePhases.Add(new MobilePhase("test4", "comment"));
                PumpMonitorDisplays.Add(new PumpDisplayViewModel("test1"));
                PumpMonitorDisplays.Add(new PumpDisplayViewModel("test2"));
                pumpName = "TestPump";
            }
#endif
            pumps = new List<IPump>();
            pumpMonitorDictionary = new Dictionary<IPump, PumpDisplayViewModel>();
            DeviceManagerBridge.DeviceAdded += Manager_DeviceAdded;
            DeviceManagerBridge.DeviceRemoved += Manager_DeviceRemoved;
            CurrentPump = 0;
            SetupCommands();
        }


        private int currentPump;
        private string pumpName = "";
        private string tackType = "Untack";

        private readonly ReactiveList<MobilePhase> mobilePhases = new ReactiveList<MobilePhase>();
        private readonly ReactiveList<PumpDisplayViewModel> pumpMonitorDisplays = new ReactiveList<PumpDisplayViewModel>();

        private int CurrentPump
        {
            get { return currentPump; }
            set { this.RaiseAndSetIfChanged(ref currentPump, value); }
        }

        public string PumpName
        {
            get { return pumpName; }
            set { this.RaiseAndSetIfChanged(ref pumpName, value); }
        }

        public string TackType
        {
            get { return tackType; }
            set { this.RaiseAndSetIfChanged(ref tackType, value); }
        }

        public ReactiveList<MobilePhase> MobilePhases => mobilePhases;

        public ReactiveList<PumpDisplayViewModel> PumpMonitorDisplays => pumpMonitorDisplays;

        /// <summary>
        /// Gets or sets whether the window is tacked.
        /// </summary>
        public bool IsTacked { get; set; }

        public event EventHandler Tack;
        public event EventHandler UnTack;

        private void ChangeWindowTack()
        {
            IsTacked = (IsTacked == false);
            if (IsTacked)
            {
                Tack?.Invoke(this, new EventArgs());

                //TODO: WPF form of "UnTack": pumpDisplaysWindow = new PumpDisplaysWindow();
                //TODO: WPF form of "UnTack": pumpDisplaysWindow.DataContext = this;
                //TODO: WPF form of "UnTack": pumpDisplaysWindow.Show();
                TackType = "Tack";
            }
            else
            {
                UnTack?.Invoke(this, new EventArgs());

                //TODO: WPF form of "Tack": pumpDisplaysWindow?.Close();
                TackType = "Untack";
            }
        }

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

            using (MobilePhases.SuppressChangeNotifications())
            {
                MobilePhases.Clear();
                if (pump.MobilePhases != null)
                {
                    MobilePhases.AddRange(pump.MobilePhases);
                }
            }

            PumpName = pump.Name;
        }

        #region Device Manager Event Handlers

        /// <summary>
        /// Handles when the device manager removes a device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceRemoved(object sender, IDevice device)
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
            PumpMonitorDisplays.Remove(pumpMonitorDictionary[pump]);
            pumpMonitorDictionary.Remove(pump);

            if (pumps.Contains(pump))
                pumps.Remove(pump);

            MoveLeft();
        }

        /// <summary>
        /// Checks to update the name of the device.
        /// </summary>
        /// <param name="sender"></param>
        void pump_DeviceSaveRequired(object sender, EventArgs e)
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
        void Manager_DeviceAdded(object sender, IDevice device)
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

            pump.MonitoringDataReceived += DisplayPumpData;
            pump.DeviceSaveRequired += pump_DeviceSaveRequired;

            // Make sure we reference this pump
            pumpMonitorDictionary.Add(pump, display);
            PumpMonitorDisplays.Add(display);

            if (pump.MobilePhases != null)
            {
                pumps.Add(pump);

                CurrentPump = pumps.Count - 1;
                UpdateLabel();

                using (MobilePhases.SuppressChangeNotifications())
                {
                    MobilePhases.Clear();
                    MobilePhases.AddRange(pump.MobilePhases);
                }
            }
        }

        /// <summary>
        /// Updates the appropriate control
        /// </summary>
        void DisplayPumpData(object sender, PumpDataEventArgs e)
        {
            pumpMonitorDictionary[e.Pump].DisplayMonitoringData(sender, e);
        }

        #endregion

        public ReactiveCommand MoveLeftCommand { get; private set; }
        public ReactiveCommand MoveRightCommand { get; private set; }
        public ReactiveCommand ChangeTackCommand { get; private set; }

        private void SetupCommands()
        {
            MoveLeftCommand = ReactiveCommand.Create(() => this.MoveLeft(), this.WhenAnyValue(x => x.CurrentPump).Select(x => 0 <= x - 1 && x - 1 < pumps.Count));
            MoveRightCommand = ReactiveCommand.Create(() => this.MoveRight(), this.WhenAnyValue(x => x.CurrentPump).Select(x => 0 <= x + 1 && x + 1 < pumps.Count));
            ChangeTackCommand = ReactiveCommand.Create(() => this.ChangeWindowTack());
        }
    }
}
