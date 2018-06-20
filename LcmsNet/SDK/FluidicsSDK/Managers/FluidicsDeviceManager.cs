using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using FluidicsSDK.Base;
using FluidicsSDK.Devices;
using LcmsNetSDK.Devices;

namespace FluidicsSDK.Managers
{
    public class FluidicsDeviceManager
    {
        #region Members
        private static FluidicsDeviceManager m_instance;
        private readonly List<FluidicsDevice> m_devices;
        public event EventHandler<FluidicsDeviceChangeEventArgs> DeviceAdded;
        public event EventHandler<FluidicsDeviceChangeEventArgs> DeviceRemoved;
        public event EventHandler DeviceChanged;
        #endregion

        #region Methods
        /// <summary>
        ///  default constructor
        /// </summary>
        private FluidicsDeviceManager()
        {
            m_devices = new List<FluidicsDevice>();
        }

        /// <summary>
        /// Tries to add specified IDevice to the fluidics manager
        /// </summary>
        /// <param name="device">an IDevice object</param>
        public void Add(IDevice device)
        {
            // Make sure someone doesnt add this device twice....
            foreach (var fdevice in m_devices)
            {
                if (fdevice.IDevice == device)
                {
                    throw new FluidicsDeviceExistsException("The specified device already exists in the model.");
                }
            }

            // Then parse out the potential fluidics device types so we can add to the user interface.
            var type = device.GetType();

            var fluidicsDevice = FluidicsDeviceTypeFactory.CreateDevice(type);
            if (fluidicsDevice != null)
            {
                m_devices.Add(fluidicsDevice);
                fluidicsDevice.RegisterDevice(device);
                //fluidicsDevice.MoveBy(new Point(GetBoundingBox(false).Width, 0));
                fluidicsDevice.DeviceChanged += fluidicsDevice_DeviceChanged;
                DeviceAdded?.Invoke(this, new FluidicsDeviceChangeEventArgs(fluidicsDevice));
            }
        }

        void fluidicsDevice_DeviceChanged(object sender, FluidicsDevChangeEventArgs e)
        {
            DeviceChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// Try to remove specified IDevice from fluidics manager
        /// </summary>
        /// <param name="device">an IDevice object</param>
        /// <exception cref="ArgumentException">the device was not found</exception>
        public void Remove(IDevice device)
        {
            var tempLst = new List<FluidicsDevice>(m_devices);
            var deviceRemoved = false;
            FluidicsDevice deviceToRemove = null;
            foreach (var fluidicsDevice in tempLst)
            {
                if (fluidicsDevice.IDevice == device)
                {
                    deviceRemoved = true;
                    deviceToRemove = fluidicsDevice;
                }
            }
            if (!deviceRemoved)
            {
                throw new ArgumentException("Specified device not found.");
            }

            m_devices.Remove(deviceToRemove);
            GC.SuppressFinalize(deviceToRemove);
            GC.Collect();
            DeviceRemoved?.Invoke(this, new FluidicsDeviceChangeEventArgs(deviceToRemove));
        }

        /// <summary>
        /// render all fluidics devices to screen
        /// </summary>
        /// <param name="g">a System.Windows.Media DrawingContext object</param>
        /// <param name="alpha">an integer representing the alpha value to draw the devices with</param>
        /// <param name="scale">a float repsenting how much to scale the devices by</param>
        public void Render(DrawingContext g, byte alpha, float scale)
        {
            foreach (var device in m_devices)
            {
                try
                {
                    device.Render(g, alpha, scale);
                }
                catch (Exception)
                {
                    //TODO: Propagate errors out but allow for the rest of the devices to be rendered.

                }

            }
        }

        /// <summary>
        /// try to find a fluidics device at the selected location
        /// </summary>
        /// <param name="location">a Point object representing the location clicked</param>
        /// <returns>a classFluidicsDevice object if one is found, or null</returns>
        public FluidicsDevice Select(Point location)
        {
            // search devices from back of list to front..this provides inherent z ordering, since
            // devices that were created after others(or were moved in front of others), will appear at the
            // back of the list.
            var tmpList = new List<FluidicsDevice>(m_devices);
            tmpList.Reverse();
            foreach (var device in tmpList)
            {
                if (device.Contains(location))
                {
                    return device;
                }
            }
            return null;
        }

        /// <summary>
        /// Get bounding box around all devices
        /// </summary>
        /// <returns></returns>
        public Rect GetBoundingBox(bool addBuffer)
        {
            if (m_devices.Count == 0)
            {
                return new Rect(0, 0, 0, 0);
            }

            // extra pixels around edge of bounding box to ensure images taken using bounding box get full image
            var buffer = 200;
            if (!addBuffer)
            {
                buffer = 10;
            }

            // Make sure all of the device graphics have been drawn at least once at full scale
            if (m_devices.Any(z => !z.RenderedOnceFullScale))
            {
                var drawContext = new DrawingVisual().RenderOpen();
                float scale = 1;
                Render(drawContext, 255, scale);
                drawContext.Close();
            }

            var deviceBounds = m_devices.Select(x => x.Bounds).ToList();
            var maxX = deviceBounds.Max(x => x.Right);
            var maxY = deviceBounds.Max(x => x.Bottom);
            var minX = deviceBounds.Min(x => x.X);
            var minY = deviceBounds.Min(x => x.Y);

            return new Rect(minX - buffer, minY - buffer, maxX - minX + buffer * 2, maxY - minY + buffer * 2);
        }

        /// <summary>
        /// confirm selection of a fluidics device, allows selection hilighting for user
        /// </summary>
        /// <param name="device">a classFluidicsDevice object</param>
        /// <param name="mouse_location">location the mouse was clicked at</param>
        internal void ConfirmSelect(FluidicsDevice device, Point mouse_location)
        {
            device.Select(mouse_location);
        }

        /// <summary>
        /// deslect specified device
        /// </summary>
        /// <param name="device">a classFluidicsDevice object</param>
        internal void Deselect(FluidicsDevice device)
        {
            device.Deselect();
        }

        /// <summary>
        ///  reorder the list to make sure devices when moved, are brought to front
        /// </summary>
        public void BringToFront(List<FluidicsDevice> devices)
        {
            //remove each device from the list, and then add them back to
            //the end of the list, preserving order of the devices, but putting them
            // at the "top" of the list, since searches are done from back to front
            var oldOrder = new List<FluidicsDevice>(m_devices);
            foreach (var device in oldOrder)
            {
                if (devices.Contains(device))
                {
                    m_devices.Remove(device);
                    m_devices.Add(device);
                    //this keeps the same device from being moved more than once.
                    devices.Remove(device);
                }
            }
        }

        /// <summary>
        ///  return information about devices consisting of name, status, and error type for each device
        /// </summary>
        /// <returns>a list of tuples</returns>
        public List<Tuple<string, string, string>> ListDevicesAndStatus()
        {
            var listOfDevices = new List<Tuple<string, string, string>>();
            foreach (var dev in m_devices)
            {
                listOfDevices.Add(new Tuple<string, string, string>(dev.IDevice.Name, dev.IDevice.Status.ToString(), dev.IDevice.ErrorType.ToString()));
            }
            return listOfDevices;
        }


        /// <summary>
        /// gets the list of active devices
        /// </summary>
        /// <returns></returns>
        public List<FluidicsDevice> GetDevices()
        {
            return m_devices;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Property to access class instance of the fluidics device manager.
        /// </summary>
        public static FluidicsDeviceManager DeviceManager => m_instance ?? (m_instance = new FluidicsDeviceManager());

        #endregion

        /// <summary>
        /// find a device by it's associated IDevice
        /// </summary>
        /// <param name="device">the IDevice to search for</param>
        /// <returns>a fluidics device reprsenting the IDevice, or null if one is not found in the list of active devices</returns>
        public FluidicsDevice FindDevice(IDevice device)
        {
            FluidicsDevice fdevice = null;
            foreach (var fdevicei in m_devices)
            {
                if (fdevicei.IDevice == device)
                {
                    fdevice = fdevicei;
                    break;
                }
            }
            if (fdevice != null)
            {
                return fdevice;
            }

            throw new Exception("Fluidics Device matching specified IDevice not found");
        }

        /// <summary>
        /// cause a device to take an action a primitive with an action is at the location
        /// specified
        /// </summary>
        /// <param name="location">x,y location where the mouse button was clicked</param>
        /// <returns>true if an action is taken, false otherwise</returns>
        public bool TakeAction(Point location)
        {
            var actiontaken = false;
            foreach (var dev in m_devices)
            {
                actiontaken = dev.TakeAction(location);
                if (actiontaken)
                {
                    break;
                }
            }
            return actiontaken;
        }
    }
}
