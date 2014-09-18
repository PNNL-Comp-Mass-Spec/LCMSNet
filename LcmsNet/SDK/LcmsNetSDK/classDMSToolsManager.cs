//*********************************************************************************************************
// Written by Christopher Walters for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2014, Battelle Memorial Institute
// Created 09/11/2014
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;

using LcmsNetDataClasses;

namespace LcmsNetSDK
{
    /// <summary>
    /// Class that handles loading of DmsTools extensions.
    /// </summary>
    public class classDMSToolsManager
    {
        private CompositionContainer mmef_compositionContainer;
        private IDmsTools midmstools_selectedTools;
        private static classDMSToolsManager m_instance;

        /// <summary>
        /// Directory
        /// </summary>
        private const string mstring_catalogDirectory = "\\LCMSNet\\dmsExtensions";

        private classDMSToolsManager()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(classDMSToolsManager).Assembly));
            string catalogPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + mstring_catalogDirectory;
            catalog.Catalogs.Add(new DirectoryCatalog(catalogPath));
            mmef_compositionContainer = new CompositionContainer(catalog);
            mmef_compositionContainer.ComposeParts(this);
            if (ToolCount == 0)
            {
                classLCMSSettings.SetParameter("DMSTool", string.Empty);
            }
        }
        /// <summary>
        /// Select the Dms tool for future use
        /// </summary>
        /// <param name="toolName">The name of the Dms tool to use</param>
        /// <param name="toolVersion">The version of the Dms tool to use</param>
        public void SelectTool(string toolName, string toolVersion)
        {
            bool toolSelected = false;
            bool toolFound = false;
            foreach (Lazy<IDmsTools, IDmsMetaData> tool in DmsTools)
            {
                if (tool.Metadata.Name == toolName)
                {
                    toolFound = true;
                    if (tool.Metadata.Version == toolVersion)
                    {
                        toolSelected = true;
                        midmstools_selectedTools = tool.Value;
                        classLCMSSettings.SetParameter("DMSTool", toolName + "-" + toolVersion);
                    }
                }
            }
            if (!toolSelected && !toolFound)
            {
                // Tool name was not found.
                throw new ArgumentException("Tool " + toolName + " unavailable for selection", "toolName");
            }
            else if (!toolSelected)
            {
                // Tool was not found with specified version.
                throw new ArgumentException("Tool version " + toolVersion + " unavailable for selection, version not found.", "toolVersion");
            }                        
        }

        /// <summary>
        /// List the name and version of all dms tools detected. 
        /// </summary>
        /// <returns>a list of strings containing name and version for each detected IDmsTools extension.</returns>
        public List<string> ListTools()
        {
            List<string> tools = new List<string>();
            foreach (Lazy<IDmsTools, IDmsMetaData> tool in DmsTools)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(tool.Metadata.Name);
                sb.Append("-");
                sb.Append(tool.Metadata.Version);
                tools.Add(sb.ToString());
            }
            return tools;
        }

        /// <summary>
        /// Get the selected Dms Tool, if none has been selected, returns the first available tool. If no tool is available, throws
        /// </summary>
        public IDmsTools SelectedTool
        {
            get
            {
                // Check that we have a tool available, if not, report via exception otherwise, return the selected, or first available tool.
                if (midmstools_selectedTools == null && DmsTools.Count() != 0)
                {
                    string lastSelectedTool = classLCMSSettings.GetParameter("DMSTool");
                    string[] toolTokens = lastSelectedTool.Split(new char[] { '-' });
                    if (lastSelectedTool != string.Empty)
                    {
                            midmstools_selectedTools = DmsTools.Single(x => x.Metadata.Name == toolTokens[0] && x.Metadata.Version == toolTokens[1]).Value;                        
                    }
                    else
                    {
                        midmstools_selectedTools = DmsTools.First().Value; // Just grab the first off the list.
                        classLCMSSettings.SetParameter("DMSTool", DmsTools.First().Metadata.Name + "-" + DmsTools.First().Metadata.Version);
                    }
                }
                else
                {
                    throw new InvalidOperationException("No dms tools available");
                }
                return midmstools_selectedTools;
            }
        }

        /// <summary>
        /// Contains the MEF references to DmsTools
        /// </summary>
        [ImportMany]
        private IEnumerable<Lazy<IDmsTools, IDmsMetaData>> DmsTools
        {
            get;
            set;
        }

        /// <summary>
        /// Number of Dms Tools loaded.
        /// </summary>
        public int ToolCount
        {
            get
            {
                return DmsTools.Count();
            }
        }

        /// <summary>
        /// Get a reference to the classDMSTools instance.
        /// </summary>
        public static classDMSToolsManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new classDMSToolsManager();
                }
                return m_instance;
            }
        }
    }
}