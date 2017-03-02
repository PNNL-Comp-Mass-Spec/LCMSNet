/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 10/22/2013
 * 
 * Last Modified 11/08/2013 By Christopher Walters 
 ********************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetSDK;
using LcmsNetSDK.Data;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Data;
using LcmsNetDataClasses.Devices;
using System.Drawing;
using System.IO;

namespace PDFGenerator
{
    public class PDFGen:IPDF
    {
        #region Methods
        /// <summary>
        /// create a PDF and write it to documentPath.
        /// </summary>
        /// <param name="documentPath">the path to where the document should be saved</param>
        /// <param name="title">the name of the file to write/title of the document</param>
        /// <param name="sample">the sample that was run</param>
        /// <param name="numEnabledColumns">a string containing the number of columns that were enabled during the run</param>
        /// <param name="columnData">a list of classColumnData for each enabled column</param>
        /// <param name="DevicesAndStatus">a list of 3-tuples of string containing device name, status, and error type</param>
        /// <param name="fluidicsImage">a bitmap containing the current fluidics design</param>
        public void WritePDF(string documentPath, string title, classSampleData sample, string numEnabledColumns, List<LcmsNetDataClasses.Configuration.classColumnData> columnData,
            List<IDevice> devices, Bitmap fluidicsImage)
        {
            // instantiate PDFSharp writer library, EMSL document model, and setup options.
            EMSL.DocumentGenerator.Core.Services.IDocumentWriter writer = new EMSL.DocumentGenerator.PDFSharp.PDFWriter();
            EMSL.DocumentGenerator.Core.Document doc = new EMSL.DocumentGenerator.Core.Document();
            doc.DocumentWriter = writer;
            doc.FontSize = 11;
            doc.Font = "Courier New";
            doc.Title = title;
            //enter document into model                      
            doc.AddHeader(EMSL.DocumentGenerator.Core.Model.HeaderLevel.H1, "Dataset - " + sample.DmsData.DatasetName);       
            doc.AddParagraph(CreateDatasetParagraph(sample));            
            doc.AddHeader(EMSL.DocumentGenerator.Core.Model.HeaderLevel.H1, "LC Configuration");
            int[] FieldWidths = {-20, -20};
            string[] cartData = { "Cart-Name:", sample.DmsData.CartName };
            string cartConfigString = FormatString(FieldWidths, cartData);
            
            string[] configData = { "Enabled-Columns:", numEnabledColumns };
            cartConfigString += FormatString(FieldWidths, configData);
            doc.AddParagraph(cartConfigString);
            
            doc.AddHeader(EMSL.DocumentGenerator.Core.Model.HeaderLevel.H2, "Columns");
            string columnDataString = CreateColumnString(columnData);
            doc.AddParagraph(columnDataString);
                //solvent data currently unavailable. TODO: implement if/when solvent information is available
            doc.AddHeader(EMSL.DocumentGenerator.Core.Model.HeaderLevel.H2, "Mobile Phases");
                ////TODO: get solvent info from lcmsnet
            List<LcmsNetDataClasses.Devices.Pumps.IPump> pumps = new List<LcmsNetDataClasses.Devices.Pumps.IPump>();
            foreach(IDevice device in devices)
            {
                if(device is LcmsNetDataClasses.Devices.Pumps.IPump)
                {
                    pumps.Add(device as LcmsNetDataClasses.Devices.Pumps.IPump);
                }
            }
            StringBuilder mobilePhases = new StringBuilder();
            int[] fieldWidths = { -5, -15, -13, -15, -8, -15 }; 
            foreach(LcmsNetDataClasses.Devices.Pumps.IPump pump in pumps)
            {                           
                if (pump.MobilePhases.Count > 0)
                {                                      
                    foreach (MobilePhase phase in pump.MobilePhases)
                    {                        
                        string[] mobilePhaseData = {"Pump:", pump.Name, "Mobile Phase:", phase.Name, "Comment:", phase.Comment };
                        mobilePhases.Append(FormatString(fieldWidths, mobilePhaseData));
                    }
                }
                else
                {
                    string[] mobilePhaseData = {"Pump:", pump.Name, "Mobile Phase:", "None", "Comment:", "None"};
                    mobilePhases.Append(FormatString(fieldWidths, mobilePhaseData));
                }                
            }
            doc.AddParagraph(mobilePhases.ToString());

            doc.AddHeader(EMSL.DocumentGenerator.Core.Model.HeaderLevel.H1, "Mass Spectrometer Configuration");
            //TODO: Get mass spectrometer name from lcmsnet
            doc.AddParagraph(string.Format("Mass Spectrometer: {0}", classLCMSSettings.GetParameter(classLCMSSettings.PARAM_INSTNAME)));
            doc.AddPageBreak();
            //fluidics design section          
            doc.AddHeader(EMSL.DocumentGenerator.Core.Model.HeaderLevel.H1, "LC Method");
            doc.AddParagraph("Method Name: " + sample.LCMethod.Name);
            doc.AddParagraph(""); // adds an empty line
            doc.AddParagraph(CreateLCMethodString(sample));
            //TODO pump method info
            //doc.AddHeader(EMSL.DocumentGenerator.Core.Model.HeaderLevel.H1, "Pump Method");
            //doc.AddParagraph("Pump Method Info");
            doc.AddPageBreak();

            doc.AddHeader(EMSL.DocumentGenerator.Core.Model.HeaderLevel.H1, "Fluidics Configuration");
            doc.AddHeader(EMSL.DocumentGenerator.Core.Model.HeaderLevel.H2, "Table of Devices");

            string fluidicsDevString = CreateDeviceString(devices);
            doc.AddParagraph(fluidicsDevString);
            doc.AddHeader(EMSL.DocumentGenerator.Core.Model.HeaderLevel.H2, "Configuration");
            Bitmap rfluidicsImage = ScaleImage(fluidicsImage);
            EMSL.DocumentGenerator.Core.Model.ImageContent imageC = new EMSL.DocumentGenerator.Core.Model.ImageContent(rfluidicsImage);
            doc.AddImage(imageC);
            
            //error section
            //doc.AddHeader(EMSL.DocumentGenerator.Core.Model.HeaderLevel.H1, "Errors");
            //doc.AddParagraph("Error Info");

            //write the document to file 
            doc.WriteDocument(documentPath);
            rfluidicsImage.Dispose();
        }

        /// <summary>
        /// create a formatted paragraph of device data
        /// </summary>
        /// <param name="devAndStatus">Tuples of strings contaning device names, status, and error types</param>
        /// <returns>a formatted string of device data</returns>
        private static string CreateDeviceString(List<IDevice> devices)
        {
            string formattedString = string.Empty;
            int[] fieldWidths = { -20, -20, -20 };
            string[] headers = { "Device", "Status", "Error" };
            formattedString += FormatString(fieldWidths, headers);
            foreach (IDevice device in devices)
            {
                string[] data = {device.Name, device.Status.ToString(), device.ErrorType.ToString() };
                formattedString += FormatString(fieldWidths, data);
            }
            return formattedString;
        }

        /// <summary>
        /// create scaled image of fluidics design, with intact aspect ratio
        /// </summary>
        /// <param name="fluidicsImage">a bitmap containing the current fluidics design</param>
        /// <returns>a rescaled bitmap</returns>
        private static Bitmap ScaleImage(Bitmap fluidicsImage)
        {
            // create bitmap image of fluidics design, rescale it, and add it to the document model                               
            // image no wider than an 8" page with 0.5" margins---aka 7" and no taller than 5"
            // while maintaining aspect ratio
            float maxWidth = (7 * fluidicsImage.HorizontalResolution);
            float maxHeight = (5 * fluidicsImage.VerticalResolution);
            float scale = 1;
            if (maxWidth < fluidicsImage.Width || maxHeight < fluidicsImage.Height)
            {
                //the smaller of the two ratios will produce an image in correct aspect ratio, with an acceptable size
                scale = Math.Min(maxWidth / fluidicsImage.Width, maxHeight / fluidicsImage.Height);
            }

            Bitmap rfluidicsImage = resizeBitmap(fluidicsImage, (int)(fluidicsImage.Width * scale), (int)(fluidicsImage.Height * scale));
            fluidicsImage.Dispose();
            return rfluidicsImage;
        }

        /// <summary>
        /// create a formatted paragraph of LCMethod data
        /// </summary>
        /// <param name="sample">sample data</param>
        /// <returns>a formatted string of LCMethod data</returns>
        private static string CreateLCMethodString(classSampleData sample)
        {
            int[] fieldWidths = {-20, -20, -20, -20};
            string[]  row = {"Device", "Event Name", "Duration", "HadError"};
            string lcMethodString = FormatString(fieldWidths, row);

            foreach (LcmsNetDataClasses.Method.classLCEvent lcEvent in sample.LCMethod.Events)
            {
                string deviceName = lcEvent.Device.Name;
                int deviceNameLength = lcEvent.Device.Name.Length;
                string lcEventName = lcEvent.Name;
                int eventNameLength = lcEvent.Name.Length;

                // substrings are 20 characters long for columnation, each index is a column 
                string[] LCEventRow = {deviceName.Substring(0, (deviceNameLength < 20 ? deviceNameLength: 20)),     // device name
                                       lcEventName.Substring(0, (eventNameLength < 20 ? eventNameLength : 20)),     // LC-event name
                                       lcEvent.Duration.ToString(),                                                 // duration
                                       lcEvent.HadError ? "Yes":"No"};                                              // Device error column

                lcMethodString += FormatString(fieldWidths, LCEventRow);                
                // We also want to store the methods and their parameters.
                if (lcEvent.Parameters != null && lcEvent.ParameterNames != null && lcEvent.ParameterNames.Length > 0)
                {
                    string[] data = new string[lcEvent.ParameterNames.Length];
                    for (int i = 0; i < lcEvent.ParameterNames.Length; i++)
                    {
                        lcMethodString += string.Format("{0,-20}{1,-20}{2,-20}{3}", "", lcEvent.ParameterNames[i], lcEvent.Parameters[i], Environment.NewLine);
                    }
                }
                else
                {
                    lcMethodString += "No Parameters" + Environment.NewLine;
                }
            }
            return lcMethodString;
        }

        /// <summary>
        /// create a formatted paragraph of column data
        /// </summary>
        /// <param name="columnData">a list containing data for the columns</param>
        /// <returns>a formatted string of column data</returns>
        private static string CreateColumnString(List<LcmsNetDataClasses.Configuration.classColumnData> columnData)
        {
            //Column data, we want to be able to determine other column names and status..but this would only happen at the *end* of the run, perhaps not so useful?  
            int[] FieldWidths = { -20, -20};
            string[] colData = {"Column Name", "Status"};
            string columnDataString = FormatString(FieldWidths, colData);
            foreach (LcmsNetDataClasses.Configuration.classColumnData cData in columnData)
            {
                string[] data = {cData.Name, cData.Status.ToString() };
                columnDataString += FormatString(FieldWidths, data);
            }
            return columnDataString;
        }

        /// <summary>
        /// create a paragraph for the dataset info
        /// </summary>
        /// <param name="sample">sample information</param>
        /// <returns>a string consisting of formatted dataset info</returns>
        private static string CreateDatasetParagraph(classSampleData sample)
        {
            /* this paragraph contains all information in two columns, first the information identifier, for example "RequestID" and the second
               contains the actual information such as "42" */
            int[] FieldWidths = { -20, -20 };
            string[] datasetName = { "Dataset Name:", sample.DmsData.DatasetName };
            string datasetNameString = FormatString(FieldWidths, datasetName);
            string[] requestData = { "Request:", sample.DmsData.RequestName };
            string requestString = FormatString(FieldWidths, requestData);
            string[] rid = {"Request Id:", sample.DmsData.RequestID.ToString()};
            string requestIdString = FormatString(FieldWidths, rid);

            string[] sTime = {"Start Time:", sample.LCMethod.Start.ToLongTimeString() + " " + sample.LCMethod.Start.ToLongDateString()};
            string startFormatted = FormatString(FieldWidths, sTime);

            string[] eTime = {"End Time:", sample.LCMethod.End.ToLongTimeString() + " " + sample.LCMethod.End.ToLongDateString()};
            string endFormatted = FormatString(FieldWidths, eTime);

            string[] cd = { "Column:", sample.ColumnData.Name };
            string columnString = FormatString(FieldWidths, cd);

            string[] lcm = { "LCMethod:", sample.LCMethod.Name };
            string LCMethodString = FormatString(FieldWidths, lcm);

            string[] pt = { "PAL Tray:", sample.PAL.PALTray };
            string PALTrayString =  FormatString(FieldWidths, pt);

            string[] pv = { "Pal Vial:", sample.PAL.Well.ToString() };
            string PALVialString = FormatString(FieldWidths, pv);

            string[] iv = { "Injection Volume:", sample.Volume.ToString() };
            string injectionVolume = FormatString(FieldWidths, iv);

            string[] t = { "Dataset Type:", sample.DmsData.DatasetType };
            string type = FormatString(FieldWidths, t);

            string[] ba = { "Batch:", sample.DmsData.Batch.ToString() };
            string batch = FormatString(FieldWidths, ba);

            string[] bl = { "Block:", sample.DmsData.Block.ToString() };
            string block = FormatString(FieldWidths, bl);
            
            string[] ro = {"Run Order:", sample.DmsData.RunOrder.ToString()};
            string runOrder = FormatString(FieldWidths, ro);
            
            string[] co = {"Comment:", sample.DmsData.Comment};
            string comment = FormatString(FieldWidths, co);

            string paragraph = datasetNameString + requestString + requestIdString + startFormatted + endFormatted + columnString + LCMethodString + PALTrayString + PALVialString + injectionVolume + type + batch +
                block + runOrder + comment;

            return paragraph;
        }


        /// <summary>
        /// format a string into a columned string
        /// </summary>
        /// <param name="fieldWidths">width of field for each column</param>
        /// <param name="data">data to go in each column</param>
        /// <returns>a columned string filled with the provided data</returns>
        private static string FormatString(int[] fieldWidths, string[] data)
        {
            int count = 0;
            StringBuilder stringFormatter = new StringBuilder();
            string formattedString = string.Empty;
            foreach (int fieldWidth in fieldWidths)
            {
                // create format string of {<count>, <fieldwidth>}, ex {0, 20} for the first element in data and a field width of 20 characters
                stringFormatter.Append("{" + count.ToString() + "," + fieldWidth.ToString() + "}");                                          
                count++;
            }           
            stringFormatter.Append(Environment.NewLine);
            // use stringFormatter to place all elements of data into proper format
            formattedString += string.Format(stringFormatter.ToString(), data);           
            return formattedString;
        }

        /// <summary>
        /// resize a bitmap to fit on an 8.5 x 11 page, at a max of 5 x 7
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static Bitmap resizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap resizedBmp = new Bitmap(width, height);
            resizedBmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
            using (Graphics g = Graphics.FromImage(resizedBmp))
            {
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(bmp, 0, 0, resizedBmp.Width, resizedBmp.Height);
            }
            return resizedBmp;
        }
        #endregion
    }
}