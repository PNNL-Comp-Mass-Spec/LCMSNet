using System;
using System.Collections.Generic;
using System.IO;
using LcmsNet.Data;

namespace LcmsNet.IO.Sequence
{
    public class CsvCache
    {
        private static QueueCacheLcmsCSV Cache { get; }

        private static string cacheFullPath;

        private static Func<string> defaultDirectoryPathGetMethod = () => ".";

        static CsvCache()
        {
            Cache = new QueueCacheLcmsCSV();
        }

        /// <summary>
        /// Cache file name or path
        /// </summary>
        /// <remarks>Starts off as a filename, but is changed to a path by BuildConnectionString</remarks>
        public static string CacheName { get; private set; }

        /// <summary>
        /// Initialize the cache, with the provided cache filename
        /// </summary>
        /// <param name="cacheName"></param>
        public static void Initialize(string cacheName = "LCMSCache.lcms.csv")
        {
            if (CacheName.EndsWith(".que", StringComparison.OrdinalIgnoreCase))
            {
                cacheName = Path.ChangeExtension(cacheName, "lcms.csv");
            }
            CacheName = cacheName;

            var name = CacheName;
            if (File.Exists(name) && DefaultDirectoryPath != null)
            {
                var basePath = DefaultDirectoryPath;
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }

                name = Path.Combine(basePath, CacheName);
                CacheName = name;
            }

            cacheFullPath = name;
        }

        public static string DefaultDirectoryPath => defaultDirectoryPathGetMethod();

        public static void SetDefaultDirectoryPath(string path)
        {
            defaultDirectoryPathGetMethod = () => path;
        }

        public static void SetDefaultDirectoryPath(Func<string> pathGetMethod)
        {
            defaultDirectoryPathGetMethod = pathGetMethod;
        }
        /// <summary>
        /// Sets the cache location to the path provided
        /// </summary>
        /// <param name="location">New path to location of queue</param>
        /// <remarks>If location is a filename (and not a path), then updates to use AppDataFolderName</remarks>
        public static void SetCacheLocation(string location)
        {
            if (!location.Contains(@"\"))
            {
                var basePath = DefaultDirectoryPath;
                var fileName = Path.GetFileName(location);
                location = Path.Combine(basePath, fileName);
            }

            CacheName = location;
            Initialize(CacheName);
        }

        /// <summary>
        /// Retrieves a sample queue from cache CSV
        /// </summary>
        /// <returns>List containing queue data</returns>
        public static IEnumerable<SampleData> GetQueueFromCache()
        {
            if (string.IsNullOrWhiteSpace(cacheFullPath))
                Initialize(CacheName);

            return Cache.ReadCsvCache(cacheFullPath);
        }

        /// <summary>
        /// Saves the contents of specified sample queue to the CSV cache file
        /// </summary>
        /// <param name="queueData">List of SampleData containing the sample data to save</param>
        public static void SaveQueueToCache(IEnumerable<SampleData> queueData)
        {
            if (string.IsNullOrWhiteSpace(cacheFullPath))
                Initialize(CacheName);

            Cache.SaveCsvCache(cacheFullPath, queueData);
        }
    }
}
