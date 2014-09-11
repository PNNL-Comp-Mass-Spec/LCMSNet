/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 9/5/2013
 * 
 * Last Modified 1/6/2013 By Christopher Walters 
 ********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using FluidicsSDK.Devices;
using LcmsNetDataClasses.Devices;
using FluidicsSDK.Base;

namespace FluidicsSDK.Managers
{        
    public class FluidicsDeviceManager
    {
        #region Members
            private static FluidicsDeviceManager m_instance;
            private List<FluidicsDevice> m_devices;
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
                // Make sure some A-Hole doesnt add this device twice....
                foreach (FluidicsDevice fdevice in m_devices)
                {
                    if (fdevice.IDevice == device)
                    {
                        throw new FluidicsDeviceExistsException("The specified device already exists in the code.");                        
                    }      
                }
                
                // Then parse out the potential fluidics device types so we can add to the user interface.
                Type type = device.GetType();
                
                try
                {
                    FluidicsDevice fluidicsDevice = FluidicsDeviceTypeFactory.CreateDevice(type);
                    if (fluidicsDevice != null)
                    {
                        m_devices.Add(fluidicsDevice);
                        fluidicsDevice.RegisterDevice(device);
                        //fluidicsDevice.MoveBy(new Point(GetBoundingBox(false).Width, 0));
                        fluidicsDevice.DeviceChanged += new EventHandler<FluidicsDevChangeEventArgs>(fluidicsDevice_DeviceChanged);
                        if (DeviceAdded != null)
                        {
                            DeviceAdded(this, new FluidicsDeviceChangeEventArgs(fluidicsDevice));
                        }
                    }    

                }
                catch (InvalidCustomFluidicsDeviceException)
                {
                    throw;
                }                                                           
            }
            
            void fluidicsDevice_DeviceChanged(object sender, FluidicsDevChangeEventArgs e)
            {
               if(DeviceChanged != null)
               {
                   DeviceChanged(sender, e);
               }
            }

            /// <summary>
            /// Try to remove specified IDevice from fluidics manager
            /// </summary>
            /// <param name="device">an IDevice object</param>
            /// <exception cref="ArgumentException">the device was not found</exception>
            public void Remove(IDevice device)
            {
                List<FluidicsDevice> tempLst  = new List<FluidicsDevice>(m_devices);
                bool deviceRemoved            = false;
                FluidicsDevice deviceToRemove = null;
                foreach (FluidicsDevice fluidicsDevice in tempLst)
                {
                    if (fluidicsDevice.IDevice == device)
                    {                       
                        deviceRemoved   = true;
                        deviceToRemove  = fluidicsDevice;

                    }
                }
                if (!deviceRemoved)
                {                   
                    throw new ArgumentException("Specified device not found.");
                }
                else
                {
                    m_devices.Remove(deviceToRemove);
                    GC.SuppressFinalize(deviceToRemove);
                    GC.Collect();
                    if (DeviceRemoved != null)
                    {
                        DeviceRemoved(this, new FluidicsDeviceChangeEventArgs(deviceToRemove));
                    }
                }                
                
            }

            /// <summary>
            /// render all fluidics devices to screen
            /// </summary>
            /// <param name="g">a System.Drawing Graphics object</param>
            /// <param name="alpha">an integer representing the alpha value to draw the devices with</param>
            /// <param name="scale">a float repsenting how much to scale the devices by</param>
            public void Render(Graphics g, int alpha, float scale)
            {
                foreach (FluidicsDevice device in m_devices)
                {
                    try
                    {
                        device.Render(g, alpha, scale);
                    }catch(Exception ex)
                    {
                        //TODO: Propagate errors out but allow for the rest of the devices to be rendered.
                        var e = ex;
                       
                    }

                }
            }

            /// <summary>
            /// try to find a fluidics device at the selected location
            /// </summary>
            /// <param name="location">a System.Drawing.Point object representing the location clicked</param>
            /// <returns>a classFluidicsDevice object if one is found, or null</returns>
            public FluidicsDevice Select(Point location)
            {
                // search devices from back of list to front..this provides inherent z ordering, since 
                // devices that were created after others(or were moved in front of others), will appear at the 
                // back of the list.
                List<FluidicsDevice> tmpList = new List<FluidicsDevice>(m_devices);
                tmpList.Reverse();
                foreach (FluidicsDevice device in tmpList)
                {
                    if (device.Contains(location))
                    {
                        return device;
                    }
                }
                return null;
            }

            /// <summary>
            /// Get bounding box around 
            /// </summary>
            /// <returns></returns>
            public Rectangle GetBoundingBox(bool addBuffer)
            {
                if (m_devices.Count == 0)
                {
                    return new Rectangle(0, 0, 0, 0);
                }
                // extra pixels around edge of bounding box to ensure images taken using bounding box get full image
                int buffer = 200;
                if(!addBuffer)
                {
                    buffer = 10;
                }
                int maxX = m_devices.Max(z => (int)(z.Loc.X + z.Size.Width));
                int maxY = m_devices.Max(z => (int)(z.Loc.Y + z.Size.Height));
                int x = m_devices.Min(z => z.Loc.X);
                int y = m_devices.Min(z => z.Loc.Y);
                return new Rectangle(x - buffer, y - buffer , maxX  + buffer, maxY + buffer);
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
            ///  
            /// </summary>
            public void BringToFront(List<FluidicsDevice> devices)
            {
                //remove each device from the list, and then add them back to
                //the end of the list, preserving order of the devices, but putting them
                // at the "top" of the list, since searches are done from back to front
                List<FluidicsDevice> oldOrder = new List<FluidicsDevice>(m_devices);
                foreach (FluidicsDevice device in oldOrder)
                {
                    if(devices.Contains(device))
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
            List<Tuple<string, string, string>> listOfDevices = new List<Tuple<string,string, string>>();
            foreach (FluidicsDevice dev in m_devices)
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
            public static FluidicsDeviceManager DeviceManager
            {
                get
                {
                    if (m_instance == null)
                    {
                        m_instance = new FluidicsDeviceManager();
                    }
                    return m_instance;
                }
            }     

        #endregion

            /// <summary>
            /// find a device by it's associated IDevice
            /// </summary>
            /// <param name="device">the IDevice to search for</param>
            /// <returns>a fluidics device reprsenting the IDevice, or null if one is not found in the list of active devices</returns>
            public FluidicsDevice FindDevice(IDevice device)
            {
                FluidicsDevice fdevice = null;
                foreach (FluidicsDevice fdevicei in m_devices)
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
                else
                {
                    throw new Exception("Fluidics Device matching specified IDevice not found");
                }
            }

            /// <summary>
            /// cause a device to take an action a primitive with an action is at the location
            /// specified
            /// </summary>
            /// <param name="location">x,y location where the mouse button was clicked</param>
            /// <returns>true if an action is taken, false otherwise</returns>
            public bool TakeAction(Point location)
            {
                bool actiontaken = false;
                foreach (FluidicsDevice dev in m_devices)
                {
                    actiontaken = dev.TakeAction(location);
                    if(actiontaken)
                    {
                        break;
                    }
                }
                return actiontaken;
            }
    }
}
