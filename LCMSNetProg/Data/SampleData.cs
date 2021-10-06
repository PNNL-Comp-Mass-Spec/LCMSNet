using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using LcmsNetSDK;
using LcmsNetSDK.Configuration;
using LcmsNetSDK.Data;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;

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

        #region Delegate Definitions

        /// <summary>
        /// Delegate method definition to be called when a sample is modified.
        /// </summary>
        /// <param name="modifiedData">Modified sample.</param>
        /// <param name="propertyName">Name of the property that was changed.</param>
        public delegate void DelegateSamplePropertyChangedHandler(SampleData modifiedData, string propertyName);

        #endregion

        #region Constructors

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

            //
            // Set the default column to the first column,
            // and sequence number to non-existent.
            //
            sequenceNumber = -1;

            LCMethodName = "";
            Volume = CartConfiguration.MinimumSampleVolume;
            //
            // Default state is always to be queued but not waiting to run.
            //
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
            isDuplicateRequestName = other.isDuplicateRequestName;
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

        #endregion

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

        #region "Members"

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
        /// If the sample's request name is a duplicate
        /// </summary>
        private bool isDuplicateRequestName = false;

        private string sampleErrors = null;
        private int dmsRequestId = 0;

        #endregion

        #region "Properties"

        /// <summary>
        /// Name of the sample (also used for the file name)
        /// </summary>
        [PersistenceSetting(ColumnName = "DMS.DatasetName", IsUniqueColumn = true)]
        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        [PersistenceSetting(IgnoreProperty = true)]
        public string SampleErrors
        {
            get => sampleErrors;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = null; // For UI purposes, we want this to be null if it is blank (because it then prevents appearance of the tooltip).
                }
                this.RaiseAndSetIfChanged(ref sampleErrors, value, nameof(SampleErrors));
            }
        }

        /// <summary>
        /// Whether this is possibly a dummy sample, and a real sample needs to be looked up before we perform any operations
        /// Default value is true; exists to prevent excessive lookups of the real sample.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
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
                    OnPropertyChanged(nameof(RunningStatus));
                    return;
                }
                // Set it if it changed, and only raise the other propertyChanged notifications if it changed
                var oldHasNotRun = HasNotRun;
                var oldIsSetToRun = IsSetToRunOrHasRun;
                if (this.RaiseAndSetIfChangedRetBool(ref runningStatus, value, nameof(RunningStatus)))
                {
                    if (oldHasNotRun != HasNotRun)
                    {
                        OnPropertyChanged(nameof(HasNotRun));
                    }
                    if (oldIsSetToRun != IsSetToRunOrHasRun)
                    {
                        OnPropertyChanged(nameof(IsSetToRunOrHasRun));
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets if the sample's request name is a duplicate
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public bool IsDuplicateRequestName
        {
            get => isDuplicateRequestName;
            set => this.RaiseAndSetIfChanged(ref isDuplicateRequestName, value, nameof(IsDuplicateRequestName));
        }

        /// <summary>
        /// True when changing the Running status manually is enabled
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public bool HasNotRun => !(RunningStatus == SampleRunningStatus.Complete || RunningStatus == SampleRunningStatus.Running);

        /// <summary>
        /// True when the sample has been set to run or has run
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public bool IsSetToRunOrHasRun => RunningStatus == SampleRunningStatus.WaitingToRun || !HasNotRun;

        /// <summary>
        /// Gets or sets the instrument method.
        /// </summary>
        [PersistenceSetting(ColumnName = "Ins.MethodName")]
        public string InstrumentMethod
        {
            get => instrumentMethod;
            set => this.RaiseAndSetIfChanged(ref instrumentMethod, value, nameof(InstrumentMethod));
        }

        [PersistenceSetting(ColumnName = "DMS.RequestID", IsUniqueColumn = true)]
        public int DmsRequestId
        {
            get => DmsData?.RequestID ?? dmsRequestId;
            set => dmsRequestId = value;
        }

        /// <summary>
        /// Gets or sets the list of data downloaded from DMS for this sample
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public DMSData DmsData { get; }

        /// <summary>
        /// Gets or sets the pal data associated with this sample.
        /// </summary>
        [PersistenceSetting(ColumnNamePrefix = "PAL.")]
        public PalData PAL { get; }

        /// <summary>
        /// Gets the experiment object data.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
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

                this.RaiseAndSetIfChanged(ref actualMethod, value, nameof(ActualLCMethod));
            }
        }

        /// <summary>
        /// Gets or sets the experiment setup object data.
        /// </summary>
        [PersistenceSetting(ColumnName = "exp.ExperimentName", PropertyGetOverrideMethod = nameof(GetLcMethodToPersist))]
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

                if (this.RaiseAndSetIfChangedRetBool(ref methodName, value, nameof(LCMethodName)))
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
            set => this.RaiseAndSetIfChanged(ref sequenceNumber, value, nameof(SequenceID));
        }

        /// <summary>
        /// Gets or sets the volume of the sample to inject.
        /// </summary>
        public double Volume
        {
            get => volume;
            set
            {
                if (value < CartConfiguration.MinimumVolume)
                {
                    // Report property changed to force UI refresh
                    this.RaisePropertyChanged(nameof(Volume));
                    return;
                }
                this.RaiseAndSetIfChanged(ref volume, value, nameof(Volume));
            }
        }

        /// <summary>
        /// Gets or sets the column data this sample is/was run on.
        /// </summary>
        [PersistenceSetting(ColumnName = "Col.ID")]
        public int ColumnIndex
        {
            get => columnIndex;
            set => this.RaiseAndSetIfChanged(ref columnIndex, value);
        }

        /// <summary>
        /// Gets or sets the unique ID for a sample.
        /// Unique ID for this sample not related to request name or sequence ID.
        /// </summary>
        public long UniqueID { get; set; }

        #endregion

        #region ITriggerFilePalData Implementation

        /// <inheritdoc />
        [PersistenceSetting(IgnoreProperty = true)]
        public virtual DateTime RunStart => ActualLCMethod?.ActualStart ?? DateTime.MinValue;

        /// <inheritdoc />
        [PersistenceSetting(IgnoreProperty = true)]
        public virtual DateTime RunFinish => ActualLCMethod?.ActualEnd ?? DateTime.MinValue;

        /// <inheritdoc />
        [PersistenceSetting(IgnoreProperty = true)]
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

        #endregion

        #region "Methods"

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

        private string GetLcMethodToPersist()
        {
            var methodToExport = "";
            if (ActualLCMethod != null && RunningStatus == SampleRunningStatus.Complete)
            {
                // Store the actual LCMethod rather than the current version of it
                methodToExport = ActualLCMethod.Name;
            }
            else if (LCMethodName != null)
            {
                methodToExport = LCMethodName;
            }

            return methodToExport;
        }

        #endregion

        #region "PropertyChanged" event handlers

        private void PalDataChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals(nameof(PAL.PALTray)) || args.PropertyName.Equals(nameof(PAL.Well)))
            {
                OnPropertyChanged(nameof(PAL));
            }
        }

        #endregion

        #region "IEquatable Implementation"

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

        #endregion

        #region "INotifyPropertyChanged implementation"

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}