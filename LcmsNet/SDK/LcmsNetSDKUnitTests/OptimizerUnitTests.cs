/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 * Created 5/30/2014
 * 
 * Last Modified 6/4/2014 By Christopher Walters 
 *********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using LcmsNet;
using LcmsNetDataClasses.Method;
using LcmsNet.Method;
using DemoPluginLibrary;

namespace LcmsnetUnitTest
{
    /// <summary>
    ///  class that tests the LCMethod Optimizer, which should ensure that all methods will run without interfering with one another, while still
    ///  running as much concurrently as possible.
    /// </summary>
    [TestFixture]
    public class OptimizerUnitTests
    {
        List<classLCMethod> methods;
        classLCMethodOptimizer optimizer;
        const int SAME_REQUIRED_LC_METHOD_OFFSET = 10000; // this should be the same as CONST_REQUIRED_LC_METHOD_SPACING_SECONDS in classLCMethodOptimizer, if the tests are not passing be sure to check this.
        
        /// <summary>
        /// Setup methods and optimizer for use in tests.
        /// </summary>
        [SetUp]
        public void SetupOptmizerAndMethodsForOptimizerTests()
        {
            optimizer = new classLCMethodOptimizer();   

            methods = new List<classLCMethod>();
            methods.Add(new classLCMethod());
            methods.Add(new classLCMethod());
            classLCEvent[] events = { new classLCEvent(), new classLCEvent() }; //events for the methods
            DemoPump pump = new DemoPump();
            events[0].Device = pump;
            events[1].Device = pump;

            events[0].HadError = false;
            events[0].Name = "SetFlowRate";
            events[0].Duration = new TimeSpan(0, 0, 1);
            events[0].Parameters = new object[1];
            events[0].Parameters[0] = 1;
            events[0].ParameterNames = new string[1];
            events[0].ParameterNames[0] = "rate";
            events[0].MethodAttribute = new classLCMethodAttribute("SetFlowRate", 1.00, string.Empty, -1, false);
            events[0].Method = events[0].Device.GetType().GetMethod("SetFlowRate");
            methods[0].Events.Add(events[0]);


            events[1].HadError = false;
            events[1].Name = "SetFlowRate";
            events[1].Duration = new TimeSpan(0, 0, 1);
            events[1].Parameters = new object[1];
            events[1].Parameters[0] = 1;
            events[1].ParameterNames = new string[1];
            events[1].ParameterNames[0] = "rate";
            events[1].MethodAttribute = new classLCMethodAttribute("SetFlowRate", 1.00, string.Empty, -1, false);
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
        /// Test the optimizer to ensure that it is properly aligning methods, when they are on the same column with overlap allowed, and both
        /// methods are using the same device.
        /// This test should make the second method run after the first method completes(with an additional 500ms wait time)
        /// </summary>
        [Test]
        public void TwoMethodsOnSameColumnWithOverlapSameDevice()
        {                      
            optimizer.AlignMethods(methods);
            double expectedDifference = methods[0].Events[0].Duration.TotalMilliseconds + SAME_REQUIRED_LC_METHOD_OFFSET;
            CheckDifferenceInStartTimes(expectedDifference);           
        }

        /// <summary>
        /// Test the optimizer to ensure that it is properly aligning methods when they are on the same column, with overlap, and the methods
        /// are using different devices.
        /// This test should make the second method run after the first method completes(with an additional 500ms wait time)
        /// </summary>
        [Test]
        public void TwoMethodsOnTheSameColumnWithOverlapDifferentDevices()
        {
            //Changing method[1], Event[0] to use a different device.
            DemoValve valve = new DemoValve();
            methods[1].Events[0].Device = valve;
            methods[1].Events[0].HadError = false;
            methods[1].Events[0].Name = "SetPosition";
            methods[1].Events[0].Duration = new TimeSpan(0, 0, 1);
            methods[1].Events[0].ParameterNames = new string[1];
            methods[1].Events[0].ParameterNames[0] = "SetPosition";
            methods[1].Events[0].Parameters = new object[1];
            methods[1].Events[0].Parameters[0] = 2;
            methods[1].Events[0].MethodAttribute = new classLCMethodAttribute("SetPostion", 1.00, string.Empty, -1, false);
            methods[1].Events[0].Method = valve.GetType().GetMethod("SetPosition");
            methods[1].Column = 0;
            optimizer.AlignMethods(methods);
            double expectedDifference = methods[0].Events[0].Duration.TotalMilliseconds + SAME_REQUIRED_LC_METHOD_OFFSET;
            CheckDifferenceInStartTimes(expectedDifference);
        }

        /// <summary>
        /// Test the optimizer to ensure that it is properly aligning methods when they are on the same column with overlap disallowed, and both 
        /// methods are using the same device.
        /// This test should make the second method run after the first completes(with an additional 500ms wait time)        
        /// </summary>
        [Test]
        public void TwoMethodsOnSameColumnWithOutOverlapSameDevice()
        {
            methods[0].AllowPreOverlap = false;
            methods[0].AllowPostOverlap = false;
            optimizer.AlignMethods(methods);
            double expectedDifference = methods[0].Events[0].Duration.TotalMilliseconds + SAME_REQUIRED_LC_METHOD_OFFSET;
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
            DemoValve valve = new DemoValve();
            methods[1].Events[0].Device = valve;
            methods[1].Events[0].HadError = false;
            methods[1].Events[0].Name = "SetPosition";
            methods[1].Events[0].Duration = new TimeSpan(0, 0, 1);
            methods[1].Events[0].ParameterNames = new string[1];
            methods[1].Events[0].ParameterNames[0] = "SetPosition";
            methods[1].Events[0].Parameters = new object[1];
            methods[1].Events[0].Parameters[0] = 2;
            methods[1].Events[0].MethodAttribute = new classLCMethodAttribute("SetPostion", 1.00, string.Empty, -1, false);
            methods[1].Events[0].Method = valve.GetType().GetMethod("SetPosition");
            methods[1].Column = 0;
            optimizer.AlignMethods(methods);
            double expectedDifference = methods[0].Events[0].Duration.TotalMilliseconds + SAME_REQUIRED_LC_METHOD_OFFSET;
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
            double expectedDifference = methods[0].Events[0].Duration.TotalMilliseconds;
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
            DemoValve valve = new DemoValve();
            methods[1].Events[0].Device = valve;
            methods[1].Events[0].HadError = false;
            methods[1].Events[0].Name = "SetPosition";
            methods[1].Events[0].Duration = new TimeSpan(0, 0, 1);
            methods[1].Events[0].ParameterNames = new string[1];
            methods[1].Events[0].ParameterNames[0] = "SetPosition";
            methods[1].Events[0].Parameters = new object[1];
            methods[1].Events[0].Parameters[0] = 2;
            methods[1].Events[0].MethodAttribute = new classLCMethodAttribute("SetPostion", 1.00, string.Empty, -1, false);
            methods[1].Events[0].Method = valve.GetType().GetMethod("SetPosition");
            methods[1].Column = 1;
            optimizer.AlignMethods(methods);
            double expectedDifference = 0;
            CheckDifferenceInStartTimes(expectedDifference);
        }
        
        /// <summary>
        /// Test the optimizer to ensure that it is properly aligning methods when they are on different columns, with overlap disallowed, and both
        /// methods are using the same device.
        /// This test should make the second method start after the first one completes(with no additional wait time)
        /// </summary>
        [Test]
        public void TwoMethodsOnDifferentColumnsWithOutOverlapSameDevice()
        {
            methods[0].AllowPreOverlap = false;
            methods[0].AllowPostOverlap = false;
            methods[1].Column = 1;
            optimizer.AlignMethods(methods);
            double expectedDifference = methods[0].Events[0].Duration.TotalMilliseconds;
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
            DemoValve valve = new DemoValve();
            methods[1].Events[0].Device = valve;
            methods[1].Events[0].HadError = false;
            methods[1].Events[0].Name = "SetPosition";
            methods[1].Events[0].Duration = new TimeSpan(0, 0, 1);
            methods[1].Events[0].ParameterNames = new string[1];
            methods[1].Events[0].ParameterNames[0] = "SetPosition";
            methods[1].Events[0].Parameters = new object[1];
            methods[1].Events[0].Parameters[0] = 2;
            methods[1].Events[0].MethodAttribute = new classLCMethodAttribute("SetPostion", 1.00, string.Empty, -1, false);
            methods[1].Events[0].Method = valve.GetType().GetMethod("SetPosition");
            methods[1].Column = 0;
            optimizer.AlignMethods(methods);
            double expectedDifference = methods[1].Start.Subtract(methods[0].Start).TotalMilliseconds;
            CheckDifferenceInStartTimes(expectedDifference);
        }

        /// <summary>
        /// Simulates adding samples to already running queue.
        /// specifically, the case in which multiple methods on the same column run with "overlap" settings.
        /// </summary>
        [Test]
        public void TestAddThirdMethodWhileQueueRunningSameColumnSameDeviceOverlap()
        {
            classLCMethod method3 = new classLCMethod();
            method3.Events = new List<classLCEvent>();
            method3.Events.Add(new classLCEvent());
            methods[0].AllowPostOverlap = true;
            methods[0].AllowPreOverlap = true;
            //Changing method[1], Event[0] to use a different device.  
            DemoValve valve = new DemoValve();
            methods[1].AllowPreOverlap = true;
            methods[1].AllowPostOverlap = true;
            methods[1].Events[0].Device = valve;
            methods[1].Events[0].HadError = false;
            methods[1].Events[0].Name = "SetPosition";
            methods[1].Events[0].Duration = new TimeSpan(0, 0, 1);
            methods[1].Events[0].ParameterNames = new string[1];
            methods[1].Events[0].ParameterNames[0] = "SetPosition";
            methods[1].Events[0].Parameters = new object[1];
            methods[1].Events[0].Parameters[0] = 2;
            methods[1].Events[0].MethodAttribute = new classLCMethodAttribute("SetPostion", 1.00, string.Empty, -1, false);
            methods[1].Events[0].Method = valve.GetType().GetMethod("SetPosition");
            methods[1].Column = 0;
            method3.AllowPostOverlap = true;
            method3.AllowPreOverlap = true;
            method3.Events[0].Device = valve;
            method3.Events[0].HadError = false;
            method3.Events[0].Name = "SetPosition";
            method3.Events[0].Duration = new TimeSpan(0, 0, 1);
            method3.Events[0].ParameterNames = new string[1];
            method3.Events[0].ParameterNames[0] = "SetPosition";
            method3.Events[0].Parameters = new object[1];
            method3.Events[0].Parameters[0] = 2;
            method3.Events[0].MethodAttribute = new classLCMethodAttribute("SetPostion", 1.00, string.Empty, -1, false);
            method3.Events[0].Method = valve.GetType().GetMethod("SetPosition");
            method3.Column = 0;
            optimizer.AlignMethods(methods);
            optimizer.AlignMethods(methods, method3);
            Assert.AreEqual(SAME_REQUIRED_LC_METHOD_OFFSET, method3.Start.Subtract(methods[1].End).TotalMilliseconds);            
        }

        /// <summary>
        /// Simulates adding samples to already running queue.
        /// specifically, the case in which multiple methods on different columns run with "overlap" settings.
        /// </summary>
        [Test]
        public void TestAddThirdMethodWhileQueueRunningDifferentColumnsSameDeviceOverlap()
        {
            classLCMethod method3 = new classLCMethod();
            method3.Events = new List<classLCEvent>();
            method3.Events.Add(new classLCEvent());
            methods[0].AllowPostOverlap = true;
            methods[0].AllowPreOverlap = true;
            //Changing method[1], Event[0] to use a different device.  
            DemoValve valve = new DemoValve();
            methods[1].AllowPreOverlap = true;
            methods[1].AllowPostOverlap = true;
            methods[1].Events[0].Device = valve;
            methods[1].Events[0].HadError = false;
            methods[1].Events[0].Name = "SetPosition";
            methods[1].Events[0].Duration = new TimeSpan(0, 0, 1);
            methods[1].Events[0].ParameterNames = new string[1];
            methods[1].Events[0].ParameterNames[0] = "SetPosition";
            methods[1].Events[0].Parameters = new object[1];
            methods[1].Events[0].Parameters[0] = 2;
            methods[1].Events[0].MethodAttribute = new classLCMethodAttribute("SetPostion", 1.00, string.Empty, -1, false);
            methods[1].Events[0].Method = valve.GetType().GetMethod("SetPosition");
            methods[1].Column = 2;
            method3.AllowPostOverlap = true;
            method3.AllowPreOverlap = true;
            method3.Events[0].Device = valve;
            method3.Events[0].HadError = false;
            method3.Events[0].Name = "SetPosition";
            method3.Events[0].Duration = new TimeSpan(0, 0, 1);
            method3.Events[0].ParameterNames = new string[1];
            method3.Events[0].ParameterNames[0] = "SetPosition";
            method3.Events[0].Parameters = new object[1];
            method3.Events[0].Parameters[0] = 2;
            method3.Events[0].MethodAttribute = new classLCMethodAttribute("SetPostion", 1.00, string.Empty, -1, false);
            method3.Events[0].Method = valve.GetType().GetMethod("SetPosition");
            method3.Column = 3;
            optimizer.AlignMethods(methods);
            optimizer.AlignMethods(methods, method3);
            System.Diagnostics.Debug.WriteLine(methods[0].End);
            System.Diagnostics.Debug.WriteLine(methods[1].Start);
            System.Diagnostics.Debug.WriteLine(methods[1].End);
            System.Diagnostics.Debug.WriteLine(method3.Start);
            Assert.AreEqual(0, method3.Start.Subtract(methods[1].End).TotalMilliseconds);
        }

        /// <summary>
        /// Simulates adding samples to already running queue.
        /// specifically, the case in which multiple methods on different columns run with  no"overlap" settings.
        /// </summary>
        [Test]
        public void TestAddThirdMethodWhileQueueRunningDifferentColumnsSameDeviceNoOverlap()
        {
            classLCMethod method3 = new classLCMethod();
            method3.Events = new List<classLCEvent>();
            method3.Events.Add(new classLCEvent());
            methods[0].AllowPostOverlap = false;
            methods[0].AllowPreOverlap = false;
            //Changing method[1], Event[0] to use a different device.  
            DemoValve valve = new DemoValve();
            methods[1].AllowPreOverlap = false;
            methods[1].AllowPostOverlap = false;
            methods[1].Events[0].Device = valve;
            methods[1].Events[0].HadError = false;
            methods[1].Events[0].Name = "SetPosition";
            methods[1].Events[0].Duration = new TimeSpan(0, 0, 1);
            methods[1].Events[0].ParameterNames = new string[1];
            methods[1].Events[0].ParameterNames[0] = "SetPosition";
            methods[1].Events[0].Parameters = new object[1];
            methods[1].Events[0].Parameters[0] = 2;
            methods[1].Events[0].MethodAttribute = new classLCMethodAttribute("SetPostion", 1.00, string.Empty, -1, false);
            methods[1].Events[0].Method = valve.GetType().GetMethod("SetPosition");
            methods[1].Column = 2;
            method3.AllowPostOverlap = false;
            method3.AllowPreOverlap = false;
            method3.Events[0].Device = valve;
            method3.Events[0].HadError = false;
            method3.Events[0].Name = "SetPosition";
            method3.Events[0].Duration = new TimeSpan(0, 0, 1);
            method3.Events[0].ParameterNames = new string[1];
            method3.Events[0].ParameterNames[0] = "SetPosition";
            method3.Events[0].Parameters = new object[1];
            method3.Events[0].Parameters[0] = 2;
            method3.Events[0].MethodAttribute = new classLCMethodAttribute("SetPostion", 1.00, string.Empty, -1, false);
            method3.Events[0].Method = valve.GetType().GetMethod("SetPosition");
            method3.Column = 3;
            optimizer.AlignMethods(methods);
            optimizer.AlignMethods(methods, method3);
            System.Diagnostics.Debug.WriteLine(methods[0].End);
            System.Diagnostics.Debug.WriteLine(methods[1].Start);
            System.Diagnostics.Debug.WriteLine(methods[1].End);
            System.Diagnostics.Debug.WriteLine(method3.Start);
            Assert.AreEqual(SAME_REQUIRED_LC_METHOD_OFFSET, method3.Start.Subtract(methods[1].End).TotalMilliseconds);
        }

        /// <summary>
        /// Simulates adding samples to already running queue.
        /// specifically, the case in which multiple methods on different columns run with  no"overlap" settings and different devices.
        /// </summary>
        [Test]
        public void TestAddThirdMethodWhileQueueRunningDifferentColumnsDifferentDeviceNoOverlap()
        {
            classLCMethod method3 = new classLCMethod();
            method3.Events = new List<classLCEvent>();
            method3.Events.Add(new classLCEvent());
            methods[0].AllowPostOverlap = false;
            methods[0].AllowPreOverlap = false;
            //Changing method[1], Event[0] to use a different device.  
            DemoValve valve = new DemoValve();
            methods[1].AllowPreOverlap = false;
            methods[1].AllowPostOverlap = false;
            methods[1].Events[0].Device = valve;
            methods[1].Events[0].HadError = false;
            methods[1].Events[0].Name = "SetPosition";
            methods[1].Events[0].Duration = new TimeSpan(0, 0, 1);
            methods[1].Events[0].ParameterNames = new string[1];
            methods[1].Events[0].ParameterNames[0] = "SetPosition";
            methods[1].Events[0].Parameters = new object[1];
            methods[1].Events[0].Parameters[0] = 2;
            methods[1].Events[0].MethodAttribute = new classLCMethodAttribute("SetPostion", 1.00, string.Empty, -1, false);
            methods[1].Events[0].Method = valve.GetType().GetMethod("SetPosition");
            methods[1].Column = 2;
            method3.AllowPostOverlap = false;
            method3.AllowPreOverlap = false;
            DemoValve valve2 = new DemoValve();
            method3.Events[0].Device = valve2;
            method3.Events[0].HadError = false;
            method3.Events[0].Name = "SetPosition";
            method3.Events[0].Duration = new TimeSpan(0, 0, 1);
            method3.Events[0].ParameterNames = new string[1];
            method3.Events[0].ParameterNames[0] = "SetPosition";
            method3.Events[0].Parameters = new object[1];
            method3.Events[0].Parameters[0] = 2;
            method3.Events[0].MethodAttribute = new classLCMethodAttribute("SetPostion", 1.00, string.Empty, -1, false);
            method3.Events[0].Method = valve2.GetType().GetMethod("SetPosition");
            method3.Column = 3;
            optimizer.AlignMethods(methods);
            optimizer.AlignMethods(methods, method3);
            System.Diagnostics.Debug.WriteLine(methods[0].End);
            System.Diagnostics.Debug.WriteLine(methods[1].Start);
            System.Diagnostics.Debug.WriteLine(methods[1].End);
            System.Diagnostics.Debug.WriteLine(method3.Start);
            Assert.AreEqual(SAME_REQUIRED_LC_METHOD_OFFSET, method3.Start.Subtract(methods[1].End).TotalMilliseconds);
        }

        /// <summary>
        /// Simulates adding samples to already running queue.
        /// specifically, the case in which multiple methods on different columns run with "overlap" settings and different devices.
        /// </summary>
        [Test]
        public void TestAddThirdMethodWhileQueueRunningDifferentColumnsDifferentDeviceOverlap()
        {
            classLCMethod method3 = new classLCMethod();
            method3.Events = new List<classLCEvent>();
            method3.Events.Add(new classLCEvent());
            methods[0].AllowPostOverlap = true;
            methods[0].AllowPreOverlap = true;
            //Changing method[1], Event[0] to use a different device.  
            DemoValve valve = new DemoValve();
            methods[1].AllowPreOverlap = true;
            methods[1].AllowPostOverlap = true;
            methods[1].Events[0].Device = valve;
            methods[1].Events[0].HadError = false;
            methods[1].Events[0].Name = "SetPosition";
            methods[1].Events[0].Duration = new TimeSpan(0, 0, 1);
            methods[1].Events[0].ParameterNames = new string[1];
            methods[1].Events[0].ParameterNames[0] = "SetPosition";
            methods[1].Events[0].Parameters = new object[1];
            methods[1].Events[0].Parameters[0] = 2;
            methods[1].Events[0].MethodAttribute = new classLCMethodAttribute("SetPostion", 1.00, string.Empty, -1, false);
            methods[1].Events[0].Method = valve.GetType().GetMethod("SetPosition");
            methods[1].Column = 2;
            method3.AllowPostOverlap = true;
            method3.AllowPreOverlap = true;
            DemoValve valve2 = new DemoValve();
            method3.Events[0].Device = valve2;
            method3.Events[0].HadError = false;
            method3.Events[0].Name = "SetPosition";
            method3.Events[0].Duration = new TimeSpan(0, 0, 1);
            method3.Events[0].ParameterNames = new string[1];
            method3.Events[0].ParameterNames[0] = "SetPosition";
            method3.Events[0].Parameters = new object[1];
            method3.Events[0].Parameters[0] = 2;
            method3.Events[0].MethodAttribute = new classLCMethodAttribute("SetPostion", 1.00, string.Empty, -1, false);
            method3.Events[0].Method = valve2.GetType().GetMethod("SetPosition");
            method3.Column = 3;
            optimizer.AlignMethods(methods);
            optimizer.AlignMethods(methods, method3);
            System.Diagnostics.Debug.WriteLine(methods[0].End);
            System.Diagnostics.Debug.WriteLine(methods[1].Start);
            System.Diagnostics.Debug.WriteLine(methods[1].End);
            System.Diagnostics.Debug.WriteLine(method3.Start);
            Assert.AreEqual(methods[1].Start, method3.Start);
        }

        /// <summary>
        /// Simulates adding samples to already running queue.
        /// specifically, the case in which multiple methods on different columns run with no "overlap" settings.
        /// </summary>
        [Test]
        public void TestAddThirdMethodWhileQueueRunningSameColumnsSameDeviceNoOverlap()
        {
            classLCMethod method3 = new classLCMethod();
            method3.Events = new List<classLCEvent>();
            method3.Events.Add(new classLCEvent());
            methods[0].AllowPostOverlap = false;
            methods[0].AllowPreOverlap = false;
            //Changing method[1], Event[0] to use a different device.  
            DemoValve valve = new DemoValve();
            methods[1].AllowPreOverlap = false;
            methods[1].AllowPostOverlap = false;
            methods[1].Events[0].Device = valve;
            methods[1].Events[0].HadError = false;
            methods[1].Events[0].Name = "SetPosition";
            methods[1].Events[0].Duration = new TimeSpan(0, 0, 1);
            methods[1].Events[0].ParameterNames = new string[1];
            methods[1].Events[0].ParameterNames[0] = "SetPosition";
            methods[1].Events[0].Parameters = new object[1];
            methods[1].Events[0].Parameters[0] = 2;
            methods[1].Events[0].MethodAttribute = new classLCMethodAttribute("SetPostion", 1.00, string.Empty, -1, false);
            methods[1].Events[0].Method = valve.GetType().GetMethod("SetPosition");
            methods[1].Column = 1;
            method3.AllowPostOverlap = false;
            method3.AllowPreOverlap = false;
            method3.Events[0].Device = valve;
            method3.Events[0].HadError = false;
            method3.Events[0].Name = "SetPosition";
            method3.Events[0].Duration = new TimeSpan(0, 0, 1);
            method3.Events[0].ParameterNames = new string[1];
            method3.Events[0].ParameterNames[0] = "SetPosition";
            method3.Events[0].Parameters = new object[1];
            method3.Events[0].Parameters[0] = 2;
            method3.Events[0].MethodAttribute = new classLCMethodAttribute("SetPostion", 1.00, string.Empty, -1, false);
            method3.Events[0].Method = valve.GetType().GetMethod("SetPosition");
            method3.Column = 1;
            optimizer.AlignMethods(methods);
            optimizer.AlignMethods(methods, method3);
            Assert.AreEqual(SAME_REQUIRED_LC_METHOD_OFFSET, method3.Start.Subtract(methods[1].End).TotalMilliseconds);
        }

        /// <summary>
        /// Simulates adding samples to already running queue.
        /// specifically, the case in which multiple methods on different columns run with  no"overlap" settings and different devices.
        /// </summary>
        [Test]
        public void TestAddThirdMethodWhileQueueRunningSameColumnsDifferentDeviceOverlap()
        {
            classLCMethod method3 = new classLCMethod();
            method3.Events = new List<classLCEvent>();
            method3.Events.Add(new classLCEvent());
            methods[0].AllowPostOverlap = true;
            methods[0].AllowPreOverlap = true;
            //Changing method[1], Event[0] to use a different device.  
            DemoValve valve = new DemoValve();
            methods[1].AllowPreOverlap = true;
            methods[1].AllowPostOverlap = true;
            methods[1].Events[0].Device = valve;
            methods[1].Events[0].HadError = false;
            methods[1].Events[0].Name = "SetPosition";
            methods[1].Events[0].Duration = new TimeSpan(0, 0, 1);
            methods[1].Events[0].ParameterNames = new string[1];
            methods[1].Events[0].ParameterNames[0] = "SetPosition";
            methods[1].Events[0].Parameters = new object[1];
            methods[1].Events[0].Parameters[0] = 2;
            methods[1].Events[0].MethodAttribute = new classLCMethodAttribute("SetPostion", 1.00, string.Empty, -1, false);
            methods[1].Events[0].Method = valve.GetType().GetMethod("SetPosition");
            methods[1].Column = 1;
            method3.AllowPostOverlap = true;
            method3.AllowPreOverlap = true;
            DemoValve valve2 = new DemoValve();
            method3.Events[0].Device = valve2;
            method3.Events[0].HadError = false;
            method3.Events[0].Name = "SetPosition";
            method3.Events[0].Duration = new TimeSpan(0, 0, 1);
            method3.Events[0].ParameterNames = new string[1];
            method3.Events[0].ParameterNames[0] = "SetPosition";
            method3.Events[0].Parameters = new object[1];
            method3.Events[0].Parameters[0] = 2;
            method3.Events[0].MethodAttribute = new classLCMethodAttribute("SetPostion", 1.00, string.Empty, -1, false);
            method3.Events[0].Method = valve2.GetType().GetMethod("SetPosition");
            method3.Column = 1;
            optimizer.AlignMethods(methods);
            optimizer.AlignMethods(methods, method3);
            System.Diagnostics.Debug.WriteLine(methods[0].End);
            System.Diagnostics.Debug.WriteLine(methods[1].Start);
            System.Diagnostics.Debug.WriteLine(methods[1].End);
            System.Diagnostics.Debug.WriteLine(method3.Start);
            Assert.AreEqual(SAME_REQUIRED_LC_METHOD_OFFSET, method3.Start.Subtract(methods[1].End).TotalMilliseconds);
        }

        /// <summary>
        /// Test to see if the optimizer properly handles a DST transition that happens between two methods.
        /// </summary>
        [Test]
        public void TestDSTTransition()
        {
            methods[0].SetStartTime(new DateTime(2014, 11, 2, 1, 59, 59));
            methods[1].SetStartTime(new DateTime(2014, 11, 2, 2, 0, 10));
            optimizer.AlignMethods(methods[0], methods[1], true);
            double  expectedDifference = new TimeSpan(1,0,1).TotalMilliseconds;
            CheckDifferenceInStartTimes(expectedDifference);
        }



        /// <summary>
        /// used for all tests, since the only difference we care about is that the start time of method 2 is different from method 1.
        /// Each individual test determines its own expected difference that ensures the optimization is correct.
        /// </summary>
        /// <param name="methods"></param>
        /// <param name="expectedDifference"></param>
        private void CheckDifferenceInStartTimes(double expectedDifference)
        {
            double difference = methods[1].Start.Subtract(methods[0].Start).TotalMilliseconds;            
            Assert.AreEqual(expectedDifference, difference);
        }

    }
}
