//*********************************************************************************************************
// Written by Christopher Walters for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2014, Battelle Memorial Institute
// Created 09/11/2014
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using LcmsNetSDK.Data;

namespace LcmsNetSDK
{
    /// <summary>
    /// Class that handles loading of DmsTools extensions using the Managed Extensibility Framework (MEF)
    /// </summary>
    /// <remarks>
    /// Looks for DLLs in C:\Users\Username\AppData\Roaming\LCMSNet\dmsExtensions
    /// that have classes marked with attribute Export(typeof(IDmsTools)) or attribute Export(typeof(IDMSValidator))
    /// Typically classes classDBTools and classDMSSampleValidator in LcmsNetDmsTools.dll have those attributes
    /// </remarks>
    [Obsolete("Deprecated; use direct references to classDBTools and classDMSSampleValidator")]
    public class DMSToolsManager
    {
        private static DMSToolsManager m_instance;

        /// <summary>
        /// Reference to metadata of our selectedTools.
        /// </summary>
        private IDmsMetaData m_DMSMetaData;

        private IDmsTools m_SelectedDMSTools;

        /// <summary>
        /// Constructor
        /// </summary>
        private DMSToolsManager()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof (DMSToolsManager).Assembly));

            // Construct the catalog path, for example
            // C:\Users\d3l243\AppData\Roaming\LCMSNet\dmsExtensions
            var catalogPath = GetDMSExtensionsDllFolderPathForUser();

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

            // Find DLLs that have classes marked with attribute Export(typeof(IDmsTools)) or attribute Export(typeof(IDMSValidator))
            // Typically classes classDBTools and classDMSSampleValidator in LcmsNetDmsTools.dll have those attributes
            // For each found DLL, update auto-properties DmsTools and Validators to include instances of the corresponding class
            var mefCompositionContainer = new CompositionContainer(catalog);
            mefCompositionContainer.ComposeParts(this);

            if (ToolCount == 0)
            {
                LCMSSettings.SetParameter(LCMSSettings.PARAM_DMSTOOL, string.Empty);
            }
        }

        public static string GetDMSExtensionsDllFolderPathForUser()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dmsExtensionsDllFolderPath = Path.Combine(appDataFolder, "LCMSNet", "dmsExtensions");
            return dmsExtensionsDllFolderPath;
        }

        /// <summary>
        /// Get the selected Dms Tool, if none has been selected, returns the first available tool. If no tool is available, throws
        /// </summary>
        public IDmsTools SelectedTool
        {
            get
            {
                // Check that we have a tool available, if not, report via exception otherwise, return the selected, or first available tool.
                if (m_SelectedDMSTools != null)
                    return m_SelectedDMSTools;

                if (m_SelectedDMSTools == null && DmsTools.Count() != 0)
                {
                    var lastSelectedTool = LCMSSettings.GetParameter(LCMSSettings.PARAM_DMSTOOL);
                    if (!string.IsNullOrWhiteSpace(lastSelectedTool))
                    {
                        // lastSelectedTool is of the form: PrismDMSTools-1.0

                        var toolTokens = lastSelectedTool.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                        var query = (from item in DmsTools
                                     where item.Metadata.Name == toolTokens[0] && item.Metadata.Version == toolTokens[1]
                                     select item).ToList();

                        if (query.Count > 0)
                        {
                            m_SelectedDMSTools = query.First().Value;
                            m_DMSMetaData = query.First().Metadata;
                            return m_SelectedDMSTools;
                        }
                    }

                    // Just grab the first off the list
                    var firstTool = DmsTools.First();
                    m_SelectedDMSTools = firstTool.Value;
                    m_DMSMetaData = firstTool.Metadata;
                    LCMSSettings.SetParameter(LCMSSettings.PARAM_DMSTOOL, firstTool.Metadata.Name + "-" + firstTool.Metadata.Version);

                    return m_SelectedDMSTools;
                }

                var folderPath = GetDMSExtensionsDllFolderPathForUser();
                throw new InvalidOperationException(string.Format("No DMS tools available; assure that {0} has file LcmsNetDmsTools.dll", folderPath));
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
                                x.Metadata.RelatedToolName == m_DMSMetaData.Name &&
                                Convert.ToDouble(m_DMSMetaData.Version) >=
                                Convert.ToDouble(x.Metadata.RequiredDMSToolVersion)).Value;
                }
                catch (InvalidCastException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Contains the Managed Extensibility Framework (MEF) references to DmsTools
        /// </summary>
        [ImportMany]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local (the setter is used when we run mefCompositionContainer.ComposeParts)
        private IEnumerable<Lazy<IDmsTools, IDmsMetaData>> DmsTools { get; set; }

        /// <summary>
        /// Contains the MEF references to the validators
        /// </summary>
        [ImportMany]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local (the setter is used when we run mefCompositionContainer.ComposeParts)
        private IEnumerable<Lazy<IDMSValidator, IDMSValidatorMetaData>> Validators { get; set; }

        /// <summary>
        /// Number of Dms Tools loaded.
        /// </summary>
        public int ToolCount => DmsTools.Count();

        /// <summary>
        /// Get a reference to the classDMSTools instance.
        /// </summary>
        public static DMSToolsManager Instance => m_instance ?? (m_instance = new DMSToolsManager());

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
                        m_SelectedDMSTools = tool.Value;
                        LCMSSettings.SetParameter(LCMSSettings.PARAM_DMSTOOL, toolName + "-" + toolVersion);
                    }
                }
            }
            if (!toolSelected && !toolFound)
            {
                // Tool name was not found.
                throw new ArgumentException("Tool " + toolName + " unavailable for selection", nameof(toolName));
            }
            if (!toolSelected)
            {
                // Tool was not found with specified version.
                throw new ArgumentException(
                    "Tool version " + toolVersion + " unavailable for selection, version not found.", nameof(toolVersion));
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