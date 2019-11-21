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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;
using LcmsNetData.Logging;

namespace LcmsNetData.Data
{
    [Serializable]
    public abstract class LcmsNetDataClassBase : ICacheInterface
    {
        //*********************************************************************************************************
        // Base class for LCMS data storage classes. Contains methods and properties common
        //      to all data storage classes
        //**********************************************************************************************************

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
                        tempProp.SetValue(this, ConvertToType(currentValue, tempProp.PropertyType));
                    }
                    catch (Exception ex)
                    {
                        var msg = "Error parsing property for key " + currentKey + ": " + tempProp + "; " + ex.Message;
                        Console.WriteLine(msg);
                        ApplicationLogger.LogError(0, msg);
                        // Continue on to the next property
                    }
                }
            }
        }

        private object ConvertToType(string value, Type targetType)
        {
            if (targetType.IsEnum && !string.IsNullOrWhiteSpace(value))
            {
                return Enum.Parse(targetType, value);
            }
            if (targetType == typeof(string))
            {
                return value;
            }
            if (targetType.IsPrimitive)
            {
                return Convert.ChangeType(value, targetType);
            }
            if (targetType == typeof(DateTime))
            {
                var tempValue = (DateTime)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                return TimeZoneInfo.ConvertTimeToUtc(tempValue);
            }
            if (targetType == typeof(Color))
            {
                var convertFromString = TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(value);
                if (convertFromString != null)
                {
                    return (Color)convertFromString;
                }

                return new Color();
            }
            if (targetType.ToString().StartsWith("System.Nullable"))
            {
                var wrappedType = targetType.GenericTypeArguments[0];
                if (value == null || string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                // We're dealing with nullable types here, and the default
                // value for those is null, so we shouldn't have to set the
                // value to null if parsing doesn't work.
                try
                {
                    return ConvertToType(value, wrappedType);
                }
                catch (InvalidCastException) {}
                catch (FormatException) {}
                catch (OverflowException) {}

                return null;
            }

            throw new Exception("LcmsNetDataClassBase.LoadPropertyValues(), Invalid property type specified: " + targetType);
        }
    }
}
