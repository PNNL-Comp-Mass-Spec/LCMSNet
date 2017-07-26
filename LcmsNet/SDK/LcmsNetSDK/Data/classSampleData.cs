//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/07/2009
//
// Updates:
// - 1/16/2009: Brian LaMarche
//    - Added Name for the sample
//    - Added some comments for the get or set accesors for private member variables.
//    - Made the class implement the ICloneable interface for shallow copies of references.
//    - Added the constant value region for default sample names.
// - 1/19/2009: Brian LaMarche
//     - Added Column ID, and Sequence ID
//     - Added a property to the PAL data.
// - 2/10/2009: Dave Clark
//     - Added method for retrieving current values of all properties at once
// - 2/17/2009: Dave Clark
//     - Changed to inherit from classDataClassBase and override data prep method
// - 2/20/2009: Dave Clark
//     - Changed clone method to deep copy
// - 2/27/2009: Dave Clark
//     - Changed event attributes to non-serialized to fix deep clone bug
// - 3/16/2009:  Brian LaMarche
//     - Added the Instrument and Experiment Method Name properties.
// - 3/19/2009: Dave Clark
//     - Modified cache property storage/retrieval methods to reflect new properties added 3/16
// - 4/3/2009: Dave Clark
//     - Corrected bug in restoration of experiment data from cache
// - 10/8/2009: Brian LaMarche
//     - Added the LC Method data...not sure if the experiment data should be removed.
// - 7/16/2010: Brian LaMarche
//     - Added a fullname option to display date and cart name appended to the DMS request name.
//
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Data;
using LcmsNetDataClasses.Method;
using LcmsNetSDK;

namespace LcmsNetDataClasses
{
    /// <summary>
    /// Enumeration describing the status of a sample.
    /// </summary>
    public enum enumSampleRunningStatus
    {
        /// <summary>
        /// Queued but not told to execute.
        /// </summary>
        Queued,

        /// <summary>
        /// Stopped
        /// </summary>
        Stopped,

        /// <summary>
        /// Waiting to run.
        /// </summary>
        WaitingToRun,

        /// <summary>
        /// Sample is currently running.
        /// </summary>
        Running,

        /// <summary>
        /// Sample successfully finished running.
        /// </summary>
        Complete,

        /// <summary>
        /// Error occurred during the run.
        /// </summary>
        Error
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class NotStoredPropertyAttribute : Attribute { }

    /// <summary>
    /// Class to hold data for one sample (more specifically, one instrument dataset)
    /// </summary>
    [Serializable]
    public class classSampleData : classDataClassBase, ICloneable, INotifyPropertyChangedExt, IEquatable<classSampleData>
    {
        #region Delegate Definitions

        /// <summary>
        /// Delegate method definition to be called when a sample is modified.
        /// </summary>
        /// <param name="modifiedData">Modified sample.</param>
        /// <param name="propertyName">Name of the property that was changed.</param>
        public delegate void DelegateSamplePropertyChangedHandler(classSampleData modifiedData, string propertyName);

        #endregion

        /// <summary>
        /// The minimum sample volume for this system.
        /// </summary>
        public const double CONST_MIN_SAMPLE_VOLUME = 0.1;

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="isDummySample">If this is possibly a dummy or unchecked sample, and the real sample needs to be found in the queue/list</param>
        public classSampleData(bool isDummySample = true)
        {
            IsDummySample = isDummySample;
            m_DmsData = new classDMSData();

            //
            // Set the default column to the first column,
            // and sequence number to non-existent.
            //
            m_sequenceNumber = -1;

            m_palData = new classPalData();
            ColumnData = new classColumnData();
            m_instrumentData = new classInstrumentInfo();
            m_method = null;
            Volume = CONST_MIN_SAMPLE_VOLUME;
            //
            // Default state is always to be queued but not waiting to run.
            //
            RunningStatus = enumSampleRunningStatus.Queued;
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Makes a deep copy of this object
        /// </summary>
        /// <returns>Deep copy of object</returns>
        public object Clone() // TODO: do a normal clone, not the serialize/deserialize oddness
        {
            //Create a formatter and a memory stream
            IFormatter formatter = new BinaryFormatter();
            var ms = new MemoryStream();
            //Serialize the input object
            formatter.Serialize(ms, this);
            //Reset the stream to its beginning and de-serialize into the return object
            ms.Seek(0, SeekOrigin.Begin);
            var newSample = formatter.Deserialize(ms) as classSampleData;
            return newSample;
        }

        #endregion

        public static string BuildCartColumnName(classSampleData sample)
        {
            var cartName = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTNAME);
            var columnName = "";

            if (!String.IsNullOrEmpty(sample.ColumnData?.Name))
            {
                columnName = sample.ColumnData.Name;
            }

            return cartName + "_" + columnName;
        }

        /// <summary>
        /// Sets the dataset name
        /// </summary>
        /// <param name="sample"></param>
        public static void AddDateCartColumnToDatasetName(classSampleData sample)
        {
            var oldName = sample.DmsData.DatasetName;
            var now = TimeKeeper.Instance.Now;
            var dateName = now.ToString("ddMMMyy");

            var cartColumn = BuildCartColumnName(sample);
            var name = string.Format("_{0}_{1}", dateName, cartColumn);

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
        public static void ResetDatasetNameToRequestName(classSampleData sample)
        {
            sample.DmsData.DatasetName = sample.DmsData.RequestName;
        }

        public static string GetTriggerFileName(classSampleData sample, string extension)
        {
            var datasetName = sample.DmsData.DatasetName;
            var outFileName =
                string.Format("{0}_{1}_{2}{3}",
                    classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTNAME),
                    //DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).ToString("MM.dd.yyyy_hh.mm.ss_"),
                    sample.LCMethod.Start.ToString("MM.dd.yyyy_hh.mm.ss"),
                    datasetName,
                    extension);
            return outFileName;
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
        classDMSData m_DmsData;

        /// <summary>
        /// Sequence order of the sample to run.
        /// </summary>
        private long m_sequenceNumber;

        /// <summary>
        /// Pal Data reference.
        /// </summary>
        private classPalData m_palData;

        /// <summary>
        /// Volume of sample to inject.
        /// </summary>
        private double m_volume;

        /// <summary>
        /// Information regarding what column the sample is to be, or did run on.
        /// </summary>
        private classColumnData m_columnData;

        /// <summary>
        /// Unique ID for this sample not related to request name or sequence ID.
        /// </summary>
        private long m_uniqueID;

        /// <summary>
        /// LC Method that controls all of the hardware via the scheduling interface.
        /// </summary>
        private classLCMethod m_method;

        /// <summary>
        /// Instrument info.
        /// </summary>
        private classInstrumentInfo m_instrumentData;

        /// <summary>
        /// Operator performing LC run
        /// </summary>
        private string m_Operator = "";

        /// <summary>
        /// Status of the sample running on a column thread or waiting in a queue.
        /// </summary>
        private enumSampleRunningStatus m_RunningStatus;

        /// <summary>
        /// If the sample's request name is a duplicate
        /// </summary>
        private bool m_IsDuplicateRequestName = false;

        private string m_SampleErrors = null;

        #endregion

        #region "Properties"

        [NotStoredProperty]
        public string SampleErrors
        {
            get { return m_SampleErrors; }
            set { this.RaiseAndSetIfChanged(ref m_SampleErrors, value); }
        }

        /// <summary>
        /// Whether this is possibly a dummy sample, and a real sample needs to be looked up before we perform any operations
        /// Default value is true; exists to prevent excessive lookups of the real sample.
        /// </summary>
        public bool IsDummySample { get; private set; }

        /// <summary>
        /// Gets or sets the status of the sample running on a column thread or waiting in a queue.
        /// </summary>
        public enumSampleRunningStatus RunningStatus
        {
            get { return m_RunningStatus; }
            set
            {
                // Set it if it changed, and only raise the other propertyChanged notifications if it changed
                var oldHasNotRun = HasNotRun;
                var oldIsSetToRun = IsSetToRunOrHasRun;
                if (this.RaiseAndSetIfChangedRetBool(ref m_RunningStatus, value))
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
        [NotStoredProperty]
        public bool IsDuplicateRequestName
        {
            get { return m_IsDuplicateRequestName; }
            set { this.RaiseAndSetIfChanged(ref m_IsDuplicateRequestName, value); }
        }

        /// <summary>
        /// True when changing the Running status manually is enabled
        /// </summary>
        [NotStoredProperty]
        public bool HasNotRun
        {
            get { return !(RunningStatus == enumSampleRunningStatus.Complete || RunningStatus == enumSampleRunningStatus.Running); }
        }

        /// <summary>
        /// True when the sample has been set to run or has run
        /// </summary>
        [NotStoredProperty]
        public bool IsSetToRunOrHasRun
        {
            get { return RunningStatus == enumSampleRunningStatus.WaitingToRun || !HasNotRun; }
        }

        /// <summary>
        /// Gets or sets the instrument object data.
        /// </summary>
        public classInstrumentInfo InstrumentData
        {
            get { return m_instrumentData; }
            set { this.RaiseAndSetIfChanged(ref m_instrumentData, value); }
        }

        /// <summary>
        /// Gets or sets the experiment object data.
        /// </summary>
        public classLCMethod LCMethod
        {
            get { return m_method; }
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref m_method, value) && m_method != null && m_method.Column != ColumnData.ID)
                {
                    if (m_method.Column >= 0)
                    {
                        ColumnData = classCartConfiguration.Columns[m_method.Column];
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the list of data downloaded from DMS for this sample
        /// </summary>
        public classDMSData DmsData
        {
            get { return m_DmsData; }
            set { this.RaiseAndSetIfChanged(ref m_DmsData, value); }
        }

        /// <summary>
        /// Gets or sets the sequence number that the sample is run in.
        /// </summary>
        public long SequenceID
        {
            get { return m_sequenceNumber; }
            set { this.RaiseAndSetIfChanged(ref m_sequenceNumber, value); }
        }

        /// <summary>
        /// Gets or sets the pal data associated with this sample.
        /// </summary>
        public classPalData PAL
        {
            get { return m_palData; }
            set { this.RaiseAndSetIfChanged(ref m_palData, value); }
        }

        /// <summary>
        /// Gets or sets the volume of the sample to inject.
        /// </summary>
        public double Volume
        {
            get { return m_volume; }
            set
            {
                if (value < classCartConfiguration.MinimumVolume)
                {
                    // Report property changed to force UI refresh
#if DotNET4
                    this.OnPropertyChanged(nameof(Volume));
#else
                    this.OnPropertyChanged();
#endif
                    return;
                }
                this.RaiseAndSetIfChanged(ref m_volume, value);
            }
        }

        /// <summary>
        /// Gets or sets the column data this sample is/was run on.
        /// </summary>
        public classColumnData ColumnData
        {
            get { return m_columnData; }
            set
            {
                var oldColumn = m_columnData;
                if (this.RaiseAndSetIfChangedRetBool(ref m_columnData, value))
                {
                    if (oldColumn != null)
                    {
                        oldColumn.NameChanged -= m_columnData_NameChanged;
                    }
                    if (m_columnData != null)
                    {
                        m_columnData.NameChanged += m_columnData_NameChanged;
                    }
                }
            }
        }

        public void CloneLCMethod()
        {
            var method = this.LCMethod;
            this.m_method = null; // Avoid firing the change notifications twice, by setting just the backing variable to null...
            this.LCMethod = method.Clone() as classLCMethod;
        }

        void m_columnData_NameChanged(object sender, string name, string oldName)
        {
            if (DmsData == null || oldName == "")
                return;

            var cartName = "";
            if (DmsData.CartName != null)
            {
                cartName = DmsData.CartName;
            }
            var cartColumn = cartName + "_" + oldName;

            // Make sure we actually have a cart and column name.
            var length = oldName.Length;
            if (DmsData.CartName != null)
                length += DmsData.CartName.Length;

            if (length < 1)
                return;

            var contains = DmsData.DatasetName.Contains(cartColumn);
            if (contains)
            {
                var cartColumnNew = BuildCartColumnName(this);
                DmsData.DatasetName = DmsData.DatasetName.Replace(cartColumn, cartColumnNew);
            }
        }

        /// <summary>
        /// Gets or sets the unique ID for a sample.
        /// </summary>
        public long UniqueID
        {
            get { return m_uniqueID; }
            set { m_uniqueID = value; }
        }

        public string Operator
        {
            get { return m_Operator; }
            set { m_Operator = value; }
        }

#endregion

#region "Methods"

        /// <summary>
        /// Gets current values for all the properties in the class in key/value format
        /// </summary>
        /// <returns>String dictionary containing current values of all properties</returns>
        public override Dictionary<string, string> GetPropertyValues()
        {
            //NOTE: This method must be modified if new property representing an object is added
            var newDictionary = new Dictionary<string, string>();

            // Use reflection to get the name and value for each property and store in a dictionary
            var classType = GetType();
            var properties = classType.GetProperties();
            foreach (var property in properties)
            {
                // Ignore flagged properties
                if (Attribute.IsDefined(property, typeof(NotStoredPropertyAttribute)))
                {
                    continue;
                }
                switch (property.PropertyType.ToString())
                {
                    case "LcmsNetDataClasses.classDMSData":
                        // Special case - get the DMS data for this object and add properties to string dictionary
                        var dmsDict = DmsData.GetPropertyValues();
                        foreach (var entry in dmsDict)
                        {
                            newDictionary.Add("DMS." + entry.Key, entry.Value);
                        }
                        break;
                    case "LcmsNetDataClasses.Data.classPalData":
                        // Special case - get the PAL data for this object and add properties to string dictionary
                        var palDict = PAL.GetPropertyValues();
                        foreach (var entry in palDict)
                        {
                            newDictionary.Add("PAL." + entry.Key, entry.Value);
                        }
                        break;
                    case "LcmsNetDataClasses.Configuration.classColumnData":
                        if (ColumnData != null)
                        {
                            // Special case - get the column data for this object and add properties to string dictionary
                            var colDict = ColumnData.GetPropertyValues();
                            foreach (var entry in colDict)
                            {
                                newDictionary.Add("Col." + entry.Key, entry.Value);
                            }
                        }
                        break;
                    case "LcmsNetDataClasses.Method.classLCMethod":
                        if (LCMethod != null)
                        {
                            // Special case - get the experiment data for this object and add properties to string dictionary
                            var expDict = LCMethod.GetPropertyValues();
                            foreach (var entry in expDict)
                            {
                                //TODO: Do we need to change the name from exp to LCMethod to be consistent.
                                newDictionary.Add("exp." + entry.Key, entry.Value.ToString());
                            }
                        }
                        else
                        {
                            var method = new classLCMethod();
                            var expDict = method.GetPropertyValues();
                            foreach (var entry in expDict)
                            {
                                //TODO: Do we need to change the name from exp to LCMethod to be consistent.
                                newDictionary.Add("exp." + entry.Key, entry.Value.ToString());
                            }
                        }
                        break;
                    case "LcmsNetDataClasses.classInstrumentInfo":
                        // Special case - get the experiment data for this object and add properties to string dictionary
                        var instDict = InstrumentData.GetPropertyValues();
                        foreach (var entry in instDict)
                        {
                            newDictionary.Add("Ins." + entry.Key, entry.Value);
                        }
                        break;
                    default:
                        newDictionary.Add(property.Name, property.GetValue(this, null).ToString());
                        break;
                }
            }

            return newDictionary;
        }

        public override void LoadPropertyValues(Dictionary<string, string> propValues)
        {
            //NOTE: This method must be modified if new property representing an object is added
            var baseProps = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var dmsProps = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var palProps = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var colProps = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var expProps = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var instProps = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            // Separate the properties into class dictionaries
            foreach (var property in propValues)
            {
                var keyName = property.Key;
                if (keyName.Length < 4)
                {
                    // Property name is too short to be one of the properties that holds a class, so add
                    //      it to the main class dictionary
                    baseProps.Add(keyName, property.Value);
                    continue;
                }
                // Stuff string dictionaries for each property holding a class, and main class
                switch (keyName.Substring(0, 4).ToLower())
                {
                    case "dms.":
                        dmsProps.Add(keyName.Substring(4, keyName.Length - 4), property.Value);
                        break;
                    case "pal.":
                        palProps.Add(keyName.Substring(4, keyName.Length - 4), property.Value);
                        break;
                    case "col.":
                        colProps.Add(keyName.Substring(4, keyName.Length - 4), property.Value);
                        break;
                    case "exp.":
                        expProps.Add(keyName.Substring(4, keyName.Length - 4), property.Value);
                        break;
                    case "ins.":
                        instProps.Add(keyName.Substring(4, keyName.Length - 4), property.Value);
                        break;
                    default:
                        baseProps.Add(keyName, property.Value);
                        break;
                }
            }

            // Call each of LoadPropertyValues methods for properties that represent objects
            base.LoadPropertyValues(baseProps);
            DmsData.LoadPropertyValues(dmsProps);
            PAL.LoadPropertyValues(palProps);

            if (colProps.Count > 0)
            {
                ColumnData = new classColumnData();
                ColumnData.LoadPropertyValues(colProps);
            }

            if (expProps.Count > 0)
            {
                LCMethod = new classLCMethod();
                LCMethod.LoadPropertyValues(expProps);
            }
        }

#endregion

#region "INotifyPropertyChanged implementation"

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

#if DotNET4
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

#else
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
#endif
#endregion

#region "IEquatable Implementation"

        public bool Equals(classSampleData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return m_uniqueID == other.m_uniqueID;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((classSampleData) obj);
        }

        public override int GetHashCode()
        {
            return m_uniqueID.GetHashCode();
        }

#endregion
    }
}