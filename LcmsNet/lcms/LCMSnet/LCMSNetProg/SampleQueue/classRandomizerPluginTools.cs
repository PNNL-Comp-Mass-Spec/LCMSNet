//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 03/04/2009
//
// Last modified 03/04/2009
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using LcmsNetDataClasses;
using System.IO;
using System.Reflection;

namespace LcmsNet.SampleQueue
{
    public class classRandomizerPluginTools
    {
        //*********************************************************************************************************
        // Tools for finding and loading randomizer plugins
        //**********************************************************************************************************

        #region "Methods"

        public static Dictionary<string, Type> GetRandomizerPlugins()
        {
            Dictionary<string, Type> RetDict = new Dictionary<string, Type>();

            // Get list of DLL's in plugin folder
            FileInfo fi = new FileInfo(System.Windows.Forms.Application.ExecutablePath);
            string pluginFolder = Path.Combine(fi.DirectoryName, classLCMSSettings.GetParameter(classLCMSSettings.PARAM_PLUGINFOLDER));
            string[] dllFiles = Directory.GetFiles(pluginFolder, "*.dll");
            if (dllFiles.GetLength(0) == 0)
            {
                // No dll's found in folder
                return RetDict;
            }

            // Load each dll and determine if it implmements IRandomizerInterface
            foreach (string dllName in dllFiles)
            {
                //Load the assembly
                Assembly testAssmbly = Assembly.LoadFrom(dllName);
                // Test to determine if any types in assembly implements IRandomizerInterface
                foreach (Type testType in testAssmbly.GetTypes())
                {
                    if (typeof (IRandomizerInterface).IsAssignableFrom(testType) == true)
                    {
                        // This type implements the interface, so get the display name
                        string dispName = GetPluginNameFromAttributes(testType);
                        // Load the display name and type into the return dictionary
                        RetDict.Add(dispName, testType);
                    }
                }
            }
            // Return dictionary
            return RetDict;
        }

        /// <summary>
        /// Gets the plugin name from a randomizer plugin
        /// </summary>
        /// <param name="Plugin">A randomizer plugin type</param>
        /// <returns>The display name for the plugin</returns>
        static string GetPluginNameFromAttributes(Type Plugin)
        {
            bool found = false;
            string pluginName = "";
            // Get all the custom attributes for the type
            object[] attributes = Plugin.GetCustomAttributes(false);
            foreach (object testAttribute in attributes)
            {
                // Determine if custom attribute is the correct type
                classPlugInDisplayNameAttribute pluginAttr = testAttribute as classPlugInDisplayNameAttribute;
                if (pluginAttr != null)
                {
                    pluginName = pluginAttr.ToString();
                    found = true;
                    break;
                }
            }

            // If the plugin name was found, return the name. Otherwise return "No Name"
            if (found)
            {
                return pluginName;
            }
            else
            {
                return "No Name";
            }
        }

        #endregion
    }
} // End namespace