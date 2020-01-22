using System;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;

namespace LcmsNetPlugins.Teledyne.Pumps
{
    public class IscoPumpDisplayViewModel : ReactiveObject
    {
        #region "Constructors"

        public IscoPumpDisplayViewModel()
        {
            this.WhenAnyValue(x => x.MinFlowSp, x => x.MaxFlowSp, x => x.MinPressSp, x => x.MaxPressSp).Subscribe(x => UpdateSetpointLimits());
            UpdateSetpointLimits();

            SetSetpointCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(ChangeSetpoint));
            StartPumpCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(StartPumpOperation));
            StopPumpCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(StopPumpOperation));
            RefillCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(Refill));
        }

        public IscoPumpDisplayViewModel(int index) : this()
        {
            SetPumpIndex(index);
        }

        #endregion

        #region "Events"

        public event DelegateIscoPumpDisplaySetpointHandler SetpointChanged;
        public event DelegateIscoPumpDisplayHandler StartRefill;
        public event DelegateIscoPumpDisplayHandler StartPump;
        public event DelegateIscoPumpDisplayHandler StopPump;

        #endregion

        #region "Class variables"

        private int pumpIndex;
        private IscoOperationMode operationMode;
        private string pumpName = "X";
        private double flowRate;
        private double pressure;
        private double volume;
        private double setpoint;
        private double minSetpoint;
        private double maxSetpoint;
        private string setpointUnits = "PSI";
        private string setpointType = "Set Press";
        private IscoProblemStatus problemStatus = IscoProblemStatus.None;
        private string problemStatusString = "NA";
        private double maxFlowSp = 25D;
        private double minFlowSp = 0.0010D;
        private double maxFlowLimit = 50D;
        private double maxPressSp = 10000D;
        private double minPressSp = 10D;

        #endregion

        #region "Properties"

        public string PumpName
        {
            get => pumpName;
            private set => this.RaiseAndSetIfChanged(ref pumpName, value);
        }

        /// <summary>
        /// Pump flow
        /// </summary>
        public double FlowRate
        {
            get => flowRate;
            set => this.RaiseAndSetIfChanged(ref flowRate, value);
        }

        /// <summary>
        /// Pump pressure
        /// </summary>
        public double Pressure
        {
            get => pressure;
            set => this.RaiseAndSetIfChanged(ref pressure, value);
        }

        /// <summary>
        /// Pump volume
        /// </summary>
        public double Volume
        {
            get => volume;
            set => this.RaiseAndSetIfChanged(ref volume, value);
        }

        /// <summary>
        /// Pressure or flow setpoint
        /// </summary>
        public double Setpoint
        {
            get => setpoint;
            set => this.RaiseAndSetIfChanged(ref setpoint, value);
        }

        public double MinSetpoint
        {
            get => minSetpoint;
            private set => this.RaiseAndSetIfChanged(ref minSetpoint, value);
        }

        public double MaxSetpoint
        {
            get => maxSetpoint;
            private set => this.RaiseAndSetIfChanged(ref maxSetpoint, value);
        }

        public string SetpointUnits
        {
            get => setpointUnits;
            private set => this.RaiseAndSetIfChanged(ref setpointUnits, value);
        }

        public string SetpointType
        {
            get => setpointType;
            private set => this.RaiseAndSetIfChanged(ref setpointType, value);
        }

        /// <summary>
        /// Pump index
        /// </summary>
        public int PumpIndex => pumpIndex;

        /// <summary>
        /// Operation mode
        /// </summary>
        public IscoOperationMode OperationMode
        {
            get => operationMode;
            set
            {
                this.RaiseAndSetIfChanged(ref operationMode, value);
                SetOperationModeDisplays();
            }
        }

        /// <summary>
        /// Max flow setpoint
        /// </summary>
        public double MaxFlowSp
        {
            get => maxFlowSp;
            set => this.RaiseAndSetIfChanged(ref maxFlowSp, value);
        }

        /// <summary>
        /// Min flow setpoint
        /// </summary>
        public double MinFlowSp
        {
            get => minFlowSp;
            set => this.RaiseAndSetIfChanged(ref minFlowSp, value);
        }

        /// <summary>
        /// Max flow limit
        /// </summary>
        public double MaxFlowLimit
        {
            get => maxFlowLimit;
            set => this.RaiseAndSetIfChanged(ref maxFlowLimit, value);
        }

        /// <summary>
        /// Max pressure setpoint (PSI)
        /// </summary>
        public double MaxPressSp
        {
            get => maxPressSp;
            set => this.RaiseAndSetIfChanged(ref maxPressSp, value);
        }

        /// <summary>
        /// Min pressure setpoint
        /// </summary>
        public double MinPressSp
        {
            get => minPressSp;
            set => this.RaiseAndSetIfChanged(ref minPressSp, value);
        }

        /// <summary>
        /// Problem status
        /// </summary>
        public IscoProblemStatus ProblemStatus
        {
            get => problemStatus;
            set
            {
                this.RaiseAndSetIfChanged(ref problemStatus, value);
                UpdateProblemLabel();
            }
        }

        public string ProblemStatusString
        {
            get => problemStatusString;
            private set => this.RaiseAndSetIfChanged(ref problemStatusString, value);
        }

        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> SetSetpointCommand { get; }
        public ReactiveCommand<Unit, Unit> StartPumpCommand { get; }
        public ReactiveCommand<Unit, Unit> StopPumpCommand { get; }
        public ReactiveCommand<Unit, Unit> RefillCommand { get; }

        #endregion

        #region "Methods"

        /// <summary>
        /// Initializes the pump display
        /// </summary>
        /// <param name="pumpIndx"></param>
        /// <returns></returns>
        public bool InitViewModel(int pumpIndx)
        {
            // Store the index and update display
            SetPumpIndex(pumpIndx);

            return true;
        }

        /// <summary>
        /// Storees the pump index and updates the user control display
        /// </summary>
        /// <param name="indx">Zero-based index for pump</param>
        private void SetPumpIndex(int indx)
        {
            pumpIndex = indx;
            var pumpStr = "";
            switch (pumpIndex)
            {
                case 0:
                    pumpStr = "A";
                    break;
                case 1:
                    pumpStr = "B";
                    break;
                case 2:
                    pumpStr = "C";
                    break;
            }

            PumpName = "Pump " + pumpStr;
        }

        /// <summary>
        /// Sets the displays based on operation mode
        /// </summary>
        private void SetOperationModeDisplays()
        {
            switch (OperationMode)
            {
                case IscoOperationMode.ConstantFlow:
                    SetpointUnits = IscoConversions.GetFlowUnitsString();
                    SetpointType = "Set Flow";
                    break;
                case IscoOperationMode.ConstantPressure:
                    SetpointUnits = IscoConversions.GetPressUnitsString();
                    SetpointType = "Set Press";
                    break;
            }
            UpdateSetpointLimits();
        }

        /// <summary>
        /// Determines if test value is within specified range
        /// </summary>
        /// <param name="testVal">Test value</param>
        /// <param name="minVal">Minimum value</param>
        /// <param name="maxVal">Maximum value</param>
        /// <returns>TRUE if value is within range; FALSE otherwise</returns>
        private bool IsValueInRange(double testVal, double minVal, double maxVal)
        {
            return testVal >= minVal && testVal <= maxVal;
        }

        /// <summary>
        /// Updates problem status display
        /// </summary>
        private void UpdateProblemLabel()
        {
            switch (ProblemStatus)
            {
                case IscoProblemStatus.CylinderBottom:
                    ProblemStatusString = "Bottom";
                    break;
                case IscoProblemStatus.CylinderEmpty:
                    ProblemStatusString = "Empty";
                    break;
                case IscoProblemStatus.MotorFailure:
                    ProblemStatusString = "Mot Fail";
                    break;
                case IscoProblemStatus.None:
                    ProblemStatusString = "";
                    break;
                case IscoProblemStatus.OverPressure:
                    ProblemStatusString = "Over Press";
                    break;
                case IscoProblemStatus.UnderPressure:
                    ProblemStatusString = "Under Press";
                    break;
                default:
                    ProblemStatusString = "";
                    break;
            }
        }

        private void UpdateSetpointLimits()
        {
            if (operationMode == IscoOperationMode.ConstantFlow)
            {
                MinSetpoint = MinFlowSp;
                MaxSetpoint = MaxFlowSp;
            }
            else
            {
                MinSetpoint = MinPressSp;
                MaxSetpoint = MaxPressSp;
            }

            Setpoint = Math.Max(Setpoint, MinSetpoint);
            Setpoint = Math.Min(Setpoint, MaxSetpoint);
        }

        /// <summary>
        /// Set Press or Set Flow button clicked
        /// </summary>
        private void ChangeSetpoint()
        {
            var input = Setpoint;
            bool validInput;

            if (operationMode == IscoOperationMode.ConstantFlow)
            {
                validInput = IsValueInRange(input, MinFlowSp, MaxFlowSp);
            }
            else
                validInput = IsValueInRange(input, MinPressSp, MaxPressSp);

            if (validInput)
            {
                SetpointChanged?.Invoke(this, pumpIndex, input);
            }
            else
                MessageBox.Show("Input value not in valid range");
        }

        /// <summary>
        /// Refill button clicked
        /// </summary>
        private void Refill()
        {
            StartRefill?.Invoke(this, pumpIndex);
        }

        /// <summary>
        /// Start Pump button clicked
        /// </summary>
        private void StartPumpOperation()
        {
            StartPump?.Invoke(this, pumpIndex);
        }

        /// <summary>
        /// Stop Pump button clicked
        /// </summary>
        private void StopPumpOperation()
        {
            StopPump?.Invoke(this, pumpIndex);
        }

        #endregion
    }
}
