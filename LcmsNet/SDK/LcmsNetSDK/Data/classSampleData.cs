//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/07/2009
//
/* Last modified 01/16/2009
 *
 *      1/16/2009: Brian LaMarche
 *         - Added Name for the sample
 *         - Added some comments for the get or set accesors for private member
 *              variables.
 *         - Made the class implement the ICloneable interface for shallow
 *              copies of references.
 *         - Added the constant value region for default sample names.
 *
 *      1/19/2009: Brian LaMarche
 *          - Added Column ID, and Sequence ID
 *          - Added a property to the PAL data.
 *
 *      2/10/2009: Dave Clark
 *          - Added method for retrieving current values of all properties at once
 *
 *      2/17/2009: Dave Clark
 *          - Changed to inherit from classDataClassBase and override data prep method
 *
 *      2/20/2009: Dave Clark
 *          - Changed clone method to deep copy
 *
 *      2/27/2009: Dave Clark
 *          - Changed event attributes to non-serialized to fix deep clone bug
 *
 *      3-16-2009:  Brian LaMarche
 *          - Added the Instrument and Experiment Method Name properties.
 *
 *      3/19/2009: Dave Clark
 *          - Modified cache property storage/retrieval methods to reflect new properties added 3/16
 *
 *      4/3/2009: Dave Clark
 *          - Corrected bug in restoration of experiment data from cache
 *
 *      10/8/2009: Brian LaMarche
 *          - Added the LC Method data...not sure if the experiment data should be removed.
 *      7/16/2010: Brian LaMarche
 *          - Added a fullname option to display date and cart name appended to the DMS request name.
 * */
//*********************************************************************************************************

using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
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

    /// <summary>
    /// Class to hold data for one sample (instrument run)
    /// </summary>
    [Serializable]
    public class classSampleData : classDataClassBase, ICloneable
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
        public const double CONST_MIN_SAMPLE_VOLUME = 5;

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
            mlong_sequenceNumber = -1;

            m_palData = new classPalData();
            m_columnData = new classColumnData();
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
        public object Clone()
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
            
            if (!string.IsNullOrEmpty(sample.ColumnData?.Name))
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
            var now = TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)); ;
            var months = new[]
            {
                "Jan",
                "Feb",
                "Mar",
                "Apr",
                "May",
                "Jun",
                "Jul",
                "Aug",
                "Sep",
                "Oct",
                "Nov",
                "Dec"
            };
            var dateName = string.Format("{0}{1}{2}", now.Day, months[now.Month - 1], now.Year - 2000);
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
            var name = base.ToString();
            if (DmsData != null)
            {
                name = DmsData.DatasetName;
            }
            return name;
        }

        #region "Members"

        /// <summary>
        /// DMS Data structure.
        /// </summary>
        classDMSData m_DmsData;

        /// <summary>
        /// Sequence order of the sample to run.
        /// </summary>
        private long mlong_sequenceNumber;

        /// <summary>
        /// Pal Data reference.
        /// </summary>
        private classPalData m_palData;

        /// <summary>
        /// Volume of sample to inject.
        /// </summary>
        private double mdouble_volume;

        /// <summary>
        /// Information regarding what column the sample is to be, or did run on.
        /// </summary>
        private classColumnData m_columnData;

        /// <summary>
        /// Unique ID for this sample not related to request name or sequence ID.
        /// </summary>
        private long mlong_uniqueID;

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

        #endregion

        #region "Properties"

        /// <summary>
        /// Whether this is possibly a dummy sample, and a real sample needs to be looked up before we perform any operations
        /// Default value is true; exists to prevent excessive lookups of the real sample.
        /// </summary>
        public bool IsDummySample { get; private set; }

        /// <summary>
        /// Gets or sets the status of the sample running on a column thread or waiting in a queue.
        /// </summary>
        public enumSampleRunningStatus RunningStatus { get; set; }

        /// <summary>
        /// Gets or sets the instrument object data.
        /// </summary>
        public classInstrumentInfo InstrumentData
        {
            get { return m_instrumentData; }
            set { m_instrumentData = value; }
        }

        /// <summary>
        /// Gets or sets the experiment object data.
        /// </summary>
        public classLCMethod LCMethod
        {
            get { return m_method; }
            set { m_method = value; }
        }

        /// <summary>
        /// Gets or sets the list of data downloaded from DMS for this sample
        /// </summary>
        public classDMSData DmsData
        {
            get { return m_DmsData; }
            set { m_DmsData = value; }
        }

        /// <summary>
        /// Gets or sets the sequence number that the sample is run in.
        /// </summary>
        public long SequenceID
        {
            get { return mlong_sequenceNumber; }
            set { mlong_sequenceNumber = value; }
        }

        /// <summary>
        /// Gets or sets the pal data associated with this sample.
        /// </summary>
        public classPalData PAL
        {
            get { return m_palData; }
            set { m_palData = value; }
        }

        /// <summary>
        /// Gets or sets the volume of the sample to inject.
        /// </summary>
        public double Volume
        {
            get { return mdouble_volume; }
            set { mdouble_volume = value; }
        }

        /// <summary>
        /// Gets or sets the column data this sample is/was run on.
        /// </summary>
        public classColumnData ColumnData
        {
            get { return m_columnData; }
            set
            {
                if (m_columnData != null && m_columnData != value)
                {
                    m_columnData.NameChanged -= m_columnData_NameChanged;
                }

                var sameData = (m_columnData == value);

                m_columnData = value;

                if (m_columnData != null && !sameData)
                {
                    m_columnData.NameChanged += m_columnData_NameChanged;
                }
            }
        }

        void m_columnData_NameChanged(object sender, string name, string oldName)
        {
            if (oldName == "")
                return;

            var cartName = "";
            if (DmsData?.CartName != null)
            {
                cartName = DmsData.CartName;
            }
            var cartColumn = DmsData.CartName + "_" + oldName;

            // Make sure we actually have a cart and column name.
            var length = DmsData.CartName.Length + oldName.Length;
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
            get { return mlong_uniqueID; }
            set { mlong_uniqueID = value; }
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
        public override StringDictionary GetPropertyValues()
        {
            //NOTE: This method must be modified if new property representing an object is added
            var TempDict = new StringDictionary();

            // Use reflection to get the name and value for each property and store in a string dictionary
            var classType = GetType();
            var properties = classType.GetProperties();
            foreach (var tempProp in properties)
            {
                switch (tempProp.PropertyType.ToString())
                {
                    case "LcmsNetDataClasses.classDMSData":
                        // Special case - get the DMS data for this object and add properties to string dictionary
                        var dmsDict = DmsData.GetPropertyValues();
                        foreach (DictionaryEntry de in dmsDict)
                        {
                            TempDict.Add("DMS." + de.Key, de.Value.ToString());
                        }
                        break;
                    case "LcmsNetDataClasses.Data.classPalData":
                        // Special case - get the PAL data for this object and add properties to string dictionary
                        var palDict = PAL.GetPropertyValues();
                        foreach (DictionaryEntry de in palDict)
                        {
                            TempDict.Add("PAL." + de.Key, de.Value.ToString());
                        }
                        break;
                    case "LcmsNetDataClasses.Configuration.classColumnData":
                        if (ColumnData != null)
                        {
                            // Special case - get the column data for this object and add properties to string dictionary
                            var colDict = ColumnData.GetPropertyValues();
                            foreach (DictionaryEntry de in colDict)
                            {
                                TempDict.Add("Col." + de.Key, de.Value.ToString());
                            }
                        }
                        break;
                    case "LcmsNetDataClasses.Method.classLCMethod":
                        if (LCMethod != null)
                        {
                            // Special case - get the experiment data for this object and add properties to string dictionary
                            var expDict = LCMethod.GetPropertyValues();
                            foreach (DictionaryEntry de in expDict)
                            {
                                //TODO: Do we need to change the name from exp to LCMethod to be consistent.
                                TempDict.Add("exp." + de.Key, de.Value.ToString());
                            }
                        }
                        else
                        {
                            var method = new classLCMethod();
                            var expDict = method.GetPropertyValues();
                            foreach (DictionaryEntry de in expDict)
                            {
                                //TODO: Do we need to change the name from exp to LCMethod to be consistent.
                                TempDict.Add("exp." + de.Key, de.Value.ToString());
                            }
                        }
                        break;
                    case "LcmsNetDataClasses.classInstrumentInfo":
                        // Special case - get the experiment data for this object and add properties to string dictionary
                        var instDict = InstrumentData.GetPropertyValues();
                        foreach (DictionaryEntry de in instDict)
                        {
                            TempDict.Add("Ins." + de.Key, de.Value.ToString());
                        }
                        break;
                    default:
                        TempDict.Add(tempProp.Name, tempProp.GetValue(this, null).ToString());
                        break;
                }
            }
            //Return the string dictionary
            return TempDict;
        }

        public override void LoadPropertyValues(StringDictionary PropValues)
        {
            //NOTE: This method must be modified if new property representing an object is added
            var baseProps = new StringDictionary();
            var dmsProps = new StringDictionary();
            var palProps = new StringDictionary();
            var colProps = new StringDictionary();
            var expProps = new StringDictionary();
            var instProps = new StringDictionary();

            // Separate the properties into class dictionaries
            foreach (DictionaryEntry testEntry in PropValues)
            {
                var keyName = testEntry.Key.ToString();
                if (keyName.Length < 4)
                {
                    // Property name is too short to be one of the properties that holds a class, so add
                    //      it to the main class dictionary
                    baseProps.Add(keyName, testEntry.Value.ToString());
                }
                // Stuff string dictionaries for each property holding a class, and main class
                switch (keyName.Substring(0, 4))
                {
                    case "dms.":
                        dmsProps.Add(keyName.Substring(4, keyName.Length - 4), testEntry.Value.ToString());
                        break;
                    case "pal.":
                        palProps.Add(keyName.Substring(4, keyName.Length - 4), testEntry.Value.ToString());
                        break;
                    case "col.":
                        colProps.Add(keyName.Substring(4, keyName.Length - 4), testEntry.Value.ToString());
                        break;
                    case "exp.":
                        expProps.Add(keyName.Substring(4, keyName.Length - 4), testEntry.Value.ToString());
                        break;
                    case "ins.":
                        instProps.Add(keyName.Substring(4, keyName.Length - 4), testEntry.Value.ToString());
                        break;
                    default:
                        baseProps.Add(keyName, testEntry.Value.ToString());
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
    }
} // End namespace