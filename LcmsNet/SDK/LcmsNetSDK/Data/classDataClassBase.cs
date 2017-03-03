//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 02/17/2009
//
// Last modified 02/17/2009
//                      - 02/20/2009 (DAC) - Added ICache interface
//*********************************************************************************************************

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using LcmsNetDataClasses.Configuration;

namespace LcmsNetDataClasses
{
    [Serializable]
    public class classDataClassBase : ICacheInterface
    {
        //*********************************************************************************************************
        // Base class for LCMS data storage classes. Contains methods and properties common
        //      to all data storage classes
        //**********************************************************************************************************

        #region "Constants"

        #endregion

        #region "Class variables"

        #endregion

        #region "Events"

        #endregion

        #region "Properties"

        #endregion

        #region "Methods"

        /// <summary>
        /// Gets current values for all the properties in the class in key/value format
        /// </summary>
        /// <returns>String dictionary containing current values of all properties</returns>
        public virtual StringDictionary GetPropertyValues()
        {
            var TempDict = new StringDictionary();
            // Use reflection to get the name and value for each property and store in a string dictionary
            var classType = GetType();
            var properties = classType.GetProperties();
            foreach (var tempProp in properties)
            {
                if (tempProp.PropertyType != typeof (Color))
                {
                    var tempObject = tempProp.GetValue(this, null);
                    if (tempObject == null)
                    {
                        TempDict.Add(tempProp.Name, "");
                    }
                    else
                    {
                        TempDict.Add(tempProp.Name, tempObject.ToString());
                    }
                }
                else
                {
                    var c = (Color) tempProp.GetValue(this, null);
                    TempDict.Add(tempProp.Name, TypeDescriptor.GetConverter(c).ConvertToString(c));
                }
            }
            //Return the string dictionary
            return TempDict;
        }

        /// <summary>
        /// Loads the class properties from a string dictionary
        /// </summary>
        /// <param name="PropValues">String dictionary containing property names and values</param>
        public virtual void LoadPropertyValues(StringDictionary PropValues)
        {
            var classType = GetType();
            var properties = classType.GetProperties();
            foreach (DictionaryEntry currentEntry in PropValues)
            {
                var currentKey = currentEntry.Key.ToString();
                var currentValue = currentEntry.Value.ToString();
                foreach (var tempProp in properties)
                {
                    if (tempProp.Name.ToLower() == currentKey.ToLower())
                    {
                        switch (tempProp.PropertyType.ToString())
                        {
                            case "System.String":
                                tempProp.SetValue(this, currentValue, null);
                                break;
                            case "System.Int32":
                                tempProp.SetValue(this, int.Parse(currentValue), null);
                                break;
                            case "System.Boolean":
                                tempProp.SetValue(this, bool.Parse(currentValue), null);
                                break;
                            case "System.Int64":
                                tempProp.SetValue(this, Int64.Parse(currentValue), null);
                                break;
                            case "System.Double":
                                tempProp.SetValue(this, double.Parse(currentValue), null);
                                break;
                            case "System.Drawing.Color":
                                var c =
                                    (Color) TypeDescriptor.GetConverter(typeof (Color)).ConvertFromString(currentValue);
                                tempProp.SetValue(this, c, null);
                                break;
                            case "LcmsNetDataClasses.Configuration.enumColumnStatus":
                                var tempEnum =
                                    (enumColumnStatus) Enum.Parse(typeof (enumColumnStatus), currentValue);
                                tempProp.SetValue(this, tempEnum, null);
                                break;
                            case "LcmsNetDataClasses.enumSampleRunningStatus":
                                var tempStatus =
                                    (enumSampleRunningStatus) Enum.Parse(typeof (enumSampleRunningStatus), currentValue);
                                tempProp.SetValue(this, tempStatus, null);
                                break;
                            case "System.DateTime":
                                break;
                            default:
                                var tpName = tempProp.PropertyType.ToString();
                                if (tpName.StartsWith("System.Nullable"))
                                {
                                    if (tpName.Contains("System.Int32"))
                                    {
                                        // We're dealing with nullable types here, and the default
                                        // value for those is null, so we shouldn't have to set the
                                        // value to null if parsing doesn't work.
                                        int value;
                                        var worked = int.TryParse(currentValue, out value);
                                        if (worked)
                                            tempProp.SetValue(this, value, null);
                                    }
                                    else if (tpName.Contains("System.DateTime"))
                                    {
                                        DateTime value;
                                        var worked = DateTime.TryParse(currentValue, out value);
                                        if (worked)
                                            tempProp.SetValue(this, value, null);
                                    }
                                    else
                                    {
                                        throw new Exception(
                                            "classDataClassBase.LoadPropertyValues(), Invalid property type specified: " +
                                            tempProp.PropertyType);
                                    }
                                }
                                else
                                {
                                    throw new Exception(
                                        "classDataClassBase.LoadPropertyValues(), Invalid property type specified: " +
                                        tempProp.PropertyType);
                                }
                                break;
                        } // End switch
                    } // End if
                }
            }
        }

        #endregion
    }
}
