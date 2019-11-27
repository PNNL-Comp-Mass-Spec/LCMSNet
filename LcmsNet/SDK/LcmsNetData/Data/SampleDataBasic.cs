using System;
using System.ComponentModel;
using LcmsNetData.Method;

namespace LcmsNetData.Data
{
    public class SampleDataBasic : ITriggerFilePalData, INotifyPropertyChangedExt
    {
        public SampleDataBasic()
        {
            DmsData = new DMSData();

            PAL = new PalData();
            columnData = new ColumnData();
            InstrumentData = new InstrumentInfo();
            methodBasic = null;
        }

        public virtual SampleDataBasic GetNewNonDummy()
        {
            return new SampleDataBasic();
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
        /// DMS Data structure.
        /// </summary>
        private DMSData dmsData;

        /// <summary>
        /// Pal Data reference.
        /// </summary>
        private PalData palData;

        /// <summary>
        /// Information regarding what column the sample is to be, or did run on.
        /// </summary>
        private ColumnData columnData;

        /// <summary>
        /// LC Method that controls all of the hardware via the scheduling interface - UI consistent version.
        /// </summary>
        private LCMethodBasic methodBasic;

        /// <summary>
        /// Instrument info.
        /// </summary>
        private InstrumentInfo instrumentData;

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets or sets the instrument object data.
        /// </summary>
        [PersistenceSetting(ColumnNamePrefix = "Ins.")]
        public InstrumentInfo InstrumentData
        {
            get { return instrumentData; }
            set
            {
                var oldValue = instrumentData;
                if (this.RaiseAndSetIfChangedRetBool(ref instrumentData, value, nameof(InstrumentData)))
                {
                    if (oldValue != null)
                    {
                        oldValue.PropertyChanged -= InstrumentDataChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += InstrumentDataChanged;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the experiment setup object data.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public virtual LCMethodBasic ActualLCMethodBasic => LCMethodBasic;

        /// <summary>
        /// Gets or sets the experiment setup object data.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public virtual LCMethodBasic LCMethodBasic
        {
            get { return methodBasic; }
            set { this.RaiseAndSetIfChanged(ref methodBasic, value, nameof(LCMethodBasic)); }
        }

        /// <summary>
        /// Gets or sets the list of data downloaded from DMS for this sample
        /// </summary>
        [PersistenceSetting(ColumnNamePrefix = "DMS.")]
        public DMSData DmsData
        {
            get { return dmsData; }
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
        public PalData PAL
        {
            get { return palData; }
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
        /// Gets or sets the column data this sample is/was run on.
        /// </summary>
        public virtual ColumnData ColumnData
        {
            get { return columnData; }
            set { this.RaiseAndSetIfChanged(ref columnData, value, nameof(ColumnData)); }
        }

        /// <summary>
        /// Operator performing LC run
        /// </summary>
        public string Operator { get; set; } = "";

        #endregion

        #region ITriggerFilePalData Implementation

        /// <inheritdoc />
        [PersistenceSetting(IgnoreProperty = true)]
        public virtual DateTime RunStart => ActualLCMethodBasic.ActualStart;

        /// <inheritdoc />
        [PersistenceSetting(IgnoreProperty = true)]
        public virtual DateTime RunFinish => ActualLCMethodBasic.ActualEnd;

        /// <inheritdoc />
        [PersistenceSetting(IgnoreProperty = true)]
        public virtual string ColumnName => ColumnData.Name;

        /// <inheritdoc />
        [PersistenceSetting(IgnoreProperty = true)]
        public virtual string InstrumentName => "";

        /// <inheritdoc />
        [PersistenceSetting(IgnoreProperty = true)]
        public virtual string SeparationType => "";

        /// <inheritdoc />
        [PersistenceSetting(IgnoreProperty = true)]
        public virtual string CaptureSubdirectoryPath => "";

        /// <inheritdoc />
        [PersistenceSetting(IgnoreProperty = true)]
        public virtual string InterestRating => "Unreviewed";

        #endregion

        #region "PropertyChanged" event handlers

        private void InstrumentDataChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals(nameof(InstrumentData.MethodName)))
            {
                OnPropertyChanged(nameof(InstrumentData));
            }
        }

        private void DmsDataChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals(nameof(DmsData.DatasetName)) || args.PropertyName.Equals(nameof(DmsData.RequestName)) ||
                args.PropertyName.Equals(nameof(DmsData.CartConfigName)) || args.PropertyName.Equals(nameof(DmsData.DatasetType)) ||
                args.PropertyName.Equals(nameof(DmsData.RunOrder)) || args.PropertyName.Equals(nameof(DmsData.Batch)) ||
                args.PropertyName.Equals(nameof(DmsData.Block)) || args.PropertyName.Equals(nameof(DmsData.EMSLUsageType)) ||
                args.PropertyName.Equals(nameof(DmsData.UserList)) || args.PropertyName.Equals(nameof(DmsData.Experiment)) ||
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
