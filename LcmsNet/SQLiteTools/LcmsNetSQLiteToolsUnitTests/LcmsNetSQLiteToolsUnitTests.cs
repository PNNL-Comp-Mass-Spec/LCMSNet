using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Data;
using LcmsNetSQLiteTools;
using NUnit.Framework;

namespace LcmsNetSQLiteToolsUnitTests
{
    [TestFixture]
    public class LcmsNetSQLiteToolsUnitTests
    {
        const string CONST_TEST_FOLDER = "LCMSNetUnitTests";
        const string CONST_TEST_CACHE = "classSQLiteToolsUnitTests.que";

        private const bool DELETE_CACHE_DB = true;

        public LcmsNetSQLiteToolsUnitTests()
        {

            classSQLiteTools.Initialize(CONST_TEST_FOLDER);

            var appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var file = Path.Combine(appPath, CONST_TEST_FOLDER, CONST_TEST_CACHE);
            if (File.Exists(file) && DELETE_CACHE_DB)
            {
                File.Delete(file);
            }

            // Note that this will call BuildConnectionString
            classSQLiteTools.SetCacheLocation(file);

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Tests named TestA-TestZ  to get NUnit to execute them in order
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// tests that the connection string being used by SQLiteTools is correct.
        /// </summary>
        [Test]
        public void TestA()
        {
            //actual buildconnectionstring call is in constructor
            var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                          CONST_TEST_FOLDER, CONST_TEST_CACHE);
            Console.WriteLine("ConnectionString: " + classSQLiteTools.ConnString);
            Assert.AreEqual("data source=" + folderPath, classSQLiteTools.ConnString);
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
            classSQLiteTools.SaveSingleColumnListToCache(testSeparationTypes, enumTableTypes.SeparationTypeList);
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
            var retrieved = classSQLiteTools.GetSepTypeList(false);
            Assert.IsTrue(retrieved.SequenceEqual(testList)); // If this is equal, both TestB and C worked, and we read the information back from the cache
        }

        /// <summary>
        /// Checks that SaveUserListToCache works(or thinks it does)
        /// </summary>
        [Test]
        public void TestD()
        {
            var usersExampleData = new List<classUserInfo>();
            var example = new classUserInfo
            {
                PayrollNum = "1",
                UserName = "Test User"
            };
            usersExampleData.Add(example);
            classSQLiteTools.SaveUserListToCache(usersExampleData);
        }


        /// <summary>
        /// Tests that GetUserList returns correct users. Specifically the one added by TestD.
        /// </summary>
        [Test]
        public void TestE()
        {
            var users = classSQLiteTools.GetUserList(false);
            Assert.AreEqual(1, users.Count);
            Assert.IsTrue(users.Exists(x => x.UserName == "Test User" && x.PayrollNum == "1"));
        }

        /// <summary>
        /// Tests that the default separation type is saved.
        /// </summary>
        [Test]
        public void TestF()
        {
            classSQLiteTools.SaveSelectedSeparationType("Separation3");
        }

        /// <summary>
        /// Tests that GetDefaultSeparationType returns a non-empty string.
        /// </summary>
        [Test]
        public void TestG()
        {
            var result = classSQLiteTools.GetDefaultSeparationType();
            Assert.AreEqual("Separation3", result);
        }

        /// <summary>
        /// Tests that SaveQueueToCache works by saving the "waiting queue"
        /// </summary>
        [Test]
        public void TestH()
        {
            var samples = new List<classSampleData>
            {
                new classSampleData()
            };
            samples[0].UniqueID = 1;
            samples[0].DmsData.DatasetName = "Test";

            samples.Add(new classSampleData());
            samples[1].UniqueID = 2;

            classSQLiteTools.SaveQueueToCache(samples, enumTableTypes.WaitingQueue);
        }

        /// <summary>
        /// Tests that getqueuefromcache gets the "waiting queue" from the cache
        /// </summary>
        [Test]
        public void TestI()
        {
            var samples = classSQLiteTools.GetQueueFromCache(enumTableTypes.WaitingQueue);
            Assert.AreEqual(1, samples[0].UniqueID);
        }

        /// <summary>
        /// Tests that savequeuetocache works by saving the "running queue"
        /// </summary>
        [Test]
        public void TestJ()
        {
            var samples = new List<classSampleData>
            {
                new classSampleData()
            };
            samples[0].UniqueID = 2;
            classSQLiteTools.SaveQueueToCache(samples, enumTableTypes.RunningQueue);
        }

        /// <summary>
        /// Tests that getqueuefromcache gets the "running queue" from the cache
        /// </summary>
        [Test]
        public void TestK()
        {
            var samples = classSQLiteTools.GetQueueFromCache(enumTableTypes.RunningQueue);
            Assert.AreEqual(2, samples[0].UniqueID);
        }


        /// <summary>
        /// Test that experiments are saved to cache correctly
        /// </summary>
        [Test]
        public void TestL()
        {
            var experiments = new List<classExperimentData>();
            var experiment = new classExperimentData
            {
                ID = 1,
                Experiment = "Test",
                Created = DateTime.Now,
                Researcher = "Staff",
                Reason = "Software testing"
            };
            experiments.Add(experiment);
            classSQLiteTools.SaveExperimentListToCache(experiments);
        }


        /// <summary>
        /// Tests that experiments are read from cache
        /// </summary>
        [Test]
        public void TestM()
        {
            var experiments = classSQLiteTools.GetExperimentList();
            Assert.AreEqual(1, experiments[0].ID);
            Assert.AreEqual("Test", experiments[0].Experiment);
        }

        /// <summary>
        /// Test that instruments are saved to cache correctly
        /// </summary>
        [Test]
        public void TestN()
        {
            var instInfo = new List<classInstrumentInfo>();
            var inst = new classInstrumentInfo
            {
                CommonName = "Test instrument"
            };
            instInfo.Add(inst);
            classSQLiteTools.SaveInstListToCache(instInfo);
        }

        /// <summary>
        /// Tests that instruments are read from cache
        /// </summary>
        [Test]
        public void TestO()
        {
            var insts = classSQLiteTools.GetInstrumentList(false);
            Assert.AreEqual("Test instrument", insts[0].CommonName);
        }

        /// <summary>
        /// Test that proposal users are saved to cache correctly
        /// </summary>
        [Test]
        public void TestP()
        {
            var users = new List<classProposalUser>();
            var user = new classProposalUser
            {
                UserID = 1
            };
            users.Add(user);
            classSQLiteTools.SaveProposalUsers(users, new List<classUserIDPIDCrossReferenceEntry>(), new Dictionary<string, List<classUserIDPIDCrossReferenceEntry>>());
        }

        /// <summary>
        /// Tests that proposal users are read from cache
        /// </summary>
        [Test]
        public void TestQ()
        {
            List<classProposalUser> users;
            Dictionary<string, List<classUserIDPIDCrossReferenceEntry>> dict;
            classSQLiteTools.GetProposalUsers(out users, out dict);
            Assert.AreEqual(1, users[0].UserID);
        }

        /// <summary>
        /// Tests that LCColumns are saved to cache
        /// </summary>
        [Test]
        public void TestR()
        {
            var cols = new List<classLCColumn>();
            var col = new classLCColumn
            {
                LCColumn = "ColTest1"
            };
            cols.Add(col);
            classSQLiteTools.SaveEntireLCColumnListToCache(cols);
        }

        /// <summary>
        /// Tests that LCColumns are read from cache
        /// </summary>
        [Test]
        public void TestS()
        {
            var cols = classSQLiteTools.GetEntireLCColumnList();
            Assert.AreEqual(1, cols.Count);
            Assert.IsTrue(cols[0].LCColumn == "ColTest1");
        }
    }
}
