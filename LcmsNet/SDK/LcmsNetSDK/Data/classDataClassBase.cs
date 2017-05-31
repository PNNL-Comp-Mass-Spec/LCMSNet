//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 02/17/2009
//
// Updates
// - 02/20/2009 (DAC) - Added ICache interface
//*********************************************************************************************************

using System;
using System.ComponentModel;
using System.Drawing;
using LcmsNetDataClasses.Configuration;
using System.Collections.Generic;
using LcmsNetDataClasses.Logging;

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
        public virtual Dictionary<string, string> GetPropertyValues()
        {
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

                if (property.PropertyType != typeof(Color))
                {
                    var tempObject = property.GetValue(this, null);
                    if (tempObject == null)
                    {
                        newDictionary.Add(property.Name, "");
                    }
                    else
                    {
                        newDictionary.Add(property.Name, tempObject.ToString());
                    }
                }
                else
                {
                    var c = (Color)property.GetValue(this, null);
                    newDictionary.Add(property.Name, TypeDescriptor.GetConverter(c).ConvertToString(c));
                }
            }

            return newDictionary;
        }

        /// <summary>
        /// Loads the class properties from a string dictionary
        /// </summary>
        /// <param name="propValues">String dictionary containing property names and values</param>
        public virtual void LoadPropertyValues(Dictionary<string, string> propValues)
        {
            var classType = GetType();
            var properties = classType.GetProperties();
            foreach (var currentEntry in propValues)
            {
                var currentKey = currentEntry.Key;
                var currentValue = currentEntry.Value;

                if (string.IsNullOrWhiteSpace(currentKey))
                {
                    Console.WriteLine("Ignoring empty key");
                    continue;
                }

                foreach (var tempProp in properties)
                {
                    if (!string.Equals(tempProp.Name, currentKey, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    // Ignore flagged properties
                    if (Attribute.IsDefined(tempProp, typeof(NotStoredPropertyAttribute)))
                    {
                        continue;
                    }

                    try
                    {

                        switch (tempProp.PropertyType.ToString())
                        {
                            case "System.String":
                                tempProp.SetValue(this, currentValue, null);
                                break;
                            case "System.Int32":
                                if (!string.IsNullOrWhiteSpace(currentValue))
                                    tempProp.SetValue(this, int.Parse(currentValue), null);
                                break;
                            case "System.Boolean":
                                if (!string.IsNullOrWhiteSpace(currentValue))
                                    tempProp.SetValue(this, bool.Parse(currentValue), null);
                                break;
                            case "System.Int64":
                                if (!string.IsNullOrWhiteSpace(currentValue))
                                    tempProp.SetValue(this, long.Parse(currentValue), null);
                                break;
                            case "System.Double":
                                if (!string.IsNullOrWhiteSpace(currentValue))
                                    tempProp.SetValue(this, double.Parse(currentValue), null);
                                break;
                            case "System.Drawing.Color":
                                var convertFromString = TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(currentValue);
                                if (convertFromString != null)
                                {
                                    var c = (Color)convertFromString;
                                    tempProp.SetValue(this, c, null);
                                }
                                break;
                            case "LcmsNetDataClasses.Configuration.enumColumnStatus":
                                if (!string.IsNullOrWhiteSpace(currentValue))
                                {
                                    var tempEnum =
                                        (enumColumnStatus)Enum.Parse(typeof(enumColumnStatus), currentValue);
                                    tempProp.SetValue(this, tempEnum, null);
                                }
                                break;
                            case "LcmsNetDataClasses.enumSampleRunningStatus":
                                if (!string.IsNullOrWhiteSpace(currentValue))
                                {
                                    var tempStatus =
                                        (enumSampleRunningStatus)Enum.Parse(typeof(enumSampleRunningStatus), currentValue);
                                    tempProp.SetValue(this, tempStatus, null);
                                }
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
                        }

                    }
                    catch (Exception ex)
                    {
                        var msg = "Error parsing property for key " + currentKey + ": " + tempProp + "; " + ex.Message;
                        Console.WriteLine(msg);
                        classApplicationLogger.LogError(0, msg);
                        // Continue on to the next property
                    }
                }
            }
        }

        #endregion
    }
}
