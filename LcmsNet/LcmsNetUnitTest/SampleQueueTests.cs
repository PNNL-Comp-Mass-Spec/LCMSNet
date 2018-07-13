/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 * Created 6/5/2014
 *
 ********************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Media;
using DemoPluginLibrary;
using LcmsNet.SampleQueue;
using LcmsNetData;
using LcmsNetData.Configuration;
using LcmsNetData.Data;
using LcmsNetSDK.Data;
using LcmsNetSDK.Method;
using NUnit.Framework;

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
                var columnOne = new ColumnData
                {
                    ID = 0,
                    Name = LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNNAME0),
                    Status = ColumnStatus.Idle,
                    Color = Colors.Tomato,
                    First = true
                };

                var columnTwo = new ColumnData
                {
                    ID = 1,
                    Name = LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNNAME1),
                    Status =ColumnStatus.Idle,
                    Color = Colors.Lime
                };

                var columnThree = new ColumnData
                {
                    ID = 2,
                    Name = LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNNAME2),
                    Status =ColumnStatus.Idle,
                    Color = Colors.LightSteelBlue
                };

                var columnFour = new ColumnData
                {
                    ID = 3,
                    Name = LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNNAME3),
                    Status =ColumnStatus.Idle,
                    Color = Colors.LightSalmon
                };

                CartConfiguration.Columns = new List<ColumnData> {
                    columnOne, columnTwo, columnThree, columnFour};

                optimizer = new LCMethodOptimizer();
            }
            q = new SampleQueue();
            samples = new List<SampleData>();
        }

        LCMethodOptimizer optimizer;
        SampleQueue q;
        List<SampleData> samples;


        [Test]
        public void TestSampleQueueAddToQueue()
        {
            var sampleA = new SampleData();
            var sampleB = new SampleData();

            var lcEvent = new LCEvent(); // use the same event for both samples LCMethod.
            var pump = new DemoPump();
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
            lcEvent.MethodAttribute = new LCMethodEventAttribute("SetFlowRate", 1.00, string.Empty, -1, false);
            lcEvent.Method = pump.GetType().GetMethod("SetFlowRate");

            sampleA.ColumnData.ID = 0;
            sampleA.LCMethod = new LCMethod();
            sampleA.LCMethod.Events.Add(lcEvent);
            sampleA.LCMethod.Column = 0;
            sampleA.Volume = 5;
            sampleA.SequenceID = 1;
            sampleA.DmsData.DatasetType = "HMS";
            sampleA.DmsData.DatasetName = "testDataset";
            sampleA.DmsData.CartName = "Emulated";

            var lcEvent2 = lcEvent.Clone() as LCEvent;

            sampleB.ColumnData.ID = 0;
            sampleB.LCMethod = new LCMethod();
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
            var sampleA = new SampleData();
            var sampleB = new SampleData();

            var lcEvent = new LCEvent(); // use the same event for both samples LCMethod.
            var pump = new DemoPump();
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
            lcEvent.MethodAttribute = new LCMethodEventAttribute("SetFlowRate", 1.00, string.Empty, -1, false);
            lcEvent.Method = pump.GetType().GetMethod("SetFlowRate");

            sampleA.ColumnData.ID = 0;
            sampleA.LCMethod = new LCMethod();
            sampleA.LCMethod.Events.Add(lcEvent);
            sampleA.LCMethod.Column = 0;
            sampleA.Volume = 5;
            sampleA.SequenceID = 1;
            sampleA.DmsData.DatasetType = "HMS";
            sampleA.DmsData.DatasetName = "testDataset";
            sampleA.DmsData.CartName = "Emulated";

            var lcEvent2 = lcEvent.Clone() as LCEvent;

            sampleB.ColumnData.ID = 0;
            sampleB.LCMethod = new LCMethod();
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
            var ids = new List<long>();
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
            var sampleA = new SampleData();
            var sampleB = new SampleData();

            var lcEvent = new LCEvent(); // use the same event for both samples LCMethod.
            var pump = new DemoPump();
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
            lcEvent.MethodAttribute = new LCMethodEventAttribute("SetFlowRate", 1.00, string.Empty, -1, false);
            lcEvent.Method = pump.GetType().GetMethod("SetFlowRate");

            sampleA.ColumnData.ID = 0;
            sampleA.LCMethod = new LCMethod();
            sampleA.LCMethod.Events.Add(lcEvent);
            sampleA.LCMethod.Column = 0;
            sampleA.Volume = 5;
            sampleA.SequenceID = 1;
            sampleA.DmsData.DatasetType = "HMS";
            sampleA.DmsData.DatasetName = "testDataset";
            sampleA.DmsData.CartName = "Emulated";

            var lcEvent2 = lcEvent.Clone() as LCEvent;

            sampleB.ColumnData.ID = 0;
            sampleB.LCMethod = new LCMethod();
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
            var ids = new List<long>();
            ids.Add(sampleA.UniqueID);
            ids.Add(sampleB.UniqueID);
            q.RemoveSample(ids, enumColumnDataHandling.LeaveAlone);
        }
    }
}