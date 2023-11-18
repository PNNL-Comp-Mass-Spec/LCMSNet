using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using ReactiveUI;
using Zaber.Motion;

namespace LcmsNetPlugins.ZaberStage.UI
{
    public class StageControlViewModel : ReactiveObject
    {
        [Obsolete("For WPF Design time use only.", true)]
        public StageControlViewModel() : this(new StageControl("D"))
        {
        }

        public StageControlViewModel(StageControl settings)
        {
            Stage = settings;

            stageDisplayName = Stage.WhenAnyValue(x => x.DeviceStageName, x => x.StageDisplayName)
                .Select(x => !string.IsNullOrWhiteSpace(x.Item2) ? x.Item2 : x.Item1)
                .ToProperty(this, x => x.StageDisplayName);

            GetPositionCommand = ReactiveCommand.Create(ReadPosition);
            MoveHomeCommand = ReactiveCommand.Create(MoveAxisHome);
            DecStepCommand = ReactiveCommand.Create(() => StepAxis(MoveDirection.Decrease));
            IncStepCommand = ReactiveCommand.Create(() => StepAxis(MoveDirection.Increase));
            DecJogCommand = ReactiveCommand.Create(() => JogAxis(MoveDirection.Decrease));
            IncJogCommand = ReactiveCommand.Create(() => JogAxis(MoveDirection.Increase));
            StopCommand = ReactiveCommand.Create(StopAxis);

            positionMM = this.WhenAnyValue(x => x.PositionMMThreaded).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.PositionMM);

            movePositionReadTimer = new Timer(TimerReadPosition, this, Timeout.InfiniteTimeSpan, timerPeriod);

            this.WhenAnyValue(x => x.SelectedJogSpeed).Subscribe(x =>
            {
                this.RaisePropertyChanged(nameof(JogLowSpeedSelected));
                this.RaisePropertyChanged(nameof(JogMediumSpeedSelected));
                this.RaisePropertyChanged(nameof(JogHighSpeedSelected));
            });
        }

        private readonly ObservableAsPropertyHelper<string> stageDisplayName;
        private double positionMMThreaded;
        private bool joggingAxis = false;
        private readonly Timer movePositionReadTimer;
        private readonly ObservableAsPropertyHelper<double> positionMM;
        private readonly TimeSpan timerPeriod = TimeSpan.FromMilliseconds(100);
        private double stepSizeMM = 0.1;
        private JogSpeed selectedJogSpeed = JogSpeed.Low;

        private double PositionMMThreaded
        {
            get => positionMMThreaded;
            set => this.RaiseAndSetIfChanged(ref positionMMThreaded, value);
        }

        public StageControl Stage { get; }
        public string StageDisplayName => stageDisplayName.Value;
        public double PositionMM => positionMM.Value;

        public double StepSizeMM
        {
            get => stepSizeMM;
            set => this.RaiseAndSetIfChanged(ref stepSizeMM, value);
        }

        public JogSpeed SelectedJogSpeed
        {
            get => selectedJogSpeed;
            set => this.RaiseAndSetIfChanged(ref selectedJogSpeed, value);
        }

        public bool JogLowSpeedSelected
        {
            get => selectedJogSpeed == JogSpeed.Low;
            set => SelectedJogSpeed = value ? JogSpeed.Low : SelectedJogSpeed;
    }

        public bool JogMediumSpeedSelected
        {
            get => selectedJogSpeed == JogSpeed.Medium;
            set => SelectedJogSpeed = value ? JogSpeed.Medium : SelectedJogSpeed;
        }

        public bool JogHighSpeedSelected
        {
            get => selectedJogSpeed == JogSpeed.High;
            set => SelectedJogSpeed = value ? JogSpeed.High : SelectedJogSpeed;
        }

        public ReactiveCommand<Unit, Unit> GetPositionCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveHomeCommand { get; }
        public ReactiveCommand<Unit, Unit> DecStepCommand { get; }
        public ReactiveCommand<Unit, Unit> IncStepCommand { get; }
        public ReactiveCommand<Unit, Unit> DecJogCommand { get; }
        public ReactiveCommand<Unit, Unit> IncJogCommand { get; }
        public ReactiveCommand<Unit, Unit> StopCommand { get; }

        public double GetPositionMM()
        {
            ReadPosition();
            return PositionMMThreaded;
        }

        public void ReadPosition()
        {
            PositionMMThreaded = Stage.GetPositionMM();
        }

        public void MoveAxis(double distance, Units units)
        {
            Stage.MoveRelative(distance, units);
            ReadPosition();
        }

        public void MoveAxisVel(double velocity, Units units)
        {
            Stage.StartMove(velocity, units);
            joggingAxis = true;
            movePositionReadTimer.Change(timerPeriod, timerPeriod);
        }

        public void StepAxis(MoveDirection direction)
        {
            Stage.MoveRelative(direction, StepSizeMM, Units.Length_Millimetres);
            ReadPosition();
        }

        public void JogAxis(MoveDirection direction)
        {
            Stage.StartJog(direction, SelectedJogSpeed);
            joggingAxis = true;
            movePositionReadTimer.Change(timerPeriod, timerPeriod);
        }

        public void StopAxis()
        {
            Stage.Stop(false);
            if (joggingAxis)
            {
                joggingAxis = false;
                movePositionReadTimer.Change(Timeout.InfiniteTimeSpan, timerPeriod);
                ReadPosition();
            }
        }

        public void MoveAxisHome()
        {
            Stage.Home();
            ReadPosition();
        }

        private void TimerReadPosition(object state)
        {
            PositionMMThreaded = Stage.GetPositionMM();
        }
    }
}
