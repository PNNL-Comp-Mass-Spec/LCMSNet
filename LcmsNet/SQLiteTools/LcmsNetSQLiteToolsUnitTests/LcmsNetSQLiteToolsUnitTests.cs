using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using LcmsNetSQLiteTools;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Data;
using System.IO;

namespace LcmsNetSQLiteToolsUnitTests
{
    [TestFixture]
    public class LcmsNetSQLiteToolsUnitTests
    {
        const string CONST_TEST_FOLDER = "LCMSNet\\";
        const string CONST_TEST_CACHE = "classSQLiteToolsUnitTests.que";

        public LcmsNetSQLiteToolsUnitTests()
        {
            //For some reason, in order to get a cache that is not LCMSCache.que you have to perform the following two 
            //operations in this specific order...There appears to be a logic error in the interaction between the SQLiteTools constructor and BuildConnectionString
            //that is causing this. TODO: FIX IT! If time is found
            classSQLiteTools.CacheName = CONST_TEST_CACHE; // causes the classSQLiteTools construtor to run, which would otherwise overwrite our following SetParameter
            classLCMSSettings.SetParameter("CacheFileName", CONST_TEST_CACHE);
            string appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string file = Path.Combine(appPath, CONST_TEST_FOLDER);
            file = Path.Combine(file, CONST_TEST_CACHE);
            if(File.Exists(file))
            {
                File.Delete(file);
            }
            classSQLiteTools.BuildConnectionString(false); 
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
           Assert.AreEqual("data source="+ Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + CONST_TEST_FOLDER + CONST_TEST_CACHE, classSQLiteTools.ConnString);
       }

        /// <summary>
        /// Tests that SaveSingleColumnListToCache works(or at least thinks it did)
        /// </summary>
        [Test]
        public void TestB()
        {
            List<string> testSeperationTypes = new List<string>();
            testSeperationTypes.Add("Separation 1");
            testSeperationTypes.Add("Separation 2");
            testSeperationTypes.Add("Separation 3");
            // if the following line doesn't throw an exception, it "worked".
            classSQLiteTools.SaveSingleColumnListToCache(testSeperationTypes, enumTableTypes.SeparationTypeList);
        }

        /// <summary>
        /// Tests that getseptypelist gets separations..specifically those stored by TestB.
        /// </summary>
        [Test]
        public void TestC()
        {
            List<string> testList = new List<string>();
            testList.Add("Separation 1");
            testList.Add("Separation 2");
            testList.Add("Separation 3");
            List<string> retrieved = classSQLiteTools.GetSepTypeList(false);
            Assert.IsTrue(retrieved.SequenceEqual(testList)); // If this is equal, both TestB and C worked, and we read the information back from the cache
        }

        /// <summary>
        /// Checks that SaveUserListToCache works(or thinks it does)
        /// </summary>
        [Test]
        public void TestD()
        {
            List<classUserInfo> usersExampleData = new List<classUserInfo>();
            classUserInfo example = new classUserInfo();
            example.PayrollNum = "1";
            example.UserName = "Test User";
            usersExampleData.Add(example);
            classSQLiteTools.SaveUserListToCache(usersExampleData);
        }


        /// <summary>
        /// Tests that GetUserList returns correct users. Specifically the one added by TestD.
        /// </summary>
        [Test]
        public void TestE()
        {          
            List<classUserInfo> users = classSQLiteTools.GetUserList(false);
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
            string result = classSQLiteTools.GetDefaultSeparationType();
            Assert.AreEqual("Separation3", result);
        }

        /// <summary>
        /// Tests that savequeuetocache works by saving the "waiting queue"
        /// </summary>
        [Test]
        public void TestH()
        {
            List<classSampleData> samples = new List<classSampleData>();
            samples.Add(new classSampleData());
            samples[0].UniqueID = 1;
            classSQLiteTools.SaveQueueToCache(samples, enumTableTypes.WaitingQueue);            
        }

        /// <summary>
        /// Tests that getqueuefromcache gets the "waiting queue" from the cache
        /// </summary>
        [Test]
        public void TestI()
        {
            List<classSampleData> samples = classSQLiteTools.GetQueueFromCache(enumTableTypes.WaitingQueue);
            Assert.AreEqual(1, samples[0].UniqueID);
        }

        /// <summary>
        /// Tests that savequeuetocache works by saving the "running queue"
        /// </summary>
        [Test]
        public void TestJ()
        {
            List<classSampleData> samples = new List<classSampleData>();
            samples.Add(new classSampleData());
            samples[0].UniqueID = 2;
            classSQLiteTools.SaveQueueToCache(samples, enumTableTypes.RunningQueue);
        }

        /// <summary>
        /// Tests that getqueuefromcache gets the "running queue" from the cache
        /// </summary>
        [Test]
        public void TestK()
        {
            List<classSampleData> samples = classSQLiteTools.GetQueueFromCache(enumTableTypes.RunningQueue);
            Assert.AreEqual(2, samples[0].UniqueID);
        }


        /// <summary>
        /// Test that experiments are saved to cache correctly
        /// </summary>
        [Test]
        public void TestL()
        {
            List<classExperimentData> experiments = new List<classExperimentData>();
            classExperimentData experiment = new classExperimentData();
            experiment.ID = 1;
            experiments.Add(experiment);
            classSQLiteTools.SaveExperimentListToCache(experiments);
        }


        /// <summary>
        /// Tests that experiments are read from cache
        /// </summary>
        [Test]
        public void TestM()
        {
            List<classExperimentData> experiments = classSQLiteTools.GetExperimentList();
            Assert.AreEqual(1, experiments[0].ID);
        }

        /// <summary>
        /// Test that instruments are saved to cache correctly
        /// </summary>
        [Test]
        public void TestN()
        {
            List<classInstrumentInfo> instInfo = new List<classInstrumentInfo>();
            classInstrumentInfo inst = new classInstrumentInfo();
            inst.CommonName = "Test instrument";
            instInfo.Add(inst);
            classSQLiteTools.SaveInstListToCache(instInfo);
        }
        
        /// <summary>
        /// Tests that instruments are read from cache
        /// </summary>
        [Test]
        public void TestO()
        {
            List<classInstrumentInfo> insts = classSQLiteTools.GetInstrumentList(false);
            Assert.AreEqual("Test instrument", insts[0].CommonName);
        }

        /// <summary>
        /// Test that instruments are saved to cache correctly
        /// </summary>
        [Test]
        public void TestP()
        {
            List<classProposalUser> users = new List<classProposalUser>();
            classProposalUser user = new classProposalUser();
            user.UserID = 1;
            users.Add(user);
            classSQLiteTools.SaveProposalUsers(users, new List<classUserIDPIDCrossReferenceEntry>(), new Dictionary<string, List<classUserIDPIDCrossReferenceEntry>>());
        }

        /// <summary>
        /// Tests that instruments are read from cache
        /// </summary>
        [Test]
        public void TestQ()
        {
            List<classProposalUser> users = new List<classProposalUser>();
            Dictionary<string, List<classUserIDPIDCrossReferenceEntry>> dict = new Dictionary<string,List<classUserIDPIDCrossReferenceEntry>>();
            classSQLiteTools.GetProposalUsers(out users, out dict);
            Assert.AreEqual(1, users[0].UserID);
        }

        /// <summary>
        /// Tests that LCColumns are saved to cache
        /// </summary>
       [Test]
       public void TestR()
       {
           List<classLCColumn> cols = new List<classLCColumn>();
           classLCColumn col = new classLCColumn();
           col.LCColumn = "ColTest1";
           cols.Add(col);
           classSQLiteTools.SaveEntireLCColumnListToCache(cols);
       }
      
        /// <summary>
        /// Tests that LCColumns are read from cache
        /// </summary>
        [Test]
       public void TestS()
        {
            List<classLCColumn> cols = classSQLiteTools.GetEntireLCColumnList();
            Assert.AreEqual(1, cols.Count);
            Assert.IsTrue(cols[0].LCColumn == "ColTest1");
        }
    }
}
