/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 * Created 6/5/2014
 *
 * Last Modified 6/5/2014 By Christopher Walters
 ********************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using LcmsNet.SampleQueue;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses;
using LcmsNet.Method;
using LcmsNet.Configuration;
using LcmsNetDataClasses.Configuration;
using System.Drawing;
using DemoPluginLibrary;
using System.Reflection;

namespace LcmsnetUnitTest
{
    [TestFixture]
    public class SampleQueueTests
    {
        [SetUp]
        public void SetUp()
        {
            if (q == null)
            {
                //
                // Create system data with columns.
                //
                var columnOne = new classColumnData
                {
                    ID = 0,
                    Name = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_COLUMNNAME0),
                    Status = enumColumnStatus.Idle,
                    Color = Color.Tomato,
                    First = true
                };

                var columnTwo = new classColumnData
                {
                    ID = 1,
                    Name = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_COLUMNNAME1),
                    Status = enumColumnStatus.Idle,
                    Color = Color.Lime
                };

                var columnThree = new classColumnData
                {
                    ID = 2,
                    Name = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_COLUMNNAME2),
                    Status = enumColumnStatus.Idle,
                    Color = Color.LightSteelBlue
                };

                var columnFour = new classColumnData
                {
                    ID = 3,
                    Name = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_COLUMNNAME3),
                    Status = enumColumnStatus.Idle,
                    Color = Color.LightSalmon
                };

                classCartConfiguration.Columns = new List<classColumnData> {
                    columnOne, columnTwo, columnThree, columnFour};

                optimizer = new classLCMethodOptimizer();
            }
            q = new classSampleQueue();
            samples = new List<classSampleData>();
        }

        classLCMethodOptimizer optimizer = null;
        classSampleQueue q = null;
        List<classSampleData> samples = null;


        [Test]
        public void TestSampleQueueAddToQueue()
        {
            classSampleData sampleA = new classSampleData();
            classSampleData sampleB = new classSampleData();

            classLCEvent lcEvent = new classLCEvent(); // use the same event for both samples LCMethod.
            DemoPump pump = new DemoPump();
            lcEvent.Device = pump;
            lcEvent.Duration = new TimeSpan(0, 0, 0, 1);
            lcEvent.HadError = false;
            lcEvent.HasDiscreteStates = false;
            lcEvent.IsIndeterminant = false;
            lcEvent.Name = "SetFlowRate";
            lcEvent.Duration = new TimeSpan(0, 0, 1);
            lcEvent.Parameters = new object[1];
            lcEvent.Parameters[0] = 1;
            lcEvent.ParameterNames = new string[1];
            lcEvent.ParameterNames[0] = "rate";
            lcEvent.MethodAttribute = new classLCMethodAttribute("SetFlowRate", 1.00, string.Empty, -1, false);
            lcEvent.Method = pump.GetType().GetMethod("SetFlowRate");

            sampleA.ColumnData.ID = 0;
            sampleA.LCMethod = new classLCMethod();
            sampleA.LCMethod.Events.Add(lcEvent);
            sampleA.LCMethod.Column = 0;
            sampleA.Volume = 5;
            sampleA.SequenceID = 1;
            sampleA.DmsData.DatasetType = "HMS";
            sampleA.DmsData.DatasetName = "testDataset";
            sampleA.DmsData.CartName = "Emulated";

            classLCEvent lcEvent2 = lcEvent.Clone() as classLCEvent;

            sampleB.ColumnData.ID = 0;
            sampleB.LCMethod = new classLCMethod();
            sampleB.LCMethod.Events.Add(lcEvent2);
            sampleB.LCMethod.Column = 0;
            sampleB.Volume = 5;
            sampleB.SequenceID = 2;
            sampleB.DmsData.DatasetType = "HMS";
            sampleB.DmsData.DatasetName = "testDataset2";
            sampleB.DmsData.CartName = "Emulated";

            samples.Add(sampleA);
            samples.Add(sampleB);

            optimizer.AlignSamples(samples);
            q.QueueSamples(samples, enumColumnDataHandling.LeaveAlone);
            Assert.IsTrue(q.GetWaitingQueue().Contains(sampleA) && q.GetWaitingQueue().Contains(sampleB));
        }

        [Test]
        public void TestSampleQueueQuery()
        {
            classSampleData sampleA = new classSampleData();
            classSampleData sampleB = new classSampleData();

            classLCEvent lcEvent = new classLCEvent(); // use the same event for both samples LCMethod.
            DemoPump pump = new DemoPump();
            lcEvent.Device = pump;
            lcEvent.Duration = new TimeSpan(0, 0, 0, 1);
            lcEvent.HadError = false;
            lcEvent.HasDiscreteStates = false;
            lcEvent.IsIndeterminant = false;
            lcEvent.Name = "SetFlowRate";
            lcEvent.Duration = new TimeSpan(0, 0, 1);
            lcEvent.Parameters = new object[1];
            lcEvent.Parameters[0] = 1;
            lcEvent.ParameterNames = new string[1];
            lcEvent.ParameterNames[0] = "rate";
            lcEvent.MethodAttribute = new classLCMethodAttribute("SetFlowRate", 1.00, string.Empty, -1, false);
            lcEvent.Method = pump.GetType().GetMethod("SetFlowRate");

            sampleA.ColumnData.ID = 0;
            sampleA.LCMethod = new classLCMethod();
            sampleA.LCMethod.Events.Add(lcEvent);
            sampleA.LCMethod.Column = 0;
            sampleA.Volume = 5;
            sampleA.SequenceID = 1;
            sampleA.DmsData.DatasetType = "HMS";
            sampleA.DmsData.DatasetName = "testDataset";
            sampleA.DmsData.CartName = "Emulated";

            classLCEvent lcEvent2 = lcEvent.Clone() as classLCEvent;

            sampleB.ColumnData.ID = 0;
            sampleB.LCMethod = new classLCMethod();
            sampleB.LCMethod.Events.Add(lcEvent2);
            sampleB.LCMethod.Column = 0;
            sampleB.Volume = 5;
            sampleB.SequenceID = 2;
            sampleB.DmsData.DatasetType = "HMS";
            sampleB.DmsData.DatasetName = "testDataset2";
            sampleB.DmsData.CartName = "Emulated";

            samples.Add(sampleA);
            samples.Add(sampleB);

            optimizer.AlignSamples(samples);
            q.QueueSamples(samples, enumColumnDataHandling.LeaveAlone);
            q.MoveSamplesToRunningQueue(samples);
            q.StartSamples();

            //ensure that the query returns the sample that will actually be returned to run
            Assert.AreSame(q.NextSampleQuery(), q.NextSampleStart());
            q.StopRunningQueue();
            List<long> ids = new List<long>();
            ids.Add(sampleA.UniqueID);
            ids.Add(sampleB.UniqueID);
            q.RemoveSample(ids, enumColumnDataHandling.LeaveAlone);
        }

        /// <summary>
        /// This test ensures that the next sample to be run is the sample that SHOULD be next.
        /// </summary>
        [Test]
        public void TestSampleQueueReturnCorrectSample()
        {
            classSampleData sampleA = new classSampleData();
            classSampleData sampleB = new classSampleData();

            classLCEvent lcEvent = new classLCEvent(); // use the same event for both samples LCMethod.
            DemoPump pump = new DemoPump();
            lcEvent.Device = pump;
            lcEvent.Duration = new TimeSpan(0, 0, 0, 1);
            lcEvent.HadError = false;
            lcEvent.HasDiscreteStates = false;
            lcEvent.IsIndeterminant = false;
            lcEvent.Name = "SetFlowRate";
            lcEvent.Duration = new TimeSpan(0, 0, 1);
            lcEvent.Parameters = new object[1];
            lcEvent.Parameters[0] = 1;
            lcEvent.ParameterNames = new string[1];
            lcEvent.ParameterNames[0] = "rate";
            lcEvent.MethodAttribute = new classLCMethodAttribute("SetFlowRate", 1.00, string.Empty, -1, false);
            lcEvent.Method = pump.GetType().GetMethod("SetFlowRate");

            sampleA.ColumnData.ID = 0;
            sampleA.LCMethod = new classLCMethod();
            sampleA.LCMethod.Events.Add(lcEvent);
            sampleA.LCMethod.Column = 0;
            sampleA.Volume = 5;
            sampleA.SequenceID = 1;
            sampleA.DmsData.DatasetType = "HMS";
            sampleA.DmsData.DatasetName = "testDataset";
            sampleA.DmsData.CartName = "Emulated";

            classLCEvent lcEvent2 = lcEvent.Clone() as classLCEvent;

            sampleB.ColumnData.ID = 0;
            sampleB.LCMethod = new classLCMethod();
            sampleB.LCMethod.Events.Add(lcEvent2);
            sampleB.LCMethod.Column = 0;
            sampleB.Volume = 5;
            sampleB.SequenceID = 2;
            sampleB.DmsData.DatasetType = "HMS";
            sampleB.DmsData.DatasetName = "testDataset2";
            sampleB.DmsData.CartName = "Emulated";

            samples.Add(sampleA);
            samples.Add(sampleB);

            optimizer.AlignSamples(samples);
            q.QueueSamples(samples, enumColumnDataHandling.LeaveAlone);
            q.MoveSamplesToRunningQueue(samples);
            q.StartSamples();

            //Sample A should be run first.
            Assert.AreSame(sampleA, q.NextSampleStart());
            q.StopRunningQueue();
            List<long> ids = new List<long>();
            ids.Add(sampleA.UniqueID);
            ids.Add(sampleB.UniqueID);
            q.RemoveSample(ids, enumColumnDataHandling.LeaveAlone);
        }
    }
}