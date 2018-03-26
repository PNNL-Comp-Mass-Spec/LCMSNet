using System;
using System.Reactive.Linq;
using System.Windows.Media;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Method;
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

        public classSampleData Sample { get; private set; }

        [Obsolete("For WPF Design time use only.", true)]
        public SampleViewModel()
        { }

        public SampleViewModel(classSampleData sample)
        {
            Sample = sample;
            isChecked = Sample.IsSetToRunOrHasRun;

            this.WhenAnyValue(x => x.Sample.ColumnData).Subscribe(x =>
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
                //this.RaisePropertyChanged(nameof(IsChecked));
                SetRowColors();
            });

            this.WhenAnyValue(x => x.Sample.LCMethod).Subscribe(x =>
            {
                this.RaisePropertyChanged(nameof(ColumnNumber));
                this.RaisePropertyChanged(nameof(ColumnNumberBgColor));
            });

            this.WhenAnyValue(x => x.Sample.DmsData).Subscribe(x => this.RaisePropertyChanged(nameof(RequestName)));
            this.WhenAnyValue(x => x.Sample.DmsData.DatasetName).Subscribe(x =>
            {
                this.RaisePropertyChanged(nameof(RequestName));
                this.SetRowColors();
            });
            this.WhenAnyValue(x => x.Sample.DmsData.RequestName).Subscribe(x =>
            {
                this.RaisePropertyChanged(nameof(RequestName));
                this.SetRowColors();
            });

            this.WhenAnyValue(x => x.Sample.InstrumentData).Subscribe(x => this.RaisePropertyChanged(nameof(InstrumentMethod)));
            this.WhenAnyValue(x => x.Sample.InstrumentData.MethodName).Subscribe(x => this.RaisePropertyChanged(nameof(InstrumentMethod)));

            this.WhenAnyValue(x => x.Sample.IsDuplicateRequestName).Subscribe(x => this.SetRowColors());

            this.WhenAnyValue(x => x.Sample.SampleErrors).Subscribe(x => this.SetRowColors());

            this.WhenAnyValue(x => x.Sample.ActualLCMethod, x => x.Sample.ActualLCMethod.Start, x => x.Sample.ActualLCMethod.End,
                x => x.Sample.ActualLCMethod.ActualStart, x => x.Sample.ActualLCMethod.ActualEnd).Subscribe(x => this.RaisePropertyChanged(nameof(SequenceToolTipText)));

            // Extras to trigger the collection monitor when nested properties change
            this.WhenAnyValue(x => x.Sample.DmsData.Block, x => x.Sample.DmsData.RunOrder, x => x.Sample.DmsData.Batch,
                    x => x.Sample.DmsData.CartConfigName, x => x.Sample.DmsData.DatasetType, x => x.Sample.PAL.PALTray, x => x.Sample.PAL.Well)
                .Subscribe(x => this.RaisePropertyChanged(nameof(RequestName)));
            this.WhenAnyValue(x => x.Sample.SequenceID, x => x.Sample.Volume).Subscribe(x => this.RaisePropertyChanged(nameof(RequestName)));
            this.WhenAnyValue(x => x.Sample.LCMethod).Subscribe(x => this.RaisePropertyChanged(nameof(Sample.LCMethod)));

            Sample.WhenAnyValue(x => x.InstrumentData, x => x.PAL, x => x.DmsData, x => x.LCMethod)
                .Subscribe(x => this.RaisePropertyChanged(nameof(Sample)));

            Sample.WhenAnyValue(x => x.ActualLCMethod).Subscribe(x => this.RaisePropertyChanged(nameof(LcMethodCueBannerText)));
        }

        // Local "wrappers" around the static class options, for data binding purposes
        public IReadOnlyReactiveList<classLCMethod> LcMethodComboBoxOptions => SampleDataManager.LcMethodOptions;
        public IReadOnlyReactiveList<string> DatasetTypeComboBoxOptions => SampleDataManager.DatasetTypeOptions;
        public IReadOnlyReactiveList<string> PalTrayComboBoxOptions => SampleDataManager.PalTrayOptions;
        public IReadOnlyReactiveList<string> InstrumentMethodComboBoxOptions => SampleDataManager.InstrumentMethodOptions;
        public IReadOnlyReactiveList<string> CartConfigComboBoxOptions => SampleDataManager.CartConfigOptions;
        public string CartConfigError => SampleDataManager.CartConfigOptionsError;

        #region Row and cell colors

        private SolidColorBrush rowBackColor;
        private SolidColorBrush rowForeColor;
        private SolidColorBrush rowSelectionBackColor;
        private SolidColorBrush rowSelectionForeColor;
        private SolidColorBrush requestNameBackColor = null;

        public SolidColorBrush RowBackColor
        {
            get { return rowBackColor; }
            private set { this.RaiseAndSetIfChanged(ref rowBackColor, value); }
        }

        public SolidColorBrush RowForeColor
        {
            get { return rowForeColor; }
            private set { this.RaiseAndSetIfChanged(ref rowForeColor, value); }
        }

        public SolidColorBrush RowSelectionBackColor
        {
            get { return rowSelectionBackColor; }
            private set { this.RaiseAndSetIfChanged(ref rowSelectionBackColor, value); }
        }

        public SolidColorBrush RowSelectionForeColor
        {
            get { return rowSelectionForeColor; }
            private set { this.RaiseAndSetIfChanged(ref rowSelectionForeColor, value); }
        }

        public SolidColorBrush RequestNameBackColor
        {
            get
            {
                if (requestNameBackColor == null)
                {
                    return RowBackColor;
                }
                return requestNameBackColor;
            }
            private set { this.RaiseAndSetIfChanged(ref requestNameBackColor, value); }
        }

        /// <summary>
        /// Sets the row colors based on the sample data
        /// </summary>
        public void SetRowColors()
        {
            // We need to color the sample based on its status.
            // Make sure selected rows column colors don't change for running and waiting to run
            // but only for queued, or completed (including error) sample status.
            switch (Sample.RunningStatus)
            {
                case enumSampleRunningStatus.Running:
                    RowBackColor = Brushes.Lime;
                    RowForeColor = Brushes.Black;
                    RowSelectionBackColor = RowBackColor;
                    RowSelectionForeColor = RowForeColor;
                    break;
                case enumSampleRunningStatus.WaitingToRun:
                    RowForeColor = Brushes.Black;
                    RowBackColor = Brushes.Yellow;
                    RowSelectionBackColor = RowBackColor;
                    RowSelectionForeColor = RowForeColor;
                    break;
                case enumSampleRunningStatus.Error:
                    if (Sample.DmsData.Block > 0)
                    {
                        RowBackColor = Brushes.Orange;
                        RowForeColor = Brushes.Black;
                    }
                    else
                    {
                        RowBackColor = Brushes.DarkRed;
                        RowForeColor = Brushes.White;
                    }
                    RowSelectionForeColor = Brushes.White;
                    RowSelectionBackColor = Brushes.Navy;
                    break;
                case enumSampleRunningStatus.Stopped:
                    if (Sample.DmsData.Block > 0)
                    {
                        RowBackColor = Brushes.SeaGreen;
                        RowForeColor = Brushes.White;
                    }
                    else
                    {
                        RowBackColor = Brushes.Tomato;
                        RowForeColor = Brushes.Black;
                    }
                    RowSelectionForeColor = Brushes.White;
                    RowSelectionBackColor = Brushes.Navy;
                    break;
                case enumSampleRunningStatus.Complete:
                    RowBackColor = Brushes.DarkGreen;
                    RowForeColor = Brushes.White;
                    RowSelectionForeColor = Brushes.White;
                    RowSelectionBackColor = Brushes.Navy;
                    break;
                case enumSampleRunningStatus.Queued:
                    goto default;
                default:
                    RowBackColor = Brushes.White;
                    RowForeColor = Brushes.Black;
                    RowSelectionForeColor = Brushes.White;
                    RowSelectionBackColor = Brushes.Navy;
                    break;
            }

            var status = Sample.ColumnData.Status;
            if (status == enumColumnStatus.Disabled)
            {
                RowBackColor = new SolidColorBrush(Color.FromArgb(RowBackColor.Color.A, (byte)Math.Max(0, RowBackColor.Color.R - 128),
                    (byte)Math.Max(0, RowBackColor.Color.G - 128),
                    (byte)Math.Max(0, RowBackColor.Color.B - 128)));
                RowForeColor = Brushes.LightGray;
            }

            // If the name of the sample is "(unused)", it means that the Sample Queue has backfilled the
            // samples to help the user normalize samples on columns.
            if (Sample.DmsData.DatasetName.Contains(classSampleQueue.CONST_DEFAULT_INTEGRATE_SAMPLENAME))
            {
                RowBackColor = Brushes.LightGray;
                RowForeColor = Brushes.DarkGray;
            }

            if (!string.IsNullOrEmpty(Sample.SampleErrors))
            {
                RowBackColor = Brushes.DeepPink;
                RowForeColor = Brushes.Black;
            }

            // Specially color any rows with duplicate request names
            if (Sample.IsDuplicateRequestName)
            {
                RequestNameBackColor = Brushes.Crimson;
                RequestNameToolTipText = "Duplicate Request Name Found!";
            }
            else if (!Sample.DmsData.DatasetNameCharactersValid())
            {
                RequestNameBackColor = Brushes.Crimson;
                RequestNameToolTipText = "Request name contains invalid characters!\n" + classDMSData.ValidDatasetNameCharacters;
            }
            else
            {
                RequestNameBackColor = null;
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

        private string requestNameToolTipText = "";
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
                    case enumSampleRunningStatus.Complete:
                        statusMessage = "The sample ran successfully.";
                        SequenceToolTipFormat = "Started: {2}\nFinished: {3}";
                        break;
                    case enumSampleRunningStatus.Error:
                        if (Sample.DmsData.Block > 0)
                        {
                            statusMessage =
                                "There was an error and this sample was part of a block.  You should re-run the block of samples";
                        }
                        else
                        {
                            statusMessage = "An error occured while running this sample.";
                        }
                        break;
                    case enumSampleRunningStatus.Stopped:
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
                    case enumSampleRunningStatus.Queued:
                        statusMessage = "The sample is queued but not scheduled to run.";
                        SequenceToolTipFormat = null;
                        break;
                    case enumSampleRunningStatus.Running:
                        statusMessage = "The sample is running.";
                        SequenceToolTipFormat = "Started: {2}\nEstimated End: {1}";
                        break;
                    case enumSampleRunningStatus.WaitingToRun:
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
            get { return requestNameToolTipText; }
            private set { this.RaiseAndSetIfChanged(ref requestNameToolTipText, value); }
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
            get { return sequenceToolTipFormat; }
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
            get { return isChecked; }
            set { this.RaiseAndSetIfChanged(ref isChecked, value); }
        }

        public bool EditAllowed
        {
            get { return !Sample.IsSetToRunOrHasRun; }
        }

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
                SetRowColors();
                switch (Sample.RunningStatus)
                {
                    case enumSampleRunningStatus.Complete:
                        statusMessage = "Complete";
                        break;
                    case enumSampleRunningStatus.Error:
                        if (Sample.DmsData.Block > 0)
                        {
                            statusMessage = "Block Error";
                        }
                        else
                        {
                            statusMessage = "Error";
                        }
                        break;
                    case enumSampleRunningStatus.Stopped:
                        statusMessage = "Stopped";
                        break;
                    case enumSampleRunningStatus.Queued:
                        statusMessage = "Queued";
                        break;
                    case enumSampleRunningStatus.Running:
                        statusMessage = "Running";
                        break;
                    case enumSampleRunningStatus.WaitingToRun:
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
                if (Sample.DmsData.DatasetName.Contains(classSampleQueue.CONST_DEFAULT_INTEGRATE_SAMPLENAME))
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
            get { return Sample.InstrumentData.MethodName; }
            set
            {
                if (Sample.InstrumentData == null)
                {
                    Sample.InstrumentData = new classInstrumentInfo();
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
