using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DynamicData;
using LcmsNetCommonControls.Devices;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNetPlugins.Newport.ESP300
{
    public class NewportStageViewModel : BaseDeviceControlViewModelReactive, IDeviceControl
    {
        public NewportStageViewModel()
        {
            positionsList.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var positionsListBound).Subscribe();
            PositionsList = positionsListBound;

            PropertyChanged += NewportStageViewModel_PropertyChanged;

            RefreshPositionCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(RefreshPosition));
            Axis1ForwardCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(Axis1FwdPress));
            Axis2ForwardCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(Axis2FwdPress));
            Axis3ForwardCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(Axis3FwdPress));
            Axis1ForwardReleaseCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(Axis1FwdRelease));
            Axis2ForwardReleaseCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(Axis2FwdRelease));
            Axis3ForwardReleaseCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(Axis3FwdRelease));
            Axis1BackwardCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(Axis1BackPress));
            Axis2BackwardCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(Axis2BackPress));
            Axis3BackwardCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(Axis3BackPress));
            Axis1BackwardReleaseCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(Axis1BackRelease));
            Axis2BackwardReleaseCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(Axis2BackRelease));
            Axis3BackwardReleaseCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(Axis3BackRelease));
            ResetToHomePositionCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(ResetToHome));
            GoToSelectedPositionCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(GoToSelectedPosition));
            DeletePositionCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(RemovePosition));
            SetPositionCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(SetPosition));
            Axis1MotorPowerCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(ToggleAxis1Motor));
            Axis2MotorPowerCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(ToggleAxis2Motor));
            Axis3MotorPowerCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(ToggleAxis3Motor));
            GetErrorsCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(GetErrors));
            ClearErrorsCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(ClearErrors));
            OpenPortCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(OpenPort));
            ClosePortCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(ClosePort));
        }

        #region Members

        private NewportStage newportStage;
        private const string units = "mm";
        private const string positionNotDefined = "NoPosition";
        private readonly SourceList<string> positionsList = new SourceList<string>();
        private string axis1MotorStatus = "";
        private string axis2MotorStatus = "";
        private string axis3MotorStatus = "";
        private string axis1Position = "";
        private string axis2Position = "";
        private string axis3Position = "";
        private string selectedPosition = "";
        private string currentPosition = "";
        private string newPosition = "";
        private bool controlsTabSelected = true;

        #endregion

        #region Properties

        public ReadOnlyObservableCollection<string> PositionsList { get; }

        public string Axis1MotorStatus
        {
            get => axis1MotorStatus;
            set => this.RaiseAndSetIfChanged(ref axis1MotorStatus, value);
        }

        public string Axis2MotorStatus
        {
            get => axis2MotorStatus;
            set => this.RaiseAndSetIfChanged(ref axis2MotorStatus, value);
        }

        public string Axis3MotorStatus
        {
            get => axis3MotorStatus;
            set => this.RaiseAndSetIfChanged(ref axis3MotorStatus, value);
        }

        public string Axis1Position
        {
            get => axis1Position;
            set => this.RaiseAndSetIfChanged(ref axis1Position, value);
        }

        public string Axis2Position
        {
            get => axis2Position;
            set => this.RaiseAndSetIfChanged(ref axis2Position, value);
        }

        public string Axis3Position
        {
            get => axis3Position;
            set => this.RaiseAndSetIfChanged(ref axis3Position, value);
        }

        public string SelectedPosition
        {
            get => selectedPosition;
            set => this.RaiseAndSetIfChanged(ref selectedPosition, value);
        }

        public string CurrentPosition
        {
            get => currentPosition;
            set => this.RaiseAndSetIfChanged(ref currentPosition, value);
        }

        public string NewPosition
        {
            get => newPosition;
            set => this.RaiseAndSetIfChanged(ref newPosition, value);
        }

        public bool ControlsTabSelected
        {
            get => controlsTabSelected;
            set => this.RaiseAndSetIfChanged(ref controlsTabSelected, value);
        }

        public NewportStage NewportStage
        {
            get => newportStage;
            private set => this.RaiseAndSetIfChanged(ref newportStage, value);
        }

        public override IDevice Device
        {
            get => NewportStage;
            set => RegisterDevice(value);
        }

        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> RefreshPositionCommand { get; }
        public ReactiveCommand<Unit, Unit> Axis1ForwardCommand { get; }
        public ReactiveCommand<Unit, Unit> Axis2ForwardCommand { get; }
        public ReactiveCommand<Unit, Unit> Axis3ForwardCommand { get; }
        public ReactiveCommand<Unit, Unit> Axis1BackwardCommand { get; }
        public ReactiveCommand<Unit, Unit> Axis2BackwardCommand { get; }
        public ReactiveCommand<Unit, Unit> Axis3BackwardCommand { get; }
        public ReactiveCommand<Unit, Unit> Axis1ForwardReleaseCommand { get; }
        public ReactiveCommand<Unit, Unit> Axis2ForwardReleaseCommand { get; }
        public ReactiveCommand<Unit, Unit> Axis3ForwardReleaseCommand { get; }
        public ReactiveCommand<Unit, Unit> Axis1BackwardReleaseCommand { get; }
        public ReactiveCommand<Unit, Unit> Axis2BackwardReleaseCommand { get; }
        public ReactiveCommand<Unit, Unit> Axis3BackwardReleaseCommand { get; }
        public ReactiveCommand<Unit, Unit> ResetToHomePositionCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToSelectedPositionCommand { get; }
        public ReactiveCommand<Unit, Unit> DeletePositionCommand { get; }
        public ReactiveCommand<Unit, Unit> SetPositionCommand { get; }
        public ReactiveCommand<Unit, Unit> Axis1MotorPowerCommand { get; }
        public ReactiveCommand<Unit, Unit> Axis2MotorPowerCommand { get; }
        public ReactiveCommand<Unit, Unit> Axis3MotorPowerCommand { get; }
        public ReactiveCommand<Unit, Unit> GetErrorsCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearErrorsCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenPortCommand { get; }
        public ReactiveCommand<Unit, Unit> ClosePortCommand { get; }

        #endregion

        public override UserControl GetDefaultView()
        {
            return new NewportStageView();
        }

        private void NewportStageViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(ControlsTabSelected)) && ControlsTabSelected)
            {
                UpdateAxisPositions();
            }
        }

        public void RegisterDevice(IDevice device)
        {
            NewportStage = device as NewportStage;
            SetBaseDevice(NewportStage);
            NewportStage.PositionsLoaded += PositionLoadHandler;
            NewportStage.PositionChanged += PositionChangedHandler;
            NewportStage.StatusUpdate += m_obj_StatusUpdate;
            UpdateAxisPositions();
            UpdatePositionListBox();
            UpdatePositionLabel(NewportStage.CurrentPos);
            UpdateMotorStatus();
        }

        private void m_obj_StatusUpdate(object sender, DeviceStatusEventArgs e)
        {
            var tokens = e.Message.Split(new char[' ']);
            if (e.Notification == "Motor")
            {
                switch (tokens[0])
                {
                    case "1":
                        Axis1MotorStatus = tokens[1];
                        break;
                    case "2":
                        Axis2MotorStatus = tokens[1];
                        break;
                    case "3":
                        Axis3MotorStatus = tokens[1];
                        break;
                    default:
                        // we don't care about other updates...for now.
                        break;
                }
            }
            else if (e.Notification == "Initialized")
            {
                UpdateMotorStatus();
            }
        }

        private void RefreshPosition()
        {
            UpdateAxisPositions();
        }

        private void UpdateMotorStatus()
        {
            Axis1MotorStatus = NewportStage.GetMotorStatus(1) ? "On" : "Off";
            Axis2MotorStatus = NewportStage.GetMotorStatus(2) ? "On" : "Off";
            Axis3MotorStatus = NewportStage.GetMotorStatus(3) ? "On" : "Off";
        }

        private void UpdateAxisPositions()
        {
            if (NewportStage == null)
            {
                return;
            }
            try
            {
                Axis1Position = NewportStage.QueryPosition(1).ToString("0.00") + units;
                Axis2Position = NewportStage.QueryPosition(2).ToString("0.00") + units;
                Axis3Position = NewportStage.QueryPosition(3).ToString("0.00") + units;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error getting Position: " + e.Message);
            }
        }

        private void UpdatePositionLabel(string pos)
        {
            CurrentPosition = pos;
        }

        private void ResetToHome()
        {
            NewportStage.FindHome(1);
            NewportStage.FindHome(2);
            NewportStage.FindHome(3);
        }

        private void Axis1BackPress()
        {
            NewportStage.MoveAxis(1, true);
            CurrentPosition = positionNotDefined;
        }

        private void Axis1BackRelease()
        {
            NewportStage.StopMotion(1);
            Axis1Position = NewportStage.QueryPosition(1).ToString("0.00") + units;
            CurrentPosition = positionNotDefined;
        }

        private void Axis1FwdPress()
        {
            NewportStage.MoveAxis(1, false);
        }

        private void Axis1FwdRelease()
        {
            NewportStage.StopMotion(1);
            Axis1Position = NewportStage.QueryPosition(1).ToString("0.00") + units;
            CurrentPosition = positionNotDefined;
        }

        private void Axis2BackPress()
        {
            NewportStage.MoveAxis(2, true);
        }

        private void Axis2BackRelease()
        {
            NewportStage.StopMotion(2);
            Axis2Position = NewportStage.QueryPosition(2).ToString("0.00") + units;
            CurrentPosition = positionNotDefined;
        }

        private void Axis2FwdPress()
        {
            NewportStage.MoveAxis(2, false);
        }

        private void Axis2FwdRelease()
        {
            NewportStage.StopMotion(2);
            Axis2Position = NewportStage.QueryPosition(2).ToString("0.00") + units;
            CurrentPosition = positionNotDefined;
        }

        private void Axis3FwdPress()
        {
            NewportStage.MoveAxis(3, false);
        }

        private void Axis3FwdRelease()
        {
            NewportStage.StopMotion(3);
            Axis3Position = NewportStage.QueryPosition(3).ToString("0.00") + units;
            CurrentPosition = positionNotDefined;
        }

        private void Axis3BackPress()
        {
            NewportStage.MoveAxis(3, true);
        }

        private void Axis3BackRelease()
        {
            NewportStage.StopMotion(3);
            Axis3Position = NewportStage.QueryPosition(3).ToString("0.00") + units;
            CurrentPosition = positionNotDefined;
        }

        private void GoToSelectedPosition()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(SelectedPosition))
                {
                    var pos = SelectedPosition;
                    NewportStage.GoToPosition(5000, pos);
                    UpdateAxisPositions();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void SetPosition()
        {
            var pos = NewPosition;
            if (pos != string.Empty)
            {
                var axis1Pos = NewportStage.QueryPosition(1);
                var axis2Pos = NewportStage.QueryPosition(2);
                var axis3Pos = NewportStage.QueryPosition(3);
                NewportStage.SetPositionCoordinates(pos, Convert.ToSingle(axis1Pos), Convert.ToSingle(axis2Pos), Convert.ToSingle(axis3Pos));
                UpdatePositionListBox();
                CurrentPosition = pos;
            }
            else
            {
                MessageBox.Show("A position name is required.", "Error", MessageBoxButton.OK);
            }
        }

        private void GetErrors()
        {
            MessageBox.Show(NewportStage.GetErrors(), "ESP300 Errors", MessageBoxButton.OK);
        }

        private void UpdatePositionListBox()
        {
            positionsList.Edit(list =>
            {
                list.Clear();
                list.AddRange(NewportStage.Positions.Keys);
            });
        }

        private void PositionLoadHandler(object sender, EventArgs e)
        {
            UpdatePositionListBox();
        }

        private void PositionChangedHandler(object sender, EventArgs e)
        {
            var stage = sender as NewportStage;
            if (stage != null)
                UpdatePositionLabel(stage.CurrentPos);
        }

        private void RemovePosition()
        {
            if (!string.IsNullOrWhiteSpace(SelectedPosition))
            {
                var pos = SelectedPosition;
                NewportStage.RemovePosition(pos);
                UpdatePositionListBox();
                if (CurrentPosition == pos)
                {
                    CurrentPosition = positionNotDefined;
                }
            }
        }

        private void ClearErrors()
        {
            NewportStage.ClearErrors();
        }

        private void OpenPort()
        {
            NewportStage.OpenPort();
        }

        private void ClosePort()
        {
            NewportStage.ClosePort();
        }

        private bool ToggleMotor(int axis)
        {
            var motorOn = NewportStage.GetMotorStatus(axis);
            if (motorOn)
            {
                NewportStage.MotorOff(axis);
            }
            else
            {
                NewportStage.MotorOn(axis);
            }
            return NewportStage.GetMotorStatus(axis);
        }

        private void ToggleAxis1Motor()
        {
            switch (ToggleMotor(1))
            {
                case true:
                    Axis1MotorStatus = "On";
                    break;
                case false:
                    Axis1MotorStatus = "Off";
                    break;
                default:
                    throw new Exception("Something went wrong when toggling Axis1 Motor of Newport stage");
            }
        }

        private void ToggleAxis2Motor()
        {
            switch (ToggleMotor(2))
            {
                case true:
                    Axis2MotorStatus = "On";
                    break;
                case false:
                    Axis2MotorStatus = "Off";
                    break;
                default:
                    throw new Exception("Something went wrong when toggling Axis2 Motor of Newport stage");
            }
        }

        private void ToggleAxis3Motor()
        {
            switch (ToggleMotor(3))
            {
                case true:
                    Axis3MotorStatus = "On";
                    break;
                case false:
                    Axis3MotorStatus = "Off";
                    break;
                default:
                    throw new Exception("Something went wrong when toggling Axis3 Motor of Newport stage");
            }
        }
    }
}
