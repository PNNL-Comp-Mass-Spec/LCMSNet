
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 11/11/2009
//
// Last modified 11/11/2009
//*********************************************************************************************************
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.Configuration;
using System.Collections.Generic;

namespace NetStartTestApp
{
    /// <summary>
    /// Formats and parses messages passed between instrument and cart
    /// </summary>
    class classXmlMsgTools
    {
        #region "Methods"
            /// <summary>
            /// Creates an XML-formatted string representing a Start Acquisition response
            /// </summary>
            /// <param name="responseData">Object containing response parameters</param>
            /// <returns>XML string</returns>
            public static string CreateStartAcqResponse(classResponseData responseData)
            {
                string xmlText = "";

                // Create a memory stream to write an XML document to
                MemoryStream memStream = new MemoryStream();
                using (XmlTextWriter xWriter = new XmlTextWriter(memStream, System.Text.Encoding.UTF8))
                {
                    xWriter.Formatting = Formatting.Indented;
                    xWriter.Indentation = 2;

                    // Create the document
                    xWriter.WriteStartDocument(true);
                    // Root level element
                    xWriter.WriteStartElement("Root");

                    xWriter.WriteStartElement("NetCommand");
                    xWriter.WriteElementString("Command", "StartAcquisitionResponse");
                    xWriter.WriteEndElement(); // NetCommand section

                    xWriter.WriteStartElement("Params");
                    xWriter.WriteElementString("Cart", responseData.Cart);
                    xWriter.WriteElementString("Instrument", Properties.Settings.Default.InstName);
                    xWriter.WriteElementString("SampleName", responseData.SampleName);
                    xWriter.WriteElementString("LCMethodNameRunning", responseData.LcMethod);
                    xWriter.WriteElementString("InstMethodNameRunning", responseData.InstMethod);
                    xWriter.WriteElementString("StartAcquisitionStatus", responseData.Status);
                    xWriter.WriteElementString("StartAcquisitionMessage", responseData.Message);
                    xWriter.WriteEndElement();  // Params section

                    xWriter.WriteEndElement();  // Root section

                    // Close the document, but don't close the writer
                    xWriter.WriteEndDocument();
                    xWriter.Flush();

                    // Use a streamreader to copy the XML text to a string variable
                    memStream.Seek(0, SeekOrigin.Begin);
                    StreamReader memStreamReader = new StreamReader(memStream);
                    xmlText = memStreamReader.ReadToEnd();

                    memStreamReader.Close();
                    memStream.Close();

                    // Since the document is now a string, we can get rid of the XMLWriter
                    xWriter.Close();
                }   // End using

                // Return the XML string
                return xmlText;
            }

            /// <summary>
            /// Creates an XML-formatted string representing a response to a Stop Acquisition command
            /// </summary>
            /// <param name="responseData">Object containing response parameters</param>
            /// <returns>XML string</returns>
            public static string CreateStopAcqResponse(classResponseData responseData)
            {
                string xmlText = "";

                // Create a memory stream to write an XML document to
                MemoryStream memStream = new MemoryStream();
                using (XmlTextWriter xWriter = new XmlTextWriter(memStream, System.Text.Encoding.UTF8))
                {
                    xWriter.Formatting = Formatting.Indented;
                    xWriter.Indentation = 2;

                    // Create the document
                    xWriter.WriteStartDocument(true);
                    // Root level element
                    xWriter.WriteStartElement("Root");

                    xWriter.WriteStartElement("NetCommand");
                    xWriter.WriteElementString("Command", "StopAcquisitionResponse");
                    xWriter.WriteEndElement(); // NetCommand section

                    xWriter.WriteStartElement("Params");
                    xWriter.WriteElementString("Cart", responseData.Cart);
                    xWriter.WriteElementString("Instrument", Properties.Settings.Default.InstName);
                    xWriter.WriteElementString("StopAcquisitionStatus", responseData.Status);
                    xWriter.WriteElementString("StopAcquisitionMessage", responseData.Message);
                    xWriter.WriteEndElement();  // Params section

                    xWriter.WriteEndElement();  // Root section

                    // Close the document, but don't close the writer
                    xWriter.WriteEndDocument();
                    xWriter.Flush();

                    // Use a streamreader to copy the XML text to a string variable
                    memStream.Seek(0, SeekOrigin.Begin);
                    StreamReader memStreamReader = new StreamReader(memStream);
                    xmlText = memStreamReader.ReadToEnd();

                    memStreamReader.Close();
                    memStream.Close();

                    // Since the document is now a string, we can get rid of the XMLWriter
                    xWriter.Close();
                }   // End using

                // Return the XML string
                return xmlText;
            }

            /// <summary>
            /// Creates an XML-formatted command to get a list of instrument methods
            /// </summary>
            /// <param name="cartName">Name of cart that requested list (required for message validation)</param>
            /// <param name="methodNames">List of method names found on instrument</param>
            /// <returns>XML string</returns>
            public static string CreateMethodRequestResponse(string cartName, List<string> methodNames)
            {
                string xmlText = "";

                // Create a memory stream to write an XML document to
                MemoryStream memStream = new MemoryStream();
                using (XmlTextWriter xWriter = new XmlTextWriter(memStream, System.Text.Encoding.UTF8))
                {
                    xWriter.Formatting = Formatting.Indented;
                    xWriter.Indentation = 2;

                    // Create the document
                    xWriter.WriteStartDocument(true);
                    // Root level element
                    xWriter.WriteStartElement("Root");

                    xWriter.WriteStartElement("NetCommand");
                    xWriter.WriteElementString("Command", "MethodNamesRequestResponse");
                    xWriter.WriteEndElement(); // NetCommand section

                    xWriter.WriteStartElement("Params");
                    xWriter.WriteElementString("Cart", cartName);
                    xWriter.WriteElementString("Instrument", Properties.Settings.Default.InstName);

                    xWriter.WriteStartElement("MethodNames");
                    foreach (string methodName in methodNames)
                    {
                        xWriter.WriteElementString("Method", methodName);
                    }
                    xWriter.WriteEndElement();  // Method names section
                    xWriter.WriteEndElement();  // Params section

                    xWriter.WriteEndElement();  // Root section

                    // Close the document, but don't close the writer
                    xWriter.WriteEndDocument();
                    xWriter.Flush();

                    // Use a streamreader to copy the XML text to a string variable
                    memStream.Seek(0, SeekOrigin.Begin);
                    StreamReader memStreamReader = new StreamReader(memStream);
                    xmlText = memStreamReader.ReadToEnd();

                    memStreamReader.Close();
                    memStream.Close();

                    // Since the document is now a string, we can get rid of the XMLWriter
                    xWriter.Close();
                }   // End using
                // Return the XML string
                return xmlText;
                
            }

            /// <summary>
            /// Parses an XML response received from an instrument
            /// </summary>
            /// <param name="inputMsg">XML message that was received</param>
            /// <returns>String dictionary containing the respose parameters</returns>
            public static StringDictionary ParseCartCommand(string inputMsg)
            {
                string xPathStr;

                // Load the received string into an XML document
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(inputMsg);

                // Find the node containing the command
                xPathStr = "//Command";
                XmlElement cmdElement = (XmlElement)doc.SelectSingleNode(xPathStr);
                if (cmdElement == null)
                {
                    //TODO: Deal with this problem!
                }

                // Fill a string dictionary with the message data
                StringDictionary returnDict = new StringDictionary();
                returnDict.Add("ResponseType", cmdElement.InnerText);
                switch (cmdElement.InnerText)
                {
                    case "StartAcquisition":
                        // Response to a StartAcquisition command
                        returnDict.Add("CartName", doc.SelectSingleNode("//Cart").InnerText);
                        returnDict.Add("InstName", doc.SelectSingleNode("//Instrument").InnerText);
                        returnDict.Add("SampleName", doc.SelectSingleNode("//SampleName").InnerText);
                        returnDict.Add("LCMethodName", doc.SelectSingleNode("//LCMethodName").InnerText);
                        returnDict.Add("InstMethodName", doc.SelectSingleNode("//InstMethodName").InnerText);
                        break;
                    case "StopAcquisition":
                        returnDict.Add("CartName", doc.SelectSingleNode("//Cart").InnerText);
                        returnDict.Add("InstName", doc.SelectSingleNode("//Instrument").InnerText);
                        break;
                    case "MethodNamesRequest":
                        // Response to a RequestMethodNames command
                        returnDict.Add("CartName", doc.SelectSingleNode("//Cart").InnerText);
                        returnDict.Add("InstName", doc.SelectSingleNode("//Instrument").InnerText);
                        break;
                    default:
                        // Shouldn't ever get here
                        break;
                }

                return returnDict;
            }
        #endregion
    }
}   // End region
