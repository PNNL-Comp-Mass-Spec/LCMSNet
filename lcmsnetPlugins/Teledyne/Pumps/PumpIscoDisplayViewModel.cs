using System;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;

namespace LcmsNetPlugins.Teledyne.Pumps
{
    public class PumpIscoDisplayViewModel : ReactiveObject
    {
        #region "Constructors"

        public PumpIscoDisplayViewModel()
        {
            SetupCommands();
            this.WhenAnyValue(x => x.MinFlowSp, x => x.MaxFlowSp, x => x.MinPressSp, x => x.MaxPressSp).Subscribe(x => UpdateSetpointLimits());
            UpdateSetpointLimits();
        }

        public PumpIscoDisplayViewModel(int index) : this()
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
        private enumIscoOperationMode operationMode;
        private string pumpName = "X";
        private double flowRate;
        private double pressure;
        private double volume;
        private double setpoint;
        private double minSetpoint;
        private double maxSetpoint;
        private string setpointUnits = "PSI";
        private string setpointType = "Set Press";
        private enumIscoProblemStatus problemStatus = enumIscoProblemStatus.None;
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
            get { return pumpName; }
            private set { this.RaiseAndSetIfChanged(ref pumpName, value); }
        }

        /// <summary>
        /// Pump flow
        /// </summary>
        public double FlowRate
        {
            get { return flowRate; }
            set { this.RaiseAndSetIfChanged(ref flowRate, value); }
        }

        /// <summary>
        /// Pump pressure
        /// </summary>
        public double Pressure
        {
            get { return pressure; }
            set { this.RaiseAndSetIfChanged(ref pressure, value); }
        }

        /// <summary>
        /// Pump volume
        /// </summary>
        public double Volume
        {
            get { return volume; }
            set { this.RaiseAndSetIfChanged(ref volume, value); }
        }

        /// <summary>
        /// Pressure or flow setpoint
        /// </summary>
        public double Setpoint
        {
            get { return setpoint; }
            set { this.RaiseAndSetIfChanged(ref setpoint, value); }
        }

        public double MinSetpoint
        {
            get { return minSetpoint; }
            private set { this.RaiseAndSetIfChanged(ref minSetpoint, value); }
        }

        public double MaxSetpoint
        {
            get { return maxSetpoint; }
            private set { this.RaiseAndSetIfChanged(ref maxSetpoint, value); }
        }

        public string SetpointUnits
        {
            get { return setpointUnits; }
            private set { this.RaiseAndSetIfChanged(ref setpointUnits, value); }
        }

        public string SetpointType
        {
            get { return setpointType; }
            private set { this.RaiseAndSetIfChanged(ref setpointType, value); }
        }

        /// <summary>
        /// Pump index
        /// </summary>
        public int PumpIndex => pumpIndex;

        /// <summary>
        /// Operation mode
        /// </summary>
        public enumIscoOperationMode OperationMode
        {
            get { return operationMode; }
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
            get { return maxFlowSp; }
            set { this.RaiseAndSetIfChanged(ref maxFlowSp, value); }
        }

        /// <summary>
        /// Min flow setpoint
        /// </summary>
        public double MinFlowSp
        {
            get { return minFlowSp; }
            set { this.RaiseAndSetIfChanged(ref minFlowSp, value); }
        }

        /// <summary>
        /// Max flow limit
        /// </summary>
        public double MaxFlowLimit
        {
            get { return maxFlowLimit; }
            set { this.RaiseAndSetIfChanged(ref maxFlowLimit, value); }
        }

        /// <summary>
        /// Max pressure setpoint (PSI)
        /// </summary>
        public double MaxPressSp
        {
            get { return maxPressSp; }
            set { this.RaiseAndSetIfChanged(ref maxPressSp, value); }
        }

        /// <summary>
        /// Min pressure setpoint
        /// </summary>
        public double MinPressSp
        {
            get { return minPressSp; }
            set { this.RaiseAndSetIfChanged(ref minPressSp, value); }
        }

        /// <summary>
        /// Problem status
        /// </summary>
        public enumIscoProblemStatus ProblemStatus
        {
            get { return problemStatus; }
            set
            {
                this.RaiseAndSetIfChanged(ref problemStatus, value);
                UpdateProblemLabel();
            }
        }

        public string ProblemStatusString
        {
            get { return problemStatusString; }
            private set { this.RaiseAndSetIfChanged(ref problemStatusString, value); }
        }

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
                case enumIscoOperationMode.ConstantFlow:
                    SetpointUnits = classIscoConversions.GetFlowUnitsString();
                    SetpointType = "Set Flow";
                    break;
                case enumIscoOperationMode.ConstantPressure:
                    SetpointUnits = classIscoConversions.GetPressUnitsString();
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
                case enumIscoProblemStatus.CylinderBottom:
                    ProblemStatusString = "Bottom";
                    break;
                case enumIscoProblemStatus.CylinderEmpty:
                    ProblemStatusString = "Empty";
                    break;
                case enumIscoProblemStatus.MotorFailure:
                    ProblemStatusString = "Mot Fail";
                    break;
                case enumIscoProblemStatus.None:
                    ProblemStatusString = "";
                    break;
                case enumIscoProblemStatus.OverPressure:
                    ProblemStatusString = "Over Press";
                    break;
                case enumIscoProblemStatus.UnderPressure:
                    ProblemStatusString = "Under Press";
                    break;
                default:
                    ProblemStatusString = "";
                    break;
            }
        }

        private void UpdateSetpointLimits()
        {
            if (operationMode == enumIscoOperationMode.ConstantFlow)
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

            if (operationMode == enumIscoOperationMode.ConstantFlow)
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

        #region Commands

        public ReactiveCommand<Unit, Unit> SetSetpointCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> StartPumpCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> StopPumpCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> RefillCommand { get; private set; }

        private void SetupCommands()
        {
            SetSetpointCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => ChangeSetpoint()));
            StartPumpCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => StartPumpOperation()));
            StopPumpCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => StopPumpOperation()));
            RefillCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Refill()));
        }

        #endregion
    }
}
