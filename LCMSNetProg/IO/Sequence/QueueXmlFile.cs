using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using LcmsNet.Data;
using LcmsNet.SampleQueue;
using LcmsNetSDK.Logging;

namespace LcmsNet.IO.Sequence
{
    /// <summary>
    /// Imports/Exports an XML file into/from LCMSNet
    /// </summary>
    internal class QueueXmlFile : ISampleQueueReader, ISampleQueueWriter
    {
        private XmlDocument document;

        /// <summary>
        /// Reads the XML file into a list
        /// </summary>
        /// <param name="path">Name and path of file to import</param>
        /// <returns>List containing samples read from XML file</returns>
        public List<SampleData> ReadSamples(string path)
        {
            var returnList = new List<SampleData>();

            // Verify input file exists
            if (!File.Exists(path))
            {
                var errMsg = "Import file " + path + " not found";
                ApplicationLogger.LogMessage(0, errMsg);
                return returnList;
            }

            // Open the file
            var doc = new XmlDocument();
            try
            {
                doc.Load(path);
            }
            catch (Exception ex)
            {
                var errMsg = "Exception loading XML file " + path;
                ApplicationLogger.LogError(0, errMsg, ex);
                throw new DataImportException(errMsg, ex);
            }

            // Get all the nodes under QueueSettings node
            var nodeList = doc.SelectNodes("//QueueSettings/*");

            // If no nodes found, report and exit
            if (nodeList == null || nodeList.Count < 1)
            {
                var errMsg = "No sample data found for import in file " + path;
                ApplicationLogger.LogMessage(0, errMsg);
                return returnList;
            }

            // Get the data for each sample and add it to the return list
            foreach (XmlNode currentNode in nodeList)
            {
                if (currentNode.Name.StartsWith("Item") && !currentNode.Name.Equals("ItemCount"))
                {
                    try
                    {
                        var newSample = ConvertXMLNodeToSample(currentNode);
                        returnList.Add(newSample);
                    }
                    catch (Exception ex)
                    {
                        var ErrMsg = "Exception converting XML item node to sample " + currentNode.Name;
                        ApplicationLogger.LogError(0, ErrMsg, ex);
                        throw new DataImportException(ErrMsg, ex);
                    }
                }
            }

            return returnList;
        }

        /// <summary>
        /// Converts an individual XML node into a sampledata object
        /// </summary>
        /// <param name="itemNode">XML node containing data for 1 sample</param>
        /// <returns>SampleData object containing data from the XML node</returns>
        private SampleData ConvertXMLNodeToSample(XmlNode itemNode)
        {
            var retData = new SampleData(false);

            // Description (DMS.Name)
            var tempStr = GetNodeValue(itemNode, "Description");
            // Value is mandatory for this field, so check for it
            if (tempStr != "")
            {
                retData.Name = tempStr;
            }
            else
            {
                ApplicationLogger.LogMessage(0, "Description field empty or missing. Import cannot be performed");
                return null;
            }

            // Selection Method (PAL.Method)
            retData.PAL.Method = GetNodeValue(itemNode, "Selection/Method");

            // Tray (PAL.Tray) (aka wellplate)
            retData.PAL.PALTray = GetNodeValue(itemNode, "Selection/Tray");

            // Vial (PAL.Vial) (aka well)
            retData.PAL.Well = (int)ConvertNullToDouble(GetNodeValue(itemNode, "Selection/Vial"));

            // Volume (Volume)
            retData.Volume = ConvertNullToDouble(GetNodeValue(itemNode, "Selection/Volume"));

            // Separation Method (Experiment.ExperimentName)
            var methodName = GetNodeValue(itemNode, "Separation/Method");
            retData.LCMethodName = methodName;

            // Acquisition Method (InstrumentMethod)
            retData.InstrumentMethod = GetNodeValue(itemNode, "Acquisition/Method");

            // DMS RequestNumber (DMSData.RequestID)
            retData.DmsRequestId = (int)ConvertNullToDouble(GetNodeValue(itemNode, "DMS/RequestNumber"));

            // It's all in, so return
            return retData;
        }

        /// <summary>
        /// Utility method to convert a null or empty string value in the XML file to a double
        /// </summary>
        /// <param name="value">String from XML parser</param>
        /// <returns>If input string is empty or null, returns 0.0. Otherwise returns input string converted to double</returns>
        private double ConvertNullToDouble(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }
            return double.TryParse(value, out double number) ? number : 0;
        }

        private string GetNodeValue(XmlNode itemNode, string nodeName)
        {
            var valueNode = itemNode.SelectSingleNode(nodeName);

            var value = valueNode?.InnerText;

            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            return value;
        }

        /// <summary>
        /// Exports a queue as an XML file for use by LCMS-old
        /// </summary>
        /// <param name="inputSamples">List to be exported</param>
        /// <param name="exportFileNamePath">Full name and path of file to export</param>
        public void WriteSamples(string exportFileNamePath, List<SampleData> inputSamples)
        {
            // Verify there are samples to export
            if (!inputSamples.Any())
            {
                // No data to export found in list
                throw new DataExportException("No data to export", new Exception());
            }

            // Create and initialize the document object
            document = new XmlDocument();
            var docDeclaration = document.CreateXmlDeclaration("1.0", null, null);
            document.AppendChild(docDeclaration);

            // Add MetaData (root) element
            var rootElement = document.CreateElement("MetaData");
            document.AppendChild(rootElement);

            // Add QueueSettings node
            var queueSettingsElement = AddElementNoAttributes("QueueSettings", rootElement);

            // Add ItemCount node
            var itemCount = AddElementWithTypeAttribute("ItemCount", queueSettingsElement, "Integer",
                inputSamples.Count.ToString());

            // Loop through the queue and add nodes for each queued sample
            var itemCounter = 1;
            foreach (var currentSample in inputSamples)
            {
                var itemNum = "Item" + itemCounter.ToString("0000");
                var currentElement = AddElementNoAttributes(itemNum, queueSettingsElement);
                AddOneSample(currentSample, currentElement);
                itemCounter++;
            }

            // Add Auto Refill checkbox node. Return value not needed because node has no children
            AddElementWithTypeAttribute("chkAutoRefill", queueSettingsElement, "Integer", "1");

            // Add Auto Increment node. Return value not needed because node has no children
            AddElementWithTypeAttribute("mnuAutoIncrement", queueSettingsElement, "Boolean", "True");

            // Add Well Plate Graphic node. Return value not needed because node has no children
            AddElementWithTypeAttribute("mnuWellPlateGraphic", queueSettingsElement, "Boolean", "False");

            // Write the XML object to the output file and exit
            SaveQueue(document, exportFileNamePath);

            // Notify user
            ApplicationLogger.LogMessage(0, "Export complete");
        }

        /// <summary>
        /// Converts a single SampleData object to XML
        /// </summary>
        /// <param name="inputSample">SampleData object to convert</param>
        /// <param name="parentElement">XML element that will be the parent of this element</param>
        private void AddOneSample(SampleData inputSample, XmlElement parentElement)
        {
            // Description element
            var descElement = AddElementWithTypeAttribute("Description", parentElement, "String",
                inputSample.Name);

            // Selection element (PAL data)
            var selectionElement = AddElementNoAttributes("Selection", parentElement);
            // No return type needed, since these elements don't have any children
            AddElementWithTypeAttribute("Method", selectionElement, "String", inputSample.PAL.Method);
            AddElementWithTypeAttribute("Tray", selectionElement, "String", inputSample.PAL.PALTray);
            AddElementWithTypeAttribute("Vial", selectionElement, "Integer", inputSample.PAL.Well.ToString());
            AddElementWithTypeAttribute("Volume", selectionElement, "Integer", inputSample.Volume.ToString());

            // Separation element. These will be blank or zero for now
            var separationElement = AddElementNoAttributes("Separation", parentElement);
            // No return type needed, since these elements don't have any children
            var name = "";
            if (inputSample.LCMethodName != null)
            {
                name = inputSample.LCMethodName;
            }
            AddElementWithTypeAttribute("Method", separationElement, "String", name);

            // Acquisition element. These will be blank or zero for now
            var acquisitionElement = AddElementNoAttributes("Acquisition", parentElement);
            // No return type needed, since these elements don't have any children
            AddElementWithTypeAttribute("Method", acquisitionElement, "String", inputSample.InstrumentMethod);

            // DMS element
            var dmsElement = AddElementNoAttributes("DMS", parentElement);
            // No return type needed, since these elements don't have any children
            AddElementWithTypeAttribute("RequestNumber", dmsElement, "String", inputSample.DmsRequestId.ToString());
        }

        /// <summary>
        /// Saves the XML document to the specified file
        /// </summary>
        /// <param name="inputDoc">XML document to save</param>
        /// <param name="filePath">Full name and path where document will be saved</param>
        private void SaveQueue(XmlDocument inputDoc, string filePath)
        {
            try
            {
                var outputFile = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                inputDoc.Save(outputFile);
                outputFile.Close();
            }
            catch (Exception ex)
            {
                var errMsg = "Exception saving file " + filePath + ": " + ex.Message;
                ApplicationLogger.LogError(0, errMsg, ex);
                throw new DataExportException(errMsg, ex);
            }
        }

        /// <summary>
        /// Adds an XML element with no attributes to an XML document
        /// </summary>
        /// <param name="elementName">Name of the element to add</param>
        /// <param name="parent">XML element that will be the parent of this element</param>
        /// <returns>XMLElement that was added to document</returns>
        private XmlElement AddElementNoAttributes(string elementName, XmlElement parent)
        {
            var newElement = document.CreateElement(elementName);
            parent.AppendChild(newElement);
            return newElement;
        }

        /// <summary>
        /// Adds an XML element with an element type and value to an XML document
        /// </summary>
        /// <param name="elementName">Name of element to add</param>
        /// <param name="parent">XML element that will be the parent of this element</param>
        /// <param name="typeName">Data type of element</param>
        /// <param name="elemValue">String representing value of element</param>
        /// <returns>XML element that was added to document</returns>
        private XmlElement AddElementWithTypeAttribute(string elementName, XmlElement parent, string typeName, string elemValue)
        {
            var newElement = AddElementNoAttributes(elementName, parent);
            var newAttribute = document.CreateAttribute("type");
            newAttribute.Value = typeName;
            newElement.Attributes.Append(newAttribute);
            newElement.InnerText = elemValue;
            return newElement;
        }
    }
}
