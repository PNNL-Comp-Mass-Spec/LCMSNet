/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 * Created 6/2/2014
 *
 * Last Modified 6/4/2014 By Christopher Walters
 *********************************************************************************************************/

using System.Linq;
using FluidicsSDK.Managers;
using FluidicsSDK.ModelCheckers;
using FluidicsSDK;
using NUnit.Framework;
using LcmsNetDataClasses.Devices;
using DemoPluginLibrary;

namespace FluidicsSDKUnitTests
{
    /// <summary>
    /// Test class for fluidicsSDK model checkers
    /// </summary>
    [TestFixture]
    public class ModelCheckerTestClass
    {
        ConnectionManager cm;
        FluidicsDeviceManager dm;
       
        [SetUp]
        public void SetUp()
        {            
            if(cm == null)
            {
                cm = ConnectionManager.GetConnectionManager;
                dm = FluidicsDeviceManager.DeviceManager;
                var pump1 = new DemoPump();
                var pump2 = new DemoPump();
                pump2.Name = "Stupid Pump2";
                var t = new DemoTee();
                dm.Add(pump1);
                dm.Add(pump2);
                dm.Add(t);
                var p1 = dm.FindDevice(pump1 as IDevice) as FluidicsPump;
                var p2 = dm.FindDevice(pump2 as IDevice) as FluidicsPump;
                var t1 = dm.FindDevice(t as IDevice) as FluidicsTee;
                cm.Connect(t1.Ports[2], p2.Ports[0]);
                cm.Connect(t1.Ports[0], p1.Ports[0]); // these two lines create a loop in the system.
                cm.Connect(t1.Ports[1], p1.Ports[0]);
            }
        }

        /// <summary>
        /// Tests that the No Sinks Model Check finds the no sink condition
        /// </summary>
        [Test]
        public void Testa()
        {
            var noSinkCheck = new NoSinksModelCheck();
            var status = noSinkCheck.CheckModel().ToList();
            Assert.IsNotEmpty(status); // there will be either 0 or 1 entries(1 if working) at this point. If there is a cycle in the graph and the model check is not working properly, it would not get to this point because it would be stuck in an infinite loop.
        }

        /// <summary>
        /// Tests that the No Sinks Model Check doesn't report an error when a sink is found
        /// </summary>
        [Test]
        public void Testb()
        {
            var needle = new FluidicsSprayNeedle();
            cm.Connect(needle.Ports[0], FluidicsDeviceManager.DeviceManager.GetDevices().Find(x => x.DeviceName == "Test Tee").Ports[1]);
            var noSinkCheck = new NoSinksModelCheck();
            var status = noSinkCheck.CheckModel().ToList();
            Assert.IsEmpty(status);
        }

        /// <summary>
        /// Tests that the multiple sources on path check finds multiple sources that have intersecting paths
        /// </summary>
        [Test]
        public void TestC()
        {
            var multipleSourceCheck = new MultipleSourcesModelCheck();
            var status = multipleSourceCheck.CheckModel().ToList();
            Assert.IsNotEmpty(status); // there will be either 0 or 1 entries(1 if working at this point). If there is a cycle in the graph and the model check is not working properly, it would not get to this point because it would be stuck in an infinite loop.
        }

        /// <summary>
        /// Test that muliple sources doesn't find multiple sources where no path intersection exists
        /// </summary>
        [Test]
        public void TestE()
        {
            var pumps = dm.GetDevices().FindAll(x => x.DeviceName.Contains("Stupid Pump")).ToList();
            cm.RemoveConnections(pumps[1].Ports[0]);
            var multipleSourceCheck = new MultipleSourcesModelCheck();
            var status = multipleSourceCheck.CheckModel().ToList();
            Assert.IsEmpty(status); // there will be either 0 or 1 entries(1 if working at this point). If there is a cycle in the graph and the model check is not working properly, it would not get to this point because it would be stuck in an infinite loop.
        }
        
        /// <summary>
        /// Tests that the Fluidics Cycle check is working.
        /// </summary>
        [Test]
        public void TestD()
        {
            var fluidicsCycleCheck = new FluidicsCycleCheck();
            var status = fluidicsCycleCheck.CheckModel().ToList();
            Assert.AreEqual(2, status.Count); // It will find the loop from both sources, if working correctly.
        }

        /// <summary>
        /// Tests that the Fluidics Cycle Check does not find a cycle in the graph when one is not present.
        /// </summary>
        [Test]
        public void TestF()
        {
            var t = dm.GetDevices().Find(x => x.DeviceName == "Test Tee");
            cm.RemoveConnections(t.Ports[0]);
            cm.RemoveConnections(t.Ports[1]);
            cm.RemoveConnections(t.Ports[2]);
            var fluidicsCycleCheck = new FluidicsCycleCheck();
            var status = fluidicsCycleCheck.CheckModel().ToList();
            Assert.AreEqual(0, status.Count); // It will find no loop if working correctly.
        }
    }
}