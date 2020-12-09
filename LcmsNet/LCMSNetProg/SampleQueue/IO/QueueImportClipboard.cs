﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LcmsNetSDK.Data;

namespace LcmsNet.SampleQueue.IO
{
    public static class QueueImportClipboard //: ISampleQueueReader
    {
        public static List<SampleData> ReadSamples()
        {
            return ReadClipboard().Where(x => !string.IsNullOrWhiteSpace(x.DatasetName)).Select(x => x.GetSampleData()).ToList();
        }

        public static List<SampleImportInfo> ReadClipboard()
        {
            if (Clipboard.ContainsData(DataFormats.CommaSeparatedValue))
            {
                // Handle data copied from Excel.
                var data = Clipboard.GetText(TextDataFormat.CommaSeparatedValue);
                return QueueImportCSV.ReadCsvString(data);
            }

            // Clipboard not from Excel - assume a single row, or (if not) ignore all after the first space (in each line)
            var importData = new List<SampleImportInfo>();
            if (Clipboard.ContainsData(DataFormats.Text))
            {
                var data = Clipboard.GetText(TextDataFormat.Text);
                foreach (var line in data.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries))
                {
                    var start = line.Trim().Split(' ')[0];
                    importData.Add(new SampleImportInfo { DatasetName = start });
                }
            }

            return importData;
        }
    }
}
