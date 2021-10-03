using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LcmsNet.IO.SQLite;
using LcmsNetSDK.Data;
using NUnit.Framework;

namespace LcmsnetUnitTest
{
    [TestFixture]
    public class SQLiteTests
    {
        const string CONST_TEST_FOLDER = "LCMSNetUnitTests";
        const string CONST_TEST_CACHE = "SQLiteToolsUnitTests.que";

        private const bool DELETE_CACHE_DB = true;

        public SQLiteTests()
        {

            SQLiteTools.Initialize(CONST_TEST_FOLDER);

            var appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var file = Path.Combine(appPath, CONST_TEST_FOLDER, CONST_TEST_CACHE);
            if (File.Exists(file) && DELETE_CACHE_DB)
            {
                File.Delete(file);
            }

            // Note that this will call BuildConnectionString
            SQLiteTools.SetCacheLocation(file);
        }


        /**
         * Tests are named TestA-TestZ  to get NUnit to execute them in order
         */

        /// <summary>
        /// tests that the connection string being used by SQLiteTools is correct.
        /// </summary>
        [Test]
        public void TestA()
        {
            //actual buildconnectionstring call is in constructor
            var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                          CONST_TEST_FOLDER, CONST_TEST_CACHE);
            Console.WriteLine("ConnectionString: " + SQLiteTools.ConnString);
            Assert.AreEqual("data source=" + folderPath, SQLiteTools.ConnString);
        }

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
            samples[0].DmsData.DatasetName = "Test";

            samples.Add(new SampleData());
            samples[1].UniqueID = 2;

            SQLiteTools.SaveQueueToCache(samples, DatabaseTableTypes.WaitingQueue);
        }

        /// <summary>
        /// Tests that getqueuefromcache gets the "waiting queue" from the cache
        /// </summary>
        [Test]
        public void TestI()
        {
            var samples = SQLiteTools.GetQueueFromCache(DatabaseTableTypes.WaitingQueue).ToList();
            Assert.AreEqual(1, samples[0].UniqueID);
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
            SQLiteTools.SaveQueueToCache(samples, DatabaseTableTypes.RunningQueue);
        }

        /// <summary>
        /// Tests that getqueuefromcache gets the "running queue" from the cache
        /// </summary>
        [Test]
        public void TestK()
        {
            var samples = SQLiteTools.GetQueueFromCache(DatabaseTableTypes.RunningQueue).ToList();
            Assert.AreEqual(2, samples[0].UniqueID);
        }
    }
}
