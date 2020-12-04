﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LcmsNetSDK;
using LcmsNetSDK.Devices;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Tools for finding and loading randomizer plugins
    /// </summary>
    public class RandomizerPluginTools
    {
        #region "Methods"

        public static Dictionary<string, Type> GetRandomizerPlugins()
        {
            var RetDict = new Dictionary<string, Type>();

            // Get list of DLL's in plugin folder
            var fi = new FileInfo(Assembly.GetEntryAssembly().Location);
            var pluginFolder = Path.Combine(fi.DirectoryName, DeviceManager.CONST_DEVICE_PLUGIN_PATH);
            var dllFiles = Directory.GetFiles(pluginFolder, "*.dll");
            if (dllFiles.GetLength(0) == 0)
            {
                // No dll's found in folder
                return RetDict;
            }

            // Load each dll and determine if it implements IRandomizerInterface
            foreach (var dllName in dllFiles)
            {
                //Load the assembly
                var testAssmbly = Assembly.LoadFrom(dllName);
                // Test to determine if any types in assembly implements IRandomizerInterface
                foreach (var testType in testAssmbly.GetTypes())
                {
                    if (typeof (IRandomizerInterface).IsAssignableFrom(testType))
                    {
                        // This type implements the interface, so get the display name
                        var dispName = GetPluginNameFromAttributes(testType);
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
            var found = false;
            var pluginName = "";
            // Get all the custom attributes for the type
            var attributes = Plugin.GetCustomAttributes(false);
            foreach (var testAttribute in attributes)
            {
                // Determine if custom attribute is the correct type
                var pluginAttr = testAttribute as PlugInDisplayNameAttribute;
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
            return "No Name";
        }

        #endregion
    }
}