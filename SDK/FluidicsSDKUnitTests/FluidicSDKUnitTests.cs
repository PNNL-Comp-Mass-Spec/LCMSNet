﻿using System;
using System.Linq;
using System.Windows;
using NUnit.Framework;
using FluidicsSDK.Base;
using DemoPluginLibrary;
using FluidicsSDK.Managers;

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
            Assert.That(PortManager.GetPortManager.Ports.Count, Is.EqualTo(1));
            //remove
            PortManager.GetPortManager.RemovePort(pt);
            Assert.That(PortManager.GetPortManager.Ports.Count, Is.EqualTo(0));
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
            Assert.That(FluidicsDeviceManager.DeviceManager.GetDevices().Count, Is.EqualTo(1));
            //remove
            PortManager.GetPortManager.RemovePorts(FluidicsDeviceManager.DeviceManager.FindDevice(p));
            FluidicsDeviceManager.DeviceManager.Remove(p); // cleanup
            Assert.That(FluidicsDeviceManager.DeviceManager.GetDevices().Count, Is.EqualTo(0));
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
            Assert.That(ConnectionManager.GetConnectionManager.GetConnections().ToList().Count, Is.EqualTo(1));
            //remove
            ConnectionManager.GetConnectionManager.Remove(ConnectionManager.GetConnectionManager.FindConnection(p1,p2));
            Assert.That(ConnectionManager.GetConnectionManager.GetConnections().ToList().Count, Is.EqualTo(0));
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
            Assert.That(fp.Loc.X == 10 && fp.Loc.Y == 10, Is.True); // Device starts at point 0,0 So when moved 10x10 should be at point (10,10)
            PortManager.GetPortManager.RemovePorts(fp);
            FluidicsDeviceManager.DeviceManager.Remove(p);
            fp = null;
            p = null;
        }
    }
}
