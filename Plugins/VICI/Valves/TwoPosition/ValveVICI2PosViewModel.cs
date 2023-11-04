using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows.Controls;
using FluidicsSDK.Base;
using FluidicsSDK.Devices.Valves;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNetPlugins.VICI.Valves
{
    public class ValveVICI2PosViewModel : ValveVICIViewModelBase
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ValveVICI2PosViewModel()
        {
            SetPositionACommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(SetPositionA));
            SetPositionBCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(SetPositionB));
            SetInjectionVolumeCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(SetInjectionVolume));
        }

        protected override void RegisterDevice(IDevice device)
        {
            //TODO: Throw error!
            if (!(device is ValveVICI2Pos v2p))
            {
                return;
            }

            valve = v2p;
            valve.PositionChanged += OnPosChanged;

            RegisterBaseDevice(valve);

            if (Device is ISixPortInjectionValve injector)
            {
                InjectionVolume = injector.InjectionVolume;
                injector.InjectionVolumeChanged += Injector_InjectionVolumeChanged;
                IsInjectionValve = true;
            }
            else
            {
                IsInjectionValve = false;
            }
        }

        /// <summary>
        /// The valve object
        /// </summary>
        private ValveVICI2Pos valve;

        /// <summary>
        /// Gets or sets the associated device.
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

        private bool isInjectionValve = false;
        private double injectionVolume = 0;

        public bool IsInjectionValve
        {
            get => isInjectionValve;
            private set => this.RaiseAndSetIfChanged(ref isInjectionValve, value);
        }

        public double InjectionVolume
        {
            get => injectionVolume;
            set => this.RaiseAndSetIfChanged(ref injectionVolume, value);
        }

        public ReactiveCommand<Unit, Unit> SetPositionACommand { get; }
        public ReactiveCommand<Unit, Unit> SetPositionBCommand { get; }
        public ReactiveCommand<Unit, Unit> SetInjectionVolumeCommand { get; }

        public override UserControl GetDefaultView()
        {
            return new ValveVICI2PosView();
        }

        /// <summary>
        /// Indicates that the valve's position has changed.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="newPosition">The new position</param>
        public virtual void OnPosChanged(object sender, ValvePositionEventArgs<TwoPositionState> newPosition)   // DAC changed
        {
            UpdatePositionTextBox(newPosition.Position);
        }

        /// <summary>
        /// Updates the position textbox on the control.
        /// </summary>
        /// <param name="position"></param>
        private void UpdatePositionTextBox(TwoPositionState position)
        {
            RxApp.MainThreadScheduler.Schedule(() => CurrentValvePosition = position.GetEnumDescription());
        }

        private void SetPositionA()
        {
            try
            {
                /*Thread p = new Thread(() => valve.SetPosition(TwoPositionState.PositionA));
                p.Start();
                p = null;*/
                valve.SetPosition(TwoPositionState.PositionA);
                //UpdatePositionTextBox(valve.LastMeasuredPosition);
            }
            catch (ValveExceptionReadTimeout ex)
            {
                ShowError("Timeout (read) when attempting to set valve position", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                ShowError("Timeout (write) when attempting to set valve position", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access when attempting to set valve position", ex);
            }
            catch (ValveExceptionPositionMismatch ex)
            {
                ShowError("Valve position mismatch", ex);
            }
            catch (Exception Ex)
            {
                ShowError("Exception in valve control", Ex);
            }
        }

        private void SetPositionB()
        {
            try
            {
                /*Thread p = new Thread(() => valve.SetPosition(TwoPositionState.PositionB));
                p.Start();
                p = null;*/
                valve.SetPosition(TwoPositionState.PositionB);
                //UpdatePositionTextBox(valve.LastMeasuredPosition);
            }
            catch (ValveExceptionReadTimeout ex)
            {
                ShowError("Timeout (read) when attempting to set valve position", ex);
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                ShowError("Timeout (write) when attempting to set valve position", ex);
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                ShowError("Unauthorized access when attempting to set valve position", ex);
            }
            catch (ValveExceptionPositionMismatch ex)
            {
                ShowError("Valve position mismatch", ex);
            }
        }

        private void SetInjectionVolume()
        {
            if (Device is ISixPortInjectionValve injector)
                injector.InjectionVolume = InjectionVolume;
        }

        private void Injector_InjectionVolumeChanged(object sender, System.EventArgs e)
        {
            if (Device is ISixPortInjectionValve injector)
            {
                InjectionVolume = injector.InjectionVolume;
            }
        }
    }
}
