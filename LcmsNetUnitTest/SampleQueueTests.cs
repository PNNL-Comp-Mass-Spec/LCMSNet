using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using DemoPluginLibrary;
using LcmsNet.Configuration;
using LcmsNet.Data;
using LcmsNet.Method;
using LcmsNet.SampleQueue;
using LcmsNetSDK;
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
                // Create system data with columns.
                CartConfiguration.Initialize();

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
            lcEvent.MethodAttribute = new LCMethodEventAttribute("SetFlowRate", 1.00);
            lcEvent.Method = pump.GetType().GetMethod("SetFlowRate");
            var testLcMethod = new LCMethod()
            {
                Name = "TestLCMethodQueueAddToQueue",
                Column = 0
            };
            testLcMethod.Events.Add(lcEvent);
            LCMethodManager.Manager.AddOrUpdateMethod(testLcMethod);

            sampleA.LCMethodName = testLcMethod.Name;
            sampleA.ColumnIndex = 0;
            sampleA.Volume = 5;
            sampleA.SequenceID = 1;
            sampleA.Name = "testDataset";

            sampleA.LCMethodName = testLcMethod.Name;
            sampleB.ColumnIndex = 0;
            sampleB.Volume = 5;
            sampleB.SequenceID = 2;
            sampleB.Name = "testDataset2";

            samples.Add(sampleA);
            samples.Add(sampleB);

            optimizer.AlignSamples(samples.Cast<ISampleInfo>().ToList());
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
            lcEvent.MethodAttribute = new LCMethodEventAttribute("SetFlowRate", 1.00);
            lcEvent.Method = pump.GetType().GetMethod("SetFlowRate");
            var testLcMethod = new LCMethod()
            {
                Name = "TestLCMethodSampleQueueQuery",
                Column = 0
            };
            testLcMethod.Events.Add(lcEvent);
            LCMethodManager.Manager.AddOrUpdateMethod(testLcMethod);

            sampleA.LCMethodName = testLcMethod.Name;
            sampleA.ColumnIndex = 0;
            sampleA.Volume = 5;
            sampleA.SequenceID = 1;
            sampleA.Name = "testDataset";

            sampleA.LCMethodName = testLcMethod.Name;
            sampleB.ColumnIndex = 0;
            sampleB.Volume = 5;
            sampleB.SequenceID = 2;
            sampleB.Name = "testDataset2";

            samples.Add(sampleA);
            samples.Add(sampleB);

            optimizer.AlignSamples(samples.Cast<ISampleInfo>().ToList());
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
            lcEvent.MethodAttribute = new LCMethodEventAttribute("SetFlowRate", 1.00);
            lcEvent.Method = pump.GetType().GetMethod("SetFlowRate");
            var testLcMethod = new LCMethod()
            {
                Name = "TestLCMethodQueueReturnCorrectSample",
                Column = 0
            };
            testLcMethod.Events.Add(lcEvent);
            LCMethodManager.Manager.AddOrUpdateMethod(testLcMethod);

            sampleA.LCMethodName = testLcMethod.Name;
            sampleA.ColumnIndex = 0;
            sampleA.Volume = 5;
            sampleA.SequenceID = 1;
            sampleA.Name = "testDataset";

            sampleA.LCMethodName = testLcMethod.Name;
            sampleB.ColumnIndex = 0;
            sampleB.Volume = 5;
            sampleB.SequenceID = 2;
            sampleB.Name = "testDataset2";

            samples.Add(sampleA);
            samples.Add(sampleB);

            optimizer.AlignSamples(samples.Cast<ISampleInfo>().ToList());
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
