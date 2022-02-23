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
        #region Constants

        /// <summary>
        /// Index Offset for going from a zero based array for configuration data to the
        /// user-readable column display.
        /// </summary>
        protected const int CONST_COLUMN_INDEX_OFFSET = 1;

        #endregion

        public SampleData Sample { get; }

        [Obsolete("For WPF Design time use only.", true)]
        public SampleViewModel()
        { }

        public SampleViewModel(SampleData sample)
        {
            Sample = sample;
            isChecked = Sample.IsSetToRunOrHasRun;

            columnData = this.WhenAnyValue(x => x.Sample.ColumnIndex).Select(x => CartConfiguration.Columns[x]).ToProperty(this, x => x.ColumnData);

            this.WhenAnyValue(x => x.Sample.ColumnIndex, x => x.Sample.LCMethodName).Subscribe(x =>
            {
                this.RaisePropertyChanged(nameof(ColumnNumber));
                this.RaisePropertyChanged(nameof(ColumnNumberBgColor));
            });
            this.WhenAnyValue(x => x.Sample.ColumnIndex).Subscribe(x => this.RaisePropertyChanged(nameof(ColumnNumber)));
            columnNumberBgColor = this.WhenAnyValue(x => x.ColumnData.Color).Select(x => new SolidColorBrush(x)).ToProperty(this, x => x.ColumnNumberBgColor, Brushes.RoyalBlue);

            this.WhenAnyValue(x => x.Sample.IsSetToRunOrHasRun).Subscribe(x => this.IsChecked = x);
            this.WhenAnyValue(x => x.Sample.RunningStatus).Subscribe(x =>
            {
                this.RaisePropertyChanged(nameof(Status));
                this.RaisePropertyChanged(nameof(StatusToolTipText));
            });

            this.WhenAnyValue(x => x.Sample.Name).Subscribe(x =>
            {
                this.RaisePropertyChanged(nameof(Name));
                this.RaisePropertyChanged(nameof(IsUnusedSample));
                this.RaisePropertyChanged(nameof(NameHasInvalidChars));
                this.CheckDatasetName();
            });

            this.WhenAnyValue(x => x.Sample.InstrumentMethod).Subscribe(x => this.RaisePropertyChanged(nameof(InstrumentMethod)));

            this.WhenAnyValue(x => x.Sample.IsDuplicateRequestName).Subscribe(x => this.CheckDatasetName());

            this.WhenAnyValue(x => x.Sample.SampleErrors).Subscribe(x => this.RaisePropertyChanged(nameof(HasError)));

            this.WhenAnyValue(x => x.Sample.ActualLCMethod, x => x.Sample.ActualLCMethod.Start, x => x.Sample.ActualLCMethod.End,
                x => x.Sample.ActualLCMethod.ActualStart, x => x.Sample.ActualLCMethod.ActualEnd).Subscribe(x => this.RaisePropertyChanged(nameof(SequenceToolTipText)));

            // Extras to trigger the collection monitor when nested properties change
            this.WhenAnyValue(x => x.Sample.PAL.PALTray, x => x.Sample.PAL.Well)
                .Subscribe(x => this.RaisePropertyChanged(nameof(Name)));
            this.WhenAnyValue(x => x.Sample.SequenceID, x => x.Sample.Volume).Subscribe(x => this.RaisePropertyChanged(nameof(Name)));
            this.WhenAnyValue(x => x.Sample.LCMethodName).Subscribe(x => this.RaisePropertyChanged(nameof(Sample.LCMethodName)));

            Sample.WhenAnyValue(x => x.InstrumentMethod, x => x.PAL, x => x.LCMethodName)
                .Subscribe(x => this.RaisePropertyChanged(nameof(Sample)));

            Sample.WhenAnyValue(x => x.ActualLCMethod).Subscribe(x => this.RaisePropertyChanged(nameof(LcMethodCueBannerText)));
        }

        // Local "wrappers" around the static class options, for data binding purposes
        public ReadOnlyObservableCollection<string> LcMethodComboBoxOptions => SampleDataManager.LcMethodNameOptions;
        public ReadOnlyObservableCollection<string> PalTrayComboBoxOptions => SampleDataManager.PalTrayOptions;
        public ReadOnlyObservableCollection<string> InstrumentMethodComboBoxOptions => SampleDataManager.InstrumentMethodOptions;

        #region Row and cell color control

        public bool IsBlockedSample => (Sample.DmsData?.Block ?? 0) > 0;

        /// <summary>
        /// If the name of the sample is "(unused)", it means that the Sample Queue has backfilled the
        /// samples to help the user normalize samples on columns.
        /// </summary>
        public bool IsUnusedSample => Sample.Name.Contains(SampleQueue.CONST_DEFAULT_INTEGRATE_SAMPLENAME);

        public bool HasError => !string.IsNullOrWhiteSpace(Sample.SampleErrors);

        public bool NameHasInvalidChars => !Sample.NameCharactersValid();

        /// <summary>
        /// Sets the row colors based on the sample data
        /// </summary>
        public void CheckDatasetName()
        {
            // Specially color any rows with duplicate request names
            if (Sample.IsDuplicateRequestName)
            {
                RequestNameToolTipText = "Duplicate Request Name Found!";
            }
            else if (!Sample.NameCharactersValid())
            {
                RequestNameToolTipText = "Request name contains invalid characters!\n" + SampleData.ValidNameCharacters;
            }
            else
            {
                RequestNameToolTipText = null;
            }
        }

        private readonly ObservableAsPropertyHelper<SolidColorBrush> columnNumberBgColor;

        /// <summary>
        /// Define the background color for the LC Column
        /// </summary>
        public SolidColorBrush ColumnNumberBgColor => columnNumberBgColor != null ? columnNumberBgColor.Value : Brushes.RoyalBlue;

        #endregion

        #region ToolTips

        private string requestNameToolTipText = null;
        private string sequenceToolTipFormat = null;

        /// <summary>
        /// Tool tip text displaying status details
        /// </summary>
        public string StatusToolTipText
        {
            get
            {
                var statusMessage = "";
                switch (Sample.RunningStatus)
                {
                    case SampleRunningStatus.Complete:
                        statusMessage = "The sample ran successfully.";
                        SequenceToolTipFormat = "Started: {2}\nFinished: {3}";
                        break;
                    case SampleRunningStatus.Error:
                        if (Sample.DmsData != null && Sample.DmsData.Block > 0)
                        {
                            statusMessage =
                                "There was an error and this sample was part of a block.  You should re-run the block of samples";
                        }
                        else
                        {
                            statusMessage = "An error occurred while running this sample.";
                        }
                        break;
                    case SampleRunningStatus.Stopped:
                        if (Sample.DmsData != null && Sample.DmsData.Block > 0)
                        {
                            statusMessage =
                                "The sample was stopped but was part of a block.  You should re-run the block of samples";
                        }
                        else
                        {
                            statusMessage = "The sample execution was stopped.";
                        }
                        break;
                    case SampleRunningStatus.Queued:
                        statusMessage = "The sample is queued but not scheduled to run.";
                        SequenceToolTipFormat = null;
                        break;
                    case SampleRunningStatus.Running:
                        statusMessage = "The sample is running.";
                        SequenceToolTipFormat = "Started: {2}\nEstimated End: {1}";
                        break;
                    case SampleRunningStatus.WaitingToRun:
                        statusMessage = "The sample is scheduled to run and waiting.";
                        SequenceToolTipFormat = "Estimated Start: {0}\nEstimated End: {1}";
                        break;
                    default:
                        // Should never get here
                        break;
                }

                return statusMessage;
            }
        }

        /// <summary>
        /// Tool tip text displaying details about a request name error (usually for duplicate request names)
        /// </summary>
        public string RequestNameToolTipText
        {
            get => requestNameToolTipText;
            private set => this.RaiseAndSetIfChanged(ref requestNameToolTipText, value);
        }

        public string SequenceToolTipText
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SequenceToolTipFormat))
                {
                    return string.Format(SequenceToolTipFormat, Sample.ActualLCMethod?.Start, Sample.ActualLCMethod?.End, Sample.ActualLCMethod?.ActualStart, Sample.ActualLCMethod?.ActualEnd);
                }
                return null;
            }
        }

        private string SequenceToolTipFormat
        {
            get => sequenceToolTipFormat;
            set
            {
                this.RaiseAndSetIfChanged(ref sequenceToolTipFormat, value);
                this.RaisePropertyChanged(nameof(SequenceToolTipText));
            }
        }

        #endregion

        #region Column data

        private bool isChecked;

        public bool IsChecked
        {
            get => isChecked;
            set => this.RaiseAndSetIfChanged(ref isChecked, value);
        }

        private readonly ObservableAsPropertyHelper<ColumnData> columnData;

        public ColumnData ColumnData => columnData.Value;

        public bool EditAllowed => !Sample.IsSetToRunOrHasRun;

        public string SpecialColumnNumber { get; set; }

        public string ColumnNumber
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SpecialColumnNumber))
                {
                    return SpecialColumnNumber;
                }
                return (Sample.ColumnIndex + CONST_COLUMN_INDEX_OFFSET).ToString();
            }
        }

        public string Status
        {
            get
            {
                var statusMessage = "";
                CheckDatasetName();
                switch (Sample.RunningStatus)
                {
                    case SampleRunningStatus.Complete:
                        statusMessage = "Complete";
                        break;
                    case SampleRunningStatus.Error:
                        if (Sample.DmsData != null && Sample.DmsData.Block > 0)
                        {
                            statusMessage = "Block Error";
                        }
                        else
                        {
                            statusMessage = "Error";
                        }
                        break;
                    case SampleRunningStatus.Stopped:
                        statusMessage = "Stopped";
                        break;
                    case SampleRunningStatus.Queued:
                        statusMessage = "Queued";
                        break;
                    case SampleRunningStatus.Running:
                        statusMessage = "Running";
                        break;
                    case SampleRunningStatus.WaitingToRun:
                        statusMessage = "Waiting";
                        break;
                    default:
                        // Should never get here
                        break;
                }

                return statusMessage;
            }
        }

        /// <summary>
        /// Sample request name
        /// </summary>
        public string Name
        {
            get => Sample.Name;
            set
            {
                if (Sample.Name != value)
                {
                    Sample.Name = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Sample Instrument Method
        /// </summary>
        public string InstrumentMethod
        {
            get => Sample.InstrumentMethod;
            set
            {
                if (!object.Equals(Sample.InstrumentMethod, value))
                {
                    Sample.InstrumentMethod = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string LcMethodCueBannerText
        {
            get
            {
                if (Sample.IsSetToRunOrHasRun)
                {
                    if (Sample.ActualLCMethod != null)
                    {
                        return Sample.ActualLCMethod.Name;
                    }

                    return Sample.LCMethodName;
                }

                return "Select";
            }
        }

        #endregion

        #region Equality

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

        #endregion
    }
}
