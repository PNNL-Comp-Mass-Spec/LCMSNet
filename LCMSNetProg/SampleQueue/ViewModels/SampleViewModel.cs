using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Media;
using LcmsNet.Data;
using LcmsNetSDK.Configuration;
using LcmsNetSDK.Data;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class SampleViewModel : ReactiveObject, IEquatable<SampleViewModel>
    {
        /// <summary>
        /// Index Offset for going from a zero based array for configuration data to the
        /// user-readable column display.
        /// </summary>
        protected const int CONST_COLUMN_INDEX_OFFSET = 1;

        public SampleData Sample { get; }

        [Obsolete("For WPF Design time use only.", true)]
        public SampleViewModel()
        {
            Sample = new SampleData(true);
        }

        public SampleViewModel(SampleData sample)
        {
            Sample = sample;

            columnData = this.WhenAnyValue(x => x.Sample.ColumnIndex).Select(x => CartConfiguration.Columns[x]).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.ColumnData);

            columnNumber = Sample.WhenAnyValue(x => x.ColumnIndex, x => x.SpecialColumnNumber).Select(x => !string.IsNullOrWhiteSpace(x.Item2) ? x.Item2 : (x.Item1 + CONST_COLUMN_INDEX_OFFSET).ToString()).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.ColumnNumber);
            columnNumberBgColor = this.WhenAnyValue(x => x.ColumnData.Color).Select(x => new SolidColorBrush(x)).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.ColumnNumberBgColor, Brushes.RoyalBlue);

            editAllowed = Sample.WhenAnyValue(x => x.IsSetToRunOrHasRun).Select(x => !x).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.EditAllowed);

            status = Sample.WhenAnyValue(x => x.RunningStatus).Select(x => {
                switch (x)
                {
                    case SampleRunningStatus.Complete:
                        return "Complete";
                    case SampleRunningStatus.Error:
                        return IsBlockedSample ? "Block Error" : "Error";
                    case SampleRunningStatus.Stopped:
                        return "Stopped";
                    case SampleRunningStatus.Queued:
                        return "Queued";
                    case SampleRunningStatus.Running:
                        return "Running";
                    case SampleRunningStatus.WaitingToRun:
                        return "Waiting";
                }

                return null;
            }).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.Status);

            statusToolTipText = Sample.WhenAnyValue(x => x.RunningStatus).Select(x =>
            {
                switch (x)
                {
                    case SampleRunningStatus.Complete:
                        return "The sample ran successfully.";
                    case SampleRunningStatus.Error:
                        return IsBlockedSample ? "There was an error and this sample was part of a block.  You should re-run the block of samples" : "An error occurred while running this sample.";
                    case SampleRunningStatus.Stopped:
                        return IsBlockedSample ? "The sample was stopped but was part of a block.  You should re-run the block of samples" : "The sample execution was stopped.";
                    case SampleRunningStatus.Queued:
                        return "The sample is queued but not scheduled to run.";
                    case SampleRunningStatus.Running:
                        return "The sample is running.";
                    case SampleRunningStatus.WaitingToRun:
                        return "The sample is scheduled to run and waiting.";
                }

                return null;
            }).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.StatusToolTipText);

            isUnusedSample = Sample.WhenAnyValue(x => x.Name).Select(x => x.Contains(SampleQueue.CONST_DEFAULT_INTEGRATE_SAMPLENAME)).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.IsUnusedSample);
            nameHasInvalidChars = this.WhenAnyValue(x => x.Sample, x => x.Sample.Name).Select(x => !x.Item1.NameCharactersValid()).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.NameHasInvalidChars);

            sampleNameToolTipText = this.WhenAnyValue(x => x.Sample, x => x.Sample.Name, x => x.Sample.IsDuplicateName).Select(
                x =>
                {
                    // Specially color any rows with duplicate request names
                    if (x.Item3)
                        return "Duplicate Request Name Found!";

                    if (!x.Item1.NameCharactersValid())
                        return "Request name contains invalid characters!\n" + SampleData.ValidNameCharacters;

                    return null;
                }).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.SampleNameToolTipText);

            hasError = Sample.WhenAnyValue(x => x.SampleErrors).Select(x => !string.IsNullOrWhiteSpace(x)).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.HasError);

            sequenceToolTipText = Sample.WhenAnyValue(x => x.RunningStatus, x => x.ActualLCMethod, x => x.ActualLCMethod.Start, x => x.ActualLCMethod.End,
                x => x.ActualLCMethod.ActualStart, x => x.ActualLCMethod.ActualEnd).Select(x =>
            {
                switch (Sample.RunningStatus)
                {
                    case SampleRunningStatus.Complete:
                        return $"Started: {x.Item5}\nFinished: {x.Item6}";
                    case SampleRunningStatus.Queued:
                    case SampleRunningStatus.Error:
                    case SampleRunningStatus.Stopped:
                        return null;
                    case SampleRunningStatus.Running:
                        return $"Started: {x.Item5}\nEstimated End: {x.Item4}";
                    case SampleRunningStatus.WaitingToRun:
                        return $"Estimated Start: {x.Item3}\nEstimated End: {x.Item4}";
                }

                return null;
            }).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.SequenceToolTipText);

            // Extras to trigger the collection monitor when nested properties change
            //Sample.WhenAnyValue(x => x.InstrumentMethod, x => x.PAL, x => x.LCMethodName)
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Subscribe(x => this.RaisePropertyChanged(nameof(Sample)));

            lcMethodCueBannerText = Sample.WhenAnyValue(x => x.IsSetToRunOrHasRun, x => x.LCMethodName, x => x.ActualLCMethod).Select(x => {
                if (x.Item1)
                {
                    // Prefer the ActualMethodName over the MethodName
                    return x.Item3?.Name ?? x.Item2;
                }

                return "Select";
            }).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.LcMethodCueBannerText);
        }

        // Local "wrappers" around the static class options, for data binding purposes
        public ReadOnlyObservableCollection<string> LcMethodComboBoxOptions => SampleDataManager.LcMethodNameOptions;
        public ReadOnlyObservableCollection<string> PalTrayComboBoxOptions => SampleDataManager.PalTrayOptions;
        public ReadOnlyObservableCollection<string> InstrumentMethodComboBoxOptions => SampleDataManager.InstrumentMethodOptions;

        private readonly ObservableAsPropertyHelper<bool> isUnusedSample;
        private readonly ObservableAsPropertyHelper<bool> hasError;
        private readonly ObservableAsPropertyHelper<bool> nameHasInvalidChars;
        private readonly ObservableAsPropertyHelper<string> statusToolTipText;
        private readonly ObservableAsPropertyHelper<string> sampleNameToolTipText;
        private readonly ObservableAsPropertyHelper<string> sequenceToolTipText;

        public bool IsBlockedSample => (Sample.DmsData?.Block ?? 0) > 0; // Never updates

        /// <summary>
        /// If the name of the sample is "(unused)", it means that the Sample Queue has backfilled the
        /// samples to help the user normalize samples on columns.
        /// </summary>
        public bool IsUnusedSample => isUnusedSample.Value;

        public bool HasError => hasError.Value;

        public bool NameHasInvalidChars => nameHasInvalidChars.Value;

        private readonly ObservableAsPropertyHelper<SolidColorBrush> columnNumberBgColor;

        /// <summary>
        /// Define the background color for the LC Column
        /// </summary>
        public SolidColorBrush ColumnNumberBgColor => columnNumberBgColor.Value;

        /// <summary>
        /// Tool tip text displaying status details
        /// </summary>
        public string StatusToolTipText => statusToolTipText.Value;

        /// <summary>
        /// Tool tip text displaying details about a request name error (usually for duplicate request names)
        /// </summary>
        public string SampleNameToolTipText => sampleNameToolTipText.Value;

        public string SequenceToolTipText => sequenceToolTipText.Value;

        private readonly ObservableAsPropertyHelper<ColumnData> columnData;
        private readonly ObservableAsPropertyHelper<string> columnNumber;
        private readonly ObservableAsPropertyHelper<bool> editAllowed;
        private readonly ObservableAsPropertyHelper<string> status;
        private readonly ObservableAsPropertyHelper<string> lcMethodCueBannerText;

        public ColumnData ColumnData => columnData.Value;

        public string ColumnNumber => columnNumber.Value;

        public bool EditAllowed => editAllowed.Value;

        public string Status => status.Value;

        public string LcMethodCueBannerText => lcMethodCueBannerText.Value;

        public bool Equals(SampleViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Sample, other.Sample);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SampleViewModel) obj);
        }

        public override int GetHashCode()
        {
            return (Sample != null ? Sample.GetHashCode() : 0);
        }
    }
}
