using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LcmsNet.IO.DMS.Data;
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
        /// Tests that SaveSingleColumnListToCache works
        /// </summary>
        [Test]
        public void TestB()
        {
            var testSeparationTypes = new List<string>
            {
                "Separation 1",
                "Separation 2",
                "Separation 3"
            };

            // if the following line doesn't throw an exception, it "worked".
            SQLiteTools.SaveSeparationTypeListToCache(testSeparationTypes);
        }

        /// <summary>
        /// Tests that getseptypelist gets separations..specifically those stored by TestB.
        /// </summary>
        [Test]
        public void TestC()
        {
            var testList = new List<string>
            {
                "Separation 1",
                "Separation 2",
                "Separation 3"
            };
            var retrieved = SQLiteTools.GetSepTypeList(false);
            Assert.IsTrue(retrieved.SequenceEqual(testList)); // If this is equal, both TestB and C worked, and we read the information back from the cache
        }

        /// <summary>
        /// Checks that SaveUserListToCache works(or thinks it does)
        /// </summary>
        [Test]
        public void TestD()
        {
            var usersExampleData = new List<UserInfo>();
            var example = new UserInfo
            {
                PayrollNum = "1",
                UserName = "Test User"
            };
            usersExampleData.Add(example);
            SQLiteTools.SaveUserListToCache(usersExampleData);
        }


        /// <summary>
        /// Tests that GetUserList returns correct users. Specifically the one added by TestD.
        /// </summary>
        [Test]
        public void TestE()
        {
            var users = SQLiteTools.GetUserList(false).ToList();
            Assert.AreEqual(1, users.Count);
            Assert.IsTrue(users.Exists(x => x.UserName == "Test User" && x.PayrollNum == "1"));
        }

        /// <summary>
        /// Tests that the default separation type is saved.
        /// </summary>
        [Test]
        public void TestF()
        {
            SQLiteTools.SaveSelectedSeparationType("Separation3");
        }

        /// <summary>
        /// Tests that GetDefaultSeparationType returns a non-empty string.
        /// </summary>
        [Test]
        public void TestG()
        {
            var result = SQLiteTools.GetDefaultSeparationType();
            Assert.AreEqual("Separation3", result);
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

            SQLiteTools.SaveQueueToCache<SampleData>(samples, DatabaseTableTypes.WaitingQueue);
        }

        /// <summary>
        /// Tests that getqueuefromcache gets the "waiting queue" from the cache
        /// </summary>
        [Test]
        public void TestI()
        {
            var samples = SQLiteTools.GetQueueFromCache<SampleData>(DatabaseTableTypes.WaitingQueue).ToList();
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
            SQLiteTools.SaveQueueToCache<SampleData>(samples, DatabaseTableTypes.RunningQueue);
        }

        /// <summary>
        /// Tests that getqueuefromcache gets the "running queue" from the cache
        /// </summary>
        [Test]
        public void TestK()
        {
            var samples = SQLiteTools.GetQueueFromCache<SampleData>(DatabaseTableTypes.RunningQueue).ToList();
            Assert.AreEqual(2, samples[0].UniqueID);
        }


        /// <summary>
        /// Test that experiments are saved to cache correctly
        /// </summary>
        [Test]
        public void TestL()
        {
            var experiments = new List<ExperimentData>();
            var experiment = new ExperimentData
            {
                ID = 1,
                Experiment = "Test",
                Created = DateTime.Now,
                Researcher = "Staff",
                Reason = "Software testing"
            };
            experiments.Add(experiment);
            SQLiteTools.SaveExperimentListToCache(experiments);
        }


        /// <summary>
        /// Tests that experiments are read from cache
        /// </summary>
        [Test]
        public void TestM()
        {
            var experiments = SQLiteTools.GetExperimentList().ToList();
            Assert.AreEqual(1, experiments[0].ID);
            Assert.AreEqual("Test", experiments[0].Experiment);
        }

        /// <summary>
        /// Test that instruments are saved to cache correctly
        /// </summary>
        [Test]
        public void TestN()
        {
            var instInfo = new List<InstrumentInfo>();
            var inst = new InstrumentInfo
            {
                CommonName = "Test instrument"
            };
            instInfo.Add(inst);
            SQLiteTools.SaveInstListToCache(instInfo);
        }

        /// <summary>
        /// Tests that instruments are read from cache
        /// </summary>
        [Test]
        public void TestO()
        {
            var insts = SQLiteTools.GetInstrumentList(false).ToList();
            Assert.AreEqual("Test instrument", insts[0].CommonName);
        }

        /// <summary>
        /// Test that proposal users are saved to cache correctly
        /// </summary>
        [Test]
        public void TestP()
        {
            var users = new List<ProposalUser>();
            var user = new ProposalUser
            {
                UserID = 1
            };
            users.Add(user);
            SQLiteTools.SaveProposalUsers(users, new List<UserIDPIDCrossReferenceEntry>(), new Dictionary<string, List<UserIDPIDCrossReferenceEntry>>());
        }

        /// <summary>
        /// Tests that proposal users are read from cache
        /// </summary>
        [Test]
        public void TestQ()
        {
            List<ProposalUser> users;
            Dictionary<string, List<UserIDPIDCrossReferenceEntry>> dict;
            SQLiteTools.GetProposalUsers(out users, out dict);
            Assert.AreEqual(1, users[0].UserID);
        }

        /// <summary>
        /// Tests that LCColumns are saved to cache
        /// </summary>
        [Test]
        public void TestR()
        {
            var cols = new List<string>();
            var col = "ColTest1";
            cols.Add(col);
            SQLiteTools.SaveColumnListToCache(cols);
        }

        /// <summary>
        /// Tests that LCColumns are read from cache
        /// </summary>
        [Test]
        public void TestS()
        {
            var cols = SQLiteTools.GetColumnList(false).ToList();
            Assert.AreEqual(1, cols.Count);
            Assert.IsTrue(cols[0] == "ColTest1");
        }
    }
}
