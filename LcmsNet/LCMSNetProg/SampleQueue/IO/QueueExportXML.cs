//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 03/11/2009
//
// Updates
// - 04/08/2009 (DAC) - Added output to logging function when export complete
// - 04/09/2009 (DAC) - Added output to log file on exception
// - 12/01/2009 (DAC) - Modified to accomodate change of vial from string to int
//
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using LcmsNetData.Logging;
using LcmsNetSDK.Data;

namespace LcmsNet.SampleQueue.IO
{
    /// <summary>
    /// Class for generating XML export files
    /// </summary>
    class QueueExportXML : ISampleQueueWriter
    {
        #region "Class variables"

        XmlDocument m_ExportDoc;

        #endregion

        #region "Methods"

        /// <summary>
        /// Exports a queue as an XML file for use by LCMS-old
        /// </summary>
        /// <param name="InpSamples">List to be exported</param>
        /// <param name="ExportFileNamePath">Full name and path of file to export</param>
        public void WriteSamples(string ExportFileNamePath, List<SampleData> InpSamples)
        {
            // Verify there are samples to export
            if (!InpSamples.Any())
            {
                // No data to export found in list
                throw new DataExportException("No data to export", new Exception());
            }

            // Create and initialize the document object
            m_ExportDoc = new XmlDocument();
            var docDeclaration = m_ExportDoc.CreateXmlDeclaration("1.0", null, null);
            m_ExportDoc.AppendChild(docDeclaration);

            // Add MetaData (root) element
            var rootElement = m_ExportDoc.CreateElement("MetaData");
            m_ExportDoc.AppendChild(rootElement);

            // Add QueueSettings node
            var queueSettingsElement = AddElementNoAttributes("QueueSettings", rootElement);

            // Add ItemCount node
            var itemCount = AddElementWithTypeAttribute("ItemCount", queueSettingsElement, "Integer",
                InpSamples.Count().ToString());

            // Loop through the queue and add nodes for each queued sample
            var itemCntr = 1;
            foreach (var currentSample in InpSamples)
            {
                var itemNum = "Item" + itemCntr.ToString("0000");
                var currentElement = AddElementNoAttributes(itemNum, queueSettingsElement);
                AddOneSample(currentSample, currentElement);
                itemCntr++;
            }

            // Add Auto Refill checkbox node. Return value not needed because node has no children
            AddElementWithTypeAttribute("chkAutoRefill", queueSettingsElement, "Integer", "1");

            // Add Auto Increment node. Return value not needed because node has no children
            AddElementWithTypeAttribute("mnuAutoIncrement", queueSettingsElement, "Boolean", "True");

            // Add Well Plate Graphic node. Return value not needed because node has no children
            AddElementWithTypeAttribute("mnuWellPlateGraphic", queueSettingsElement, "Boolean", "False");

            // Write the XML object to the output file and exit
            SaveQueue(m_ExportDoc, ExportFileNamePath);

            // Notify user
            ApplicationLogger.LogMessage(0, "Export complete");
        }

        /// <summary>
        /// Converts a single SampleData object to XML
        /// </summary>
        /// <param name="InpSample">SampleData object to convert</param>
        /// <param name="ParentElement">XML element that will be the parent of this element</param>
        void AddOneSample(SampleData InpSample, XmlElement ParentElement)
        {
            // Description element
            var descElement = AddElementWithTypeAttribute("Description", ParentElement, "String",
                InpSample.DmsData.DatasetName);

            // Selection element (PAL data)
            var selectionElement = AddElementNoAttributes("Selection", ParentElement);
            // No return type needed, since these elements don't have any children
            AddElementWithTypeAttribute("Method", selectionElement, "String", InpSample.PAL.Method);
            AddElementWithTypeAttribute("Tray", selectionElement, "String", InpSample.PAL.PALTray);
            AddElementWithTypeAttribute("Vial", selectionElement, "Integer", InpSample.PAL.Well.ToString());
            AddElementWithTypeAttribute("Volume", selectionElement, "Integer", InpSample.Volume.ToString());

            // Separation element. These will be blank or zero for now
            var separationElement = AddElementNoAttributes("Separation", ParentElement);
            // No return type needed, since these elements don't have any children
            var name = "";
            if (InpSample.LCMethod != null)
            {
                name = InpSample.LCMethod.Name;
            }
            AddElementWithTypeAttribute("Method", separationElement, "String", name);

            // Acquisition element. These will be blank or zero for now
            var acquisitionElement = AddElementNoAttributes("Acquisition", ParentElement);
            // No return type needed, since these elements don't have any children
            AddElementWithTypeAttribute("Method", acquisitionElement, "String", InpSample.InstrumentMethod);

            // DMS element
            var dmsElement = AddElementNoAttributes("DMS", ParentElement);
            // No return type needed, since these elements don't have any children
            AddElementWithTypeAttribute("RequestNumber", dmsElement, "String", InpSample.DmsData.RequestID.ToString());
            AddElementWithTypeAttribute("Comment", dmsElement, "String", InpSample.DmsData.Comment);
            AddElementWithTypeAttribute("DatasetType", dmsElement, "String", InpSample.DmsData.DatasetType);
            AddElementWithTypeAttribute("Experiment", dmsElement, "String", InpSample.DmsData.Experiment);
            AddElementWithTypeAttribute("EMSLProposalID", dmsElement, "String", InpSample.DmsData.EMSLProposalID);
            AddElementWithTypeAttribute("EMSLUsageType", dmsElement, "String", InpSample.DmsData.EMSLUsageType);
            AddElementWithTypeAttribute("EMSLUser", dmsElement, "String", InpSample.DmsData.UserList);
        }

        /// <summary>
        /// Saves the XML document to the specified file
        /// </summary>
        /// <param name="InpDoc">XML document to save</param>
        /// <param name="FileNamePath">Full name and path where document will be saved</param>
        void SaveQueue(XmlDocument InpDoc, string FileNamePath)
        {
            try
            {
                var outputFile = new FileStream(FileNamePath, FileMode.Create, FileAccess.Write);
                InpDoc.Save(outputFile);
                outputFile.Close();
            }
            catch (Exception ex)
            {
                var ErrMsg = "Exception saving file " + FileNamePath + ": " + ex.Message;
                ApplicationLogger.LogError(0, ErrMsg, ex);
                throw new DataExportException(ErrMsg, ex);
            }
        }

        /// <summary>
        /// Adds an XML element with no attributes to an XML document
        /// </summary>
        /// <param name="ElementName">Name of the element to add</param>
        /// <param name="Parent">XML element that will be the parent of this element</param>
        /// <returns>XMLElement that was added to document</returns>
        XmlElement AddElementNoAttributes(string ElementName, XmlElement Parent)
        {
            var newElement = m_ExportDoc.CreateElement(ElementName);
            Parent.AppendChild(newElement);
            return newElement;
        }

        /// <summary>
        /// Adds an XML element with an element type and value to an XML document
        /// </summary>
        /// <param name="ElementName">Name of element to add</param>
        /// <param name="Parent">XML element that will be the parent of this element</param>
        /// <param name="TypeName">Data type of element</param>
        /// <param name="ElemValue">String representing value of element</param>
        /// <returns>XML element that was added to document</returns>
        XmlElement AddElementWithTypeAttribute(string ElementName, XmlElement Parent, string TypeName, string ElemValue)
        {
            var newElement = AddElementNoAttributes(ElementName, Parent);
            var newAttribute = m_ExportDoc.CreateAttribute("type");
            newAttribute.Value = TypeName;
            newElement.Attributes.Append(newAttribute);
            newElement.InnerText = ElemValue;
            return newElement;
        }

        #endregion
    }
}