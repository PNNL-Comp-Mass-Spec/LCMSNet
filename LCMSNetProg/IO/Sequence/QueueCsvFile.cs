using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using LcmsNet.Data;
using LcmsNet.SampleQueue;
using LcmsNetSDK.Logging;

namespace LcmsNet.IO.Sequence
{
    /// <summary>
    /// Imports/Exports a queue from/to a CSV file
    /// </summary>
    public class QueueCsvFile : ISampleQueueReader, ISampleQueueWriter
    {
        /// <summary>
        /// Saves the specified sample list to the specified file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public void WriteSamples(string path, List<SampleData> data)
        {
            try
            {
                WriteCsvText(path, data.Select(x => new SampleCacheData(x)));
                ApplicationLogger.LogMessage(0, "Queue exported to CSV file " + path);
            }
            catch (Exception ex)
            {
                var errMsg = "Could not write samples to file " + path;
                ApplicationLogger.LogError(0, errMsg, ex);
                throw new DataExportException(errMsg, ex);
            }
        }

        private static void WriteCsvText(string filePath, IEnumerable<SampleCacheData> samples)
        {
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (var sw = new StreamWriter(fs))
            using (var csv = new CsvWriter(sw, CultureInfo.InvariantCulture))
            {
                csv.Configuration.RegisterClassMap(new SampleCacheData.SampleCacheMap());
                csv.Configuration.Delimiter = ",";

                csv.WriteRecords(samples);
            }
        }

        /// <summary>
        /// Read CSV data from a file
        /// </summary>
        /// <param name="path">full path to file</param>
        /// <returns></returns>
        public List<SampleData> ReadSamples(string path)
        {
            List<SampleImportInfo> data = null;
            using (var importStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(importStream))
            {
                var hasHeaders = true;

                if (importStream.CanSeek)
                {
                    var headerLine = sr.ReadLine();
                    hasHeaders = CheckHeaderLine(headerLine);

                    importStream.Seek(0, SeekOrigin.Begin);
                }

                data = ReadCsvText(sr, hasHeaders);
            }

            return data.Where(x => !string.IsNullOrWhiteSpace(x.DatasetName)).Select(x => x.GetSampleData()).ToList();
        }

        /// <summary>
        /// Read CSV data from a string
        /// </summary>
        /// <param name="csvData"></param>
        /// <returns></returns>
        public static List<SampleImportInfo> ReadCsvString(string csvData)
        {
            if (string.IsNullOrWhiteSpace(csvData))
            {
                return new List<SampleImportInfo>();
            }

            var headerLine = csvData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)[0];
            // Check for presence of "name" column, if it isn't present, then read using an index
            var hasHeaders = CheckHeaderLine(headerLine);

            using (var reader = new StringReader(csvData))
            {
                return ReadCsvText(reader, hasHeaders);
            }
        }

        private static bool CheckHeaderLine(string headerLine)
        {
            return !string.IsNullOrWhiteSpace(headerLine) &&
                   (headerLine.IndexOf("Name", StringComparison.OrdinalIgnoreCase) > -1 ||
                    headerLine.IndexOf("Sample", StringComparison.OrdinalIgnoreCase) > -1 ||
                    headerLine.IndexOf("Dataset", StringComparison.OrdinalIgnoreCase) > -1);
        }

        private static List<SampleImportInfo> ReadCsvText(TextReader importReader, bool hasHeaders)
        {
            using (var csv = new CsvReader(importReader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.RegisterClassMap(new SampleImportMap());
                csv.Configuration.Delimiter = ","; // Could change to "\t" to support TSV
                csv.Configuration.PrepareHeaderForMatch = (header, index) => header?.Trim().ToLower();
                csv.Configuration.HasHeaderRecord = hasHeaders; // Set to false to allow reading a file without headers
                csv.Configuration.HeaderValidated = null; // Allows missing headers
                csv.Configuration.MissingFieldFound = null; // Allow empty fields

                var records = csv.GetRecords<SampleImportInfo>();

                return records.ToList();
            }
        }
    }
}
