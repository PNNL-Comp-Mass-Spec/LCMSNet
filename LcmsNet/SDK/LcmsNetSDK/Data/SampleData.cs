﻿using System;
using System.ComponentModel;
using LcmsNetData;
using LcmsNetData.Data;
using LcmsNetData.System;
using LcmsNetSDK.Configuration;
using LcmsNetSDK.Method;

namespace LcmsNetSDK.Data
{
    /// <summary>
    /// Class to hold data for one sample (more specifically, one instrument dataset)
    /// </summary>
    [Serializable]
    public class SampleData : ISampleData, ITriggerFilePalData, IRequestedRunDataWithPalData, ICloneable, IEquatable<SampleData>, INotifyPropertyChangedExt
    {
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
        ///  Default constructor: assumes sample is a dummy or unchecked sample.
        /// </summary>
        public SampleData() : this(true)
        {
        }

        ISampleData ISampleData.GetNewNonDummy()
        {
            return GetNewNonDummy();
        }

        public SampleData GetNewNonDummy()
        {
            return new SampleData(false);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="isDummySample">If this is possibly a dummy or unchecked sample, and the real sample needs to be found in the queue/list</param>
        public SampleData(bool isDummySample)
        {
            IsDummySample = isDummySample;
            DmsData = new DMSData();

            PAL = new PalData();
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

            Operator = LCMSSettings.GetParameter(LCMSSettings.PARAM_OPERATOR);
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Makes a deep copy of this object
        /// </summary>
        /// <returns>Deep copy of object</returns>
        public object Clone()
        {
            var newSample = new SampleData(this.IsDummySample);

            newSample.DmsData = this.DmsData?.Clone() as DMSData;
            newSample.sequenceNumber = this.sequenceNumber;
            newSample.PAL = this.PAL?.Clone() as PalData;
            newSample.volume = this.volume;
            newSample.ColumnIndex = this.ColumnIndex;
            newSample.UniqueID = this.UniqueID;
            newSample.LCMethodName = this.LCMethodName;
            newSample.actualMethod = this.actualMethod?.Clone() as LCMethod;
            newSample.InstrumentMethod = this.InstrumentMethod;
            if (!string.IsNullOrWhiteSpace(Operator))
            {
                newSample.Operator = this.Operator;
            }
            newSample.isDuplicateRequestName = this.isDuplicateRequestName;
            newSample.sampleErrors = this.sampleErrors;

            // The ability to set some properties is keyed on the value of this property, so set it last.
            newSample.runningStatus = this.runningStatus;

            return newSample;
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
            var oldName = sample.DmsData.DatasetName;
            var now = TimeKeeper.Instance.Now;
            var dateName = now.ToString("ddMMMyy");

            var cartColumn = BuildCartColumnName(sample);
            var name = $"_{dateName}_{cartColumn}";

            var containsInfoAlready = oldName.Contains(cartColumn);
            if (!containsInfoAlready)
            {
                sample.DmsData.DatasetName = oldName + name;
            }
        }

        /// <summary>
        /// Resets the dataset name to the original request name.
        /// </summary>
        /// <param name="sample"></param>
        public static void ResetDatasetNameToRequestName(SampleData sample)
        {
            sample.DmsData.DatasetName = sample.DmsData.RequestName;
        }

        public override string ToString()
        {
            if (DmsData != null)
            {
                return DmsData.ToString();
            }
            return base.ToString();
        }

        #region "Members"

        /// <summary>
        /// Sequence order of the sample to run.
        /// </summary>
        private long sequenceNumber;

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
        /// DMS Data structure.
        /// </summary>
        private DMSData dmsData;

        /// <summary>
        /// Pal Data reference.
        /// </summary>
        private IPalData palData;

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

        #endregion

        #region "Properties"

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

        /// <summary>
        /// Interface implementation of DmsData
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public IDmsData DmsBasicData => DmsData;

        /// <summary>
        /// Gets or sets the list of data downloaded from DMS for this sample
        /// </summary>
        [PersistenceSetting(ColumnNamePrefix = "DMS.")]
        public DMSData DmsData
        {
            get => dmsData;
            set
            {
                var oldValue = dmsData;
                if (this.RaiseAndSetIfChangedRetBool(ref dmsData, value, nameof(DmsData)))
                {
                    if (oldValue != null)
                    {
                        oldValue.PropertyChanged -= DmsDataChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += DmsDataChanged;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the pal data associated with this sample.
        /// </summary>
        [PersistenceSetting(ColumnNamePrefix = "PAL.")]
        public IPalData PAL
        {
            get => palData;
            set
            {
                var oldValue = palData;
                if (this.RaiseAndSetIfChangedRetBool(ref palData, value, nameof(PAL)))
                {
                    if (oldValue != null)
                    {
                        oldValue.PropertyChanged -= PalDataChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += PalDataChanged;
                    }
                }
            }
        }

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
        public long SequenceID
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
        /// Operator performing LC run
        /// </summary>
        public string Operator { get; set; }

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

        /// <inheritdoc />
        [PersistenceSetting(IgnoreProperty = true)]
        public string InstrumentName => LCMSSettings.GetParameter(LCMSSettings.PARAM_INSTNAME);

        /// <inheritdoc />
        [PersistenceSetting(IgnoreProperty = true)]
        public string SeparationType => LCMSSettings.GetParameter(LCMSSettings.PARAM_SEPARATIONTYPE);

        /// <inheritdoc />
        [PersistenceSetting(IgnoreProperty = true)]
        public virtual string CaptureShareName => "";

        /// <inheritdoc />
        [PersistenceSetting(IgnoreProperty = true)]
        public virtual string CaptureSubdirectoryPath => "";

        /// <inheritdoc />
        [PersistenceSetting(IgnoreProperty = true)]
        public virtual string InterestRating => "Unreviewed";

        #endregion

        #region "Methods"

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

        private void DmsDataChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals(nameof(DmsData.DatasetName)) || args.PropertyName.Equals(nameof(DmsData.RequestName)) ||
                args.PropertyName.Equals(nameof(DmsData.CartConfigName)) || args.PropertyName.Equals(nameof(DmsData.DatasetType)) ||
                args.PropertyName.Equals(nameof(DmsData.RunOrder)) || args.PropertyName.Equals(nameof(DmsData.Batch)) ||
                args.PropertyName.Equals(nameof(DmsData.Block)) || args.PropertyName.Equals(nameof(DmsData.EMSLUsageType)) ||
                args.PropertyName.Equals(nameof(DmsData.EMSLProposalUser)) || args.PropertyName.Equals(nameof(DmsData.Experiment)) ||
                args.PropertyName.Equals(nameof(DmsData.RequestID)) || args.PropertyName.Equals(nameof(DmsData.EMSLProposalID)))
            {
                OnPropertyChanged(nameof(DmsData));
            }
        }

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