//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 06/29/2010
//
// Last modified 06/29/2010
//*********************************************************************************************************
using System;
using System.IO;
using System.Xml;

namespace LcmsNet.Devices.BrukerStart
{
    /// <summary>
    /// Class to provide methods for building XML strings required by sXc
    /// </summary>
    class classBrukerXmlBuilder
    {
        #region "Class variables"
            static XmlDocument m_Doc;
        #endregion

        #region "Methods"
            public static string CreateXmlString(string outputPath, string methodPath, string sampleName, string MethodName)
            {
                // Create and initialize document object
                m_Doc = new XmlDocument();
                XmlDeclaration docDeclaration = m_Doc.CreateXmlDeclaration("1.0", null, null);
                m_Doc.AppendChild(docDeclaration);

                // Add root element
                XmlElement rootElement = m_Doc.CreateElement("Acquisition");
                m_Doc.AppendChild(rootElement);

                XmlAttribute newAttrbute;

                // Interface version
                newAttrbute = m_Doc.CreateAttribute("interfaceVersion");
                newAttrbute.Value = "1024";
                rootElement.Attributes.Append(newAttrbute);

                // Run Time
                newAttrbute = m_Doc.CreateAttribute("runTime");
                newAttrbute.Value = "2.00";
                rootElement.Attributes.Append(newAttrbute);

                // Time Offset
                newAttrbute = m_Doc.CreateAttribute("timeMSOffset");
                newAttrbute.Value = "0.00";
                rootElement.Attributes.Append(newAttrbute);

                // MethodMS
                newAttrbute = m_Doc.CreateAttribute("methodMS");
                newAttrbute.Value = Path.Combine(methodPath, MethodName);
                rootElement.Attributes.Append(newAttrbute);

                // Result Method Path
                newAttrbute = m_Doc.CreateAttribute("resultMethodPath");
                string tmpPath = Path.Combine(outputPath, sampleName);
                newAttrbute.Value = Path.Combine(tmpPath, MethodName);
                rootElement.Attributes.Append(newAttrbute);

                // Sample ID
                newAttrbute = m_Doc.CreateAttribute("sampleID");
                newAttrbute.Value = sampleName;
                rootElement.Attributes.Append(newAttrbute);

                // Sample Comment
                newAttrbute = m_Doc.CreateAttribute("sampleComment");
                newAttrbute.Value = "";
                rootElement.Attributes.Append(newAttrbute);

                // sample Position
                newAttrbute = m_Doc.CreateAttribute("samplePosition");
                newAttrbute.Value = "1";
                rootElement.Attributes.Append(newAttrbute);

                // Threshold
                newAttrbute = m_Doc.CreateAttribute("Threshold");
                newAttrbute.Value = "0";
                rootElement.Attributes.Append(newAttrbute);

                //// Interval0
                //newAttrbute = m_Doc.CreateAttribute("Interval0");
                //newAttrbute.Value = "BPC,ALL";
                //rootElement.Attributes.Append(newAttrbute);

                //// Interval1
                //newAttrbute = m_Doc.CreateAttribute("interval1");
                //newAttrbute.Value = "";
                //rootElement.Attributes.Append(newAttrbute);

                MemoryStream memStream = new MemoryStream();
                XmlTextWriter xmWriter = new XmlTextWriter(memStream, System.Text.Encoding.UTF8);

                m_Doc.Save(memStream);
//              m_Doc.Save(@"D:\Temporary\BrukerXMLTest.xml");  // Debug statement

                memStream.Seek(0, SeekOrigin.Begin);
                StreamReader memStreamReader = new StreamReader(memStream);
                string xmlText = memStreamReader.ReadToEnd();

                memStreamReader.Close();
                memStream.Close();
                xmWriter = null;

                return xmlText + Environment.NewLine;
            }   
        #endregion
    }   
}
