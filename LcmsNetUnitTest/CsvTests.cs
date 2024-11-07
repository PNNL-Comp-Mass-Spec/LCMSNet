using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LcmsNet.Data;
using LcmsNet.IO.Sequence;
using NUnit.Framework;

namespace LcmsnetUnitTest
{
    [TestFixture]
    public class CsvTests
    {
        const string CONST_TEST_FOLDER = "LCMSNetUnitTests";
        const string CONST_TEST_CACHE = "SQLiteToolsUnitTests.lcms.csv";

        private const bool DELETE_CACHE_DB = true;

        public CsvTests()
        {
            CsvCache.Initialize(CONST_TEST_FOLDER);

            var appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var file = Path.Combine(appPath, CONST_TEST_FOLDER, CONST_TEST_CACHE);
            if (File.Exists(file) && DELETE_CACHE_DB)
            {
                File.Delete(file);
            }

            // Note that this will call BuildConnectionString
            CsvCache.SetCacheLocation(file);
        }

        /**
         * Tests are named TestA-TestZ  to get NUnit to execute them in order
         */

        /// <summary>
        /// Tests that SaveQueueToCache works by saving the "waiting queue"
        /// </summary>
        [Test]
        public void TestH()
        {
            var samples = new List<SampleData>
            {
                new SampleData()
            };
            samples[0].UniqueID = 1;
            samples[0].Name = "Test";

            samples.Add(new SampleData());
            samples[1].UniqueID = 2;

            CsvCache.SaveQueueToCache(samples);
        }

        /// <summary>
        /// Tests that getqueuefromcache gets the "waiting queue" from the cache
        /// </summary>
        [Test]
        public void TestI()
        {
            var samples = CsvCache.GetQueueFromCache().ToList();
            Assert.That(samples[0].UniqueID, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that savequeuetocache works by saving the "running queue"
        /// </summary>
        [Test]
        public void TestJ()
        {
            var samples = new List<SampleData>
            {
                new SampleData()
            };
            samples[0].UniqueID = 2;
            CsvCache.SaveQueueToCache(samples);
        }

        /// <summary>
        /// Tests that getqueuefromcache gets the "running queue" from the cache
        /// </summary>
        [Test]
        public void TestK()
        {
            var samples = CsvCache.GetQueueFromCache().ToList();
            Assert.That(samples[0].UniqueID, Is.EqualTo(2));
        }
    }
}
