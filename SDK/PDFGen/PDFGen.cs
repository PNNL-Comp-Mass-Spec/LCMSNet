﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LcmsNetSDK;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using PDFGenerator.Core.Model;
using PDFGenerator.Core.Services;

namespace PDFGenerator
{
    /// <summary>
    /// Class to facilitate the creation of a method and sample report
    /// </summary>
    public class PDFGen : IPDF
    {
        /// <summary>
        /// create a PDF and write it to documentPath.
        /// </summary>
        /// <param name="documentPath">the path to where the document should be saved</param>
        /// <param name="title">the name of the file to write/title of the document</param>
        /// <param name="sample">the sample that was run</param>
        /// <param name="numEnabledColumns">a string containing the number of columns that were enabled during the run</param>
        /// <param name="columnData">a list of classColumnData for each enabled column</param>
        /// <param name="devices">a list of 3-tuples of string containing device name, status, and error type</param>
        /// <param name="fluidicsImage">a bitmap containing the current fluidics design</param>
        public void WritePDF(string documentPath, string title, ISampleInfo sample, string numEnabledColumns, IReadOnlyList<IColumn> columnData,
            IReadOnlyList<IDevice> devices, BitmapSource fluidicsImage)
        {
            // instantiate PDFSharp writer library, EMSL document model, and setup options.
            IDocumentWriter writer = new PDFSharp.PDFWriter();
            var doc = new Document
            {
                DocumentWriter = writer,
                FontSize = 11,
                Font = "Courier New",
                Title = title
            };
            //enter document into model
            doc.AddHeader(HeaderLevel.H1, "Dataset - " + sample.Name);
            doc.AddParagraph(CreateDatasetParagraph(sample));
            doc.AddHeader(HeaderLevel.H1, "LC Configuration");
            int[] FieldWidths = {-20, -20};
            string[] cartData = { "Cart-Name:", LCMSSettings.GetParameter(LCMSSettings.PARAM_CARTNAME) };
            var cartConfigString = FormatString(FieldWidths, cartData);

            string[] configData = { "Enabled-Columns:", numEnabledColumns };
            cartConfigString += FormatString(FieldWidths, configData);
            doc.AddParagraph(cartConfigString);

            doc.AddHeader(HeaderLevel.H2, "Columns");
            var columnDataString = CreateColumnString(columnData);
            doc.AddParagraph(columnDataString);
                //solvent data currently unavailable. TODO: implement if/when solvent information is available
            doc.AddHeader(HeaderLevel.H2, "Mobile Phases");
                ////TODO: get solvent info from lcmsnet
            var pumps = new List<IPump>();
            foreach(var device in devices)
            {
                if(device is IPump)
                {
                    pumps.Add(device as IPump);
                }
            }
            var mobilePhases = new StringBuilder();
            int[] fieldWidths = { -5, -15, -13, -15, -8, -15 };
            foreach(var pump in pumps)
            {
                if (pump.MobilePhases.Count > 0)
                {
                    foreach (var phase in pump.MobilePhases)
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

            doc.AddHeader(HeaderLevel.H1, "Mass Spectrometer Configuration");
            //TODO: Get mass spectrometer name from lcmsnet
            doc.AddPageBreak();
            //fluidics design section
            doc.AddHeader(HeaderLevel.H1, "LC Method");
            doc.AddParagraph("Method Name: " + sample.ActualLCMethod.Name);
            doc.AddParagraph(""); // adds an empty line
            doc.AddParagraph(CreateLCMethodString(sample));
            //TODO pump method info
            //doc.AddHeader(HeaderLevel.H1, "Pump Method");
            //doc.AddParagraph("Pump Method Info");
            doc.AddPageBreak();

            doc.AddHeader(HeaderLevel.H1, "Fluidics Configuration");
            doc.AddHeader(HeaderLevel.H2, "Table of Devices");

            var fluidicsDevString = CreateDeviceString(devices);
            doc.AddParagraph(fluidicsDevString);
            doc.AddHeader(HeaderLevel.H2, "Configuration");
            var rfluidicsImage = ScaleImage(fluidicsImage);
            var imageC = new ImageContent(rfluidicsImage);
            doc.AddImage(imageC);

            //error section
            //doc.AddHeader(HeaderLevel.H1, "Errors");
            //doc.AddParagraph("Error Info");

            //write the document to file
            doc.WriteDocument(documentPath);
        }

        /// <summary>
        /// create a formatted paragraph of device data
        /// </summary>
        /// <param name="devices">Tuples of strings containing device names, status, and error types</param>
        /// <returns>a formatted string of device data</returns>
        private static string CreateDeviceString(IReadOnlyList<IDevice> devices)
        {
            var formattedString = string.Empty;
            int[] fieldWidths = { -20, -20, -20 };
            string[] headers = { "Device", "Status", "Error" };
            formattedString += FormatString(fieldWidths, headers);
            foreach (var device in devices)
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
        private static BitmapSource ScaleImage(BitmapSource fluidicsImage)
        {
            // create bitmap image of fluidics design, rescale it, and add it to the document model
            // image no wider than an 8" page with 0.5" margins---aka 7" and no taller than 5"
            // while maintaining aspect ratio
            var maxWidth = (7 * fluidicsImage.DpiX);
            var maxHeight = (5 * fluidicsImage.DpiY);
            double scale = 1;
            if (maxWidth < fluidicsImage.Width || maxHeight < fluidicsImage.Height)
            {
                //the smaller of the two ratios will produce an image in correct aspect ratio, with an acceptable size
                scale = Math.Min(maxWidth / fluidicsImage.Width, maxHeight / fluidicsImage.Height);
            }

            return new TransformedBitmap(fluidicsImage, new ScaleTransform(scale, scale));
        }

        /// <summary>
        /// create a formatted paragraph of LCMethod data
        /// </summary>
        /// <param name="sample">sample data</param>
        /// <returns>a formatted string of LCMethod data</returns>
        private static string CreateLCMethodString(ISampleInfo sample)
        {
            int[] fieldWidths = {-20, -20, -20, -20};
            string[]  row = {"Device", "Event Name", "Duration", "HadError"};
            var lcMethodString = FormatString(fieldWidths, row);

            foreach (var lcEvent in sample.ActualLCMethod.Events)
            {
                var deviceName = lcEvent.Device.Name;
                var deviceNameLength = lcEvent.Device.Name.Length;
                var lcEventName = lcEvent.Name;
                var eventNameLength = lcEvent.Name.Length;

                // substrings are 20 characters long for columnation, each index is a column
                string[] LCEventRow = {deviceName.Substring(0, (deviceNameLength < 20 ? deviceNameLength: 20)),     // device name
                                       lcEventName.Substring(0, (eventNameLength < 20 ? eventNameLength : 20)),     // LC-event name
                                       lcEvent.Duration.ToString(),                                                 // duration
                                       lcEvent.HadError ? "Yes":"No"};                                              // Device error column

                lcMethodString += FormatString(fieldWidths, LCEventRow);
                // We also want to store the methods and their parameters.
                if (lcEvent.Parameters != null && lcEvent.ParameterNames != null && lcEvent.ParameterNames.Length > 0)
                {
                    var data = new string[lcEvent.ParameterNames.Length];
                    for (var i = 0; i < lcEvent.ParameterNames.Length; i++)
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
        private static string CreateColumnString(IReadOnlyList<IColumn> columnData)
        {
            //Column data, we want to be able to determine other column names and status..but this would only happen at the *end* of the run, perhaps not so useful?
            int[] FieldWidths = { -20, -20};
            string[] colData = {"Column Name", "Status"};
            var columnDataString = FormatString(FieldWidths, colData);
            foreach (var cData in columnData)
            {
                string[] data = {cData.Name, cData.StatusString };
                columnDataString += FormatString(FieldWidths, data);
            }
            return columnDataString;
        }

        /// <summary>
        /// create a paragraph for the dataset info
        /// </summary>
        /// <param name="sample">sample information</param>
        /// <returns>a string consisting of formatted dataset info</returns>
        private static string CreateDatasetParagraph(ISampleInfo sample)
        {
            /* this paragraph contains all information in two columns, first the information identifier, for example "RequestID" and the second
               contains the actual information such as "42" */
            int[] FieldWidths = { -20, -20 };
            string[] datasetName = { "Dataset Name:", sample.Name };
            var datasetNameString = FormatString(FieldWidths, datasetName);

            string[] sTime = {"Start Time:", sample.ActualLCMethod.Start.ToLongTimeString() + " " + sample.ActualLCMethod.Start.ToLongDateString()};
            var startFormatted = FormatString(FieldWidths, sTime);

            string[] eTime = {"End Time:", sample.ActualLCMethod.End.ToLongTimeString() + " " + sample.ActualLCMethod.End.ToLongDateString()};
            var endFormatted = FormatString(FieldWidths, eTime);

            string[] cd = { "Column:", sample.ColumnName };
            var columnString = FormatString(FieldWidths, cd);

            string[] lcm = { "LCMethod:", sample.ActualLCMethod.Name };
            var LCMethodString = FormatString(FieldWidths, lcm);

            string[] pt = { "PAL Tray:", sample.PAL.PALTray };
            var PALTrayString =  FormatString(FieldWidths, pt);

            string[] pv = { "Pal Vial:", sample.PAL.Well.ToString() };
            var PALVialString = FormatString(FieldWidths, pv);

            string[] iv = { "Injection Volume:", sample.Volume.ToString("0.00") };
            var injectionVolume = FormatString(FieldWidths, iv);

            var additionalLines = "";
            foreach (var entry in sample.GetExportValuePairs())
            {
                additionalLines += FormatString(FieldWidths, entry);
            }

            var paragraph = datasetNameString + startFormatted + endFormatted + columnString + LCMethodString + PALTrayString + PALVialString + injectionVolume + additionalLines;

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
            var count = 0;
            var stringFormatter = new StringBuilder();
            var formattedString = string.Empty;
            foreach (var fieldWidth in fieldWidths)
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
    }
}
