/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 10/25/2013
 *
 * Last Modified 11/8/2013 By Christopher Walters
 ********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using LcmsNetSDK;
using LcmsNetDataClasses;
using System.Drawing;
using System.IO;
using TestPluginLibrary;
using Eksigent;
using PDFGenerator;
using LcmsNet.SampleQueue;
using LcmsNet.Method;
using LcmsNet.Configuration;
using LcmsNetDataClasses.Configuration;
using System.Threading;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Devices;

namespace PDFGeneratorTests
{
    [TestFixture]
    class PDFGenTests
    {
        string docPath;
        classSampleData sample;
        List<LcmsNetDataClasses.Configuration.classColumnData> cd;
        Bitmap fimage;
        List<IDevice> devices;

        [SetUp]
        public void SetupTests()
        {
            sample = new classSampleData();
            sample.LCMethod = new LcmsNetDataClasses.Method.classLCMethod();
            sample.LCMethod.Name = "LCMethod9";
            sample.LCMethod.Column = 1;
            sample.LCMethod.SetStartTime(DateTime.Now.AddSeconds(10));
            sample.LCMethod.Events.Add(new LcmsNetDataClasses.Method.classLCEvent());
            sample.LCMethod.Events.Add(new LcmsNetDataClasses.Method.classLCEvent());
            TestPump testPump = new TestPump();
            testPump.Emulation = true;
            testPump.Status = LcmsNetDataClasses.Devices.enumDeviceStatus.Initialized;
            testPump.ErrorType = LcmsNetDataClasses.Devices.enumDeviceErrorStatus.NoError;
            string err = "Derpy error";
            testPump.Initialize(ref err);
            LcmsNetDataClasses.Devices.IDevice testValve = new TestValve();
            testValve.Emulation = true;
            testValve.Status = LcmsNetDataClasses.Devices.enumDeviceStatus.Initialized;
            testValve.ErrorType = LcmsNetDataClasses.Devices.enumDeviceErrorStatus.NoError;
            classLCMSSettings.SetParameter("CreateMethodFolders", "false");
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
            sample.LCMethod.Events[0].MethodAttribute = new classLCMethodAttribute("Start Method", 10000.00, string.Empty, -1, false);
            sample.LCMethod.Events[0].Method = testPump.GetType().GetMethod("StartMethod");

            sample.LCMethod.Events[1].Device = testValve;
            sample.LCMethod.Events[1].HadError = false;
            sample.LCMethod.Events[1].Name = "SetPosition";
            sample.LCMethod.Events[1].Duration = new TimeSpan(0, 0, 10);
            sample.LCMethod.Events[1].ParameterNames = new string[1];
            sample.LCMethod.Events[1].ParameterNames[0] = "newPosition";
            sample.LCMethod.Events[1].Parameters = new object[1];
            sample.LCMethod.Events[1].Parameters[0] = FluidicsSDK.Base.TwoPositionState.PositionB;
            sample.LCMethod.Events[1].MethodAttribute = new classLCMethodAttribute("SetPostion", 10000.00, string.Empty, -1, false);
            sample.LCMethod.Events[1].Method = testValve.GetType().GetMethod("SetPosition");
            sample.Operator = "TestOp";
            sample.Volume = 5;

            sample.ColumnData = new LcmsNetDataClasses.Configuration.classColumnData();
            sample.ColumnData.ID = 1;
            sample.ColumnData.Name = "Column1";
            sample.ColumnData.Status = LcmsNetDataClasses.Configuration.enumColumnStatus.Idle;
            sample.ColumnData.Color = Color.Red;

            sample.DmsData = new classDMSData();
            sample.DmsData.RequestName = "TestReq";
            sample.DmsData.RequestID = 9001;
            sample.DmsData.ProposalID = "A1009Z";
            sample.DmsData.Batch = 42;
            sample.DmsData.Block = 24;
            sample.DmsData.Comment = "This is only a test.";
            sample.DmsData.CartName = "thatcart";
            sample.DmsData.DatasetType = "HMS";
            sample.DmsData.DatasetName = "test dataset";
            sample.RunningStatus = enumSampleRunningStatus.WaitingToRun;

            sample.PAL.PALTray = "Tray1";
            classLCMSSettings.SetParameter("InstName", "Mass Spec Test.1");

            devices = new List<IDevice>();
            devices.Add(testPump);
            devices.Add(testValve);
            docPath = Path.Combine(@"D:\", "");
            fimage = (Bitmap)Bitmap.FromFile(Path.Combine(docPath, "test.bmp"));
            cd = new List<LcmsNetDataClasses.Configuration.classColumnData>();
            for (int i = 0; i < 4; i++)
            {
                LcmsNetDataClasses.Configuration.classColumnData dat = new LcmsNetDataClasses.Configuration.classColumnData();
                dat.Name = "Column " + i.ToString();
                dat.ID = i;
                cd.Add(dat);
            }
        }

        [Test]
        public void PDFGenTest()
        {
            /* create test data and then create a pdf*/
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
            List<classColumnData> f = new List<classColumnData>();
            f.Add(sample.ColumnData);
            classCartConfiguration.Columns = f;
            classSampleQueue queue = new classSampleQueue();
            List<classSampleData> d = new List<classSampleData>();
            d.Add(sample);
            classLCMethodManager.Manager.Methods.Add(sample.LCMethod.Name, sample.LCMethod);
            classLCMethodScheduler sched = new classLCMethodScheduler(queue);
            sched.SampleProgress += WritePDFOnMethodComplete;
            sched.Initialize();
            queue.QueueSamples(d, enumColumnDataHandling.CreateUnused);
            queue.GetNextSample();
            queue.StartSamples();
            Thread.Sleep(new TimeSpan(0, 0, 40));
            Assert.IsTrue(File.Exists(file));
        }


        public void WritePDFOnMethodComplete(object sender, classSampleProgressEventArgs e)
        {
            IPDF pg = new PDFGen();
            if (e.ProgressType == enumSampleProgress.Complete)
            {
                pg.WritePDF(Path.Combine(docPath, "pdfIntegrationTest.pdf"), "PDFIntegrationTest", sample, classCartConfiguration.NumberOfEnabledColumns.ToString(), classCartConfiguration.Columns, devices, fimage);
            }
        }
    }
}