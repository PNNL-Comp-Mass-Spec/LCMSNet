using System;
using System.Reactive.Linq;
using System.Windows.Media;
using LcmsNetData;
using LcmsNetData.Data;
using LcmsNetSDK.Data;
using LcmsNetSDK.Method;
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

        public SampleData Sample { get; private set; }

        [Obsolete("For WPF Design time use only.", true)]
        public SampleViewModel()
        { }

        public SampleViewModel(SampleData sample)
        {
            Sample = sample;
            isChecked = Sample.IsSetToRunOrHasRun;

            this.WhenAnyValue(x => x.Sample.ColumnData, x => x.Sample.LCMethod).Subscribe(x =>
            {
                this.RaisePropertyChanged(nameof(ColumnNumber));
                this.RaisePropertyChanged(nameof(ColumnNumberBgColor));
            });
            this.WhenAnyValue(x => x.Sample.ColumnData.ID).Subscribe(x => this.RaisePropertyChanged(nameof(ColumnNumber)));
            this.WhenAnyValue(x => x.Sample.ColumnData.Color).Select(x => new SolidColorBrush(x)).ToProperty(this, x => x.ColumnNumberBgColor, out columnNumberBgColor, Brushes.RoyalBlue);

            this.WhenAnyValue(x => x.Sample.IsSetToRunOrHasRun).Subscribe(x => this.IsChecked = x);
            this.WhenAnyValue(x => x.Sample.RunningStatus).Subscribe(x =>
            {
                this.RaisePropertyChanged(nameof(Status));
                this.RaisePropertyChanged(nameof(StatusToolTipText));
            });

            this.WhenAnyValue(x => x.Sample.DmsData, x => x.Sample.DmsData.DatasetName, x => x.Sample.DmsData.RequestName).Subscribe(x =>
            {
                this.RaisePropertyChanged(nameof(RequestName));
                this.RaisePropertyChanged(nameof(IsUnusedSample));
                this.RaisePropertyChanged(nameof(NameHasInvalidChars));
                this.CheckDatasetName();
            });

            this.WhenAnyValue(x => x.Sample.InstrumentData).Subscribe(x => this.RaisePropertyChanged(nameof(InstrumentMethod)));
            this.WhenAnyValue(x => x.Sample.InstrumentData.MethodName).Subscribe(x => this.RaisePropertyChanged(nameof(InstrumentMethod)));

            this.WhenAnyValue(x => x.Sample.IsDuplicateRequestName).Subscribe(x => this.CheckDatasetName());

            this.WhenAnyValue(x => x.Sample.SampleErrors).Subscribe(x => this.RaisePropertyChanged(nameof(HasError)));

            this.WhenAnyValue(x => x.Sample.ActualLCMethod, x => x.Sample.ActualLCMethod.Start, x => x.Sample.ActualLCMethod.End,
                x => x.Sample.ActualLCMethod.ActualStart, x => x.Sample.ActualLCMethod.ActualEnd).Subscribe(x => this.RaisePropertyChanged(nameof(SequenceToolTipText)));

            // Extras to trigger the collection monitor when nested properties change
            this.WhenAnyValue(x => x.Sample.DmsData.Block, x => x.Sample.DmsData.RunOrder, x => x.Sample.DmsData.Batch,
                    x => x.Sample.DmsData.CartConfigName, x => x.Sample.DmsData.DatasetType, x => x.Sample.PAL.PALTray, x => x.Sample.PAL.Well)
                .Subscribe(x => this.RaisePropertyChanged(nameof(RequestName)));
            this.WhenAnyValue(x => x.Sample.SequenceID, x => x.Sample.Volume).Subscribe(x => this.RaisePropertyChanged(nameof(RequestName)));
            this.WhenAnyValue(x => x.Sample.LCMethod).Subscribe(x => this.RaisePropertyChanged(nameof(Sample.LCMethod)));

            this.WhenAnyValue(x => x.Sample.DmsData.Block).Subscribe(x => this.RaisePropertyChanged(nameof(IsBlockedSample)));

            Sample.WhenAnyValue(x => x.InstrumentData, x => x.PAL, x => x.DmsData, x => x.LCMethod)
                .Subscribe(x => this.RaisePropertyChanged(nameof(Sample)));

            Sample.WhenAnyValue(x => x.ActualLCMethod).Subscribe(x => this.RaisePropertyChanged(nameof(LcMethodCueBannerText)));
        }

        // Local "wrappers" around the static class options, for data binding purposes
        public IReadOnlyReactiveList<LCMethod> LcMethodComboBoxOptions => SampleDataManager.LcMethodOptions;
        public IReadOnlyReactiveList<string> DatasetTypeComboBoxOptions => SampleDataManager.DatasetTypeOptions;
        public IReadOnlyReactiveList<string> PalTrayComboBoxOptions => SampleDataManager.PalTrayOptions;
        public IReadOnlyReactiveList<string> InstrumentMethodComboBoxOptions => SampleDataManager.InstrumentMethodOptions;
        public IReadOnlyReactiveList<string> CartConfigComboBoxOptions => SampleDataManager.CartConfigOptions;
        public string CartConfigError => SampleDataManager.CartConfigOptionsError;

        #region Row and cell color control

        public bool IsBlockedSample => Sample.DmsData.Block > 0;

        /// <summary>
        /// If the name of the sample is "(unused)", it means that the Sample Queue has backfilled the
        /// samples to help the user normalize samples on columns.
        /// </summary>
        public bool IsUnusedSample => Sample.DmsData.DatasetName.Contains(SampleQueue.CONST_DEFAULT_INTEGRATE_SAMPLENAME);

        public bool HasError => !string.IsNullOrWhiteSpace(Sample.SampleErrors);

        public bool NameHasInvalidChars => !Sample.DmsData.DatasetNameCharactersValid();

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
            else if (!Sample.DmsData.DatasetNameCharactersValid())
            {
                RequestNameToolTipText = "Request name contains invalid characters!\n" + DMSData.ValidDatasetNameCharacters;
            }
            else
            {
                RequestNameToolTipText = null;
            }
        }

        private ObservableAsPropertyHelper<SolidColorBrush> columnNumberBgColor;

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
                        if (Sample.DmsData.Block > 0)
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
                        if (Sample.DmsData.Block > 0)
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
                    return string.Format(SequenceToolTipFormat, Sample.ActualLCMethod.Start, Sample.ActualLCMethod.End, Sample.ActualLCMethod.ActualStart, Sample.ActualLCMethod.ActualEnd);
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
                return (Sample.ColumnData.ID + CONST_COLUMN_INDEX_OFFSET).ToString();
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
                        if (Sample.DmsData.Block > 0)
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
        public string RequestName
        {
            get
            {
                if (Sample.DmsData.DatasetName.Contains(SampleQueue.CONST_DEFAULT_INTEGRATE_SAMPLENAME))
                {
                    return Sample.DmsData.RequestName;
                }
                return Sample.DmsData.DatasetName;
            }
            set
            {
                if (Sample.DmsData.RequestName != value)
                {
                    Sample.DmsData.RequestName = value;
                    Sample.DmsData.DatasetName = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Sample Instrument Method
        /// </summary>
        public string InstrumentMethod
        {
            get => Sample.InstrumentData.MethodName;
            set
            {
                if (Sample.InstrumentData == null)
                {
                    Sample.InstrumentData = new InstrumentInfo();
                }
                if (!object.Equals(Sample.InstrumentData.MethodName, value))
                {
                    Sample.InstrumentData.MethodName = value;
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
                    return Sample.ActualLCMethod.Name;
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
