using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    /// <summary>
    /// User control for tray/vial assignment window
    /// </summary>
    public class TrayVialViewModel : ReactiveObject
    {
        #region "Constructors"

        /// <summary>
        /// Calling this constructor is only for the windows WPF designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public TrayVialViewModel()
        {
            FilteredSamples = new ReadOnlyObservableCollection<TrayVialSampleViewModel>(new ObservableCollection<TrayVialSampleViewModel>());
        }

        public TrayVialViewModel(int trayNumber, SourceList<TrayVialSampleViewModel> sampleList)
        {
            TrayNumber = trayNumber;
            if (TrayNumber <= 0)
            {
                masterView = true;
            }

            var filter = this.WhenValueChanged(x => x.ShowUnassigned).Select(x =>
                new Func<TrayVialSampleViewModel, bool>(y =>
                {
                    if (masterView)
                    {
                        if (ShowUnassigned)
                        {
                            return y.Tray == 0;
                        }

                        return true;
                    }

                    return y.Tray == TrayNumber;
                }));

            sampleList.Connect().Filter(filter).ObserveOn(RxApp.MainThreadScheduler).Bind(out var filtered).Subscribe();
            FilteredSamples = filtered;

            AssignSelectedToVialCommand = ReactiveCommand.Create(() => AssignSelectedToVial(this.AssignVial), this.WhenAnyValue(x => x.SelectedSamples.Count).Select(x => x > 0));
            AutoAssignVialsCommand = ReactiveCommand.Create(AutoAssignVials, this.WhenAnyValue(x => x.SelectedSamples.Count).Select(x => x > 0));
            MoveToTray1Command = ReactiveCommand.Create(() => UpdateTrayAssignment(1), this.WhenAnyValue(x => x.SelectedSamples.Count).Select(x => x > 0));
            MoveToTray2Command = ReactiveCommand.Create(() => UpdateTrayAssignment(2), this.WhenAnyValue(x => x.SelectedSamples.Count).Select(x => x > 0));
            MoveToTray3Command = ReactiveCommand.Create(() => UpdateTrayAssignment(3), this.WhenAnyValue(x => x.SelectedSamples.Count).Select(x => x > 0));
            MoveToTray4Command = ReactiveCommand.Create(() => UpdateTrayAssignment(4), this.WhenAnyValue(x => x.SelectedSamples.Count).Select(x => x > 0));
            MoveToTray5Command = ReactiveCommand.Create(() => UpdateTrayAssignment(5), this.WhenAnyValue(x => x.SelectedSamples.Count).Select(x => x > 0));
            MoveToTray6Command = ReactiveCommand.Create(() => UpdateTrayAssignment(6), this.WhenAnyValue(x => x.SelectedSamples.Count).Select(x => x > 0));
            MoveToUnassignedCommand = ReactiveCommand.Create(() => UpdateTrayAssignment(0), this.WhenAnyValue(x => x.SelectedSamples.Count).Select(x => x > 0));
        }

        #endregion

        #region "Class variables"

        private bool showUnassigned = true;
        private readonly bool masterView = false;
        private int assignVial = 54;
        private int maxVials = 54;

        #endregion

        #region "Properties"

        public ReadOnlyObservableCollection<TrayVialSampleViewModel> FilteredSamples { get; }

        public ObservableCollectionExtended<TrayVialSampleViewModel> SelectedSamples { get; } = new ObservableCollectionExtended<TrayVialSampleViewModel>();

        public int TrayNumber { get; }

        public bool ShowUnassigned
        {
            get => showUnassigned;
            set => this.RaiseAndSetIfChanged(ref showUnassigned, value);
        }

        public int AssignVial
        {
            get => assignVial;
            set => this.RaiseAndSetIfChanged(ref assignVial, value);
        }

        public int MaxVials
        {
            get => maxVials;
            set => this.RaiseAndSetIfChanged(ref maxVials, value);
        }

        public ReactiveCommand<Unit, Unit> AssignSelectedToVialCommand { get; }
        public ReactiveCommand<Unit, Unit> AutoAssignVialsCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveToTray1Command { get; }
        public ReactiveCommand<Unit, Unit> MoveToTray2Command { get; }
        public ReactiveCommand<Unit, Unit> MoveToTray3Command { get; }
        public ReactiveCommand<Unit, Unit> MoveToTray4Command { get; }
        public ReactiveCommand<Unit, Unit> MoveToTray5Command { get; }
        public ReactiveCommand<Unit, Unit> MoveToTray6Command { get; }
        public ReactiveCommand<Unit, Unit> MoveToUnassignedCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Updates tray assignments for selected samples
        /// </summary>
        /// <param name="newTrayNum">New tray number</param>
        private void UpdateTrayAssignment(int newTrayNum)
        {
            if (FilteredSamples.Count == 0)
            {
                return;
            }

            if (SelectedSamples.Count == 0)
            {
                MessageBox.Show("At least one sample must be selected");
                return;
            }

            foreach (var sample in SelectedSamples)
            {
                sample.Tray = newTrayNum;
            }
        }

        /// <summary>
        /// Automatically numbers all vials in the list for this tray, starting at 1. It's assumed there will never be
        /// more than 96 samples in the list.
        /// </summary>
        private void AutoAssignVials()
        {
            if (FilteredSamples.Count == 0)
            {
                return;
            }

            //TODO: May want to add a warning and allow specifying a start vial number? Possibly allow selection of sample subset?
            var vialCounter = 0;

            var mod = MaxVials;
            foreach (var sample in FilteredSamples)
            {
                sample.Vial = (vialCounter % mod) + 1;
                vialCounter++;
            }
        }

        private void AssignSelectedToVial(int vial)
        {
            if (SelectedSamples.Count == 0)
            {
                return;
            }

            foreach (var sample in SelectedSamples)
            {
                sample.Vial = vial;
            }
        }

        #endregion
    }
}
