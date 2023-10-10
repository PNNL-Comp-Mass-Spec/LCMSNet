using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Text.RegularExpressions;
using LcmsNet.Configuration;
using LcmsNet.Method;
using LcmsNetSDK;
using LcmsNetSDK.Data;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;
using ReactiveUI;

namespace LcmsNet.Data
{
    /// <summary>
    /// Class to hold data for one sample (more specifically, one instrument dataset)
    /// </summary>
    [Serializable]
    public class SampleData : ISampleInfo, IEquatable<SampleData>, INotifyPropertyChangedExt
    {
        /// <summary>
        /// The matching string to ensure only valid characters exist in a sample name
        /// </summary>
        public const string ValidNameRegexString = @"^[a-zA-Z0-9_\-]+$";

        /// <summary>
        /// The list of characters allowed in a sample name
        /// </summary>
        public const string ValidNameCharacters = @"Valid characters are: 'A-Z', 'a-z', '0-9', '-', '_' (no spaces)";

        /// <summary>
        /// Regex to use to test if a dataset name only contains valid characters
        /// </summary>
        public static readonly Regex NameValidationRegex = new Regex(ValidNameRegexString, RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Delegate method definition to be called when a sample is modified.
        /// </summary>
        /// <param name="modifiedData">Modified sample.</param>
        /// <param name="propertyName">Name of the property that was changed.</param>
        public delegate void DelegateSamplePropertyChangedHandler(SampleData modifiedData, string propertyName);

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="isDummySample">If this is possibly a dummy or unchecked sample, and the real sample needs to be found in the queue/list</param>
        /// <param name="dmsData">DMSData object for extra sample metadata</param>
        public SampleData(bool isDummySample = true, DMSData dmsData = null)
        {
            IsDummySample = isDummySample;
            DmsData = dmsData;

            Name = DmsData?.RequestName ?? "";

            PAL = new PalData();
            PAL.PropertyChanged += PalDataChanged;
            columnIndex = 0;
            InstrumentMethod = "";

            // Set the default column to the first column,
            // and sequence number to non-existent.
            sequenceNumber = -1;

            LCMethodName = "";
            Volume = CartLimits.MinimumSampleVolume;

            // Default state is always to be queued but not waiting to run.
            RunningStatus = SampleRunningStatus.Queued;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other">Object from where we copy data</param>
        /// <param name="cloneDmsData">If true, also copy the <see cref="DmsData"/> property (shallow copy).</param>
        private SampleData(SampleData other, bool cloneDmsData) : this(other.IsDummySample)
        {
            Name = other.Name;
            if (cloneDmsData)
            {
                DmsData = other.DmsData;
                DmsRequestId = other.DmsRequestId;
            }

            sequenceNumber = other.sequenceNumber;

            PAL = other.PAL.Clone();
            PAL.PropertyChanged += PalDataChanged;

            volume = other.volume;
            ColumnIndex = other.ColumnIndex;
            UniqueID = other.UniqueID;
            LCMethodName = other.LCMethodName;
            actualMethod = other.actualMethod?.Clone() as LCMethod;
            InstrumentMethod = other.InstrumentMethod;
            isDuplicateName = other.isDuplicateName;
            sampleErrors = other.sampleErrors;

            // The ability to set some properties is keyed on the value of this property, so set it last.
            runningStatus = other.runningStatus;
        }

        /// <summary>
        /// Makes a deep copy of this object
        /// </summary>
        /// <param name="cloneDmsData">If true, also copy the <see cref="DmsData"/> property (shallow copy).</param>
        /// <returns>Deep copy of object</returns>
        public SampleData Clone(bool cloneDmsData = false)
        {
            return new SampleData(this, cloneDmsData);
        }

        public static string BuildCartColumnName(SampleData sample)
        {
            var cartName = LCMSSettings.GetParameter(LCMSSettings.PARAM_CARTNAME);
            var columnName = "";
            var column = CartConfiguration.Columns[sample.ColumnIndex];

            if (!string.IsNullOrEmpty(column.Name))
            {
                columnName = column.Name;
            }

            return cartName + "_" + columnName;
        }

        /// <summary>
        /// Sets the dataset name
        /// </summary>
        /// <param name="sample"></param>
        public static void AddDateCartColumnToDatasetName(SampleData sample)
        {
            var oldName = sample.Name;
            var now = TimeKeeper.Instance.Now;
            var dateName = now.ToString("ddMMMyy");

            var cartColumn = BuildCartColumnName(sample);
            var name = $"_{dateName}_{cartColumn}";

            var containsInfoAlready = oldName.Contains(cartColumn);
            if (!containsInfoAlready)
            {
                sample.Name = oldName + name;
            }
        }

        /// <summary>
        /// Resets the dataset name to the original request name.
        /// </summary>
        /// <param name="sample"></param>
        public static void ResetDatasetNameToRequestName(SampleData sample)
        {
            if (sample.DmsData != null)
                sample.Name = sample.DmsData.RequestName;
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Name))
                return Name;

            return SequenceID.ToString();
        }

        private string name = "";

        /// <summary>
        /// Sequence order of the sample to run.
        /// </summary>
        private int sequenceNumber;

        /// <summary>
        /// Volume of sample to inject.
        /// </summary>
        private double volume;

        /// <summary>
        /// LC Method that controls all of the hardware via the scheduling interface - UI consistent version.
        /// </summary>
        private string methodName;

        /// <summary>
        /// LC Method that controls all of the hardware via the scheduling interface.
        /// </summary>
        private LCMethod actualMethod;

        /// <summary>
        /// Information regarding what column the sample is to be, or did run on.
        /// </summary>
        private int columnIndex;

        /// <summary>
        /// Instrument method.
        /// </summary>
        private string instrumentMethod;

        /// <summary>
        /// Status of the sample running on a column thread or waiting in a queue.
        /// </summary>
        private SampleRunningStatus runningStatus;

        /// <summary>
        /// If the sample's name is a duplicate
        /// </summary>
        private bool isDuplicateName = false;

        /// <summary>
        /// Backing variable for UI property
        /// </summary>
        private bool isChecked = false;

        private string sampleErrors = null;
        private int dmsRequestId = 0;

        /// <summary>
        /// Name of the sample (also used for the file name)
        /// </summary>
        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        public string SampleErrors
        {
            get => sampleErrors;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = null; // For UI purposes, we want this to be null if it is blank (because it then prevents appearance of the tooltip).
                }
                this.RaiseAndSetIfChanged(ref sampleErrors, value);
            }
        }

        /// <summary>
        /// Whether this is possibly a dummy sample, and a real sample needs to be looked up before we perform any operations
        /// Default value is true; exists to prevent excessive lookups of the real sample.
        /// </summary>
        public bool IsDummySample { get; private set; }

        /// <summary>
        /// Gets or sets the status of the sample running on a column thread or waiting in a queue.
        /// </summary>
        public SampleRunningStatus RunningStatus
        {
            get => runningStatus;
            set
            {
                if (runningStatus == SampleRunningStatus.Complete)
                {
                    OnPropertyChanged();
                    return;
                }
                // Set it if it changed, and only raise the other propertyChanged notifications if it changed
                var oldHasNotRun = HasNotRun;
                var oldIsSetToRun = IsSetToRunOrHasRun;
                if (this.RaiseAndSetIfChangedRetBool(ref runningStatus, value))
                {
                    if (oldHasNotRun != HasNotRun)
                    {
                        OnPropertyChanged(nameof(HasNotRun));
                    }
                    if (oldIsSetToRun != IsSetToRunOrHasRun)
                    {
                        OnPropertyChanged(nameof(IsSetToRunOrHasRun));
                        if (IsChecked != IsSetToRunOrHasRun)
                        {
                            RxApp.MainThreadScheduler.Schedule(() => IsChecked = IsSetToRunOrHasRun);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets if the sample's name is a duplicate
        /// </summary>
        public bool IsDuplicateName
        {
            get => isDuplicateName;
            set => this.RaiseAndSetIfChanged(ref isDuplicateName, value);
        }

        /// <summary>
        /// True when changing the Running status manually is enabled
        /// </summary>
        public bool HasNotRun => !(RunningStatus == SampleRunningStatus.Complete || RunningStatus == SampleRunningStatus.Running);

        /// <summary>
        /// True when the sample has been set to run or has run
        /// </summary>
        public bool IsSetToRunOrHasRun => RunningStatus == SampleRunningStatus.WaitingToRun || !HasNotRun;

        /// <summary>
        /// Gets or sets the instrument method.
        /// </summary>
        public string InstrumentMethod
        {
            get => instrumentMethod;
            set => this.RaiseAndSetIfChanged(ref instrumentMethod, value);
        }

        public int DmsRequestId
        {
            get => DmsData?.RequestID ?? dmsRequestId;
            set => dmsRequestId = value;
        }

        /// <summary>
        /// Gets or sets the list of data downloaded from DMS for this sample
        /// </summary>
        public DMSData DmsData { get; }

        /// <summary>
        /// Pal data associated with this sample.
        /// </summary>
        public PalData PAL { get; }

        IPalData ISampleInfo.PAL => PAL;

        /// <summary>
        /// Gets the experiment object data.
        /// </summary>
        public LCMethod ActualLCMethod
        {
            get => actualMethod;
            private set
            {
                // Disallow method changes on queued/running/complete samples
                if (IsSetToRunOrHasRun)
                {
                    return;
                }

                this.RaiseAndSetIfChanged(ref actualMethod, value);
            }
        }

        ILCMethod ISampleInfo.ActualLCMethod => ActualLCMethod;

        /// <summary>
        /// Gets or sets the experiment setup object data.
        /// </summary>
        public string LCMethodName
        {
            get => methodName;
            set
            {
                // Disallow method changes on queued/running/complete samples
                if (IsSetToRunOrHasRun)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    this.RaisePropertyChanged();
                    return;
                }

                if (this.RaiseAndSetIfChangedRetBool(ref methodName, value))
                {
                    var method = LCMethodManager.Manager.GetLCMethodByName(value);

                    if (method != null && method.Column != ColumnIndex && method.Column >= 0)
                    {
                        ColumnIndex = method.Column;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the sequence number that the sample is run in.
        /// </summary>
        public int SequenceID
        {
            get => sequenceNumber;
            set => this.RaiseAndSetIfChanged(ref sequenceNumber, value);
        }

        /// <summary>
        /// Gets or sets the volume of the sample to inject.
        /// </summary>
        public double Volume
        {
            get => volume;
            set
            {
                if (value < CartLimits.MinimumVolume)
                {
                    // Report property changed to force UI refresh
                    this.RaisePropertyChanged();
                    return;
                }
                this.RaiseAndSetIfChanged(ref volume, value);
            }
        }

        /// <summary>
        /// Gets or sets the column data this sample is/was run on.
        /// </summary>
        public int ColumnIndex
        {
            get => columnIndex;
            set => this.RaiseAndSetIfChanged(ref columnIndex, value);
        }

        public string SpecialColumnNumber { get; set; }

        /// <summary>
        /// Gets or sets the unique ID for a sample.
        /// Unique ID for this sample not related to request name or sequence ID.
        /// </summary>
        public long UniqueID { get; set; }

        /// <summary>
        /// Property for UI interaction
        /// </summary>
        public bool IsChecked
        {
            get => isChecked;
            set => this.RaiseAndSetIfChanged(ref isChecked, value);
        }

        /// <summary>
        /// The time the sample did or should start running
        /// </summary>
        public virtual DateTime RunStart => ActualLCMethod?.ActualStart ?? DateTime.MinValue;

        /// <summary>
        /// The time the sample did or should finish running
        /// </summary>
        public virtual DateTime RunFinish => ActualLCMethod?.ActualEnd ?? DateTime.MinValue;

        /// <inheritdoc />
        public virtual string ColumnName
        {
            get
            {
                if (ColumnIndex > -1 && ColumnIndex < CartConfiguration.Columns.Count)
                {
                    return CartConfiguration.Columns[ColumnIndex].Name;
                }

                return "";
            }
        }

        /// <summary>
        /// Set method name and column index without any extra lookups.
        /// </summary>
        /// <param name="newMethodName"></param>
        /// <param name="newColumnIndex"></param>
        internal void SetMethodNameAndColumnIndex(string newMethodName, int newColumnIndex)
        {
            // Disallow method changes on queued/running/complete samples
            if (IsSetToRunOrHasRun)
            {
                return;
            }

            if (newColumnIndex >= 0)
            {
                ColumnIndex = newColumnIndex;
            }

            this.RaiseAndSetIfChanged(ref methodName, newMethodName, nameof(LCMethodName));
        }

        public void ForceRefreshForUI()
        {
            this.RaisePropertyChanged(nameof(RunningStatus));
            this.RaisePropertyChanged(nameof(IsSetToRunOrHasRun));
            this.RaisePropertyChanged(nameof(HasNotRun));
            this.RaisePropertyChanged(nameof(RunStart));
            this.RaisePropertyChanged(nameof(RunFinish));
        }

        public bool NameCharactersValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = DmsData?.RequestName ?? "";
            }

            return NameValidationRegex.IsMatch(Name);
        }

        public List<string[]> GetExportValuePairs()
        {
            var exportData = new List<string[]>();
            if (DmsData != null)
            {
                exportData.AddRange(DmsData.GetExportValuePairs());
            }
            else
            {
                exportData.Add(new[] { "Request Id:", DmsRequestId.ToString() });
            }

            return exportData;
        }

        public void SetActualLcMethod()
        {
            if (string.IsNullOrWhiteSpace(LCMethodName))
            {
                return;
            }

            var method = LCMethodManager.Manager.GetLCMethodByName(LCMethodName);
            // Force a change and refresh, by setting the backing value to null first.
            actualMethod = null;
            if (method != null && LCMethodName.Equals(method.Name))
            {
                ActualLCMethod = (LCMethod) method.Clone();
            }
        }

        private void PalDataChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals(nameof(PAL.PALTray)) || args.PropertyName.Equals(nameof(PAL.Well)))
            {
                OnPropertyChanged(nameof(PAL));
            }
        }

        public bool Equals(SampleData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return UniqueID == other.UniqueID;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SampleData) obj);
        }

        public override int GetHashCode()
        {
            return UniqueID.GetHashCode();
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
