//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using System.Threading;
//using DemoPluginLibrary;
using NUnit.Framework;
//using LcmsNetSDK;
//using PDFGenerator;
//using LcmsNet.SampleQueue;
//using LcmsNet.Method;
//using LcmsNet.Configuration;
//using LcmsNetSDK.Devices;
//using LcmsNetSDK.Method;

// TODO: Has dependences on LCMSNetProg, and I don't want those in this unit test project
namespace PDFGeneratorTests
{
    [TestFixture]
    class PDFGenTests
    {
        /*
        string docPath;
        SampleData sample;
        List<ColumnData> cd;
        Bitmap fimage;
        List<IDevice> devices;

        [SetUp]
        public void SetupTests()
        {
            sample = new SampleData();
            sample.LCMethod = new LCMethod();
            sample.LCMethod.Name = "LCMethod9";
            sample.LCMethod.Column = 1;
            sample.LCMethod.SetStartTime(DateTime.Now.AddSeconds(10));
            sample.LCMethod.Events.Add(new LCEvent());
            sample.LCMethod.Events.Add(new LCEvent());
            var testPump = new DemoPump();
            testPump.Emulation = true;
            testPump.Status = DeviceStatus.Initialized;
            testPump.ErrorType = DeviceErrorStatus.NoError;
            string err = "Derpy error";
            testPump.Initialize(ref err);
            var testValve = new DemoValve();
            testValve.Emulation = true;
            testValve.Status = DeviceStatus.Initialized;
            testValve.ErrorType = DeviceErrorStatus.NoError;
            LCMSSettings.SetParameter("CreateMethodFolders", "false");
            sample.LCMethod.Events[0].Device = testPump;
            sample.LCMethod.Events[0].HadError = false;
            sample.LCMethod.Events[0].Name = "Start Method";
            sample.LCMethod.Events[0].ParameterNames = new string[3];
            sample.LCMethod.Events[0].ParameterNames[0] = "Timeout";
            sample.LCMethod.Events[0].ParameterNames[1] = "Channel";
            sample.LCMethod.Events[0].ParameterNames[2] = "Method Name";
            sample.LCMethod.Events[0].Parameters = new object[3];
            sample.LCMethod.Events[0].Parameters[0] = 10000;
            sample.LCMethod.Events[0].Parameters[1] = 1.0;
            sample.LCMethod.Events[0].Parameters[2] = "TestMethod";
            sample.LCMethod.Events[0].Duration = new TimeSpan(0, 0, 10);
            sample.LCMethod.Events[0].MethodAttribute = new LCMethodEventAttribute("Start Method", 10000.00);
            sample.LCMethod.Events[0].Method = testPump.GetType().GetMethod("StartMethod");

            sample.LCMethod.Events[1].Device = testValve;
            sample.LCMethod.Events[1].HadError = false;
            sample.LCMethod.Events[1].Name = "SetPosition";
            sample.LCMethod.Events[1].Duration = new TimeSpan(0, 0, 10);
            sample.LCMethod.Events[1].ParameterNames = new string[1];
            sample.LCMethod.Events[1].ParameterNames[0] = "newPosition";
            sample.LCMethod.Events[1].Parameters = new object[1];
            sample.LCMethod.Events[1].Parameters[0] = FluidicsSDK.Base.TwoPositionState.PositionB;
            sample.LCMethod.Events[1].MethodAttribute = new LCMethodEventAttribute("SetPostion", 10000.00);
            sample.LCMethod.Events[1].Method = testValve.GetType().GetMethod("SetPosition");
            sample.Operator = "TestOp";
            sample.Volume = 5;

            sample.ColumnData = new ColumnData();
            sample.ColumnData.ID = 1;
            sample.ColumnData.Name = "Column1";
            sample.ColumnData.Status = ColumnStatus.Idle;
            sample.ColumnData.Color = Color.Red;

            sample.DmsData = new DMSData();
            sample.DmsData.RequestName = "TestReq";
            sample.DmsData.RequestID = 9001;
            sample.DmsData.ProposalID = "A1009Z";
            sample.DmsData.Batch = 42;
            sample.DmsData.Block = 24;
            sample.DmsData.Comment = "This is only a test.";
            sample.DmsData.CartName = "thatcart";
            sample.DmsData.DatasetType = "HMS";
            sample.DmsData.DatasetName = "test dataset";
            sample.RunningStatus = SampleRunningStatus.WaitingToRun;

            sample.PAL.PALTray = "Tray1";
            LCMSSettings.SetParameter("InstName", "Mass Spec Test.1");

            devices = new List<IDevice>();
            devices.Add(testPump);
            devices.Add(testValve);
            docPath = Path.Combine(@"D:\", "");
            fimage = (Bitmap)Bitmap.FromFile(Path.Combine(docPath, "test.bmp"));
            cd = new List<ColumnData>();
            for (int i = 0; i < 4; i++)
            {
                ColumnData dat = new ColumnData();
                dat.Name = "Column " + i.ToString();
                dat.ID = i;
                cd.Add(dat);
            }
        }

        [Test]
        public void PDFGenTest()
        {
            // create test data and then create a pdf
            sample.LCMethod.ActualEnd = DateTime.Now.AddSeconds(30);
            IPDF pg = new PDFGen();
            string file = Path.Combine(docPath, "PDFGenTest.pdf");
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            pg.WritePDF(file, "PDFGenTest", sample, "4", cd, devices, fimage);
            Assert.IsTrue(File.Exists(file));
        }

        [Test]
        public void PDFIntegrationTest()
        {
            IPDF pg = new PDFGen();
            string file = Path.Combine(docPath, "PDFIntegrationTest.pdf");
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            List<ColumnData> f = new List<ColumnData>();
            f.Add(sample.ColumnData);
            CartConfiguration.Columns = f;
            SampleQueue queue = new SampleQueue();
            List<SampleData> d = new List<SampleData>();
            d.Add(sample);
            LCMethodManager.Manager.AddOrUpdateMethod(sample.LCMethod);
            LCMethodScheduler sched = new LCMethodScheduler(queue);
            sched.SampleProgress += WritePDFOnMethodComplete;
            sched.Initialize();
            queue.QueueSamples(d, ColumnDataHandling.CreateUnused);
            queue.GetNextSample();
            queue.StartSamples();
            Thread.Sleep(new TimeSpan(0, 0, 40));
            Assert.IsTrue(File.Exists(file));
        }


        public void WritePDFOnMethodComplete(object sender, SampleProgressEventArgs e)
        {
            IPDF pg = new PDFGen();
            if (e.ProgressType == SampleProgress.Complete)
            {
                pg.WritePDF(Path.Combine(docPath, "pdfIntegrationTest.pdf"), "PDFIntegrationTest", sample, CartConfiguration.NumberOfEnabledColumns.ToString(), CartConfiguration.Columns, devices, fimage);
            }
        }*/
    }
}
