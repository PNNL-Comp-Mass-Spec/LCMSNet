using System;
using System.Collections.Generic;
using DemoPluginLibrary;
using LcmsNetSDK.Method;
using NUnit.Framework;

namespace LcmsnetUnitTest
{
    /// <summary>
    /// Class that tests the LCMethod Optimizer, which should ensure that all methods will run without interfering with one another,
    /// while still running as much concurrently as possible.
    /// </summary>
    [TestFixture]
    public class OptimizerUnitTests
    {
        List<LCMethod> methods;
        LCMethodOptimizer optimizer;
        const int SAME_REQUIRED_LC_METHOD_OFFSET = 2000; // this should be the same as CONST_REQUIRED_LC_METHOD_SPACING_SECONDS in classLCMethodOptimizer, if the tests are not passing be sure to check this.

        /// <summary>
        /// Setup methods and optimizer for use in tests.
        /// </summary>
        [SetUp]
        public void SetupOptimizerAndMethodsForOptimizerTests()
        {
            optimizer = new LCMethodOptimizer();

            methods = new List<LCMethod> { new LCMethod(), new LCMethod() };

            //events for the methods
            LCEvent[] events = { new LCEvent(), new LCEvent() };

            var pump = new DemoPump();
            events[0].Device = pump;
            events[1].Device = pump;

            events[0].HadError = false;
            events[0].Name = "SetFlowRate";
            events[0].Duration = new TimeSpan(0, 0, 1);
            events[0].Parameters = new object[1];
            events[0].Parameters[0] = 1;
            events[0].ParameterNames = new string[1];
            events[0].ParameterNames[0] = "rate";
            events[0].MethodAttribute = new LCMethodEventAttribute("SetFlowRate", 1.00);
            events[0].Method = events[0].Device.GetType().GetMethod("SetFlowRate");
            methods[0].Events.Add(events[0]);


            events[1].HadError = false;
            events[1].Name = "SetFlowRate";
            events[1].Duration = new TimeSpan(0, 0, 1);
            events[1].Parameters = new object[1];
            events[1].Parameters[0] = 1;
            events[1].ParameterNames = new string[1];
            events[1].ParameterNames[0] = "rate";
            events[1].MethodAttribute = new LCMethodEventAttribute("SetFlowRate", 1.00);
            events[1].Method = events[1].Device.GetType().GetMethod("SetFlowRate");
            events[1].Start = events[0].End;
            methods[1].Events.Add(events[1]);

            methods[0].Column = 0;
            methods[1].Column = 0;
            methods[0].SetStartTime(DateTime.Now);
            methods[1].SetStartTime(DateTime.Now);
            methods[0].AllowPostOverlap = true;
            methods[0].AllowPreOverlap = true;
            methods[1].AllowPostOverlap = true;
            methods[1].AllowPreOverlap = true;
        }

        /// <summary>
        /// used for all tests, since the only difference we care about is that the start time of method 2 is different from method 1.
        /// Each individual test determines its own expected difference that ensures the optimization is correct.
        /// </summary>
        /// <param name="expectedDifference"></param>
        private void CheckDifferenceInStartTimes(double expectedDifference)
        {
            var difference = methods[1].Start.Subtract(methods[0].Start).TotalMilliseconds;
            Assert.AreEqual(expectedDifference, difference);
        }

        /// <summary>
        /// Test to see if the optimizer properly handles a DST transition that happens between two methods.
        /// </summary>
        [Test]
        public void TestDSTTransition()
        {
            methods[0].SetStartTime(new DateTime(2014, 11, 2, 1, 59, 59));
            methods[1].SetStartTime(new DateTime(2014, 11, 2, 2, 0, 10));
            methods[0].Name = "Zero";
            methods[1].Name = "One";
            optimizer.AlignMethods(methods[0], methods[1], true);
            var expectedDifference = new TimeSpan(1, 0, 1).TotalMilliseconds;
            CheckDifferenceInStartTimes(expectedDifference);
        }

        [Test]
        public void TestNoDSTTransition()
        {
            methods[0].SetStartTime(new DateTime(2014, 11, 2, 1, 58, 0));
            methods[1].SetStartTime(new DateTime(2014, 11, 2, 1, 58, 10));
            methods[0].Name = "Zero";
            methods[1].Name = "One";
            optimizer.AlignMethods(methods[0], methods[1], false);
            var expectedDifference = new TimeSpan(0, 0, 10).TotalMilliseconds;
            CheckDifferenceInStartTimes(expectedDifference);
        }

        /// <summary>
        /// Test optimizer to ensure that it is properly aligning methods when they are on different columns, with overlap disallowed, and the methods are using
        /// different devices.
        /// This test should make the second method start after the first one completes(with no additional wait time)
        /// </summary>
        [Test]
        public void TwoMethodsOnDifferentColumnsWithoutOverlapDifferentDevice()
        {
            methods[0].AllowPostOverlap = false;
            methods[0].AllowPreOverlap = false;
            //Changing method[1], Event[0] to use a different device.
            var valve = new DemoValve();
            methods[1].Events[0].Device = valve;
            methods[1].Events[0].HadError = false;
            methods[1].Events[0].Name = "SetPosition";
            methods[1].Events[0].Duration = new TimeSpan(0, 0, 1);
            methods[1].Events[0].ParameterNames = new string[1];
            methods[1].Events[0].ParameterNames[0] = "SetPosition";
            methods[1].Events[0].Parameters = new object[1];
            methods[1].Events[0].Parameters[0] = 2;
            methods[1].Events[0].MethodAttribute = new LCMethodEventAttribute("SetPosition", 1.00);
            methods[1].Events[0].Method = valve.GetType().GetMethod("SetPosition");
            methods[1].Column = 0;
            optimizer.AlignMethods(methods);
            var expectedDifference = methods[1].Start.Subtract(methods[0].Start).TotalMilliseconds;
            CheckDifferenceInStartTimes(expectedDifference);
        }

        /// <summary>
        /// Test the optimizer to ensure that it is properly aligning methods when they are on different columns, with overlap disallowed, and both
        /// methods are using the same device.
        /// This test should make the second method start after the first one completes(plus SAME_REQUIRED_LC_METHOD_OFFSET
        /// </summary>
        [Test]
        public void TwoMethodsOnDifferentColumnsWithOutOverlapSameDevice()
        {
            methods[0].AllowPreOverlap = false;
            methods[0].AllowPostOverlap = false;
            methods[1].Column = 1;
            optimizer.AlignMethods(methods);
            var expectedDifference = methods[0].Events[0].Duration.TotalMilliseconds + SAME_REQUIRED_LC_METHOD_OFFSET;
            CheckDifferenceInStartTimes(expectedDifference);
        }

        /// <summary>
        /// Test the optimizer to ensure that it is properly aligning methods when they are on different columns, with overlap, and the methods
        /// using different devices.
        /// This test should make the methods run concurrently(they should start at the same time)
        /// </summary>
        [Test]
        public void TwoMethodsOnDifferentColumnsWithOverlapDifferentDevice()
        {
            //Changing method[1], Event[0] to use a different device.
            var valve = new DemoValve();
            methods[1].Events[0].Device = valve;
            methods[1].Events[0].HadError = false;
            methods[1].Events[0].Name = "SetPosition";
            methods[1].Events[0].Duration = new TimeSpan(0, 0, 1);
            methods[1].Events[0].ParameterNames = new string[1];
            methods[1].Events[0].ParameterNames[0] = "SetPosition";
            methods[1].Events[0].Parameters = new object[1];
            methods[1].Events[0].Parameters[0] = 2;
            methods[1].Events[0].MethodAttribute = new LCMethodEventAttribute("SetPosition", 1.00);
            methods[1].Events[0].Method = valve.GetType().GetMethod("SetPosition");
            methods[1].Column = 1;
            optimizer.AlignMethods(methods);
            double expectedDifference = 0;
            CheckDifferenceInStartTimes(expectedDifference);
        }

        /// <summary>
        /// Test the optimizer to ensure that it is properly aligning methods when they are on the different columns, with overlap allowed, and both
        /// methods using the same device.
        /// This test should make the second method run after the first completes(with no additional wait time)
        /// </summary>
        [Test]
        public void TwoMethodsOnDifferentColumnsWithOverlapSameDevice()
        {
            methods[1].Column = 1;
            optimizer.AlignMethods(methods);
            var expectedDifference = methods[0].Events[0].Duration.TotalMilliseconds;
            CheckDifferenceInStartTimes(expectedDifference);
        }

        /// <summary>
        /// Test the optimizer to ensure that it is properly aligning methods when they are on the same column with overlap disallowed, and both
        /// methods are using the same device.
        /// This test should make the second method run after the first completes(with an additional SAME_REQUIRED_LC_METHOD_OFFSET wait time)
        /// </summary>
        [Test]
        public void TwoMethodsOnSameColumnWithOutOverlapSameDevice()
        {
            methods[0].AllowPreOverlap = false;
            methods[0].AllowPostOverlap = false;
            optimizer.AlignMethods(methods);
            var expectedDifference = methods[0].Events[0].Duration.TotalMilliseconds + SAME_REQUIRED_LC_METHOD_OFFSET;
            CheckDifferenceInStartTimes(expectedDifference);
        }

        /// <summary>
        /// Test the optimizer to ensure that it is properly aligning methods, when they are on the same column with overlap allowed, and both
        /// methods are using the same device.
        /// This test should make the second method run after the first method completes(with an additional SAME_REQUIRED_LC_METHOD_OFFSET wait time)
        /// </summary>
        [Test]
        public void TwoMethodsOnSameColumnWithOverlapSameDevice()
        {
            optimizer.AlignMethods(methods);
            var expectedDifference = methods[0].Events[0].Duration.TotalMilliseconds + SAME_REQUIRED_LC_METHOD_OFFSET;
            CheckDifferenceInStartTimes(expectedDifference);
        }

        /// <summary>
        /// Test the optimizer to ensure that it is properly aligning methods when they are on the same column, without overlap, and the methods
        /// are using different devices.
        /// This test should make the second method run after the first completes(with an additional SAME_REQUIRED_LC_METHOD_OFFSET wait time)
        /// </summary>
        [Test]
        public void TwoMethodsOnTheSameColumnWithoutOverlapDifferentDevices()
        {
            methods[0].AllowPreOverlap = false;
            methods[0].AllowPostOverlap = false;
            //Changing method[1], Event[0] to use a different device.
            var valve = new DemoValve();
            methods[1].Events[0].Device = valve;
            methods[1].Events[0].HadError = false;
            methods[1].Events[0].Name = "SetPosition";
            methods[1].Events[0].Duration = new TimeSpan(0, 0, 1);
            methods[1].Events[0].ParameterNames = new string[1];
            methods[1].Events[0].ParameterNames[0] = "SetPosition";
            methods[1].Events[0].Parameters = new object[1];
            methods[1].Events[0].Parameters[0] = 2;
            methods[1].Events[0].MethodAttribute = new LCMethodEventAttribute("SetPosition", 1.00);
            methods[1].Events[0].Method = valve.GetType().GetMethod("SetPosition");
            methods[1].Column = 0;
            optimizer.AlignMethods(methods);
            var expectedDifference = methods[0].Events[0].Duration.TotalMilliseconds + SAME_REQUIRED_LC_METHOD_OFFSET;
            CheckDifferenceInStartTimes(expectedDifference);
        }

        /// <summary>
        /// Test the optimizer to ensure that it is properly aligning methods when they are on the same column, with overlap, and the methods
        /// are using different devices.
        /// This test should make the second method run after the first method completes(with an additional SAME_REQUIRED_LC_METHOD_OFFSET wait time)
        /// </summary>
        [Test]
        public void TwoMethodsOnTheSameColumnWithOverlapDifferentDevices()
        {
            //Changing method[1], Event[0] to use a different device.
            var valve = new DemoValve();
            methods[1].Events[0].Device = valve;
            methods[1].Events[0].HadError = false;
            methods[1].Events[0].Name = "SetPosition";
            methods[1].Events[0].Duration = new TimeSpan(0, 0, 1);
            methods[1].Events[0].ParameterNames = new string[1];
            methods[1].Events[0].ParameterNames[0] = "SetPosition";
            methods[1].Events[0].Parameters = new object[1];
            methods[1].Events[0].Parameters[0] = 2;
            methods[1].Events[0].MethodAttribute = new LCMethodEventAttribute("SetPosition", 1.00);
            methods[1].Events[0].Method = valve.GetType().GetMethod("SetPosition");
            methods[1].Column = 0;
            optimizer.AlignMethods(methods);
            var expectedDifference = methods[0].Events[0].Duration.TotalMilliseconds + SAME_REQUIRED_LC_METHOD_OFFSET;
            CheckDifferenceInStartTimes(expectedDifference);
        }
    }
}