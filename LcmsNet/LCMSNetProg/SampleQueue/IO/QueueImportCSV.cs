using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using LcmsNetSDK.Data;

namespace LcmsNet.SampleQueue.IO
{
    public class QueueImportCSV : ISampleQueueReader
    {
        public List<SampleData> ReadSamples(string path)
        {
            return ReadCsvFile(path).Where(x => !string.IsNullOrWhiteSpace(x.DatasetName)).Select(x => x.GetSampleData()).ToList();
        }

        public static List<SampleImportInfo> ReadCsvFile(string filePath)
        {
            using (var importStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return ReadCsvStream(importStream);
            }
        }

        /// <summary>
        /// Read CSV data from a stream
        /// </summary>
        /// <param name="importStream">Input stream, must be seekable</param>
        /// <returns></returns>
        public static List<SampleImportInfo> ReadCsvStream(Stream importStream)
        {
            using (var sr = new StreamReader(importStream))
            {
                var hasHeaders = true;

                if (importStream.CanSeek)
                {
                    var headerLine = sr.ReadLine();
                    hasHeaders = CheckHeaderLine(headerLine);

                    importStream.Seek(0, SeekOrigin.Begin);
                }

                return ReadCsvText(sr, hasHeaders);
            }
        }

        public static List<SampleImportInfo> ReadCsvString(string csvData)
        {
            if (string.IsNullOrWhiteSpace(csvData))
            {
                return new List<SampleImportInfo>();
            }

            var headerLine = csvData.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)[0];
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

        public sealed class SampleImportMap : ClassMap<SampleImportInfo>
        {
            public SampleImportMap()
            {
                Map(x => x.DatasetName).Name("Request Name", "Sample Name", "Dataset Name", "Sample", "Dataset", "Name").Index(0);
                Map(x => x.PalVial).Name("PAL Vial", "Vial", "Well").Index(1).Default("");
                Map(x => x.PalTray).Name("PAL Tray", "Tray").Index(2).Default("");
                Map(x => x.LcMethod).Name("LC Method", "Method", "LCMethod").Index(3).Default("");
                Map(x => x.RunOrder).Name("Run Order").Index(4).Default(-1);
                Map(x => x.RequestId).Name("Request ID").Index(5).Default(0);
            }
        }
    }
}
