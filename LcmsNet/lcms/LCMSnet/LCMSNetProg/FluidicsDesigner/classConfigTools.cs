
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 09/03/2009
//
// Last modified 09/03/2009
//*********************************************************************************************************
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace LcmsNet.FluidicsDesigner
{
    class classConfigTools
    {
        //*********************************************************************************************************
        // Handles creation of an XML Fluidics Designer config file
        //**********************************************************************************************************

        #region "Constants"
        #endregion

        #region "Class variables"
            private static XmlDocument mobject_xDocument = null;
            //private static XmlTextWriter mobject_xWriter;

            //private static MemoryStream mobject_MemStream;
            //private static StreamReader mobject_MemStreamReader;
        #endregion

        #region "Delegates"
        #endregion

        #region "Events"
        #endregion

        #region "Properties"
        #endregion

        #region "Constructors"
        #endregion

        #region "Methods"
            /// <summary>
            /// Initializes the XML document used in creating the config file
            /// </summary>
            private static void InitConfigDoc()
            {
                // Create a new XML document
                mobject_xDocument = new XmlDocument();

                // Create a root element
                XmlElement rootElement = mobject_xDocument.CreateElement("Root");
                mobject_xDocument.AppendChild(rootElement);
            }   // End sub

            /// <summary>
            /// Creates an XML element for storing the settings for one symbol
            /// </summary>
            /// <param name="symbolName"></param>
            public static void CreateSymbolElement(string symbolName)
            {
                // Check to see if XML document has been initialized
                if (mobject_xDocument == null) InitConfigDoc();

                // Make sure we're at the root level
                XmlNode rootNode = mobject_xDocument.DocumentElement;
                if (rootNode == null)
                {
                    //TODO: This is BAD -- alert the user
                }

                // Create the new element node and add it to the root node
                XmlElement xmlEl = mobject_xDocument.CreateElement("SymbolName");
                rootNode.AppendChild(xmlEl);
                xmlEl.SetAttribute("Name", symbolName);
            }   // End sub

            /// <summary>
            /// Creates an XML element (overloaded for use when no attributes specified)
            /// </summary>
            /// <param name="parentXPath">XPath specifying parent node</param>
            /// <param name="elementName">Name of element to create</param>
            public static void CreateElement(string parentXPath, string elementName)
            {
                // Verify document has been inititialized
                if (!VerifyDocInit())
                {
                    //TOOD: This is very bad - figure out how to tell operator
                    return;
                }

                // Get the parent node
                XmlNode parentNode = GetParentNode(parentXPath);
                if (parentNode == null)
                {
                    //TODO: Report error somehow
                    return;
                }

                // Create the element
                XmlElement newElement = mobject_xDocument.CreateElement(elementName);
                parentNode.AppendChild(newElement);
            }   // End sub

            /// <summary>
            /// Creates an XML element with attributes (overloaded for specifying attributes)
            /// </summary>
            /// <param name="parentXPath">XPath specifying parent node</param>
            /// <param name="elementName">Name of element to create</param>
            /// <param name="attName">Name of attribute to create</param>
            /// <param name="attVal">Value of attribute</param>
            public static void CreateElement(string parentXPath, string elementName, string attName, string attVal)
            {
                // Verify document has been inititialized
                if (!VerifyDocInit())
                {
                    //TOOD: This is very bad - figure out how to tell operator
                    return;
                }

                // Get the parent node
                XmlNode parentNode = GetParentNode(parentXPath);
                if (parentNode == null)
                {
                    //TODO: Report error somehow
                    return;
                }

                // Create the element
                XmlElement newElement = mobject_xDocument.CreateElement(elementName);
                newElement.SetAttribute(attName, attVal);
                parentNode.AppendChild(newElement);
            }   // End sub

            /// <summary>
            /// Creates an XML element from an imported node
            /// </summary>
            /// <param name="parentXPath">XPath specifying parent node</param>
            /// <param name="importedNode">XML node to import</param>
            public static void CreateImportedElement(string parentXPath, string elementName, XmlNode nodeToImport)
            {
                // Verify document has been inititialized
                if (!VerifyDocInit())
                {
                    //TOOD: This is very bad - figure out how to tell operator
                    return;
                }

                // Get the parent node
                XmlNode parentNode = GetParentNode(parentXPath);
                if (parentNode == null)
                {
                    //TODO: Report error somehow
                    return;
                }

                // Create the element
                XmlElement newElement = mobject_xDocument.CreateElement(elementName);

                // Import the node and add it to the new element
                XmlNode importedNode = mobject_xDocument.ImportNode(nodeToImport, true);
                newElement.AppendChild(importedNode);

                // Add the new element to the parent node and we're done
                parentNode.AppendChild(newElement);
            }   // End sub

            /// <summary>
            /// Writes the XML to a file
            /// </summary>
            /// <param name="fileNamePath">Full path to config file</param>
            /// <returns>TRUE for success, FALSE for failure</returns>
            public static bool WriteConfigFile(string fileNamePath)
            {
                // Check to verify the XML document exists
                if (mobject_xDocument == null)
                {
                    //TODO: Report the error
                    return false;
                }

                // Create and configure an XML text writer to write the output file
                XmlTextWriter xWriter = new XmlTextWriter(fileNamePath, System.Text.Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xWriter.Indentation = 2;

                // Write the output file
                try
                {
                    mobject_xDocument.Save(xWriter);
                    CloseConfigDoc();   // Clear the document for the next use
                    return true;
                }
                catch (Exception ex)
                {
                    //TODO: Notify the operator
                    return false;
                }
            }   // End sub

            /// <summary>
            ///  Checks to see if config XML doc has been initialized
            /// </summary>
            /// <returns>TRUE if document initialized, FALSE otherwise</returns>
            private static bool VerifyDocInit()
            {
                if (mobject_xDocument == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }   // End sub

            /// <summary>
            /// Find the parent node specified in xPath
            /// </summary>
            /// <param name="xPath">xPath spec for parent name</param>
            /// <returns>XMLNode if found; NULL otherwise</returns>
            private static XmlNode GetParentNode(string xPath)
            {
                XmlNode retNode = mobject_xDocument.SelectSingleNode(xPath);
                return retNode;
            }   // End sub

            /// <summary>
            /// Loads the config file into an XML document
            /// </summary>
            /// <param name="fileNamePath">Fully qualified config file name</param>
            public static void LoadConfigDoc(string fileNamePath)
            {
                if (mobject_xDocument != null) mobject_xDocument = null;

                try
                {
                    mobject_xDocument = new XmlDocument();
                    mobject_xDocument.Load(fileNamePath);
                }
                catch (Exception ex)
                {
                    //TODO: Handle exception
                }
            }   // End sub

            /// <summary>
            /// Gets a list of XML nodes representing device symbols
            /// </summary>
            /// <returns>XML node list</returns>
            public static XmlNodeList GetSymbolNodes()
            {
                // Verify XML doc is loaded
                if (mobject_xDocument == null) return null;

                string xPath = "//SymbolName";
                XmlNodeList retList = mobject_xDocument.SelectNodes(xPath);
                return retList;
            }   // End sub

            /// <summary>
            /// Gets the value of specified attribute
            /// </summary>
            /// <param name="xPath">XPath query for attribute's parent node</param>
            /// <param name="AttribName">Attribute name</param>
            /// <returns>Attribute value</returns>
            public static string GetAttribValue(XmlNode currNode, string xPath, string attribName)
            {
                XmlElement currElement = (XmlElement)currNode.SelectSingleNode(xPath);
                string retStr = currElement.GetAttribute(attribName);
                return retStr;
            }   // End sub

            /// <summary>
            /// Closes the XML config doc
            /// </summary>
            public static void CloseConfigDoc()
            {
                mobject_xDocument = null;
            }   // End sub

            /// <summary>
            /// Retrieves device data config node from config file copy in memory
            /// </summary>
            /// <param name="currNode">Symbol node currently active</param>
            /// <returns>XMLNode containing saved device data</returns>
            public static XmlNode GetDeviceData(XmlNode currNode)
            {
                // Assumes current node is a symbol name node, and
                //      device data is in a node named DeviceData under the current node
                string xPath = "./DeviceData";
                return currNode.SelectSingleNode(xPath);
            }   // End sub
        #endregion
    }   // End class
}   // End namespace
