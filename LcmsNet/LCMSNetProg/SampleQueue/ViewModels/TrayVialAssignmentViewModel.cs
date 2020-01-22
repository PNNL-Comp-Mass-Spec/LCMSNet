using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using LcmsNetSDK.Data;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    /// <summary>
    /// ViewModel for making tray/vial assignments to samples
    /// </summary>
    public class TrayVialAssignmentViewModel : ReactiveObject
    {
        #region "Constructors"

        /// <summary>
        /// Calling this constructor is only for the windows WPF designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public TrayVialAssignmentViewModel()
        {
            TrayUnassigned = new TrayVialViewModel(0, sampleVms);
            Tray1 = new TrayVialViewModel(1, sampleVms);
            Tray2 = new TrayVialViewModel(2, sampleVms);
            Tray3 = new TrayVialViewModel(3, sampleVms);
            Tray4 = new TrayVialViewModel(4, sampleVms);
            Tray5 = new TrayVialViewModel(5, sampleVms);
            Tray6 = new TrayVialViewModel(6, sampleVms);
        }

        public TrayVialAssignmentViewModel(List<string> trayNamesList, List<SampleData> samples)
        {
            LoadSampleList(trayNamesList, samples);

            TrayUnassigned = new TrayVialViewModel(0, sampleVms);
            Tray1 = new TrayVialViewModel(1, sampleVms);
            Tray2 = new TrayVialViewModel(2, sampleVms);
            Tray3 = new TrayVialViewModel(3, sampleVms);
            Tray4 = new TrayVialViewModel(4, sampleVms);
            Tray5 = new TrayVialViewModel(5, sampleVms);
            Tray6 = new TrayVialViewModel(6, sampleVms);

            ApplyChangesCommand = ReactiveCommand.Create(() => UpdateSampleList());
        }

        #endregion

        #region "Properties"

        public ReactiveCommand<Unit, Unit> ApplyChangesCommand { get; }

        public List<SampleData> SampleList { get; private set; }

        #endregion

        #region "Class variables"

        private readonly ReactiveList<TrayVialSampleViewModel> sampleVms = new ReactiveList<TrayVialSampleViewModel>() { ChangeTrackingEnabled = true };

        private List<string> trayNames; // List of tray names used by PAL on this cart

        public TrayVialViewModel TrayUnassigned { get; }
        public TrayVialViewModel Tray1 { get; }
        public TrayVialViewModel Tray2 { get; }
        public TrayVialViewModel Tray3 { get; }
        public TrayVialViewModel Tray4 { get; }
        public TrayVialViewModel Tray5 { get; }
        public TrayVialViewModel Tray6 { get; }

        #endregion

        #region "Methods"

        /// <summary>
        /// Loads the samples into the form
        /// </summary>
        /// <param name="trayNamesList">List of PAL tray names</param>
        /// <param name="samples">List of samples</param>
        private void LoadSampleList(List<string> trayNamesList, List<SampleData> samples)
        {
            this.trayNames = trayNamesList;
            SampleList = samples;

            using (sampleVms.SuppressChangeNotifications())
            {
                sampleVms.Clear();
                sampleVms.AddRange(samples.Select(x => new TrayVialSampleViewModel(x, trayNames)));
            }
        }

        /// <summary>
        /// Updates the sample list tray/vial data from the data table
        /// </summary>
        private void UpdateSampleList()
        {
            foreach (var sample in this.sampleVms)
            {
                sample.StoreTrayVialToSample();
            }
        }

        #endregion
    }
}
