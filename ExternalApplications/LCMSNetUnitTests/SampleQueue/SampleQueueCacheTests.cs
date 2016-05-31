using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LCMSNetUnitTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            //
            // TODO: Add test logic	here
            //
        }

        /*
        #region Testing
        private void button1_Click(object sender, EventArgs e)
        {
            mobj_sampleQueue.CacheQueue();
            MessageBox.Show("All queues dumped to cache");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mobj_sampleQueue.RetrieveQueueFromCache();
            MessageBox.Show("All queues retrieved from cache");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<classUserInfo> userList = LcmsNetDmsTools.classSQLiteTools.GetUserList();
            listView1.Items.Clear();
            foreach (classUserInfo User in userList)
            {
                ListViewItem NewUser = new ListViewItem(User.UserName);
                NewUser.SubItems.Add(User.PayrollNum);
                listView1.Items.Add(NewUser);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<classInstrumentInfo> instList = LcmsNetDmsTools.classSQLiteTools.GetInstrumentList();
            listView1.Items.Clear();
            foreach (classInstrumentInfo inst in instList)
            {
                ListViewItem NewInst = new ListViewItem(inst.DMSName);
                NewInst.SubItems.Add(inst.CommonName);
                listView1.Items.Add(NewInst);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            List<string> CartNames = LcmsNetDmsTools.classSQLiteTools.GetCartNameList();
            listView2.Items.Clear();
            foreach (string cart in CartNames)
            {
                ListViewItem newCart = new ListViewItem(cart);
                listView2.Items.Add(newCart);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<string> dsTypes = LcmsNetDmsTools.classSQLiteTools.GetDatasetTypeList();
            listView2.Items.Clear();
            foreach (string dsType in dsTypes)
            {
                ListViewItem newType = new ListViewItem(dsType);
                listView2.Items.Add(newType);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            List<string> sepTypes = LcmsNetDmsTools.classSQLiteTools.GetSepTypeList();
            listView2.Items.Clear();
            foreach (string sepType in sepTypes)
            {
                ListViewItem newType = new ListViewItem(sepType);
                listView2.Items.Add(newType);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            List<string> colNames = LcmsNetDmsTools.classSQLiteTools.GetColumnList();
            listView2.Items.Clear();
            foreach (string colName in colNames)
            {
                ListViewItem newColName = new ListViewItem(colName);
                listView2.Items.Add(newColName);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            classSampleData tempSample = mobj_sampleQueue.GetNextSample();
            classTriggerFileTools.GenerateTriggerFile(tempSample);
        }

        private void buttonTrayVialTest_Click(object sender, EventArgs e)
        {
            List<classSampleData> samples = mobj_sampleQueue.GetWaitingQueue();
            List<string> trayNames = new List<string>();
            for (int indx = 0; indx < 6; indx++)
            {
                trayNames.Add("Tray" + (indx + 1).ToString());
            }
            formTrayVialAssignment tempTrayForm = new formTrayVialAssignment();
            tempTrayForm.LoadSampleList(trayNames, samples);
            tempTrayForm.ShowDialog();
            if (tempTrayForm.DialogResult == DialogResult.OK) mobj_sampleQueue.UpdateSamples(tempTrayForm.SampleList);
        }
        #endregion    
         * */
    }
}
