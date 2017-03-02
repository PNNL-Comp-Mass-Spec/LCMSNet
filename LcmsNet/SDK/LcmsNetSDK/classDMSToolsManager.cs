//*********************************************************************************************************
// Written by Christopher Walters for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2014, Battelle Memorial Institute
// Created 09/11/2014
// Last Modified On: 9/25/2014 By Christopher Walters
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Data;

namespace LcmsNetSDK
{
    /// <summary>
    /// Class that handles loading of DmsTools extensions.
    /// </summary>
    public class classDMSToolsManager
    {
        private static classDMSToolsManager m_instance;

        /// <summary>
        /// reference to metadata of our selectedTools.
        /// </summary>
        private IDmsMetaData mdms_metadata;

        private IDmsTools midmstools_selectedTools;
        private readonly CompositionContainer mmef_compositionContainer;

        private classDMSToolsManager()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof (classDMSToolsManager).Assembly));

            var catalogPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            catalogPath = Path.Combine(catalogPath, "LCMSNet", "dmsExtensions");

            try
            {
                var catalogFolder = new DirectoryInfo(catalogPath);
                if (!catalogFolder.Exists)
                    catalogFolder.Create();
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating folder " + catalogPath, ex);
            }

            catalog.Catalogs.Add(new DirectoryCatalog(catalogPath));
            mmef_compositionContainer = new CompositionContainer(catalog);
            mmef_compositionContainer.ComposeParts(this);
            if (ToolCount == 0)
            {
                classLCMSSettings.SetParameter(classLCMSSettings.PARAM_DMSTOOL, string.Empty);
            }
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
                    var lastSelectedTool = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_DMSTOOL);
                    var toolTokens = lastSelectedTool.Split(new[] {'-'}, StringSplitOptions.RemoveEmptyEntries);
                    if (!string.IsNullOrWhiteSpace(lastSelectedTool))
                    {
                        midmstools_selectedTools =
                            DmsTools.Single(x => x.Metadata.Name == toolTokens[0] && x.Metadata.Version == toolTokens[1])
                                .Value;
                        mdms_metadata =
                            DmsTools.Single(x => x.Metadata.Name == toolTokens[0] && x.Metadata.Version == toolTokens[1])
                                .Metadata;
                    }
                    else
                    {
                        midmstools_selectedTools = DmsTools.First().Value; // Just grab the first off the list.
                        mdms_metadata = DmsTools.First().Metadata;
                        classLCMSSettings.SetParameter(classLCMSSettings.PARAM_DMSTOOL,
                            DmsTools.First().Metadata.Name + "-" + DmsTools.First().Metadata.Version);
                    }
                }
                else if (DmsTools.Count() == 0)
                {
                    throw new InvalidOperationException("No dms tools available");
                }
                return midmstools_selectedTools;
            }
        }

        /// <summary>
        /// Find the first validator available that will work for the selected DMS tool and its version.
        /// </summary>
        public IDMSValidator Validator
        {
            get
            {
                try
                {
                    return
                        Validators.Single(
                            x =>
                                x.Metadata.RelatedToolName == mdms_metadata.Name &&
                                Convert.ToDouble(mdms_metadata.Version) >=
                                Convert.ToDouble(x.Metadata.RequiredDMSToolVersion)).Value;
                }
                catch (InvalidCastException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Contains the MEF references to DmsTools
        /// </summary>
        [ImportMany]
        private IEnumerable<Lazy<IDmsTools, IDmsMetaData>> DmsTools { get; set; }

        /// <summary>
        /// Contains the MEF references to the validators
        /// </summary>
        [ImportMany]
        private IEnumerable<Lazy<IDMSValidator, IDMSValidatorMetaData>> Validators { get; set; }

        /// <summary>
        /// Number of Dms Tools loaded.
        /// </summary>
        public int ToolCount => DmsTools.Count();

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

        /// <summary>
        /// Select the Dms tool for future use
        /// </summary>
        /// <param name="toolName">The name of the Dms tool to use</param>
        /// <param name="toolVersion">The version of the Dms tool to use</param>
        public void SelectTool(string toolName, string toolVersion)
        {
            var toolSelected = false;
            var toolFound = false;
            foreach (var tool in DmsTools)
            {
                if (tool.Metadata.Name == toolName)
                {
                    toolFound = true;
                    if (tool.Metadata.Version == toolVersion)
                    {
                        toolSelected = true;
                        midmstools_selectedTools = tool.Value;
                        classLCMSSettings.SetParameter(classLCMSSettings.PARAM_DMSTOOL, toolName + "-" + toolVersion);
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
                throw new ArgumentException(
                    "Tool version " + toolVersion + " unavailable for selection, version not found.", "toolVersion");
            }
        }

        /// <summary>
        /// List the name and version of all dms tools detected.
        /// </summary>
        /// <returns>a list of strings containing name and version for each detected IDmsTools extension.</returns>
        public List<string> ListTools()
        {
            var tools = new List<string>();
            foreach (var tool in DmsTools)
            {
                var sb = new StringBuilder();
                sb.Append(tool.Metadata.Name);
                sb.Append("-");
                sb.Append(tool.Metadata.Version);
                tools.Add(sb.ToString());
            }
            return tools;
        }
    }
}