/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 * Created 6/2/2014
 *
 * Last Modified 6/4/2014 By Christopher Walters
 *********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FluidicsSDK.Base;
using DemoPluginLibrary;
using FluidicsSDK.Managers;
using FluidicsSDK;
using System.Drawing;

namespace FluidicsSDKUnitTests
{
    [TestFixture]
    public class FluidicsSDKUnitTests
    {
        /// <summary>
        /// This test ensures that ports are being added to the port manager properly. As a bonus, it also tests that ONLY one port is being
        /// added to the manager on an add request.
        /// </summary>
        [Test]
        public void PortManagerAddAndRemove()
        {
            //add
            var pt = new Port(new Point(0,0), null); // ports add themselves to the port manager...
            Assert.AreEqual(1, PortManager.GetPortManager.Ports.Count);
            //remove
            PortManager.GetPortManager.RemovePort(pt);
            Assert.AreEqual(0, PortManager.GetPortManager.Ports.Count);
        }

        /// <summary>
        /// this test ensures that devices are being added to and removed from the device manager, and as a bonus, tests that only one device is added upon add request.
        /// </summary>
        [Test]
        public void DeviceManagerAddAndRemove()
        {
            //add
            var p = new DemoPump();
            FluidicsDeviceManager.DeviceManager.Add(p);
            Assert.AreEqual(1, FluidicsDeviceManager.DeviceManager.GetDevices().Count);
            //remove
            PortManager.GetPortManager.RemovePorts(FluidicsDeviceManager.DeviceManager.FindDevice(p));
            FluidicsDeviceManager.DeviceManager.Remove(p); // cleanup
            Assert.AreEqual(0, FluidicsDeviceManager.DeviceManager.GetDevices().Count);
            p = null;
        }

        /// <summary>
        /// this tests to ensure that connections are being added to and removed from the connection manager, and as a bonus, tests that only one connection is added upon request.
        /// </summary>
        [Test]
        public void ConnectionManagerAddAndRemove()
        {
            //add
            var p1 = new Port(new Point(0,0), null);
            var p2 = new Port(new Point(0, 0), null);
            ConnectionManager.GetConnectionManager.Connect(p1, p2);
            Assert.AreEqual(1, ConnectionManager.GetConnectionManager.GetConnections().ToList().Count);
            //remove
            ConnectionManager.GetConnectionManager.Remove(ConnectionManager.GetConnectionManager.FindConnection(p1,p2));
            Assert.AreEqual(0, ConnectionManager.GetConnectionManager.GetConnections().ToList().Count);
            PortManager.GetPortManager.RemovePort(p1);
            PortManager.GetPortManager.RemovePort(p2);
            p1 = null;
            p2 = null;
        }
        
        /// <summary>
        /// Test to ensure that a port cannot be connected to itself.
        /// </summary>
        [Test]
        public void ConnectionmanagerDoNotConnectTheSamePortToItself()
        {            
            Assert.Throws<ArgumentException>(DoNotConnectHelper);
            PortManager.GetPortManager.RemovePort(PortManager.GetPortManager.Ports[0]); //clean up the port that was added by the helper
        }

        /// <summary>
        /// helper for ConnectionmanagerDoNotConnectTheSamePortToItself
        /// </summary>
        private void DoNotConnectHelper()
        {
            var p1 = new Port(new Point(0, 0), null);
            ConnectionManager.GetConnectionManager.Connect(p1, p1);
        }

        /// <summary>
        /// Test to ensure that two ports cannot be connected to each other more than once
        /// </summary>
        [Test]
        public void ConnectionsTwoPortsOnlyConnectOnce()
        {
           
            Assert.Throws<ArgumentException>(OnlyOnceHelper);
            // clean up the ports that were added by the helper.
            PortManager.GetPortManager.RemovePort(PortManager.GetPortManager.Ports[1]);
            PortManager.GetPortManager.RemovePort(PortManager.GetPortManager.Ports[0]);
        }
        
        /// <summary>
        /// helper for ConnectionsTwoPortsOnlyConnectOnce
        /// </summary>
        private void OnlyOnceHelper()
        {
            var p1 = new Port(new Point(0, 0), null);
            var p2 = new Port(new Point(0, 0), null);
            ConnectionManager.GetConnectionManager.Connect(p1, p2);
            ConnectionManager.GetConnectionManager.Connect(p1, p2);
        }

        /// <summary>
        /// Test to ensure devices move properly.
        /// </summary>
        [Test]
        public void MoveDevice()
        {
            var p = new DemoPump();
            FluidicsDeviceManager.DeviceManager.Add(p);
            var fp = FluidicsDeviceManager.DeviceManager.FindDevice(p);
            fp.MoveBy(new Point(10, 10));
            Assert.IsTrue(fp.Loc.X == 10 && fp.Loc.Y == 10); // Device starts at point 0,0 So when moved 10x10 should be at point (10,10)
            PortManager.GetPortManager.RemovePorts(fp);
            FluidicsDeviceManager.DeviceManager.Remove(p);
            fp = null;
            p = null;
        }   
    }
}
