using System;
using System.Reactive;
using System.Windows;
using System.Windows.Controls;
using LcmsNetDataClasses.Devices;
using LcmsNetSDK;

namespace Newport.ESP300
{
    public class NewportStageViewModel : BaseDeviceControlViewModel, IDeviceControl
    {
        public NewportStageViewModel()
        {
            SetupCommands();
            PropertyChanged += NewportStageViewModel_PropertyChanged;
        }

        #region Members

        private classNewportStage newportStage;
        private const string units = "mm";
        private const string positionNotDefined = "NoPosition";
        private readonly ReactiveUI.ReactiveList<string> positionsList = new ReactiveUI.ReactiveList<string>();
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

        public ReactiveUI.IReadOnlyReactiveList<string> PositionsList => positionsList;

        public string Axis1MotorStatus
        {
            get { return axis1MotorStatus; }
            set { this.RaiseAndSetIfChanged(ref axis1MotorStatus, value); }
        }

        public string Axis2MotorStatus
        {
            get { return axis2MotorStatus; }
            set { this.RaiseAndSetIfChanged(ref axis2MotorStatus, value); }
        }

        public string Axis3MotorStatus
        {
            get { return axis3MotorStatus; }
            set { this.RaiseAndSetIfChanged(ref axis3MotorStatus, value); }
        }

        public string Axis1Position
        {
            get { return axis1Position; }
            set { this.RaiseAndSetIfChanged(ref axis1Position, value); }
        }

        public string Axis2Position
        {
            get { return axis2Position; }
            set { this.RaiseAndSetIfChanged(ref axis2Position, value); }
        }

        public string Axis3Position
        {
            get { return axis3Position; }
            set { this.RaiseAndSetIfChanged(ref axis3Position, value); }
        }

        public string SelectedPosition
        {
            get { return selectedPosition; }
            set { this.RaiseAndSetIfChanged(ref selectedPosition, value); }
        }

        public string CurrentPosition
        {
            get { return currentPosition; }
            set { this.RaiseAndSetIfChanged(ref currentPosition, value); }
        }

        public string NewPosition
        {
            get { return newPosition; }
            set { this.RaiseAndSetIfChanged(ref newPosition, value); }
        }

        public bool ControlsTabSelected
        {
            get { return controlsTabSelected; }
            set { this.RaiseAndSetIfChanged(ref controlsTabSelected, value); }
        }

        public classNewportStage NewportStage
        {
            get { return newportStage; }
            private set { this.RaiseAndSetIfChanged(ref newportStage, value); }
        }

        public IDevice Device
        {
            get { return NewportStage; }
            set { RegisterDevice(value); }
        }

        #endregion

        #region Commands

        public ReactiveUI.ReactiveCommand<Unit, Unit> RefreshPositionCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> Axis1ForwardCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> Axis2ForwardCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> Axis3ForwardCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> Axis1BackwardCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> Axis2BackwardCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> Axis3BackwardCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> Axis1ForwardReleaseCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> Axis2ForwardReleaseCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> Axis3ForwardReleaseCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> Axis1BackwardReleaseCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> Axis2BackwardReleaseCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> Axis3BackwardReleaseCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ResetToHomePositionCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> GoToSelectedPositionCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> DeletePositionCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> SetPositionCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> Axis1MotorPowerCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> Axis2MotorPowerCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> Axis3MotorPowerCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> GetErrorsCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ClearErrorsCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> OpenPortCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> ClosePortCommand { get; private set; }

        private void SetupCommands()
        {
            RefreshPositionCommand = ReactiveUI.ReactiveCommand.Create(() => RefreshPosition());
            Axis1ForwardCommand = ReactiveUI.ReactiveCommand.Create(() => Axis1FwdPress());
            Axis2ForwardCommand = ReactiveUI.ReactiveCommand.Create(() => Axis2FwdPress());
            Axis3ForwardCommand = ReactiveUI.ReactiveCommand.Create(() => Axis3FwdPress());
            Axis1ForwardReleaseCommand = ReactiveUI.ReactiveCommand.Create(() => Axis1FwdRelease());
            Axis2ForwardReleaseCommand = ReactiveUI.ReactiveCommand.Create(() => Axis2FwdRelease());
            Axis3ForwardReleaseCommand = ReactiveUI.ReactiveCommand.Create(() => Axis3FwdRelease());
            Axis1BackwardCommand = ReactiveUI.ReactiveCommand.Create(() => Axis1BackPress());
            Axis2BackwardCommand = ReactiveUI.ReactiveCommand.Create(() => Axis2BackPress());
            Axis3BackwardCommand = ReactiveUI.ReactiveCommand.Create(() => Axis3BackPress());
            Axis1BackwardReleaseCommand = ReactiveUI.ReactiveCommand.Create(() => Axis1BackRelease());
            Axis2BackwardReleaseCommand = ReactiveUI.ReactiveCommand.Create(() => Axis2BackRelease());
            Axis3BackwardReleaseCommand = ReactiveUI.ReactiveCommand.Create(() => Axis3BackRelease());
            ResetToHomePositionCommand = ReactiveUI.ReactiveCommand.Create(() => ResetToHome());
            GoToSelectedPositionCommand = ReactiveUI.ReactiveCommand.Create(() => GoToSelectedPosition());
            DeletePositionCommand = ReactiveUI.ReactiveCommand.Create(() => RemovePosition());
            SetPositionCommand = ReactiveUI.ReactiveCommand.Create(() => SetPosition());
            Axis1MotorPowerCommand = ReactiveUI.ReactiveCommand.Create(() => ToggleAxis1Motor());
            Axis2MotorPowerCommand = ReactiveUI.ReactiveCommand.Create(() => ToggleAxis2Motor());
            Axis3MotorPowerCommand = ReactiveUI.ReactiveCommand.Create(() => ToggleAxis3Motor());
            GetErrorsCommand = ReactiveUI.ReactiveCommand.Create(() => GetErrors());
            ClearErrorsCommand = ReactiveUI.ReactiveCommand.Create(() => ClearErrors());
            OpenPortCommand = ReactiveUI.ReactiveCommand.Create(() => OpenPort());
            ClosePortCommand = ReactiveUI.ReactiveCommand.Create(() => ClosePort());
        }

        #endregion

        public UserControl GetDefaultView()
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
            NewportStage = device as classNewportStage;
            SetBaseDevice(NewportStage);
            NewportStage.PositionsLoaded += PositionLoadHandler;
            NewportStage.PositionChanged += PositionChangedHandler;
            NewportStage.StatusUpdate += m_obj_StatusUpdate;
            UpdateAxisPositions();
            UpdatePositionListBox();
            UpdatePositionLabel(NewportStage.CurrentPos);
            UpdateMotorStatus();
        }

        private void m_obj_StatusUpdate(object sender, classDeviceStatusEventArgs e)
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
            using (positionsList.SuppressChangeNotifications())
            {
                positionsList.Clear();
                positionsList.AddRange(NewportStage.Positions.Keys);
            }
        }

        private void PositionLoadHandler(object sender, EventArgs e)
        {
            UpdatePositionListBox();
        }

        private void PositionChangedHandler(object sender, EventArgs e)
        {
            var stage = sender as classNewportStage;
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
