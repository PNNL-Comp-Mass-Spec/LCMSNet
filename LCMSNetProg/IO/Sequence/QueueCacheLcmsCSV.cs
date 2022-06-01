using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using LcmsNet.Data;

namespace LcmsNet.IO.Sequence
{
    public class QueueCacheLcmsCSV : ISampleQueueReader, ISampleQueueWriter
    {
        public void SaveCsvCache(string filePath, IEnumerable<SampleData> samples)
        {
            WriteCsvText(filePath, samples.Select(x => new SampleCacheData(x)));
        }

        public List<SampleData> ReadCsvCache(string filePath)
        {
            using (var importStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(importStream))
            {
                var hasHeaders = true;

                if (sr.BaseStream.CanSeek)
                {
                    var headerLine = sr.ReadLine();
                    hasHeaders = CheckHeaderLine(headerLine);

                    sr.BaseStream.Seek(0, SeekOrigin.Begin);
                    sr.DiscardBufferedData();
                }

                return ReadCsvText(sr, hasHeaders).Select(x => x.AsSampleData()).ToList();
            }
        }

        private static bool CheckHeaderLine(string headerLine)
        {
            return !string.IsNullOrWhiteSpace(headerLine) &&
                   (headerLine.IndexOf("Name", StringComparison.OrdinalIgnoreCase) > -1 ||
                    headerLine.IndexOf("Sample", StringComparison.OrdinalIgnoreCase) > -1 ||
                    headerLine.IndexOf("Dataset", StringComparison.OrdinalIgnoreCase) > -1);
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

        private static List<SampleCacheData> ReadCsvText(TextReader importReader, bool hasHeaders)
        {
            using (var csv = new CsvReader(importReader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.RegisterClassMap(new SampleCacheData.SampleCacheMap());
                csv.Configuration.Delimiter = ","; // Could change to "\t" to support TSV
                csv.Configuration.PrepareHeaderForMatch = (header, index) => header?.Trim().ToLower();
                csv.Configuration.HasHeaderRecord = hasHeaders; // Set to false to allow reading a file without headers
                csv.Configuration.HeaderValidated = null; // Allows missing headers
                csv.Configuration.MissingFieldFound = null; // Allow empty fields

                var records = csv.GetRecords<SampleCacheData>();

                return records.ToList();
            }
        }

        List<SampleData> ISampleQueueReader.ReadSamples(string path)
        {
            return ReadCsvCache(path);
        }

        void ISampleQueueWriter.WriteSamples(string path, List<SampleData> data)
        {
            SaveCsvCache(path, data);
        }
    }
}
