using System;
using System.Reactive;
using System.Reactive.Linq;
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
            Dec1mmCommand = ReactiveCommand.Create(() => MoveAxis(-1, Units.Length_Millimetres));
            Dec100umCommand = ReactiveCommand.Create(() => MoveAxis(-0.1, Units.Length_Micrometres));
            Dec1umCommand = ReactiveCommand.Create(() => MoveAxis(-1, Units.Length_Micrometres));
            Dec1Command = ReactiveCommand.Create(() => MoveAxis(-1, Units.Native));
            Inc1Command = ReactiveCommand.Create(() => MoveAxis(1, Units.Native));
            Inc1umCommand = ReactiveCommand.Create(() => MoveAxis(1, Units.Length_Micrometres));
            Inc100umCommand = ReactiveCommand.Create(() => MoveAxis(0.1, Units.Length_Micrometres));
            Inc1mmCommand = ReactiveCommand.Create(() => MoveAxis(1, Units.Length_Millimetres));
        }

        private readonly ObservableAsPropertyHelper<string> stageDisplayName;
        private double positionMM;

        public StageControl Stage { get; }
        public string StageDisplayName => stageDisplayName.Value;
        public double PositionMM
        {
            get => positionMM;
            set => this.RaiseAndSetIfChanged(ref positionMM, value);
        }

        public ReactiveCommand<Unit, Unit> GetPositionCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveHomeCommand { get; }
        public ReactiveCommand<Unit, Unit> Dec1mmCommand { get; }
        public ReactiveCommand<Unit, Unit> Dec100umCommand { get; }
        public ReactiveCommand<Unit, Unit> Dec1umCommand { get; }
        public ReactiveCommand<Unit, Unit> Dec1Command { get; }
        public ReactiveCommand<Unit, Unit> Inc1Command { get; }
        public ReactiveCommand<Unit, Unit> Inc1umCommand { get; }
        public ReactiveCommand<Unit, Unit> Inc100umCommand { get; }
        public ReactiveCommand<Unit, Unit> Inc1mmCommand { get; }

        public void ReadPosition()
        {
            PositionMM = Stage.GetPositionMM();
        }

        public void MoveAxis(double distance, Units units)
        {
            Stage.MoveRelative(distance, units);
            ReadPosition();
        }

        public void MoveAxisHome()
        {
            Stage.Home();
            ReadPosition();
        }
    }
}
